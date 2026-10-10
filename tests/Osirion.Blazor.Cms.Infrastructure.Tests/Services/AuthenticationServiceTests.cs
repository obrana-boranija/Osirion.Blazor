using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class AuthenticationServiceTests
{
    private const string UserToken = "user-token";
    private const string RejectedToken = "rejected-token";
    private const string LegacyTokenKey = "github_auth_token";

    // GitHub answers the user lookup for every token except the rejected one
    private readonly RecordingHttpMessageHandler _userLookupHandler = new(request =>
        request.Headers.Authorization?.Parameter == RejectedToken ? "{}" : """{"login":"test-user"}""");
    private readonly IStateStorageService _stateStorage = Substitute.For<IStateStorageService>();
    private readonly IGitHubTokenProvider _tokenProvider = Substitute.For<IGitHubTokenProvider>();
    private readonly IGitHubApiClientFactory _apiClientFactory = Substitute.For<IGitHubApiClientFactory>();

    public AuthenticationServiceTests()
    {
        _stateStorage.IsInitialized.Returns(true);
    }

    [Fact]
    public async Task SetAccessTokenAsync_NeverUsesGitHubApiClientFactory()
    {
        // Proves a user's token is no longer pushed onto a factory client, through which it reached the public
        // site and other users.
        var service = CreateService();

        var result = await service.SetAccessTokenAsync(UserToken);

        result.ShouldBeTrue();
        _apiClientFactory.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public async Task AuthenticateWithGitHubAsync_NeverUsesGitHubApiClientFactory()
    {
        // Proves a token obtained through OAuth is no longer pushed onto a factory client.
        _tokenProvider.ExchangeCodeForTokenAsync("code", "client-id", "client-secret").Returns(UserToken);
        var service = CreateService();

        var result = await service.AuthenticateWithGitHubAsync("code");

        result.ShouldBeTrue();
        _apiClientFactory.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public async Task InitializeAsync_WithStoredToken_NeverUsesGitHubApiClientFactory()
    {
        // Proves initializing no longer pushes a stored token onto a factory client.
        _stateStorage.GetStateAsync<string>(LegacyTokenKey).Returns(UserToken);
        var service = CreateService();

        await service.InitializeAsync();

        _apiClientFactory.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public void Constructor_WithoutConfiguredGitHubProvider_Succeeds()
    {
        var factoryWithoutProviders = GitHubTestServices.CreateFactory(new RecordingHttpMessageHandler(), new ConfigurationBuilder().Build());

        Should.NotThrow(() => CreateService(factoryWithoutProviders));
    }

    [Fact]
    public async Task SignOutAsync_ClearsTokenAndAnnouncesSignOut()
    {
        var service = CreateService();
        await service.SetAccessTokenAsync(UserToken);
        bool? announced = null;
        service.AuthenticationChanged += isAuthenticated => announced = isAuthenticated;

        await service.SignOutAsync();

        service.IsAuthenticated.ShouldBeFalse();
        service.AccessToken.ShouldBeNull();
        announced.ShouldBe(false);
    }

    [Fact]
    public async Task SetAccessTokenAsync_WithRejectedTokenWhileSignedIn_KeepsCurrentSignIn()
    {
        // Proves a mistyped token no longer signs the user out here while the admin services of the same scope,
        // which only follow the AuthenticationChanged event, keep acting with the previous token.
        var service = CreateService();
        await service.SetAccessTokenAsync(UserToken);
        var announcements = new List<bool>();
        service.AuthenticationChanged += announcements.Add;

        var result = await service.SetAccessTokenAsync(RejectedToken);

        result.ShouldBeFalse();
        service.IsAuthenticated.ShouldBeTrue();
        service.AccessToken.ShouldBe(UserToken);
        announcements.ShouldBeEmpty();
    }

    [Fact]
    public async Task AuthenticateWithGitHubAsync_WithFailedExchangeWhileSignedIn_KeepsCurrentSignIn()
    {
        // Proves a failed OAuth exchange no longer drops the signed-in token from this service alone.
        var service = CreateService();
        await service.SetAccessTokenAsync(UserToken);

        var result = await service.AuthenticateWithGitHubAsync("rejected-code");

        result.ShouldBeFalse();
        service.IsAuthenticated.ShouldBeTrue();
        service.AccessToken.ShouldBe(UserToken);
    }

    [Fact]
    public async Task InitializeAsync_WithConfiguredPersonalAccessToken_DoesNotSignIn()
    {
        // Proves the server personal access token no longer signs anyone in through the authentication service.
        var service = CreateService(personalAccessToken: "server-pat");

        await service.InitializeAsync();

        service.IsAuthenticated.ShouldBeFalse();
        service.AccessToken.ShouldBeNull();
        _userLookupHandler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task InitializeAsync_WithLegacyPlainTextToken_RemovesItWithoutUsingIt()
    {
        // Proves a token an earlier version left in plain-text storage is purged instead of restored.
        _stateStorage.GetStateAsync<string>(LegacyTokenKey).Returns(UserToken);
        _stateStorage.GetStateAsync<bool>("auth_status").Returns(true);
        var service = CreateService();

        await service.InitializeAsync();

        await _stateStorage.Received(1).RemoveStateAsync(LegacyTokenKey);
        await _stateStorage.DidNotReceive().GetStateAsync<string>(LegacyTokenKey);
        service.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task SetAccessTokenAsync_NeverWritesTokenToPlainTextStorage()
    {
        // Proves a user's token is no longer persisted in plain text.
        var service = CreateService();

        var result = await service.SetAccessTokenAsync(UserToken);

        result.ShouldBeTrue();
        await _stateStorage.DidNotReceive().SaveStateAsync(LegacyTokenKey, Arg.Any<string?>());
        await _stateStorage.DidNotReceive().SaveStateAsync(Arg.Any<string>(), UserToken);
    }

    [Fact]
    public async Task AuthenticateWithGitHubAsync_NeverWritesTokenToPlainTextStorage()
    {
        _tokenProvider.ExchangeCodeForTokenAsync("code", "client-id", "client-secret").Returns(UserToken);
        var service = CreateService();

        var result = await service.AuthenticateWithGitHubAsync("code");

        result.ShouldBeTrue();
        await _stateStorage.DidNotReceive().SaveStateAsync(LegacyTokenKey, Arg.Any<string?>());
        await _stateStorage.DidNotReceive().SaveStateAsync(Arg.Any<string>(), UserToken);
    }

    private AuthenticationService CreateService(IGitHubApiClientFactory? factory = null, string? personalAccessToken = null)
    {
        var options = new CmsAdminOptions
        {
            Authentication = new AuthenticationOptions
            {
                GitHubClientId = "client-id",
                GitHubClientSecret = "client-secret",
                PersonalAccessToken = personalAccessToken
            }
        };

        return new AuthenticationService(
            new HttpClient(_userLookupHandler),
            Options.Create(options),
            NullLogger<AuthenticationService>.Instance,
            _stateStorage,
            _tokenProvider,
            factory ?? _apiClientFactory);
    }
}
