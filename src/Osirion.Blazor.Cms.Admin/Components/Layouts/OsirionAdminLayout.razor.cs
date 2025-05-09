using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts;

/// <summary>
/// Main layout component for the Osirion CMS Admin interface with GitHub-inspired design
/// </summary>
public partial class OsirionAdminLayout
{
    [Inject] private IAuthenticationService AuthService { get; set; } = default!;
    [Inject] private CmsState AdminState { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    ///// <summary>
    ///// The main content to be rendered within the layout
    ///// </summary>
    //[Parameter] public RenderFragment? ChildContent { get; set; }

    ///// <summary>
    ///// Top navigation items to be displayed in the header
    ///// </summary>
    //[Parameter] public RenderFragment? TopNavigation { get; set; }

    ///// <summary>
    ///// Optional header content to replace the default header
    ///// </summary>
    //[Parameter] public RenderFragment? HeaderContent { get; set; }

    ///// <summary>
    ///// Optional content to be displayed in the header actions area
    ///// </summary>
    //[Parameter] public RenderFragment? HeaderActions { get; set; }

    /// <summary>
    /// Gets or sets the theme (light or dark)
    /// </summary>
    [Parameter] public string Theme { get; set; } = "light";

    /// <summary>
    /// Event callback when theme is changed
    /// </summary>
    [Parameter] public EventCallback<string> ThemeChanged { get; set; }

    private bool IsMenuOpen { get; set; }

    protected override void OnInitialized()
    {
        // Subscribe to state changes
        AdminState.StateChanged += StateHasChanged;
    }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(Theme))
        {
            Theme = "light";
        }
    }

    private async Task ToggleTheme()
    {
        Theme = Theme == "light" ? "dark" : "light";
        if (ThemeChanged.HasDelegate)
        {
            await ThemeChanged.InvokeAsync(Theme);
        }
    }

    private void ToggleMenu()
    {
        IsMenuOpen = !IsMenuOpen;
    }

    private async Task SignOut()
    {
        await AuthService.SignOutAsync();
        NavigationManager.NavigateTo("/osirion/login");
    }

    private string GetLayoutClass()
    {
        return $"osirion-admin-layout osirion-theme-{Theme}";
    }

    public void Dispose()
    {
        AdminState.StateChanged -= StateHasChanged;
    }
}