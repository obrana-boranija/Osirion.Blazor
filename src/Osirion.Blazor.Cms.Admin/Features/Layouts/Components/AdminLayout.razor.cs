using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Features.Layouts.Components;

/// <summary>Provides the shared layout for CMS administration pages.</summary>
public partial class AdminLayout : IDisposable
{
    /// <summary>Performs the authService operation.</summary>
    [Inject]
    protected IAuthenticationService authService { get; set; } = default!;

    [Inject]
    private IGitHubTokenStore TokenStore { get; set; } = default!;

    /// <summary>Gets or sets the page title.</summary>
    [Parameter]
    public string Title { get; set; } = "Osirion CMS";

    /// <summary>Gets or sets the page subtitle.</summary>
    [Parameter]
    public string? Subtitle { get; set; }

    /// <summary>Gets or sets the theme chosen in this layout.</summary>
    /// <remarks>
    /// It is not a component parameter: a parameter of this name would clash with the <c>Theme</c> parameter of
    /// <see cref="Osirion.Blazor.Components.OsirionComponentBase"/>, and Blazor refuses to render a component with
    /// two parameters of the same name.
    /// </remarks>
    public new string Theme { get; set; } = "light";

    /// <summary>Gets or sets the callback invoked when the theme changes.</summary>
    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    /// <summary>Gets or sets the status message.</summary>
    [Parameter]
    public string? StatusMessage { get; set; }

    /// <summary>Gets or sets the callback invoked when the status message changes.</summary>
    [Parameter]
    public EventCallback<string?> StatusMessageChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when the error message changes.</summary>
    [Parameter]
    public EventCallback<string?> ErrorMessageChanged { get; set; }

    /// <summary>Gets or sets the current page identifier.</summary>
    [Parameter]
    public string? CurrentPage { get; set; }

    /// <summary>Gets or sets the breadcrumb items.</summary>
    [Parameter]
    public List<BreadcrumbItem>? BreadcrumbItems { get; set; }

    /// <summary>Gets or sets the page content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets the custom header content.</summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>Gets or sets the custom navigation content.</summary>
    [Parameter]
    public RenderFragment? NavigationTemplate { get; set; }

    /// <summary>Gets or sets the custom actions content.</summary>
    [Parameter]
    public RenderFragment? ActionsTemplate { get; set; }

    /// <summary>Gets or sets the custom sidebar footer content.</summary>
    [Parameter]
    public RenderFragment? SidebarFooterTemplate { get; set; }

    /// <summary>Gets or sets the callback invoked when the user signs out.</summary>
    [Parameter]
    public EventCallback OnSignOut { get; set; }

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        AdminState.StateChanged += StateHasChanged;
    }

    // The markup branches live here so the render tree keeps a low complexity.
    // An empty user name has no initial; Substring on it threw while rendering the sidebar.
    private string? UserInitial => string.IsNullOrEmpty(authService.Username)
        ? null
        : authService.Username[..1].ToUpper();

    private bool HasBreadcrumbItems => BreadcrumbItems is not null && BreadcrumbItems.Count > 0;

    private string ThemeToggleIconClass => Theme == "light" ? "bi-moon" : "bi-sun";

    private bool IsCurrentCrumb(BreadcrumbItem item) =>
        string.IsNullOrWhiteSpace(item.Url) || item == BreadcrumbItems!.Last();

    private async Task SignOut()
    {
        await authService.SignOutAsync();
        await TokenStore.RemoveTokenAsync();

        if (OnSignOut.HasDelegate)
        {
            await OnSignOut.InvokeAsync();
        }
    }

    private async Task ClearStatusMessage()
    {
        StatusMessage = null;
        await StatusMessageChanged.InvokeAsync(null);
    }

    private async Task ClearErrorMessage()
    {
        ErrorMessage = null;
        await ErrorMessageChanged.InvokeAsync(null);
    }

    private async Task ToggleTheme()
    {
        Theme = Theme == "light" ? "dark" : "light";
        if (ThemeChanged.HasDelegate)
        {
            await ThemeChanged.InvokeAsync(Theme);
        }
    }

    /// <summary>Releases subscriptions held by the layout.</summary>
    public void Dispose()
    {
        AdminState.StateChanged -= StateHasChanged;
    }
}
