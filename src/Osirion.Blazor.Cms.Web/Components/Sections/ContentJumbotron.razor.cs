using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Components;

public partial class ContentJumbotron
{
    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public ContentItem? Item { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public bool TextEnhancement { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public BackgroundPatternType? BackgroundPattern { get; set; }

    private string GetContentViewClass()
    {
        return $"osirion-content-header-container {CssClass}".Trim();
    }
}
