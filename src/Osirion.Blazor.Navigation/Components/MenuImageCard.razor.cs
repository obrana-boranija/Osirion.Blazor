using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Renders a linked image card with an overlay caption inside a mega menu,
/// typically used as an aside to feature customer stories or news.
/// </summary>
public partial class MenuImageCard
{
    /// <summary>Gets or sets the image URL.</summary>
    [Parameter]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets the image alternative text.</summary>
    [Parameter]
    public string? ImageAlt { get; set; }

    /// <summary>Gets or sets the caption title.</summary>
    [Parameter]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets optional caption supporting text.</summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>Gets or sets the destination URL.</summary>
    [Parameter]
    public string Href { get; set; } = "#";

    /// <summary>Gets or sets how to open the destination.</summary>
    [Parameter]
    public string? Target { get; set; }
}
