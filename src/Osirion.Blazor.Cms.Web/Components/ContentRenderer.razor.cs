using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Defines the ContentRenderer type.</summary>
public partial class ContentRenderer
{
    /// <summary>Performs the Item operation.</summary>
    [Parameter]
    public ContentItem? Item { get; set; }

    private string GetContentViewClass()
    {
        return $"osirion-content-view {Class}".Trim();
    }
}
