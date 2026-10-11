using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Internal;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>Renders a prominent link at the end of a menu or mega menu.</summary>
public partial class MenuCallToAction
{
    /// <summary>Gets or sets the visible call to action text.</summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets optional supporting text.</summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>Gets or sets the destination URL.</summary>
    [Parameter]
    public string Href { get; set; } = "#";

    /// <summary>Gets or sets how to open the destination.</summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Gets the destination to render, or <see langword="null"/> when <see cref="Href"/> uses a scheme
    /// that is not safe (for example <c>javascript:</c>).
    /// </summary>
    private string? RenderedHref => SafeHref.Sanitize(Href);
}
