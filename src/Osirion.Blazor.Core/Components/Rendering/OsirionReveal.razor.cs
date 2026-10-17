using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Reveals content as it enters the viewport while retaining an SSR-visible fallback.</summary>
public partial class OsirionReveal : OsirionComponentBase
{
    /// <summary>Gets or sets the content to reveal. Required.</summary>
    [Parameter, EditorRequired] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the content starts hidden and is revealed when it scrolls into view. When
    /// <see langword="false"/>, the script does nothing and the content is shown from the start. With reduced
    /// motion requested the script does not run either.
    /// </summary>
    [Parameter] public bool Animate { get; set; } = true;

    /// <summary>
    /// Gets or sets the direction the content moves in from: <c>Up</c>, <c>Down</c>, <c>Left</c>, <c>Right</c> or
    /// <c>Fade</c> (no movement).
    /// </summary>
    [Parameter] public RevealAnimation Animation { get; set; } = RevealAnimation.Up;

    private string CssAnimation => Animation.ToString().ToLowerInvariant();

    /// <summary>Supported reveal directions.</summary>
    public enum RevealAnimation
    {
        /// <summary>Reveals content upward.</summary>
        Up,
        /// <summary>Reveals content downward.</summary>
        Down,
        /// <summary>Reveals content from the left.</summary>
        Left,
        /// <summary>Reveals content from the right.</summary>
        Right,
        /// <summary>Fades content into view.</summary>
        Fade
    }
}
