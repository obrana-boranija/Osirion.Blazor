using System.ComponentModel;

namespace Osirion.Blazor.Components;

/// <summary>
/// Defines the available animation easing functions for the AnimatedContainer component.
/// Easing functions control the rate of change of the animation over time,
/// affecting how the animation accelerates and decelerates.
/// </summary>
public enum AnimationEasing
{
    /// <summary>
    /// Linear timing function.
    /// Animation progresses at a constant rate from start to finish.
    /// Creates mechanical, uniform motion without acceleration or deceleration.
    /// Best for: Loading indicators, progress bars, or when consistent speed is desired.
    /// CSS: linear
    /// </summary>
    [Description("Linear timing")]
    Linear,

    /// <summary>
    /// Ease in timing function.
    /// Animation starts slowly and accelerates towards the end.
    /// Creates a sense of the element gaining momentum.
    /// Best for: Elements appearing or entering the viewport.
    /// CSS: cubic-bezier(0.42, 0, 1, 1)
    /// </summary>
    [Description("Slow start")]
    EaseIn,

    /// <summary>
    /// Ease out timing function.
    /// Animation starts quickly and decelerates towards the end.
    /// Creates a natural, smooth settling effect.
    /// This is the default easing as it feels most natural to users.
    /// Best for: Most UI animations, content reveals, smooth transitions.
    /// CSS: cubic-bezier(0, 0, 0.58, 1)
    /// </summary>
    [Description("Slow end")]
    EaseOut,

    /// <summary>
    /// Ease in-out timing function.
    /// Animation starts slowly, accelerates in the middle, then decelerates at the end.
    /// Creates symmetric acceleration and deceleration.
    /// Best for: Cyclical animations, toggles, or bidirectional movements.
    /// CSS: cubic-bezier(0.42, 0, 0.58, 1)
    /// </summary>
    [Description("Slow start and end")]
    EaseInOut,

    /// <summary>
    /// Bounce timing function.
    /// Animation overshoots its target and bounces back, similar to a rubber ball.
    /// Creates a playful, energetic effect that draws attention.
    /// Best for: Call-to-action buttons, success notifications, playful UI elements.
    /// CSS: cubic-bezier(0.68, -0.55, 0.265, 1.55)
    /// </summary>
    [Description("Bouncy effect")]
    Bounce,

    /// <summary>
    /// Elastic timing function.
    /// Animation oscillates around the target like a spring or elastic band.
    /// Creates a dynamic, lively effect with multiple overshoots.
    /// Best for: Interactive elements, notifications, attention-grabbing animations.
    /// CSS: cubic-bezier(0.175, 0.885, 0.32, 1.275)
    /// </summary>
    [Description("Elastic effect")]
    Elastic
}