namespace Osirion.Blazor.Components;


/// <summary>
/// Provides utility methods for working with Osirion background patterns.
/// </summary>
public static class OsirionPattern
{
    /// <summary>
    /// Returns the CSS class name for the specified <see cref="BackgroundPatternType"/>.
    /// </summary>
    /// <param name="type">The background pattern type.</param>
    /// <returns>The CSS class name corresponding to the pattern type, or an empty string if <paramref name="type"/> is null or unrecognized.</returns>
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
