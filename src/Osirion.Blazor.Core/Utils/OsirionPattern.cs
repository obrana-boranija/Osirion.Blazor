namespace Osirion.Blazor.Components;


public static class OsirionPattern
{
    public static string? BackgroundPattern(BackgroundPatternType? type)
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
            _ => null,
        };
    }
}
