using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Authentication.ViewModels;

public class LoginViewModelEdgeCaseTests
{
    private const string UserToken = "user-token";

    private readonly IAuthenticationService _authService = Substitute.For<IAuthenticationService>();
    private readonly IContentRepositoryAdapter _repositoryAdapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly IStateStorageService _stateStorage = Substitute.For<IStateStorageService>();
    private readonly IGitHubTokenProvider _tokenProvider = Substitute.For<IGitHubTokenProvider>();
    private readonly IGitHubTokenStore _tokenStore = Substitute.For<IGitHubTokenStore>();
    private readonly InMemoryGitHubOAuthStateStore _oauthStateStore = new();
    private readonly TestNavigationManager _navigationManager = new();
    private readonly CmsEventMediator _mediator = new(NullLogger<CmsEventMediator>.Instance);

    public LoginViewModelEdgeCaseTests()
    {
        _authService.SetAccessTokenAsync(UserToken).Returns(true);
        _tokenStore.SaveTokenAsync(Arg.Any<string>()).Returns(true);
    }

    private LoginViewModel CreateViewModel(IGitHubOAuthStateStore? oauthStateStore = null) =>
        new(
            _authService,
            _repositoryAdapter,
            _navigationManager,
            _mediator,
            _stateStorage,
            Options.Create(new CmsAdminOptions
            {
                Authentication = new AuthenticationOptions { GitHubClientId = "client-id", GitHubClientSecret = "client-secret" }
            }),
            NullLogger<LoginViewModel>.Instance,
            _tokenProvider,
            _tokenStore,
            oauthStateStore ?? _oauthStateStore);

    private void StartPendingLogin() =>
        _oauthStateStore.Pending = new GitHubOAuthRequest("state", "verifier", "https://admin.example/osirion/login", "/osirion/settings");

    [Fact]
    public void Constructor_MissingGitHubCollaborators_Throw()
    {
        var options = Options.Create(new CmsAdminOptions());

        Should.Throw<ArgumentNullException>(() => new LoginViewModel(_authService, _repositoryAdapter, _navigationManager, _mediator, _stateStorage, options, NullLogger<LoginViewModel>.Instance, null!, _tokenStore, _oauthStateStore));
        Should.Throw<ArgumentNullException>(() => new LoginViewModel(_authService, _repositoryAdapter, _navigationManager, _mediator, _stateStorage, options, NullLogger<LoginViewModel>.Instance, _tokenProvider, null!, _oauthStateStore));
        Should.Throw<ArgumentNullException>(() => new LoginViewModel(_authService, _repositoryAdapter, _navigationManager, _mediator, _stateStorage, options, NullLogger<LoginViewModel>.Instance, _tokenProvider, _tokenStore, null!));
    }

    [Fact]
    public void ToggleTokenInput_FlipsVisibilityAndNotifies()
    {
        var viewModel = CreateViewModel();
        var notifications = 0;
        viewModel.StateChanged += () => notifications++;

        viewModel.ToggleTokenInput();
        viewModel.IsShowingTokenInput.ShouldBeTrue();
        viewModel.ToggleTokenInput();

        viewModel.IsShowingTokenInput.ShouldBeFalse();
        notifications.ShouldBe(2);
    }

    [Fact]
    public async Task StartGitHubLoginAsync_StateStoreFails_ShowsTheGenericMessageAndStays()
    {
        var failingStore = Substitute.For<IGitHubOAuthStateStore>();
        failingStore.SaveAsync(Arg.Any<GitHubOAuthRequest>()).ThrowsAsync(new InvalidOperationException("storage detail"));
        var viewModel = CreateViewModel(failingStore);

        await viewModel.StartGitHubLoginAsync();

        viewModel.ErrorMessage.ShouldNotBeNull();
        viewModel.ErrorMessage.ShouldNotContain("storage detail");
        _navigationManager.Navigations.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task LoginWithGitHubAsync_BlankCode_DoesNothing(string code)
    {
        StartPendingLogin();
        var viewModel = CreateViewModel();

        await viewModel.LoginWithGitHubAsync(code, "state");

        _oauthStateStore.Pending.ShouldNotBeNull();
        viewModel.IsLoggingIn.ShouldBeFalse();
    }

    [Fact]
    public async Task LoginWithGitHubAsync_ExchangeReturnsNoToken_ShowsTheFailureMessage()
    {
        StartPendingLogin();
        _tokenProvider.ExchangeCodeForTokenAsync("code", "client-id", "client-secret", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(string.Empty);
        var viewModel = CreateViewModel();

        await viewModel.LoginWithGitHubAsync("code", "state");

        viewModel.ErrorMessage.ShouldBe("Failed to authenticate with GitHub. Please try again.");
        _navigationManager.Navigations.ShouldBeEmpty();
        viewModel.IsLoggingIn.ShouldBeFalse();
    }

    [Fact]
    public async Task LoginWithGitHubAsync_TokenRejected_ShowsTheFailureMessageAndKeepsTheTokenOut()
    {
        StartPendingLogin();
        _tokenProvider.ExchangeCodeForTokenAsync("code", "client-id", "client-secret", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("rejected-token");
        _authService.SetAccessTokenAsync("rejected-token").Returns(false);
        var viewModel = CreateViewModel();

        await viewModel.LoginWithGitHubAsync("code", "state");

        viewModel.ErrorMessage.ShouldBe("Failed to authenticate with GitHub. Please try again.");
        await _authService.Received(1).SetAccessTokenAsync("rejected-token");
        await _tokenStore.DidNotReceiveWithAnyArgs().SaveTokenAsync(default!);
        await _repositoryAdapter.DidNotReceiveWithAnyArgs().SetAccessTokenAsync(default!);
        _navigationManager.Navigations.ShouldBeEmpty();
        viewModel.IsLoggingIn.ShouldBeFalse();
    }

    [Fact]
    public async Task LoginWithGitHubAsync_ExchangeThrows_ShowsTheGenericMessage()
    {
        StartPendingLogin();
        _tokenProvider.ExchangeCodeForTokenAsync("code", "client-id", "client-secret", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("secret response body"));
        var viewModel = CreateViewModel();

        await viewModel.LoginWithGitHubAsync("code", "state");

        viewModel.ErrorMessage.ShouldNotBeNull();
        viewModel.ErrorMessage.ShouldNotContain("secret response body");
        viewModel.IsLoggingIn.ShouldBeFalse();
    }

    [Fact]
    public async Task LoginWithTokenAsync_BlankToken_AsksForAToken()
    {
        var viewModel = CreateViewModel();

        await viewModel.LoginWithTokenAsync();

        viewModel.ErrorMessage.ShouldBe("Please enter an access token.");
        await _authService.DidNotReceiveWithAnyArgs().SetAccessTokenAsync(default!);
    }

    [Fact]
    public async Task LoginWithTokenAsync_TokenCannotBeStored_StillSignsInForTheSession()
    {
        _tokenStore.SaveTokenAsync(UserToken).Returns(false);
        var viewModel = CreateViewModel();
        viewModel.AccessToken = UserToken;

        await viewModel.LoginWithTokenAsync();

        await _repositoryAdapter.Received(1).SetAccessTokenAsync(UserToken);
        _navigationManager.Navigations.ShouldHaveSingleItem();
        viewModel.AccessToken.ShouldBeEmpty();
    }

    [Fact]
    public async Task InitializeAsync_AlreadyAuthenticated_DoesNotReadTheStoredToken()
    {
        _authService.IsAuthenticated.Returns(true);
        var viewModel = CreateViewModel();

        await viewModel.InitializeAsync();

        await _tokenStore.DidNotReceive().GetTokenAsync();
        _navigationManager.Navigations.ShouldBeEmpty();
    }

    [Fact]
    public async Task InitializeAsync_ServiceFails_SwallowsTheFailureAndStopsLoggingIn()
    {
        _authService.InitializeAsync().ThrowsAsync(new InvalidOperationException("boom"));
        var viewModel = CreateViewModel();

        await viewModel.InitializeAsync();

        viewModel.IsLoggingIn.ShouldBeFalse();
        viewModel.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public void AuthenticationChanged_PublishesTheNewStateToTheMediator()
    {
        var published = EventRecorder.Record<AuthenticationChangedEvent>(_mediator);
        CreateViewModel();

        _authService.AuthenticationChanged += Raise.Event<Action<bool>>(true);

        published.ShouldHaveSingleItem().IsAuthenticated.ShouldBeTrue();
    }
}
