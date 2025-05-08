using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;

public class LoginViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly NavigationManager _navigationManager;
    private readonly CmsEventMediator _eventMediator;
    private readonly IStateStorageService _stateStorage;

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
        IStateStorageService stateStorage)
    {
        _authService = authService;
        _repositoryAdapter = repositoryAdapter;
        _navigationManager = navigationManager;
        _eventMediator = eventMediator;
        _stateStorage = stateStorage;

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
        if (string.IsNullOrEmpty(code))
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
                if (!string.IsNullOrEmpty(_authService.AccessToken))
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
        if (string.IsNullOrEmpty(AccessToken))
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
            // Set the token in the auth service
            var result = await _authService.SetAccessTokenAsync(AccessToken);

            if (result)
            {
                // Update repository adapter with token (important!)
                await _repositoryAdapter.SetAccessTokenAsync(AccessToken);

                // Save auth state in persistent storage
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
            }
        }
        catch (Exception ex)
        {
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
        // Check if we're already authenticated
        if (_authService.IsAuthenticated)
        {
            return;
        }

        // Try to restore PAT authentication if previously logged in with PAT
        var lastLoginMethod = await _stateStorage.GetStateAsync<string>("last_login_method");

        if (lastLoginMethod == "pat")
        {
            var token = await _stateStorage.GetStateAsync<string>("github_auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                AccessToken = token;
                await LoginWithTokenAsync();
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
        _navigationManager.NavigateTo("/admin/login");
    }

    private void OnAuthenticationChanged(bool isAuthenticated)
    {
        // Publish an event to the application
        _eventMediator.Publish(new AuthenticationChangedEvent(isAuthenticated));
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}