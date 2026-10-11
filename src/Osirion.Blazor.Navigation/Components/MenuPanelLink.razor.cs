using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Internal;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Renders a pill-style featured link inside a <see cref="MenuPanel"/>.
/// </summary>
public partial class MenuPanelLink
{
    /// <summary>Gets or sets the visible link text.</summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the destination URL.</summary>
    [Parameter]
    public string Href { get; set; } = "#";

    /// <summary>Gets or sets how to open the destination.</summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>Gets or sets an optional leading icon.</summary>
    [Parameter]
    public RenderFragment? IconTemplate { get; set; }

    /// <summary>
    /// Gets the destination to render, or <see langword="null"/> when <see cref="Href"/> uses a scheme
    /// that is not safe (for example <c>javascript:</c>).
    /// </summary>
    private string? RenderedHref => SafeHref.Sanitize(Href);
}
