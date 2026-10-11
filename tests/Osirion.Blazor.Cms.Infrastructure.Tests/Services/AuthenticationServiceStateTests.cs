using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class AuthenticationServiceStateTests
{
    private const string LegacyTokenKey = "github_auth_token";

    private readonly IStateStorageService _stateStorage = Substitute.For<IStateStorageService>();
    private readonly IGitHubTokenProvider _tokenProvider = Substitute.For<IGitHubTokenProvider>();
    private readonly IGitHubApiClientFactory _apiClientFactory = Substitute.For<IGitHubApiClientFactory>();

    public AuthenticationServiceStateTests()
    {
        _stateStorage.IsInitialized.Returns(true);
    }

    [Fact]
    public void Constructor_WithNullDependencies_Throws()
    {
        var options = Options.Create(new CmsAdminOptions());
        var logger = NullLogger<AuthenticationService>.Instance;
        using var httpClient = new HttpClient(UserLookup(HttpStatusCode.OK, "{}"));

        Should.Throw<ArgumentNullException>(() => new AuthenticationService(null!, options, logger, _stateStorage, _tokenProvider, _apiClientFactory));
        Should.Throw<ArgumentNullException>(() => new AuthenticationService(httpClient, options, null!, _stateStorage, _tokenProvider, _apiClientFactory));
        Should.Throw<ArgumentNullException>(() => new AuthenticationService(httpClient, options, logger, null!, _tokenProvider, _apiClientFactory));
        Should.Throw<ArgumentNullException>(() => new AuthenticationService(httpClient, options, logger, _stateStorage, null!, _apiClientFactory));
        Should.Throw<ArgumentNullException>(() => new AuthenticationService(httpClient, options, logger, _stateStorage, _tokenProvider, null!));
    }

    [Fact]
    public async Task SetAccessTokenAsync_WithAcceptedToken_SignsInAndPersistsTheUserWithoutTheToken()
    {
        var service = CreateService();
        var announced = new List<bool>();
        service.AuthenticationChanged += announced.Add;

        var result = await service.SetAccessTokenAsync("token");

        result.ShouldBeTrue();
        service.IsAuthenticated.ShouldBeTrue();
        service.AccessToken.ShouldBe("token");
        service.Username.ShouldBe("octocat");
        announced.ShouldBe([true]);
        await _stateStorage.Received(1).SaveStateAsync("github_username", "octocat");
        await _stateStorage.Received(1).SaveStateAsync("auth_status", true);
        await _stateStorage.Received(1).SaveStateAsync("last_login_method", "pat");
        await _stateStorage.Received(1).RemoveStateAsync(LegacyTokenKey);
        _stateStorage.ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == nameof(IStateStorageService.SaveStateAsync))
            .SelectMany(call => call.GetArguments())
            .ShouldNotContain("token");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SetAccessTokenAsync_WithBlankToken_ReturnsFalseWithoutCallingGitHub(string token)
    {
        var handler = UserLookup(HttpStatusCode.OK, """{"login":"octocat"}""");
        var service = CreateService(handler);

        var result = await service.SetAccessTokenAsync(token);

        result.ShouldBeFalse();
        handler.Calls.ShouldBe(0);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, """{"message":"Bad credentials"}""")]
    [InlineData(HttpStatusCode.OK, """{"login":"  "}""")]
    [InlineData(HttpStatusCode.OK, "null")]
    public async Task SetAccessTokenAsync_WhenGitHubDoesNotNameAUser_StaysSignedOutAndClearsStoredState(HttpStatusCode status, string body)
    {
        var service = CreateService(UserLookup(status, body));

        var result = await service.SetAccessTokenAsync("token");

        result.ShouldBeFalse();
        service.IsAuthenticated.ShouldBeFalse();
        await _stateStorage.Received(1).RemoveStateAsync("github_username");
        await _stateStorage.Received(1).RemoveStateAsync("auth_status");
        await _stateStorage.Received(1).RemoveStateAsync("last_login_method");
    }

    [Fact]
    public async Task SetAccessTokenAsync_WhenTheLookupThrows_ReturnsFalse()
    {
        var service = CreateService(new LookupHandler(_ => throw new HttpRequestException("offline")));

        var result = await service.SetAccessTokenAsync("token");

        result.ShouldBeFalse();
        service.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task SetAccessTokenAsync_WhenPersistingFails_ReturnsFalseAndStaysSignedOut()
    {
        _stateStorage.SaveStateAsync("github_username", Arg.Any<string>()).ThrowsAsync(new InvalidOperationException("storage"));
        var service = CreateService();

        var result = await service.SetAccessTokenAsync("token");

        result.ShouldBeFalse();
        service.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task SetAccessTokenAsync_WhenStorageCannotInitialize_SignsInWithoutPersisting()
    {
        _stateStorage.IsInitialized.Returns(false);
        var service = CreateService();

        var result = await service.SetAccessTokenAsync("token");

        result.ShouldBeTrue();
        service.Username.ShouldBe("octocat");
        await _stateStorage.Received(1).InitializeAsync();
        await _stateStorage.DidNotReceiveWithAnyArgs().SaveStateAsync<string>(default!, default!);
    }

    [Fact]
    public async Task SetAccessTokenAsync_WhenStorageInitializesOnDemand_PersistsTheUser()
    {
        _stateStorage.IsInitialized.Returns(false, true);
        var service = CreateService();

        await service.SetAccessTokenAsync("token");

        await _stateStorage.Received(1).InitializeAsync();
        await _stateStorage.Received(1).SaveStateAsync("github_username", "octocat");
    }

    [Fact]
    public async Task InitializeAsync_RunsOnce()
    {
        var service = CreateService();

        await service.InitializeAsync();
        await service.InitializeAsync();

        await _stateStorage.Received(1).RemoveStateAsync(LegacyTokenKey);
        service.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task InitializeAsync_WhenStorageCannotInitialize_LeavesStorageAlone()
    {
        _stateStorage.IsInitialized.Returns(false);
        var service = CreateService();

        await service.InitializeAsync();

        await _stateStorage.Received(1).InitializeAsync();
        await _stateStorage.DidNotReceiveWithAnyArgs().RemoveStateAsync(default!);
    }

    [Fact]
    public async Task SignOutAsync_WhenStorageCannotInitialize_StillSignsOut()
    {
        var service = CreateService();
        await service.SetAccessTokenAsync("token");
        _stateStorage.ClearReceivedCalls();
        _stateStorage.IsInitialized.Returns(false);
        var announced = new List<bool>();
        service.AuthenticationChanged += announced.Add;

        await service.SignOutAsync();

        service.IsAuthenticated.ShouldBeFalse();
        service.Username.ShouldBeNull();
        announced.ShouldBe([false]);
        await _stateStorage.DidNotReceiveWithAnyArgs().RemoveStateAsync(default!);
    }

    [Fact]
    public async Task AuthenticateWithGitHubAsync_WithoutClientCredentials_ReturnsFalseWithoutExchanging()
    {
        var service = CreateService(clientId: "", clientSecret: "secret");

        var result = await AuthenticateWithCodeAsync(service, "code");

        result.ShouldBeFalse();
        _tokenProvider.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public async Task AuthenticateWithGitHubAsync_WithoutClientSecret_ReturnsFalse()
    {
        var service = CreateService(clientId: "id", clientSecret: " ");

        var result = await AuthenticateWithCodeAsync(service, "code");

        result.ShouldBeFalse();
        _tokenProvider.ReceivedCalls().ShouldBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AuthenticateWithGitHubAsync_WhenNoTokenIsIssued_ReturnsFalse(string? token)
    {
        _tokenProvider.ExchangeCodeForTokenAsync("code", "id", "secret").Returns(token);
        var service = CreateService();

        var result = await AuthenticateWithCodeAsync(service, "code");

        result.ShouldBeFalse();
        service.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task AuthenticateWithGitHubAsync_WhenGitHubRejectsTheIssuedToken_ReturnsFalse()
    {
        _tokenProvider.ExchangeCodeForTokenAsync("code", "id", "secret").Returns("issued");
        var service = CreateService(UserLookup(HttpStatusCode.Unauthorized, "{}"));

        var result = await AuthenticateWithCodeAsync(service, "code");

        result.ShouldBeFalse();
        service.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task AuthenticateWithGitHubAsync_WhenTheExchangeThrows_ReturnsFalse()
    {
        _tokenProvider.ExchangeCodeForTokenAsync("code", "id", "secret").ThrowsAsync(new HttpRequestException("offline"));
        var service = CreateService();

        var result = await AuthenticateWithCodeAsync(service, "code");

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task AuthenticateWithGitHubAsync_WithIssuedToken_SignsIn()
    {
        _tokenProvider.ExchangeCodeForTokenAsync("code", "id", "secret").Returns("issued");
        var service = CreateService();

        var result = await AuthenticateWithCodeAsync(service, "code");

        result.ShouldBeTrue();
        service.AccessToken.ShouldBe("issued");
        service.Username.ShouldBe("octocat");
    }

    [Fact]
    public async Task UserLookup_SendsTheTokenWithGitHubHeaders()
    {
        var handler = UserLookup(HttpStatusCode.OK, """{"login":"octocat"}""");
        var service = CreateService(handler);

        await service.SetAccessTokenAsync("token");

        var request = handler.LastRequest.ShouldNotBeNull();
        request.RequestUri.ShouldBe(new Uri("https://api.github.com/user"));
        request.Headers.Authorization!.Scheme.ShouldBe("token");
        request.Headers.Authorization.Parameter.ShouldBe("token");
        request.Headers.Accept.ToString().ShouldBe("application/json");
        request.Headers.UserAgent.ToString().ShouldBe("OsirionBlogCMS/2.0");
    }

    // The obsolete member is exercised on purpose: hosts that still call it must keep the behaviour they had.
#pragma warning disable CS0618
    private static Task<bool> AuthenticateWithCodeAsync(AuthenticationService service, string code) =>
        service.AuthenticateWithGitHubAsync(code);
#pragma warning restore CS0618

    private static LookupHandler UserLookup(HttpStatusCode status, string body) =>
        new(_ => new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") });

    private AuthenticationService CreateService(LookupHandler? handler = null, string clientId = "id", string clientSecret = "secret")
    {
        var options = new CmsAdminOptions
        {
            Authentication = new AuthenticationOptions
            {
                GitHubClientId = clientId,
                GitHubClientSecret = clientSecret
            }
        };

        return new AuthenticationService(
            new HttpClient(handler ?? UserLookup(HttpStatusCode.OK, """{"login":"octocat"}""")),
            Options.Create(options),
            NullLogger<AuthenticationService>.Instance,
            _stateStorage,
            _tokenProvider,
            _apiClientFactory);
    }

    private sealed class LookupHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            LastRequest = request;
            return Task.FromResult(respond(request));
        }
    }
}
