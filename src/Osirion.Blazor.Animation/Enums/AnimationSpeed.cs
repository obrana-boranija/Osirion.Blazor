using System.ComponentModel;

namespace Osirion.Blazor.Components;

/// <summary>
/// Defines the available animation speed presets for the AnimatedContainer component.
/// These presets provide common animation durations suitable for different use cases.
/// </summary>
public enum AnimationSpeed
{
    /// <summary>
    /// Slow animation speed (1000ms).
    /// Best for hero elements, important content, or when you want to draw attention.
    /// Provides a leisurely, graceful animation that gives users time to appreciate the effect.
    /// </summary>
    [Description("Slow animation (1000ms)")]
    Slow,

    /// <summary>
    /// Normal animation speed (600ms).
    /// This is the default speed and works well for most content animations.
    /// Provides a good balance between being noticeable and not feeling sluggish.
    /// </summary>
    [Description("Normal speed (600ms)")]
    Normal,

    /// <summary>
    /// Fast animation speed (400ms).
    /// Good for UI feedback, secondary content, or when you have many elements animating.
    /// Provides snappy animations that don't interrupt the user experience.
    /// </summary>
    [Description("Fast animation (400ms)")]
    Fast,

    /// <summary>
    /// Extra fast animation speed (250ms).
    /// Best for micro-interactions, hover effects, or subtle UI changes.
    /// Provides immediate feedback while still being perceptible.
    /// </summary>
    [Description("Extra fast (250ms)")]
    ExtraFast
}