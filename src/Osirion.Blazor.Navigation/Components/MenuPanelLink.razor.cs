using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Internal;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Renders a pill-style featured link inside a <see cref="MenuPanel"/>.
/// </summary>
public partial class MenuPanelLink
{
    /// <summary>Gets or sets the visible text of the link.</summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

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

    /// <summary>Gets or sets an icon shown before the text, inside an <c>aria-hidden</c> span.</summary>
    [Parameter]
    public RenderFragment? IconTemplate { get; set; }

    /// <summary>
    /// Gets the destination to render, or <see langword="null"/> when <see cref="Href"/> uses a scheme
    /// that is not safe (for example <c>javascript:</c>).
    /// </summary>
    private string? RenderedHref => SafeHref.Sanitize(Href);
}
