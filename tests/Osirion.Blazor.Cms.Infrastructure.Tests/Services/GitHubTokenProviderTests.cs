using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;
using System.Net;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class GitHubTokenProviderTests
{
    private readonly HttpClient _httpClient;
    private readonly HttpMessageHandler _mockHandler;
    private readonly ILogger<GitHubTokenProvider> _logger;
    private readonly GitHubTokenProvider _tokenProvider;

    public GitHubTokenProviderTests()
    {
        _mockHandler = Substitute.For<DelegatingHandler>();
        _httpClient = new HttpClient(_mockHandler);
        _logger = Substitute.For<ILogger<GitHubTokenProvider>>();

        _tokenProvider = new GitHubTokenProvider(_httpClient, _logger);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithValidResponse_ReturnsToken()
    {
        // Arrange
        string code = "test-code";
        string clientId = "client-id";
        string clientSecret = "client-secret";
        string expectedToken = "gho_test_token";

        // Setup mock response
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("access_token=gho_test_token&scope=repo&token_type=bearer")
        };

        //_mockHandler.When(x => x.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()))
        //    .Do(x => {
        //        var request = x.Arg<HttpRequestMessage>();
        //        request.RequestUri.ShouldBe(new Uri("https://github.com/login/oauth/access_token"));
        //        request.Method.ShouldBe(HttpMethod.Post);
        //    })
        //    .Returns(responseMessage);

        // Act
        var result = await _tokenProvider.ExchangeCodeForTokenAsync(code, clientId, clientSecret);

        // Assert
        result.ShouldBe(expectedToken);
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithErrorResponse_ReturnsNull()
    {
        // Arrange
        string code = "invalid-code";
        string clientId = "client-id";
        string clientSecret = "client-secret";

        // Setup mock response
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("error=bad_verification_code&error_description=The+code+passed+is+incorrect+or+expired")
        };

        //_mockHandler.When(x => x.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()))
        //    .Do(x => {
        //        var request = x.Arg<HttpRequestMessage>();
        //        request.RequestUri.ShouldBe(new Uri("https://github.com/login/oauth/access_token"));
        //    })
        //    .Returns(responseMessage);

        // Act
        var result = await _tokenProvider.ExchangeCodeForTokenAsync(code, clientId, clientSecret);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WithMissingToken_ReturnsNull()
    {
        // Arrange
        string code = "test-code";
        string clientId = "client-id";
        string clientSecret = "client-secret";

        // Setup mock response with no token
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("scope=repo&token_type=bearer")
        };

        //_mockHandler.When(x => x.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()))
        //    .Returns(responseMessage);

        // Act
        var result = await _tokenProvider.ExchangeCodeForTokenAsync(code, clientId, clientSecret);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task ExchangeCodeForTokenAsync_WhenExceptionOccurs_ReturnsNull()
    {
        // Arrange
        string code = "test-code";
        string clientId = "client-id";
        string clientSecret = "client-secret";

        // Setup to throw exception
        //_mockHandler.When(x => x.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()))
        //    .Throws(new HttpRequestException("Network error"));

        // Act
        var result = await _tokenProvider.ExchangeCodeForTokenAsync(code, clientId, clientSecret);

        // Assert
        result.ShouldBeNull();
    }

    // Test for request format
    [Fact]
    public async Task ExchangeCodeForTokenAsync_SendsCorrectRequestParameters()
    {
        // Arrange
        string code = "test-code";
        string clientId = "client-id";
        string clientSecret = "client-secret";
        HttpContent capturedContent = null;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("access_token=test_token&scope=repo&token_type=bearer")
        };

        //_mockHandler.When(x => x.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()))
        //    .Do(x => {
        //        var request = x.Arg<HttpRequestMessage>();
        //        capturedContent = request.Content;
        //    })
        //    .Returns(responseMessage);

        // Act
        await _tokenProvider.ExchangeCodeForTokenAsync(code, clientId, clientSecret);

        // Assert
        capturedContent.ShouldNotBeNull();
        var contentString = await capturedContent.ReadAsStringAsync();
        contentString.ShouldContain("client_id=client-id");
        contentString.ShouldContain("client_secret=client-secret");
        contentString.ShouldContain("code=test-code");
        contentString.ShouldContain("grant_type=authorization_code");
    }
}