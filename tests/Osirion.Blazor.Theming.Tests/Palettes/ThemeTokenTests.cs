using System.Text.RegularExpressions;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Palettes;

/// <summary>Component styles must read tokens the themes define, or their fallback shows in both themes.</summary>
public partial class ThemeTokenTests
{
    // Fails before the fix: eleven components read --osirion-background-active, which no theme defined, so its light
    // fallback (#e0f2fe) stayed in the dark theme under light text (DocumentPage's active item: 2.21:1).
    [Fact]
    public void ComponentStyles_DoNotReadTheUndefinedActiveBackgroundToken()
    {
        // Act
        var offenders = ComponentStylesheets()
            .Where(file => File.ReadAllText(file).Contains("var(--osirion-background-active", StringComparison.Ordinal))
            .Select(file => Path.GetRelativePath(RepositoryFiles.Source(), file))
            .ToList();

        // Assert
        offenders.ShouldBeEmpty();
    }

    // Fails before the fix: hover states read --osirion-background-hover, which no theme defined, so a hovered item in
    // the dark theme turned light grey under light text.
    [Theory]
    [InlineData("light")]
    [InlineData("dark")]
    public void ThemeFile_DefinesTheHoverBackgroundToken(string mode)
    {
        // Act
        var css = File.ReadAllText(RepositoryFiles.ThemingCss("index.min.css"));
        var start = css.IndexOf($":root[data-osirion-theme={mode}]", StringComparison.Ordinal);
        var body = css[css.IndexOf('{', start)..css.IndexOf('}', start)];

        // Assert
        HoverToken().IsMatch(body).ShouldBeTrue();
    }

    private static IEnumerable<string> ComponentStylesheets() =>
        Directory.EnumerateFiles(RepositoryFiles.Source(), "*.razor.css", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                && !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal));

    [GeneratedRegex(@"--osirion-background-hover:var\(--osirion-nav-hover-background\)")]
    private static partial Regex HoverToken();
}
