using System.Text.RegularExpressions;
using Osirion.Blazor.Palettes;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Palettes;

/// <summary>
/// The framework adapters replace the primary color scale the themes build the active item from, so each adapter must
/// keep that pair readable on its own.
/// </summary>
public partial class AdapterContrastTests
{
    // Fails before the fix: in the dark theme the active side navigation item read 4.39:1 under the Bootstrap adapter
    // and 3.86:1 under the MudBlazor adapter.
    [Theory]
    [InlineData("bootstrap", "light")]
    [InlineData("bootstrap", "dark")]
    [InlineData("fluentui", "light")]
    [InlineData("fluentui", "dark")]
    [InlineData("mudblazor", "light")]
    [InlineData("mudblazor", "dark")]
    [InlineData("radzen", "light")]
    [InlineData("radzen", "dark")]
    public void Adapter_ActiveItemText_ReachesWcagAaOnItsBackground(string adapter, string mode)
    {
        // Arrange: the theme names the scale steps, the adapter gives them colors
        var theme = Block(RepositoryFiles.ThemingCss("index.min.css"),
            selector => selector.Contains($":root[data-osirion-theme={mode}]", StringComparison.Ordinal), "nav-active-text");
        var colors = Block(RepositoryFiles.ThemingCss($"adapters/{adapter}-adapter.min.css"),
            selector => selector.Contains("dark", StringComparison.Ordinal) == (mode == "dark"), "color-primary-700");
        var text = Step(theme["nav-active-text"]);
        var background = Step(theme["nav-active-background"]);

        // Act
        var ratio = Rgb.Contrast(Rgb.Parse(Hex(colors[text])), Rgb.Parse(Hex(colors[background])));

        // Assert
        ratio.ShouldBeGreaterThanOrEqualTo(4.5, $"{adapter} {mode}: {text} {colors[text]} on {background} {colors[background]}");
    }

    // Fails before the fix: in the dark theme a content category badge drew primary-600 on primary-200, 3.74:1 under
    // the Bootstrap adapter with Bootstrap's own colors, and 4.45:1 on this repository's example site.
    [Theory]
    [InlineData("bootstrap", "light")]
    [InlineData("bootstrap", "dark")]
    [InlineData("fluentui", "light")]
    [InlineData("fluentui", "dark")]
    [InlineData("mudblazor", "light")]
    [InlineData("mudblazor", "dark")]
    [InlineData("radzen", "light")]
    [InlineData("radzen", "dark")]
    public void Adapter_CategoryBadgeText_ReachesWcagAaOnItsBackground(string adapter, string mode)
    {
        // Arrange: the theme names the scale steps, the adapter gives them colors
        var theme = Block(RepositoryFiles.ThemingCss("index.min.css"),
            selector => selector.Contains($":root[data-osirion-theme={mode}]", StringComparison.Ordinal), "category-text-color");
        var colors = Block(RepositoryFiles.ThemingCss($"adapters/{adapter}-adapter.min.css"),
            selector => selector.Contains("dark", StringComparison.Ordinal) == (mode == "dark"), "color-primary-700");
        var text = Step(theme["category-text-color"]);
        var background = Step(theme["category-background-color"]);

        // Act
        var ratio = Rgb.Contrast(Rgb.Parse(Hex(colors[text])), Rgb.Parse(Hex(colors[background])));

        // Assert
        ratio.ShouldBeGreaterThanOrEqualTo(4.5, $"{adapter} {mode}: {text} {colors[text]} on {background} {colors[background]}");
    }

    // The custom properties of the first rule whose selector matches and which declares the token.
    private static Dictionary<string, string> Block(string file, Func<string, bool> selector, string token)
    {
        var rule = Rule().Matches(File.ReadAllText(file))
            .First(match => selector(match.Groups["selector"].Value)
                && match.Groups["body"].Value.Contains("--osirion-" + token + ":", StringComparison.Ordinal));
        var tokens = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (Match declaration in Declaration().Matches(rule.Groups["body"].Value))
        {
            tokens[declaration.Groups["name"].Value] = declaration.Groups["value"].Value.Trim();
        }

        return tokens;
    }

    // "var(--osirion-color-primary-700)" names the step "color-primary-700".
    private static string Step(string value) => StepReference().Match(value).Groups["name"].Value;

    // A color, or a framework variable's fallback color, as in "var(--bs-primary-bg-subtle, #cfe2ff)".
    private static string Hex(string value) => HexColor().Match(value).Value;

    [GeneratedRegex(@"(?<selector>[^{}]+)\{(?<body>[^{}]*)\}")]
    private static partial Regex Rule();

    [GeneratedRegex(@"--osirion-(?<name>[\w-]+):(?<value>[^;}]+)")]
    private static partial Regex Declaration();

    [GeneratedRegex(@"^var\(--osirion-(?<name>[\w-]+)\)$")]
    private static partial Regex StepReference();

    [GeneratedRegex(@"#[0-9a-fA-F]{6}\b|#[0-9a-fA-F]{3}\b")]
    private static partial Regex HexColor();
}
