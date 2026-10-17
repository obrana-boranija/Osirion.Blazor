using System.Text.RegularExpressions;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

/// <summary>Checks rules in the content views' own stylesheets, which no component test renders.</summary>
public partial class ContentViewStylesheetTests
{
    // Fails before the fix: scoped CSS gives `.osirion-content-body a` the view's own scope, so the rule never reached
    // the rendered body (markup without that scope) and matched only the view's own links once the view sat inside
    // another component's `.osirion-content-body`. There it painted the category and tag badges in the link color on
    // their badge backgrounds: 4.37:1 in the light theme and 3:1 in the dark one.
    [Theory]
    [InlineData("ContentView.razor.css")]
    [InlineData("LocalizedContentView.razor.css")]
    public void ContentBodyLinkRules_DoNotReachTheViewsOwnLinks(string file)
    {
        // Act
        var css = Squash(ComponentCss(file));

        // Assert
        BodyLinkSelector().IsMatch(css).ShouldBeFalse();
    }

    private static string Squash(string css) => WhitespaceRun().Replace(CssComment().Replace(css, string.Empty), " ");

    private static string ComponentCss(string file)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return File.ReadAllText(Path.Combine(directory.FullName, "src", "Osirion.Blazor.Cms.Web", "Components", file));
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }

    [GeneratedRegex(@"\.osirion-content-body a\b")]
    private static partial Regex BodyLinkSelector();

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex CssComment();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();
}
