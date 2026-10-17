using System.Text.RegularExpressions;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

/// <summary>Checks rules in the CMS navigation components' own stylesheets, which no component test renders.</summary>
public partial class NavigationStylesheetTests
{
    // Fails before the fix: the active category's count drew the link color on a 20% primary tint over the active
    // background, 3.39:1 in the light theme and 2.78:1 in the dark one.
    [Fact]
    public void CategoriesList_ActiveCount_UsesTheActiveTextOnTheActiveBackground()
    {
        // Act
        var activeCount = ActiveCategoryCountRule().Match(Squash(NavigationCss("CategoriesList.razor.css"))).Value;

        // Assert
        activeCount.ShouldContain("color:inherit;");
        activeCount.ShouldContain("background-color:transparent;");
        activeCount.ShouldNotContain("--osirion-link-color");
    }

    // Fails before the fix: the count faded the tag's text to 75% opacity, #7f8284 on #f8f9fa, 3.66:1.
    [Fact]
    public void TagCloud_Count_KeepsTheTagTextColor()
    {
        // Act
        var count = TagCountRule().Match(Squash(NavigationCss("TagCloud.razor.css"))).Value;

        // Assert
        count.ShouldNotBeEmpty();
        count.ShouldNotContain("opacity");
    }

    private static string Squash(string css) => WhitespaceRun().Replace(CssComment().Replace(css, string.Empty), string.Empty);

    private static string NavigationCss(string file)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return File.ReadAllText(Path.Combine(
                    directory.FullName, "src", "Osirion.Blazor.Cms.Web", "Components", "Navigation", file));
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }

    [GeneratedRegex(@"\.osirion-category-link\.osirion-active\.osirion-category-count\{[^}]*\}")]
    private static partial Regex ActiveCategoryCountRule();

    [GeneratedRegex(@"(?:^|\})\.osirion-tag-count\{[^}]*\}")]
    private static partial Regex TagCountRule();

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex CssComment();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();
}
