using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Components.Layouts.Navigation;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts;

/// <summary>
/// Main layout component for the Osirion CMS Admin interface with GitHub-inspired design
/// </summary>
public partial class OsirionAdminLayout
{
    [Parameter]
    public string Theme { get; set; } = "light";

    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    protected override void OnInitialized()
    {
        AdminState.StateChanged += StateHasChanged;
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
        NavigationManager.NavigateTo("/osirion/login");
    }

    public void Dispose()
    {
        AdminState.StateChanged -= StateHasChanged;
    }
}