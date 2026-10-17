using System.Text.RegularExpressions;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Configuration;

/// <summary>Checks rules in the components' own stylesheets, which no component test renders.</summary>
public partial class ComponentStylesheetTests
{
    // Fails before the fix: the heading used a fixed dark green (#123b2b) that measures 1.61:1 on the dark theme.
    [Fact]
    public void ContextCardGrid_Heading_FallsBackToTheThemeTextColor()
    {
        // Act
        var css = Squash(ComponentCss("Sections", "OsirionContextCardGrid.razor.css"));

        // Assert
        css.ShouldContain("color:var(--osirion-context-card-grid-heading-color,var(--osirion-text-primary,");
        css.ShouldNotContain("#123b2b");
    }

    private static string Squash(string css) => WhitespaceRun().Replace(CssComment().Replace(css, string.Empty), string.Empty);

    private static string ComponentCss(string folder, string file)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return File.ReadAllText(Path.Combine(directory.FullName, "src", "Osirion.Blazor.Core", "Components", folder, file));
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex CssComment();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();
}
