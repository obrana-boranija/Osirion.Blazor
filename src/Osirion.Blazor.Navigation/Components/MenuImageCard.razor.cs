using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Internal;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Renders a linked image card with an overlay caption inside a mega menu,
/// typically used as an aside to feature customer stories or news.
/// </summary>
public partial class MenuImageCard
{
    /// <summary>Gets or sets the address of the image, written as given. It loads lazily.</summary>
    [Parameter]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alternative text of the image. Set it: without it the image is rendered with no
    /// <c>alt</c> attribute.
    /// </summary>
    [Parameter]
    public string? ImageAlt { get; set; }

    /// <summary>Gets or sets the caption title, shown over a dark scrim at the bottom of the image.</summary>
    [Parameter]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets supporting text under <see cref="Title"/>.</summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the link address of the whole card. Only a relative URL or the <c>http</c>, <c>https</c>,
    /// <c>mailto</c> and <c>tel</c> schemes get an <c>href</c>.
    /// </summary>
    [Parameter]
    public string Href { get; set; } = "#";

    /// <summary>
    /// Gets or sets the <c>target</c> attribute. <c>_blank</c> also adds <c>rel="noopener noreferrer"</c>.
    /// </summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Gets the destination to render, or <see langword="null"/> when <see cref="Href"/> uses a scheme
    /// that is not safe (for example <c>javascript:</c>).
    /// </summary>
    private string? RenderedHref => SafeHref.Sanitize(Href);
}
