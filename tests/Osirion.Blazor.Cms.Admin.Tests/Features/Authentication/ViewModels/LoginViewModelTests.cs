using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Authentication.ViewModels;

public class LoginViewModelTests
{
    private const string UserToken = "user-token";
    private const string ServerToken = "server-pat-value";
    private const string ClientId = "client-id";
    private const string ClientSecret = "client-secret";
    private const string DashboardUri = TestNavigationManager.SiteBaseUri + "osirion";

    private readonly IAuthenticationService _authService = Substitute.For<IAuthenticationService>();
    private readonly IContentRepositoryAdapter _repositoryAdapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly IStateStorageService _stateStorage = Substitute.For<IStateStorageService>();
    private readonly IGitHubTokenProvider _tokenProvider = Substitute.For<IGitHubTokenProvider>();
    private readonly IGitHubTokenStore _tokenStore = Substitute.For<IGitHubTokenStore>();
    private readonly InMemoryGitHubOAuthStateStore _oauthStateStore = new();
    private readonly TestNavigationManager _navigationManager = new();
    private readonly AuthenticationOptions _authOptions = new()
    {
        GitHubClientId = ClientId,
        GitHubClientSecret = ClientSecret
    };

    public LoginViewModelTests()
    {
        _stateStorage.IsInitialized.Returns(true);
        _authService.SetAccessTokenAsync(UserToken).Returns(true);
        _tokenStore.SaveTokenAsync(Arg.Any<string>()).Returns(true);
    }

    [Theory]
    [InlineData("//evil.example")]
    [InlineData("/\\evil.example")]
    [InlineData("https://evil.example/osirion")]
    [InlineData("javascript:alert(1)")]
    [InlineData("osirion")]
    public async Task LoginWithTokenAsync_WithReturnUrlOffSite_NavigatesToDashboard(string returnUrl)
    {
        // Proves an open redirect through returnUrl is closed right before navigation, whatever the page passed in.
        var viewModel = CreateViewModel();
        viewModel.ReturnUrl = returnUrl;
        viewModel.AccessToken = UserToken;

        await viewModel.LoginWithTokenAsync();

        _navigationManager.Navigations.ShouldHaveSingleItem().Uri.ShouldBe("/osirion");
        _navigationManager.Uri.ShouldBe(DashboardUri);
    }

    [Fact]
    public async Task LoginWithTokenAsync_WithLocalReturnUrl_NavigatesToIt()
    {
        var viewModel = CreateViewModel();
        viewModel.ReturnUrl = "/osirion/content/edit?path=a.md";
        viewModel.AccessToken = UserToken;

        await viewModel.LoginWithTokenAsync();

        _navigationManager.Navigations.ShouldHaveSingleItem().Uri.ShouldBe("/osirion/content/edit?path=a.md");
    }

    [Fact]
    public async Task LoginWithTokenAsync_Success_StoresTokenOnlyInProtectedStore()
    {
        // Proves the user's token is no longer written to plain-text browser storage.
        var viewModel = CreateViewModel();
        viewModel.AccessToken = UserToken;

        await viewModel.LoginWithTokenAsync();

        await _tokenStore.Received(1).SaveTokenAsync(UserToken);
        await _stateStorage.DidNotReceiveWithAnyArgs().SaveStateAsync<string>(default!, default!);
        await _repositoryAdapter.Received(1).SetAccessTokenAsync(UserToken);
        viewModel.AccessToken.ShouldBeEmpty();
    }

    [Fact]
    public async Task LoginWithTokenAsync_WhenTokenIsRejected_StoresNothingAndShowsError()
    {
        var viewModel = CreateViewModel();
        viewModel.AccessToken = "wrong-token";

        await viewModel.LoginWithTokenAsync();

        await _tokenStore.DidNotReceiveWithAnyArgs().SaveTokenAsync(default!);
        _navigationManager.Navigations.ShouldBeEmpty();
        viewModel.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LoginWithTokenAsync_WhenServiceThrows_ShowsMessageWithoutExceptionDetail()
    {
        // Proves exception text no longer reaches the page.
        _authService.SetAccessTokenAsync(UserToken).ThrowsAsync(new InvalidOperationException("internal-detail"));
        var viewModel = CreateViewModel();
        viewModel.AccessToken = UserToken;

        await viewModel.LoginWithTokenAsync();

        viewModel.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
        viewModel.ErrorMessage.ShouldNotContain("internal-detail");
    }

    [Fact]
    public async Task InitializeAsync_WithConfiguredServerToken_DoesNotSignInOrCopyIt()
    {
        // Proves the server personal access token no longer signs in every visitor of the login page.
        _authOptions.PersonalAccessToken = ServerToken;
        var viewModel = CreateViewModel();

        await viewModel.InitializeAsync();

        await _authService.DidNotReceiveWithAnyArgs().SetAccessTokenAsync(default!);
        viewModel.AccessToken.ShouldBeEmpty();
        viewModel.IsServerCredentialConfigured.ShouldBeTrue();
        _navigationManager.Navigations.ShouldBeEmpty();
    }

    [Fact]
    public async Task InitializeAsync_RemovesLegacyPlainTextTokenWithoutReadingIt()
    {
        // Proves a token an earlier version left in plain-text storage is purged and never used. The real
        // authentication service does the purge, so the list of legacy keys lives in one place.
        _stateStorage.GetStateAsync<string>("github_auth_token").Returns(UserToken);
        _stateStorage.GetStateAsync<bool>("auth_status").Returns(true);
        _stateStorage.GetStateAsync<string>("last_login_method").Returns("pat");
        var authService = new AuthenticationService(
            new HttpClient(),
            Options.Create(new CmsAdminOptions { Authentication = _authOptions }),
            NullLogger<AuthenticationService>.Instance,
            _stateStorage,
            _tokenProvider,
            Substitute.For<IGitHubApiClientFactory>());
        var viewModel = CreateViewModel(authService);

        await viewModel.InitializeAsync();

        await _stateStorage.Received(1).RemoveStateAsync("github_auth_token");
        await _stateStorage.DidNotReceive().GetStateAsync<string>("github_auth_token");
        authService.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task InitializeAsync_LeavesLegacyTokenPurgeToAuthenticationService()
    {
        // Fails before the fix, when the view model kept its own copy of the legacy key and removed it itself.
        var viewModel = CreateViewModel();

        await viewModel.InitializeAsync();

        await _authService.Received(1).InitializeAsync();
        await _stateStorage.DidNotReceive().RemoveStateAsync("github_auth_token");
    }

    [Fact]
    public async Task InitializeAsync_WithStoredToken_SignsInAndNavigatesToSafeReturnUrl()
    {
        _tokenStore.GetTokenAsync().Returns(UserToken);
        var viewModel = CreateViewModel();
        viewModel.ReturnUrl = "//evil.example";

        await viewModel.InitializeAsync();

        await _authService.Received(1).SetAccessTokenAsync(UserToken);
        await _repositoryAdapter.Received(1).SetAccessTokenAsync(UserToken);
        _navigationManager.Navigations.ShouldHaveSingleItem().Uri.ShouldBe("/osirion");
    }

    [Fact]
    public async Task InitializeAsync_WithStoredTokenGitHubRejects_RemovesStoredToken()
    {
        _tokenStore.GetTokenAsync().Returns("revoked-token");
        var viewModel = CreateViewModel();

        await viewModel.InitializeAsync();

        await _tokenStore.Received(1).RemoveTokenAsync();
        _navigationManager.Navigations.ShouldBeEmpty();
    }

    [Fact]
    public async Task StartGitHubLoginAsync_SendsStateAndS256ChallengeToGitHub()
    {
        // Proves the authorize request carries a fresh state and a PKCE challenge of the stored verifier.
        var viewModel = CreateViewModel();
        viewModel.ReturnUrl = "//evil.example";

        await viewModel.StartGitHubLoginAsync();

        var pending = _oauthStateStore.Pending.ShouldNotBeNull();
        pending.State.Length.ShouldBe(43);
        pending.CodeVerifier.Length.ShouldBe(43);
        pending.State.ShouldNotBe(pending.CodeVerifier);
        pending.RedirectUri.ShouldBe(TestNavigationManager.SiteBaseUri + "osirion/login");
        pending.ReturnUrl.ShouldBe("/osirion");

        var navigation = _navigationManager.Navigations.ShouldHaveSingleItem();
        navigation.ForceLoad.ShouldBeTrue();
        navigation.Uri.ShouldStartWith("https://github.com/login/oauth/authorize?");
        navigation.Uri.ShouldContain($"client_id={ClientId}");
        navigation.Uri.ShouldContain($"redirect_uri={Uri.EscapeDataString(pending.RedirectUri)}");
        navigation.Uri.ShouldContain("scope=repo");
        navigation.Uri.ShouldContain($"state={pending.State}");
        navigation.Uri.ShouldContain($"code_challenge={GitHubOAuth.CreateCodeChallenge(pending.CodeVerifier)}");
        navigation.Uri.ShouldContain("code_challenge_method=S256");
    }

    [Fact]
    public async Task StartGitHubLoginAsync_WithConfiguredRedirectUri_UsesIt()
    {
        _authOptions.GitHubRedirectUri = "https://cms.example/osirion/login";
        var viewModel = CreateViewModel();

        await viewModel.StartGitHubLoginAsync();

        _oauthStateStore.Pending.ShouldNotBeNull().RedirectUri.ShouldBe("https://cms.example/osirion/login");
    }

    [Fact]
    public async Task StartGitHubLoginAsync_WithoutClientCredentials_DoesNotLeaveSite()
    {
        _authOptions.GitHubClientSecret = string.Empty;
        var viewModel = CreateViewModel();

        await viewModel.StartGitHubLoginAsync();

        _oauthStateStore.Pending.ShouldBeNull();
        _navigationManager.Navigations.ShouldBeEmpty();
        viewModel.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LoginWithGitHubAsync_WithoutPendingLogin_DoesNotExchangeCode()
    {
        // Proves login CSRF: a code planted in the callback URL is not exchanged unless this tab started a login.
        var viewModel = CreateViewModel();

        await viewModel.LoginWithGitHubAsync("attacker-code", "attacker-state");

        await AssertNoCodeExchangeAsync();
        viewModel.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
        _navigationManager.Navigations.ShouldBeEmpty();
    }

    [Fact]
    public async Task LoginWithGitHubAsync_WithMismatchedState_DoesNotExchangeCode()
    {
        var viewModel = CreateViewModel();
        _oauthStateStore.Pending = CreatePendingLogin();

        await viewModel.LoginWithGitHubAsync("code", "other-state");

        await AssertNoCodeExchangeAsync();
        viewModel.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LoginWithGitHubAsync_WithMatchingState_ExchangesCodeWithVerifierAndRedirectUri()
    {
        var pending = CreatePendingLogin();
        _oauthStateStore.Pending = pending;
        _tokenProvider
            .ExchangeCodeForTokenAsync("code", ClientId, ClientSecret, pending.RedirectUri, pending.CodeVerifier, Arg.Any<CancellationToken>())
            .Returns(UserToken);
        var viewModel = CreateViewModel();

        await viewModel.LoginWithGitHubAsync("code", pending.State);

        await _authService.Received(1).SetAccessTokenAsync(UserToken);
        await _tokenStore.Received(1).SaveTokenAsync(UserToken);
        _navigationManager.Navigations.ShouldHaveSingleItem().Uri.ShouldBe("/osirion/settings");
        viewModel.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task LoginWithGitHubAsync_ReplayedCallback_DoesNotExchangeCodeAgain()
    {
        // Proves the state is single use: replaying the callback URL never reaches GitHub a second time.
        var pending = CreatePendingLogin();
        _oauthStateStore.Pending = pending;
        _tokenProvider
            .ExchangeCodeForTokenAsync(default!, default!, default!, default, default, default)
            .ReturnsForAnyArgs(UserToken);
        var viewModel = CreateViewModel();
        await viewModel.LoginWithGitHubAsync("code", pending.State);

        await viewModel.LoginWithGitHubAsync("code", pending.State);

        await _tokenProvider.ReceivedWithAnyArgs(1)
            .ExchangeCodeForTokenAsync(default!, default!, default!, default, default, default);
        viewModel.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LoginWithGitHubAsync_WithCodeOnly_DoesNotExchangeCode()
    {
        // Proves the older single-argument entry point can no longer exchange an unverified code.
        _oauthStateStore.Pending = CreatePendingLogin();
        var viewModel = CreateViewModel();

#pragma warning disable CS0618 // The obsolete overload is the behaviour under test
        await viewModel.LoginWithGitHubAsync("code");
#pragma warning restore CS0618

        await AssertNoCodeExchangeAsync();
    }

    [Fact]
    public async Task SignOutAsync_RemovesStoredTokenAndNavigatesToLogin()
    {
        var viewModel = CreateViewModel();

        await viewModel.SignOutAsync();

        await _authService.Received(1).SignOutAsync();
        await _tokenStore.Received(1).RemoveTokenAsync();
        _navigationManager.Navigations.ShouldHaveSingleItem().Uri.ShouldBe("/osirion/login");
    }

    [Fact]
    public async Task Constructor_StateStorageInitializationFails_LogsTheFailure()
    {
        // The initialization is started without being awaited; its exception must still reach the log.
        var logger = new RecordingLogger<LoginViewModel>();
        _stateStorage.InitializeAsync().Returns(Task.FromException(new InvalidOperationException("storage unavailable")));

        _ = new LoginViewModel(
            _authService,
            _repositoryAdapter,
            _navigationManager,
            new CmsEventMediator(NullLogger<CmsEventMediator>.Instance),
            _stateStorage,
            Options.Create(new CmsAdminOptions { Authentication = _authOptions }),
            logger,
            _tokenProvider,
            _tokenStore,
            _oauthStateStore);

        await TestWait.UntilAsync(() => logger.Entries.Any(entry => entry.Exception is InvalidOperationException));
    }

    private static GitHubOAuthRequest CreatePendingLogin()
    {
        return new GitHubOAuthRequest(
            State: GitHubOAuth.CreateRandomValue(),
            CodeVerifier: GitHubOAuth.CreateRandomValue(),
            RedirectUri: TestNavigationManager.SiteBaseUri + "osirion/login",
            ReturnUrl: "/osirion/settings");
    }

    private async Task AssertNoCodeExchangeAsync()
    {
        await _tokenProvider.DidNotReceiveWithAnyArgs()
            .ExchangeCodeForTokenAsync(default!, default!, default!, default, default, default);
        await _tokenProvider.DidNotReceiveWithAnyArgs()
            .ExchangeCodeForTokenAsync(default!, default!, default!);
        await _authService.DidNotReceiveWithAnyArgs().SetAccessTokenAsync(default!);
    }

    private LoginViewModel CreateViewModel(IAuthenticationService? authService = null)
    {
        return new LoginViewModel(
            authService ?? _authService,
            _repositoryAdapter,
            _navigationManager,
            new CmsEventMediator(NullLogger<CmsEventMediator>.Instance),
            _stateStorage,
            Options.Create(new CmsAdminOptions { Authentication = _authOptions }),
            NullLogger<LoginViewModel>.Instance,
            _tokenProvider,
            _tokenStore,
            _oauthStateStore);
    }
}
