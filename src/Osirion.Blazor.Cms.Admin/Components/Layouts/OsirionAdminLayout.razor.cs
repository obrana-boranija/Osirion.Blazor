using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Components.Layouts.Navigation;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts;

/// <summary>
/// Main layout component for the Osirion CMS Admin interface with GitHub-inspired design
/// </summary>
public partial class OsirionAdminLayout
{
    /// <summary>Gets or sets the active visual theme.</summary>
    [Parameter]
    public string Theme { get; set; } = "light";

    /// <summary>Gets or sets the callback invoked after the theme changes.</summary>
    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    // True while an interactive circuit that is not signed in to GitHub yet waits for the stored sign-in
    private bool _isRestoringSession;

    /// <summary>Subscribes the layout to admin state changes.</summary>
    protected override void OnInitialized()
    {
        AdminState.StateChanged += StateHasChanged;

        // The stored token lives in browser storage, which only an interactive circuit can read. Hold the page back
        // until it is restored, because pages call GitHub from their first lifecycle method. Static and prerendered
        // output renders the page right away, as before.
        // The login page restores the sign-in itself, because it also redirects to the page the user came from.
        _isRestoringSession = RendererInfo.IsInteractive && !AuthService.IsAuthenticated && !IsLoginPage();
    }

    private bool IsLoginPage()
    {
        var path = new Uri(NavigationManager.Uri).AbsolutePath;
        return string.Equals(path, CmsConstants.Routes.Login, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Restores the stored GitHub sign-in once the circuit is interactive, then shows the page.</summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _isRestoringSession)
        {
            try
            {
                await SessionRestorer.RestoreAsync();
            }
            finally
            {
                _isRestoringSession = false;
                StateHasChanged();
            }
        }
    }

    private NavMenu.UserInfo? GetUserInfo()
    {
        if (!AuthService.IsAuthenticated)
            return null;

        return new NavMenu.UserInfo
        {
            Username = AuthService.Username,
            IsAuthenticated = AuthService.IsAuthenticated
        };
    }

    private async Task ToggleTheme()
    {
        Theme = Theme == "light" ? "dark" : "light";
        if (ThemeChanged.HasDelegate)
        {
            await ThemeChanged.InvokeAsync(Theme);
        }
    }

    private async Task SignOut()
    {
        await AuthService.SignOutAsync();
        await TokenStore.RemoveTokenAsync();
        NavigationManager.NavigateTo("/osirion/login");
    }

    /// <summary>Releases the state-change subscription held by the layout.</summary>
    public void Dispose()
    {
        AdminState.StateChanged -= StateHasChanged;
    }
}
