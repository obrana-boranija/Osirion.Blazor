using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class GitHubTokenProviderTests
{
    private const string Code = "test-code";
    private const string ClientId = "client-id";
    private const string ClientSecret = "client-secret";
    private const string RedirectUri = "https://admin.example/osirion/login";
    private const string CodeVerifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";

    private readonly TokenEndpointHandler _handler = new();
    private readonly GitHubTokenProvider _tokenProvider;

    public GitHubTokenProviderTests()
    {
        _tokenProvider = new GitHubTokenProvider(new HttpClient(_handler), NullLogger<GitHubTokenProvider>.Instance);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithValidResponse_ReturnsToken()
    {
        var result = await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret, RedirectUri, CodeVerifier);

        result.ShouldBe("gho_test_token");
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_PostsToTokenEndpointAndRequestsJson()
    {
        await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret, RedirectUri, CodeVerifier);

        var request = _handler.Request.ShouldNotBeNull();
        request.Method.ShouldBe(HttpMethod.Post);
        request.RequestUri.ShouldBe(new Uri("https://github.com/login/oauth/access_token"));
        request.Headers.Accept.ToString().ShouldBe("application/json");
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_SendsCorrectRequestParameters()
    {
        await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret);

        var form = _handler.Form.ShouldNotBeNull();
        form["client_id"].ShouldBe(ClientId);
        form["client_secret"].ShouldBe(ClientSecret);
        form["code"].ShouldBe(Code);
        form["grant_type"].ShouldBe("authorization_code");
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithRedirectUriAndCodeVerifier_SendsBoth()
    {
        // Proves the exchange repeats the redirect URI of the authorize request and proves possession of the PKCE verifier.
        await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret, RedirectUri, CodeVerifier);

        var form = _handler.Form.ShouldNotBeNull();
        form["redirect_uri"].ShouldBe(RedirectUri);
        form["code_verifier"].ShouldBe(CodeVerifier);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithoutRedirectUri_SendsNoRedirectUri()
    {
        // The previous implementation always sent a hard-coded https://localhost:5001/auth/callback.
        await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret);

        var form = _handler.Form.ShouldNotBeNull();
        form.AllKeys.ShouldNotContain("redirect_uri");
        form.AllKeys.ShouldNotContain("code_verifier");
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithErrorResponse_ReturnsNull()
    {
        _handler.StatusCode = HttpStatusCode.BadRequest;
        _handler.Body = """{"error":"bad_request"}""";

        var result = await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret, RedirectUri, CodeVerifier);

        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("bad_verification_code")]
    [InlineData("redirect_uri_mismatch")]
    [InlineData("incorrect_client_credentials")]
    public async Task ExchangeCodeForTokenAsync_WithErrorFieldInSuccessResponse_ReturnsNull(string error)
    {
        // GitHub reports a rejected exchange as 200 OK with an error field.
        _handler.Body = $$"""{"error":"{{error}}","error_description":"rejected","access_token":"should-not-be-used"}""";

        var result = await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret, RedirectUri, CodeVerifier);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithMissingToken_ReturnsNull()
    {
        _handler.Body = """{"scope":"repo","token_type":"bearer"}""";

        var result = await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret, RedirectUri, CodeVerifier);

        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("access_token=gho_test_token&scope=repo&token_type=bearer")]
    [InlineData("access_token")]
    [InlineData("")]
    public async Task ExchangeCodeForTokenAsync_WithBodyThatIsNotJson_ReturnsNull(string body)
    {
        _handler.Body = body;

        var result = await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret, RedirectUri, CodeVerifier);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WhenExceptionOccurs_ReturnsNull()
    {
        _handler.Exception = new HttpRequestException("Network error");

        var result = await _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(() =>
            _tokenProvider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret, RedirectUri, CodeVerifier, cancellation.Token));
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_InterfaceDefault_ForwardsToFullOverload()
    {
        // Callers of the original three-argument member keep working against an implementation of the new member only.
        IGitHubTokenProvider provider = new FullOverloadOnlyTokenProvider();

        var result = await provider.ExchangeCodeForTokenAsync(Code, ClientId, ClientSecret);

        result.ShouldBe($"{Code}|{ClientId}|{ClientSecret}|<none>|<none>");
    }

    private sealed class FullOverloadOnlyTokenProvider : IGitHubTokenProvider
    {
        public Task<string?> ExchangeCodeForTokenAsync(
            string code,
            string clientId,
            string clientSecret,
            string? redirectUri,
            string? codeVerifier,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string?>($"{code}|{clientId}|{clientSecret}|{redirectUri ?? "<none>"}|{codeVerifier ?? "<none>"}");
        }
    }

    /// <summary>
    /// Stands in for GitHub's token endpoint: records the request and its form body and answers with a set status and body.
    /// </summary>
    private sealed class TokenEndpointHandler : HttpMessageHandler
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public string Body { get; set; } = """{"access_token":"gho_test_token","scope":"repo","token_type":"bearer"}""";

        public Exception? Exception { get; set; }

        public HttpRequestMessage? Request { get; private set; }

        public NameValueCollection? Form { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (Exception is not null)
            {
                throw Exception;
            }

            Request = request;
            Form = HttpUtility.ParseQueryString(await request.Content!.ReadAsStringAsync(cancellationToken));

            return new HttpResponseMessage(StatusCode)
            {
                Content = new StringContent(Body, Encoding.UTF8, "application/json")
            };
        }
    }
}
