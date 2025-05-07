using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;

public class LoginViewModel
{
    private readonly AuthenticationService _authService;
    private readonly NavigationManager _navigationManager;
    private readonly CmsEventMediator _eventMediator;

    public string AccessToken { get; set; } = string.Empty;
    public bool IsLoggingIn { get; private set; }
    public bool IsShowingTokenInput { get; private set; }
    public string? ErrorMessage { get; set; }
    public string ReturnUrl { get; set; } = "/admin";

    public event Action? StateChanged;

    public LoginViewModel(
        AuthenticationService authService,
        NavigationManager navigationManager,
        CmsEventMediator eventMediator)
    {
        _authService = authService;
        _navigationManager = navigationManager;
        _eventMediator = eventMediator;
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
            var result = await _authService.LoginWithGitHubAsync(code);

            if (result)
            {
                _eventMediator.Publish(new AuthenticationChangedEvent(true));
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
            var result = await _authService.LoginWithTokenAsync(AccessToken);

            if (result)
            {
                _eventMediator.Publish(new AuthenticationChangedEvent(true));
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
        _eventMediator.Publish(new AuthenticationChangedEvent(false));
        _navigationManager.NavigateTo("/admin/login");
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}