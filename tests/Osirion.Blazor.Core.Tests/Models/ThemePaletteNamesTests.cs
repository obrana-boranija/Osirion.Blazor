using Osirion.Blazor.Components;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Models;

public class ThemePaletteNamesTests
{
    [Theory]
    [InlineData(ThemePalette.Framework, "framework", "Framework")]
    [InlineData(ThemePalette.Osirion, "osirion", "Osirion")]
    [InlineData(ThemePalette.Commit, "commit", "Commit")]
    [InlineData(ThemePalette.Editor, "editor", "Editor")]
    [InlineData(ThemePalette.Studio, "studio", "Studio")]
    [InlineData(ThemePalette.StudioBlue, "studio-blue", "Studio Blue")]
    [InlineData(ThemePalette.Contrast, "contrast", "Contrast")]
    public void Names_EveryPalette_HasAnIdThatParsesBack(ThemePalette palette, string id, string displayName)
    {
        // Act
        var parsed = ThemePaletteNames.TryParse(palette.ToId(), out var roundTripped);

        // Assert
        palette.ToId().ShouldBe(id);
        palette.ToDisplayName().ShouldBe(displayName);
        parsed.ShouldBeTrue();
        roundTripped.ShouldBe(palette);
    }

    [Theory]
    [InlineData(" Studio-Blue ", ThemePalette.StudioBlue)]
    [InlineData("COMMIT", ThemePalette.Commit)]
    public void TryParse_IdWithOtherCaseOrSpaces_Parses(string id, ThemePalette expected)
    {
        // Act
        var parsed = ThemePaletteNames.TryParse(id, out var palette);

        // Assert
        parsed.ShouldBeTrue();
        palette.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("StudioBlue")]
    [InlineData("1")]
    [InlineData("commit;")]
    public void TryParse_NotAnId_RefusesAndReturnsFramework(string? id)
    {
        // Act
        var parsed = ThemePaletteNames.TryParse(id, out var palette);

        // Assert
        parsed.ShouldBeFalse();
        palette.ShouldBe(ThemePalette.Framework);
    }

    [Fact]
    public void Names_ValueOutsideTheEnum_FallsBackToFramework()
    {
        // Act & Assert
        ((ThemePalette)42).ToId().ShouldBe("framework");
        ((ThemePalette)42).ToDisplayName().ShouldBe("Framework");
    }
}
