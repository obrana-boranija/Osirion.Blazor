using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Common.Extensions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Core.Utils;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;

/// <summary>Coordinates CMS authentication and login state.</summary>
/// <remarks>
/// The view model signs a user in with the user's own GitHub credential only: a personal access token the user
/// types, or a token obtained through GitHub OAuth. The personal access token configured for the server is never
/// copied here; admin services use it on the server when no user is signed in. Every navigation to
/// <see cref="ReturnUrl"/> is limited to a local path.
/// </remarks>
public class LoginViewModel
{
    private const string UnexpectedErrorMessage = "Authentication failed because of an unexpected error. Please try again.";

    private readonly IAuthenticationService _authService;
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly NavigationManager _navigationManager;
    private readonly CmsEventMediator _eventMediator;
    private readonly IStateStorageService _stateStorage;
    private readonly ILogger<LoginViewModel> _logger;
    private readonly AuthenticationOptions _authOptions;
    private readonly IGitHubTokenProvider _tokenProvider;
    private readonly IGitHubTokenStore _tokenStore;
    private readonly IGitHubOAuthStateStore _oauthStateStore;

    /// <summary>Gets or sets the personal access token the user typed. It is cleared after a successful login.</summary>
    public string AccessToken { get; set; } = string.Empty;
    /// <summary>Gets whether a login operation is in progress.</summary>
    public bool IsLoggingIn { get; private set; }
    /// <summary>Gets whether the token input is visible.</summary>
    public bool IsShowingTokenInput { get; private set; }
    /// <summary>Gets or sets the current error message.</summary>
    public string? ErrorMessage { get; set; }
    /// <summary>Gets or sets the URL to navigate to after login. Only a local path is ever navigated to.</summary>
    public string ReturnUrl { get; set; } = "/admin";

    /// <summary>
    /// Gets whether a personal access token is configured for the server. Admin services use it when no user is
    /// signed in with GitHub; it never reaches the browser.
    /// </summary>
    public bool IsServerCredentialConfigured => !string.IsNullOrWhiteSpace(_authOptions.PersonalAccessToken);

    /// <summary>Gets whether GitHub OAuth sign-in is configured with a client ID and a client secret.</summary>
    public bool IsGitHubOAuthConfigured =>
        !string.IsNullOrWhiteSpace(_authOptions.GitHubClientId) &&
        !string.IsNullOrWhiteSpace(_authOptions.GitHubClientSecret);

    /// <summary>
    /// Gets the URI GitHub sends the browser back to after authorization: the configured
    /// <see cref="AuthenticationOptions.GitHubRedirectUri"/>, or the admin login page of this site.
    /// </summary>
    public string GitHubRedirectUri => string.IsNullOrWhiteSpace(_authOptions.GitHubRedirectUri)
        ? _navigationManager.BaseUri + CmsConstants.Routes.Login.TrimStart('/')
        : _authOptions.GitHubRedirectUri;

    /// <summary>Occurs when the view-model state changes.</summary>
    public event Action? StateChanged;

    /// <summary>Initializes a new login view-model.</summary>
    /// <param name="authService">The authentication service of the current scope</param>
    /// <param name="repositoryAdapter">The repository adapter that receives the user's token</param>
    /// <param name="navigationManager">The navigation manager</param>
    /// <param name="eventMediator">The mediator that announces authentication changes</param>
    /// <param name="stateStorage">The plain-text browser storage, prepared here for the authentication service</param>
    /// <param name="options">The CMS admin options</param>
    /// <param name="logger">The logger instance</param>
    /// <param name="tokenProvider">The provider that exchanges GitHub OAuth codes for tokens</param>
    /// <param name="tokenStore">The store that keeps the user's token between page loads</param>
    /// <param name="oauthStateStore">The store that keeps a started GitHub login until GitHub sends the user back</param>
    public LoginViewModel(
        IAuthenticationService authService,
        IContentRepositoryAdapter repositoryAdapter,
        NavigationManager navigationManager,
        CmsEventMediator eventMediator,
        IStateStorageService stateStorage,
        IOptions<CmsAdminOptions> options,
        ILogger<LoginViewModel> logger,
        IGitHubTokenProvider tokenProvider,
        IGitHubTokenStore tokenStore,
        IGitHubOAuthStateStore oauthStateStore)
    {
        _authService = authService;
        _repositoryAdapter = repositoryAdapter;
        _navigationManager = navigationManager;
        _eventMediator = eventMediator;
        _stateStorage = stateStorage;
        _logger = logger;
        _authOptions = options.Value.Authentication;
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        _oauthStateStore = oauthStateStore ?? throw new ArgumentNullException(nameof(oauthStateStore));

        // Initialize storage
        _stateStorage.InitializeAsync().Observe(_logger, "state storage initialization");

        // Subscribe to authentication changes
        _authService.AuthenticationChanged += OnAuthenticationChanged;
    }

    /// <summary>Toggles visibility of the token input.</summary>
    public void ToggleTokenInput()
    {
        IsShowingTokenInput = !IsShowingTokenInput;
        NotifyStateChanged();
    }

    /// <summary>
    /// Starts a GitHub OAuth login: stores a single-use state value and a PKCE code verifier for this browser
    /// tab, then sends the browser to GitHub.
    /// </summary>
    /// <remarks>Call it only during an interactive render, because the pending login is kept in the browser.</remarks>
    public async Task StartGitHubLoginAsync()
    {
        if (!IsGitHubOAuthConfigured)
        {
            ErrorMessage = "GitHub OAuth login requires additional configuration.";
            NotifyStateChanged();
            return;
        }

        try
        {
            var request = new GitHubOAuthRequest(
                State: GitHubOAuth.CreateRandomValue(),
                CodeVerifier: GitHubOAuth.CreateRandomValue(),
                RedirectUri: GitHubRedirectUri,
                ReturnUrl: GetSafeReturnUrl(ReturnUrl));

            await _oauthStateStore.SaveAsync(request);

            _navigationManager.NavigateTo(GitHubOAuth.BuildAuthorizeUrl(_authOptions.GitHubClientId, request), forceLoad: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not start the GitHub login");
            ErrorMessage = UnexpectedErrorMessage;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Completes a GitHub OAuth login after GitHub sent the browser back with an authorization code.
    /// </summary>
    /// <param name="code">The authorization code from the <c>code</c> query parameter</param>
    /// <param name="state">The value of the <c>state</c> query parameter</param>
    /// <param name="cancellationToken">A token that cancels the code exchange</param>
    /// <remarks>
    /// The code is exchanged only when <paramref name="state"/> equals the value stored by
    /// <see cref="StartGitHubLoginAsync"/> in this browser tab. That value is removed on the first attempt, so a
    /// replayed or forged callback never reaches GitHub. Call it only during an interactive render.
    /// </remarks>
    public async Task LoginWithGitHubAsync(string code, string? state, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;

        IsLoggingIn = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var pendingRequest = await _oauthStateStore.TakeAsync();

            if (pendingRequest is null || !GitHubOAuth.StatesMatch(pendingRequest.State, state))
            {
                _logger.LogWarning("GitHub login rejected because its state does not match a login started in this browser tab");
                ErrorMessage = "The GitHub login could not be verified. Please try again.";
                return;
            }

            var token = await _tokenProvider.ExchangeCodeForTokenAsync(
                code,
                _authOptions.GitHubClientId,
                _authOptions.GitHubClientSecret,
                pendingRequest.RedirectUri,
                pendingRequest.CodeVerifier,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(token) || !await SignInAsync(token))
            {
                ErrorMessage = "Failed to authenticate with GitHub. Please try again.";
                return;
            }

            NavigateToReturnUrl(pendingRequest.ReturnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication error during GitHub login");
            ErrorMessage = UnexpectedErrorMessage;
        }
        finally
        {
            IsLoggingIn = false;
            NotifyStateChanged();
        }
    }

    /// <summary>Rejects the authorization code, because a code without its OAuth state cannot be verified.</summary>
    /// <param name="code">The authorization code</param>
    [Obsolete("A GitHub login cannot be verified without its OAuth state. Use LoginWithGitHubAsync(code, state).")]
    public Task LoginWithGitHubAsync(string code)
    {
        return LoginWithGitHubAsync(code, state: null);
    }

    /// <summary>Authenticates with the personal access token the user entered in <see cref="AccessToken"/>.</summary>
    public async Task LoginWithTokenAsync()
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            ErrorMessage = "Please enter an access token.";
            NotifyStateChanged();
            return;
        }

        IsLoggingIn = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            _logger.LogInformation("Attempting login with PAT");

            if (await SignInAsync(AccessToken))
            {
                _logger.LogInformation("PAT login successful");
                AccessToken = string.Empty;
                NavigateToReturnUrl(ReturnUrl);
            }
            else
            {
                ErrorMessage = "Invalid access token. Please check and try again.";
                _logger.LogWarning("PAT login failed because the token could not be validated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication error during PAT login");
            ErrorMessage = UnexpectedErrorMessage;
        }
        finally
        {
            IsLoggingIn = false;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Restores the GitHub login kept by <see cref="IGitHubTokenStore"/> and removes any token an earlier version
    /// left in plain-text browser storage.
    /// </summary>
    /// <remarks>
    /// Call it only during an interactive render, because both stores use the browser. A stored token that GitHub
    /// no longer accepts is removed.
    /// </remarks>
    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing login view model");

        try
        {
            // Removes any token an earlier version left in plain-text storage; the service owns that list of keys
            await _authService.InitializeAsync();

            if (_authService.IsAuthenticated)
            {
                _logger.LogInformation("Already authenticated, no need to restore state");
                return;
            }

            var token = await _tokenStore.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            _logger.LogInformation("Found a stored GitHub token, attempting login");
            IsLoggingIn = true;
            NotifyStateChanged();

            if (await SignInAsync(token))
            {
                NavigateToReturnUrl(ReturnUrl);
            }
            else
            {
                _logger.LogWarning("The stored GitHub token is no longer valid and was removed");
                await _tokenStore.RemoveTokenAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not restore the GitHub login");
        }
        finally
        {
            IsLoggingIn = false;
            NotifyStateChanged();
        }
    }

    /// <summary>Signs out and clears persisted authentication state.</summary>
    public async Task SignOutAsync()
    {
        // The service also removes the authentication state, and any legacy token, from plain-text storage
        await _authService.SignOutAsync();
        await _tokenStore.RemoveTokenAsync();

        // The AuthenticationChanged event will be triggered by the service
        _navigationManager.NavigateTo(CmsConstants.Routes.Login);
    }

    /// <summary>
    /// Validates <paramref name="token"/>, hands it to the admin services of this scope and keeps it for the next
    /// page load.
    /// </summary>
    private async Task<bool> SignInAsync(string token)
    {
        if (!await _authService.SetAccessTokenAsync(token))
        {
            return false;
        }

        await _repositoryAdapter.SetAccessTokenAsync(token);

        if (!await _tokenStore.SaveTokenAsync(token))
        {
            _logger.LogInformation("The GitHub login lasts for this session only");
        }

        return true;
    }

    private void NavigateToReturnUrl(string? returnUrl)
    {
        _navigationManager.NavigateTo(GetSafeReturnUrl(returnUrl));
    }

    private static string GetSafeReturnUrl(string? returnUrl)
    {
        return LocalUrl.GetSafeLocalPath(returnUrl, CmsConstants.Routes.Dashboard);
    }

    private void OnAuthenticationChanged(bool isAuthenticated)
    {
        _logger.LogInformation("Authentication state changed: {IsAuthenticated}", isAuthenticated);

        // Publish an event to the application
        _eventMediator.Publish(new AuthenticationChangedEvent(isAuthenticated));
    }

    /// <summary>Performs the NotifyStateChanged operation.</summary>
    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
