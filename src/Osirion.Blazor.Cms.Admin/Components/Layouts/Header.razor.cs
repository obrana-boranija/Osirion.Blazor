using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts;
    /// <summary>Displays the title and optional navigation content for an admin page.</summary>
public partial class Header
{
    /// <summary>Gets or sets the page title shown in the header.</summary>
    [Parameter]
    public string Title { get; set; } = "Page Title";

    /// <summary>Gets or sets the optional subtitle shown below the page title.</summary>
    [Parameter]
    public string? Subtitle { get; set; }

    /// <summary>Gets or sets a value indicating whether breadcrumb navigation is displayed.</summary>
    [Parameter]
    public bool ShowBreadcrumb { get; set; } = true;

    /// <summary>Gets or sets the breadcrumb items displayed in the header.</summary>
    [Parameter]
    public List<BreadcrumbItem>? BreadcrumbItems { get; set; }

    /// <summary>Gets or sets content rendered beside the page title.</summary>
    [Parameter]
    public RenderFragment? ActionContent { get; set; }
}
