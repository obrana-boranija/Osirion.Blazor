using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Internal;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Renders a highlighted link, with an arrow, at the end of a menu or mega menu. In a mega submenu it spans every
/// column below the groups.
/// </summary>
public partial class MenuCallToAction
{
    /// <summary>Gets or sets the visible text of the link.</summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets supporting text under <see cref="Text"/>.</summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the link address. Only a relative URL or the <c>http</c>, <c>https</c>, <c>mailto</c> and
    /// <c>tel</c> schemes get an <c>href</c>.
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
