using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts.Navigation;
/// <summary>Renders breadcrumb navigation for the current admin page.</summary>
public partial class Breadcrumb
{
    /// <summary>Gets or sets the breadcrumb items leading to the current page.</summary>
    [Parameter]
    public List<BreadcrumbItem>? Items { get; set; }

    /// <summary>Gets or sets the label for the current page.</summary>
    [Parameter]
    public string? CurrentPage { get; set; }
}
