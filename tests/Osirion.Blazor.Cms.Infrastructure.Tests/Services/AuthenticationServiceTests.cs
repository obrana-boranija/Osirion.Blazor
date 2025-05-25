using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<CmsAdminOptions> _options;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IStateStorageService _stateStorage;
    private readonly IGitHubTokenProvider _tokenProvider;
    private readonly IGitHubApiClient _apiClient;
    private readonly AuthenticationService _authService;

    public AuthenticationServiceTests()
    {
        _httpClient = Substitute.For<HttpClient>();
        _logger = Substitute.For<ILogger<AuthenticationService>>();
        _stateStorage = Substitute.For<IStateStorageService>();
        _tokenProvider = Substitute.For<IGitHubTokenProvider>();
        _apiClient = Substitute.For<IGitHubApiClient>();

        var options = new CmsAdminOptions
        {
            //GitHub = new GitHubAdminOptions
            //{
            //    Owner = "testOwner",
            //    Repository = "testRepo"
            //},
            Authentication = new AuthenticationOptions
            {
                GitHubClientId = "test-client-id",
                GitHubClientSecret = "test-client-secret",
                PersonalAccessToken = "test-pat"
            }
        };

        _options = Options.Create(options);

        //_authService = new AuthenticationService(
        //    _httpClient,
        //    _options,
        //    _logger,
        //    _stateStorage,
        //    _tokenProvider,
        //    _apiClient);

        _stateStorage.IsInitialized.Returns(true);
    }

    [Fact]
    public async Task InitializeAsync_WithSavedToken_RestoresAuthState()
    {
        // Arrange
        _stateStorage.GetStateAsync<string>("github_auth_token").Returns("saved-token");
        _stateStorage.GetStateAsync<string>("github_username").Returns("test-user");

        // Set up API client to return success
        _apiClient.When(x =>
            x.SetAccessToken(Arg.Any<string>()))
            .Do(x => { });

        // Act
        await _authService.InitializeAsync();

        // Assert
        _authService.IsAuthenticated.ShouldBeTrue();
        _authService.Username.ShouldBe("test-user");
        _authService.AccessToken.ShouldBe("saved-token");

        // Verify API client was configured
        _apiClient.Received(1).SetAccessToken("saved-token");
    }

    [Fact]
    public async Task AuthenticateWithGitHubAsync_WithValidCode_SetsAuthState()
    {
        // Arrange
        var code = "test-code";
        var token = "new-token";

        _tokenProvider.ExchangeCodeForTokenAsync(
            code,
            "test-client-id",
            "test-client-secret")
            .Returns(token);

        // Set up API client methods
        _apiClient.When(x =>
            x.SetAccessToken(Arg.Any<string>()))
            .Do(x => { });

        // Act
        var result = await _authService.AuthenticateWithGitHubAsync(code);

        // Assert
        result.ShouldBeTrue();
        _authService.IsAuthenticated.ShouldBeTrue();
        _authService.AccessToken.ShouldBe(token);

        // Verify token was set on API client
        _apiClient.Received(1).SetAccessToken(token);

        // Verify state was saved
        await _stateStorage.Received(1).SaveStateAsync("github_auth_token", token);
    }

    [Fact]
    public async Task SignOutAsync_ClearsAuthState()
    {
        // Arrange - set up initial authenticated state
        await _authService.SetAccessTokenAsync("test-token");
        _authService.IsAuthenticated.ShouldBeTrue();

        // Act
        await _authService.SignOutAsync();

        // Assert
        _authService.IsAuthenticated.ShouldBeFalse();
        _authService.AccessToken.ShouldBeNull();

        // Verify state was removed
        await _stateStorage.Received(1).RemoveStateAsync("github_auth_token");
        await _stateStorage.Received(1).RemoveStateAsync("github_username");
    }
}