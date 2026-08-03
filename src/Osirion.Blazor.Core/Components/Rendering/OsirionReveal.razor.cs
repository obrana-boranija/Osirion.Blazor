using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Reveals content as it enters the viewport while retaining an SSR-visible fallback.</summary>
public partial class OsirionReveal : OsirionComponentBase
{
    /// <summary>Gets or sets the content to reveal.</summary>
    [Parameter, EditorRequired] public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets whether the viewport enhancement is enabled.</summary>
    [Parameter] public bool Animate { get; set; } = true;

    /// <summary>Gets or sets the reveal direction.</summary>
    [Parameter] public RevealAnimation Animation { get; set; } = RevealAnimation.Up;

    private string CssAnimation => Animation.ToString().ToLowerInvariant();

    /// <summary>Supported reveal directions.</summary>
    public enum RevealAnimation { Up, Down, Left, Right, Fade }
}