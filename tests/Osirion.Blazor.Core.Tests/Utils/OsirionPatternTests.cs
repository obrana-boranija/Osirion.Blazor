using Osirion.Blazor.Components;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Utils;

public class OsirionPatternTests
{
    [Theory]
    [InlineData(BackgroundPatternType.Dots, "osirion-bg-dots")]
    [InlineData(BackgroundPatternType.Grid, "osirion-bg-grid")]
    [InlineData(BackgroundPatternType.Diagonal, "osirion-bg-diagonal")]
    [InlineData(BackgroundPatternType.Honeycomb, "osirion-bg-honeycomb")]
    [InlineData(BackgroundPatternType.GradientMesh, "osirion-bg-gradient-mesh")]
    [InlineData(BackgroundPatternType.AnimatedGrid, "osirion-bg-grid-animated")]
    [InlineData(BackgroundPatternType.TechWave, "osirion-bg-tech-wave")]
    [InlineData(BackgroundPatternType.Circuit, "osirion-bg-circuit")]
    [InlineData(BackgroundPatternType.DotsFade, "osirion-bg-dots-fade")]
    public void BackgroundPattern_KnownType_ReturnsItsCssClass(BackgroundPatternType type, string expected)
    {
        OsirionPattern.BackgroundPattern(type).ShouldBe(expected);
    }

    [Fact]
    public void BackgroundPattern_EveryDefinedType_HasACssClass()
    {
        foreach (var type in Enum.GetValues<BackgroundPatternType>())
        {
            OsirionPattern.BackgroundPattern(type).ShouldStartWith("osirion-bg-");
        }
    }

    [Fact]
    public void BackgroundPattern_Null_ReturnsEmpty()
    {
        OsirionPattern.BackgroundPattern(null).ShouldBeEmpty();
    }

    [Fact]
    public void BackgroundPattern_UndefinedValue_ReturnsEmpty()
    {
        OsirionPattern.BackgroundPattern((BackgroundPatternType)999).ShouldBeEmpty();
    }
}
