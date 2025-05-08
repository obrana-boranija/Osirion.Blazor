using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;

public class LoginViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly NavigationManager _navigationManager;
    private readonly CmsEventMediator _eventMediator;

    public string AccessToken { get; set; } = string.Empty;
    public bool IsLoggingIn { get; private set; }
    public bool IsShowingTokenInput { get; private set; }
    public string? ErrorMessage { get; set; }
    public string ReturnUrl { get; set; } = "/admin";

    public event Action? StateChanged;

    public LoginViewModel(
        IAuthenticationService authService,
        NavigationManager navigationManager,
        CmsEventMediator eventMediator)
    {
        _authService = authService;
        _navigationManager = navigationManager;
        _eventMediator = eventMediator;

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
            var result = await _authService.SetAccessTokenAsync(AccessToken);

            if (result)
            {
                // The AuthenticationChanged event will be triggered by the service
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

    public async Task SignOutAsync()
    {
        await _authService.SignOutAsync();
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