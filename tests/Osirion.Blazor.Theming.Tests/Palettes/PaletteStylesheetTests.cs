using Osirion.Blazor.Components;
using Osirion.Blazor.Palettes;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Palettes;

public class PaletteStylesheetTests
{
    private static readonly IReadOnlyList<PaletteVariant> Variants = PaletteStylesheet.BuildVariants(new List<string>());

    public static TheoryData<string, string> VariantNames()
    {
        var data = new TheoryData<string, string>();
        foreach (var variant in Variants)
        {
            data.Add(variant.Palette, variant.Mode);
        }

        return data;
    }

    [Fact]
    public void CommittedStylesheets_MatchTheGenerator()
    {
        // Arrange
        var (css, minified) = PaletteStylesheet.Generate(Variants);

        // Act
        var committedCss = File.ReadAllText(RepositoryFiles.ThemingCss("palettes.css"));
        var committedMinified = File.ReadAllText(RepositoryFiles.ThemingCss("palettes.min.css"));

        // Assert: run `dotnet run --project tools/Osirion.Blazor.Palettes` after changing PaletteDefinitions
        Normalize(committedCss).ShouldBe(Normalize(css));
        Normalize(committedMinified).ShouldBe(Normalize(minified));
    }

    [Theory]
    [MemberData(nameof(VariantNames))]
    public void Variant_ReachesItsContrastTargets(string palette, string mode)
    {
        // Arrange
        var variant = Variants.Single(v => v.Palette == palette && v.Mode == mode);
        var tokens = variant.Tokens.ToDictionary(token => token.Key, token => token.Value);
        var pairs = tokens.ContainsKey("text-primary")
            ? PaletteBuilder.ContrastPairs(variant.TextContrast)
            : PaletteBuilder.CodeContrastPairs(variant.TextContrast);

        // Act
        var failures = pairs
            .Select(pair => (pair, ratio: Rgb.Contrast(Rgb.Parse(tokens[pair.Foreground]), Rgb.Parse(tokens[pair.Background]))))
            .Where(result => result.ratio < result.pair.Minimum)
            .Select(result => $"{result.pair.Foreground} on {result.pair.Background}: {result.ratio:0.00} < {result.pair.Minimum}")
            .ToList();

        // Assert
        failures.ShouldBeEmpty();
    }

    [Fact]
    public void FullPalettes_DeclareTheSameTokens()
    {
        // Arrange
        var full = Variants.Where(v => v.Palette != "osirion").ToList();
        var expected = full[0].Tokens.Select(token => token.Key).ToHashSet();

        // Act
        var differing = full
            .Where(v => !v.Tokens.Select(token => token.Key).Except(["menu-background", "menu-color", "menu-muted-color",
                "menu-hover-background", "menu-border-color"]).ToHashSet().SetEquals(expected))
            .Select(v => $"{v.Palette} {v.Mode}")
            .ToList();

        // Assert
        differing.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(ThemePalette.Osirion)]
    [InlineData(ThemePalette.Commit)]
    [InlineData(ThemePalette.Editor)]
    [InlineData(ThemePalette.Studio)]
    [InlineData(ThemePalette.StudioBlue)]
    [InlineData(ThemePalette.Contrast)]
    public void EveryPalette_HasALightAndADarkVariantInTheStylesheet(ThemePalette palette)
    {
        // Arrange
        var minified = File.ReadAllText(RepositoryFiles.ThemingCss("palettes.min.css"));

        // Act & Assert
        foreach (var mode in new[] { "light", "dark" })
        {
            minified.ShouldContain($":root[data-osirion-palette=\"{palette.ToId()}\"][data-osirion-theme=\"{mode}\"]");
        }
    }

    [Fact]
    public void EveryPaletteId_IsKnownToThePageScripts()
    {
        // Arrange
        var scripts = new[]
        {
            File.ReadAllText(RepositoryFiles.Theming("wwwroot", "js", "osirion-styles.js")),
            File.ReadAllText(RepositoryFiles.Theming("wwwroot", "js", "theme-picker.js")),
            File.ReadAllText(RepositoryFiles.Theming("Components", "OsirionStyles.razor")),
        };

        // Act & Assert
        foreach (var palette in Enum.GetValues<ThemePalette>())
        {
            foreach (var script in scripts)
            {
                script.ShouldContain($"'{palette.ToId()}'");
            }
        }
    }

    private static string Normalize(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal);
}
