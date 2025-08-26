using System.ComponentModel;

namespace Osirion.Blazor.Components;

/// <summary>
/// Defines the available animation effects for the AnimatedContainer component.
/// Each effect determines the initial state and movement direction of the animation.
/// </summary>
public enum AnimationEffect
{
    /// <summary>
    /// Simple fade in animation without any movement.
    /// Element starts invisible and fades to full opacity.
    /// </summary>
    [Description("Simple opacity fade")]
    Fade,

    /// <summary>
    /// Fade in animation combined with upward movement.
    /// Element starts below its final position and fades in while moving up.
    /// This is the default and most commonly used effect.
    /// </summary>
    [Description("Fade in from bottom")]
    FadeUp,

    /// <summary>
    /// Fade in animation combined with downward movement.
    /// Element starts above its final position and fades in while moving down.
    /// </summary>
    [Description("Fade in from top")]
    FadeDown,

    /// <summary>
    /// Fade in animation combined with leftward movement.
    /// Element starts to the right of its final position and fades in while moving left.
    /// </summary>
    [Description("Fade in from right")]
    FadeLeft,

    /// <summary>
    /// Fade in animation combined with rightward movement.
    /// Element starts to the left of its final position and fades in while moving right.
    /// </summary>
    [Description("Fade in from left")]
    FadeRight,

    /// <summary>
    /// Slide up animation without opacity change.
    /// Element starts below its final position and slides up while remaining visible.
    /// </summary>
    [Description("Slide up without fade")]
    SlideUp,

    /// <summary>
    /// Slide down animation without opacity change.
    /// Element starts above its final position and slides down while remaining visible.
    /// </summary>
    [Description("Slide down without fade")]
    SlideDown,

    /// <summary>
    /// Slide left animation without opacity change.
    /// Element starts to the right of its final position and slides left while remaining visible.
    /// </summary>
    [Description("Slide left without fade")]
    SlideLeft,

    /// <summary>
    /// Slide right animation without opacity change.
    /// Element starts to the left of its final position and slides right while remaining visible.
    /// </summary>
    [Description("Slide right without fade")]
    SlideRight,

    /// <summary>
    /// Zoom in animation with fade.
    /// Element starts smaller than its final size and grows while fading in.
    /// </summary>
    [Description("Scale up with fade")]
    ZoomIn,

    /// <summary>
    /// Zoom out animation with fade.
    /// Element starts larger than its final size and shrinks while fading in.
    /// </summary>
    [Description("Scale down with fade")]
    ZoomOut
}