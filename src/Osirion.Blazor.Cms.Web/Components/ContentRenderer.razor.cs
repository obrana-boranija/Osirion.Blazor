using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
namespace Osirion.Blazor.Cms.Web.Components;

public partial class ContentRenderer
{
    [Parameter]
    public ContentItem? Item { get; set; }

    private string GetContentViewClass()
    {
        return $"osirion-content-view {CssClass}".Trim();
    }
}
