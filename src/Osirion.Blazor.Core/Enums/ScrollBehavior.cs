namespace Osirion.Blazor;

/// <summary>
/// Defines the behavior of scrolling animations
/// </summary>
public enum ScrollBehavior
{
    /// <summary>
    /// Browser decides the scrolling behavior
    /// </summary>
    Auto,

    /// <summary>
    /// Instant scrolling without animation
    /// </summary>
    Instant,

    /// <summary>
    /// Smooth scrolling with animation
    /// </summary>
    Smooth
}