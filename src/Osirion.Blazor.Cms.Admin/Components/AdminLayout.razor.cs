using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components;

public partial class AdminLayout
{
    /// <summary>
    /// Gets or sets the page title
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Osirion CMS";

    /// <summary>
    /// Gets or sets the page subtitle
    /// </summary>
    [Parameter]
    public string? Subtitle { get; set; }

    /// <summary>
    /// Gets or sets the current theme (light or dark)
    /// </summary>
    [Parameter]
    public string Theme { get; set; } = "light";

    /// <summary>
    /// Gets or sets the status message
    /// </summary>
    [Parameter]
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Gets or sets the event callback for status message changes
    /// </summary>
    [Parameter]
    public EventCallback<string?> StatusMessageChanged { get; set; }

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    [Parameter]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the event callback for error message changes
    /// </summary>
    [Parameter]
    public EventCallback<string?> ErrorMessageChanged { get; set; }

    /// <summary>
    /// Gets or sets the current page name for breadcrumb
    /// </summary>
    [Parameter]
    public string? CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets breadcrumb items for navigation
    /// </summary>
    [Parameter]
    public List<AdminPage.BreadcrumbItem>? BreadcrumbItems { get; set; }

    /// <summary>
    /// Gets or sets the child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the header template
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the navigation template
    /// </summary>
    [Parameter]
    public RenderFragment? NavigationTemplate { get; set; }

    /// <summary>
    /// Gets or sets the actions template
    /// </summary>
    [Parameter]
    public RenderFragment? ActionsTemplate { get; set; }

    /// <summary>
    /// Gets or sets the sidebar footer template
    /// </summary>
    [Parameter]
    public RenderFragment? SidebarFooterTemplate { get; set; }

    /// <summary>
    /// Gets or sets the event callback for sign out
    /// </summary>
    [Parameter]
    public EventCallback OnSignOut { get; set; }

    private async Task SignOut()
    {
        await AuthService.SignOutAsync();

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

    private string GetAdminLayoutClass()
    {
        return $"osirion-admin-dashboard osirion-admin-theme-{Theme} {CssClass}".Trim();
    }
}
