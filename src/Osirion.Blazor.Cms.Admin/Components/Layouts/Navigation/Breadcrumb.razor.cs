using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts.Navigation;
public partial class Breadcrumb
{
    [Parameter]
    public List<BreadcrumbItem>? Items { get; set; }

    [Parameter]
    public string? CurrentPage { get; set; }
}
