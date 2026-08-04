using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;
using Osirion.Blazor.Cms.Admin.Shared.Components;

namespace Osirion.Blazor.Cms.Admin.Features.Layouts.Components;

/// <summary>Provides the standard page layout for CMS administration views.</summary>
public partial class AdminPage : BaseComponent
{
    /// <summary>Gets or sets the page title.</summary>
    [Parameter]
    public string Title { get; set; } = "Osirion CMS";

    /// <summary>Gets or sets the page subtitle.</summary>
    [Parameter]
    public string? Subtitle { get; set; }

    /// <summary>Gets or sets the current page identifier.</summary>
    [Parameter]
    public string CurrentPage { get; set; } = string.Empty;

    /// <summary>Gets or sets the current theme.</summary>
    [Parameter]
    public new string Theme { get; set; } = "light";

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

    /// <summary>Gets or sets whether the login form is shown.</summary>
    [Parameter]
    public bool ShowLoginForm { get; set; } = false;

    /// <summary>Gets or sets the sign-out callback.</summary>
    [Parameter]
    public EventCallback OnSignOut { get; set; }

    /// <summary>Gets or sets the breadcrumb items.</summary>
    [Parameter]
    public List<BreadcrumbItem>? BreadcrumbItems { get; set; }

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        AdminState.StateChanged += StateHasChanged;

        // If no current page specified but we have a URL, extract the page name
        if (string.IsNullOrWhiteSpace(CurrentPage))
        {
            try
            {
                var uri = new Uri(NavigationManager.Uri);
                var path = uri.AbsolutePath;

                // Extract the last segment of the path as the page name
                var lastSegment = path.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (!string.IsNullOrWhiteSpace(lastSegment))
                {
                    CurrentPage = char.ToUpper(lastSegment[0]) + lastSegment.Substring(1);
                }
            }
            catch
            {
                // Ignore errors in path extraction
            }
        }
    }

    private Task HandleSignOut()
    {
        if (OnSignOut.HasDelegate)
        {
            return OnSignOut.InvokeAsync();
        }

        return Task.CompletedTask;
    }

    /// <summary>Releases subscriptions held by the page.</summary>
    public void Dispose()
    {
        // Unsubscribe from state changes when component is disposed
        AdminState.StateChanged -= StateHasChanged;
    }
}
