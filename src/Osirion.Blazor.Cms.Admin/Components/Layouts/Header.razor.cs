using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts;
public partial class Header
{
    [Parameter]
    public string Title { get; set; } = "Page Title";

    [Parameter]
    public string? Subtitle { get; set; }

    [Parameter]
    public bool ShowBreadcrumb { get; set; } = true;

    [Parameter]
    public List<BreadcrumbItem>? BreadcrumbItems { get; set; }

    [Parameter]
    public RenderFragment? ActionContent { get; set; }
}
