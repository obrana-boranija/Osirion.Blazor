namespace Osirion.Blazor.Components;

/// <summary>
/// Utility class for generating CSS class names for various pattern types.
/// </summary>
public static class OsirionPattern
{
    /// <summary>
    /// Gets the CSS class name for a specific background pattern type.
    /// </summary>
    /// <param name="type">The background pattern type.</param>
    /// <returns>A CSS class name corresponding to the pattern type, or an empty string if null.</returns>
    public static string BackgroundPattern(BackgroundPatternType? type)
    {
        return type switch
        {
            BackgroundPatternType.Dots => "osirion-bg-dots",
            BackgroundPatternType.Grid => "osirion-bg-grid",
            BackgroundPatternType.Diagonal => "osirion-bg-diagonal",
            BackgroundPatternType.Honeycomb => "osirion-bg-honeycomb",
            BackgroundPatternType.GradientMesh => "osirion-bg-gradient-mesh",
            BackgroundPatternType.AnimatedGrid => "osirion-bg-grid-animated",
            BackgroundPatternType.TechWave => "osirion-bg-tech-wave",
            BackgroundPatternType.Circuit => "osirion-bg-circuit",
            BackgroundPatternType.DotsFade => "osirion-bg-dots-fade",
            _ => string.Empty,
        };
    }
}
