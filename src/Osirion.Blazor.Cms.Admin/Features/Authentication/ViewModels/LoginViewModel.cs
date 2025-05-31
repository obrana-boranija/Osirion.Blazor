using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;

public class LoginViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly NavigationManager _navigationManager;
    private readonly CmsEventMediator _eventMediator;
    private readonly IStateStorageService _stateStorage;
    private readonly ILogger<LoginViewModel> _logger;
    private readonly AuthenticationOptions _authOptions;

    public string AccessToken { get; set; } = string.Empty;
    public bool IsLoggingIn { get; private set; }
    public bool IsShowingTokenInput { get; private set; }
    public string? ErrorMessage { get; set; }
    public string ReturnUrl { get; set; } = "/admin";

    public event Action? StateChanged;

    public LoginViewModel(
        IAuthenticationService authService,
        IContentRepositoryAdapter repositoryAdapter,
        NavigationManager navigationManager,
        CmsEventMediator eventMediator,
        IStateStorageService stateStorage,
        IOptions<CmsAdminOptions> options,
        ILogger<LoginViewModel> logger)
    {
        _authService = authService;
        _repositoryAdapter = repositoryAdapter;
        _navigationManager = navigationManager;
        _eventMediator = eventMediator;
        _stateStorage = stateStorage;
        _logger = logger;
        _authOptions = options.Value.Authentication;

        // Initialize storage
        _ = _stateStorage.InitializeAsync();

        // Subscribe to authentication changes
        _authService.AuthenticationChanged += OnAuthenticationChanged;
    }

    public void ToggleTokenInput()
    {
        IsShowingTokenInput = !IsShowingTokenInput;
        NotifyStateChanged();
    }

    public async Task LoginWithGitHubAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;

        IsLoggingIn = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var result = await _authService.AuthenticateWithGitHubAsync(code);

            if (result)
            {
                // Update repository adapter with token
                if (!string.IsNullOrWhiteSpace(_authService.AccessToken))
                {
                    await _repositoryAdapter.SetAccessTokenAsync(_authService.AccessToken);
                }

                // Save auth state in persistent storage
                await _stateStorage.SaveStateAsync("auth_status", true);
                await _stateStorage.SaveStateAsync("last_login_method", "github");

                // Small delay to ensure state is saved
                await Task.Delay(100);

                // The AuthenticationChanged event will be triggered by the service
                _navigationManager.NavigateTo(ReturnUrl);
            }
            else
            {
                ErrorMessage = "Failed to authenticate with GitHub. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication error during GitHub login");
            ErrorMessage = $"Authentication error: {ex.Message}";
        }
        finally
        {
            IsLoggingIn = false;
            NotifyStateChanged();
        }
    }

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

            // Set the token in the auth service
            var result = await _authService.SetAccessTokenAsync(AccessToken);

            if (result)
            {
                _logger.LogInformation("PAT login successful");

                // Update repository adapter with token (important!)
                await _repositoryAdapter.SetAccessTokenAsync(AccessToken);

                // Save auth state in persistent storage - this is crucial
                await _stateStorage.SaveStateAsync("auth_status", true);
                await _stateStorage.SaveStateAsync("last_login_method", "pat");
                await _stateStorage.SaveStateAsync("github_auth_token", AccessToken);

                // Ensure event is published
                _eventMediator.Publish(new AuthenticationChangedEvent(true));

                // Small delay to ensure state is saved before navigation
                await Task.Delay(100);

                // Navigate to the return URL
                _navigationManager.NavigateTo(ReturnUrl);
            }
            else
            {
                ErrorMessage = "Invalid access token. Please check and try again.";
                _logger.LogWarning("PAT login failed - token validation error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication error during PAT login");
            ErrorMessage = $"Authentication error: {ex.Message}";
        }
        finally
        {
            IsLoggingIn = false;
            NotifyStateChanged();
        }
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing login view model");

        // Check if we're already authenticated
        if (_authService.IsAuthenticated)
        {
            _logger.LogInformation("Already authenticated, no need to restore state");
            return;
        }

        // Use PAT from settings if available
        if (!string.IsNullOrWhiteSpace(_authOptions.PersonalAccessToken))
        {
            _logger.LogInformation("Using configured PAT for authentication");
            AccessToken = _authOptions.PersonalAccessToken;
            await LoginWithTokenAsync();
            return;
        }

        // Make sure storage is initialized
        if (!_stateStorage.IsInitialized)
        {
            await _stateStorage.InitializeAsync();
        }

        // Try to restore PAT authentication if previously logged in with PAT
        var authStatus = await _stateStorage.GetStateAsync<bool>("auth_status");
        var lastLoginMethod = await _stateStorage.GetStateAsync<string>("last_login_method");

        _logger.LogInformation("Checking stored auth state: Status={Status}, Method={Method}",
            authStatus, lastLoginMethod ?? "none");

        if (authStatus && lastLoginMethod == "pat")
        {
            var token = await _stateStorage.GetStateAsync<string>("github_auth_token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                _logger.LogInformation("Found stored PAT, attempting login");
                AccessToken = token;
                await LoginWithTokenAsync();
            }
            else
            {
                _logger.LogWarning("Auth status indicated PAT login, but no token found");
            }
        }
    }

    public async Task SignOutAsync()
    {
        await _authService.SignOutAsync();

        // Clear auth state from storage
        await _stateStorage.RemoveStateAsync("auth_status");
        await _stateStorage.RemoveStateAsync("last_login_method");
        await _stateStorage.RemoveStateAsync("github_auth_token");

        // The AuthenticationChanged event will be triggered by the service
        _navigationManager.NavigateTo("/osirion/login");
    }

    private void OnAuthenticationChanged(bool isAuthenticated)
    {
        _logger.LogInformation("Authentication state changed: {IsAuthenticated}", isAuthenticated);

        // Publish an event to the application
        _eventMediator.Publish(new AuthenticationChangedEvent(isAuthenticated));
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}