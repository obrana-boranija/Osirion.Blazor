using System.Text.RegularExpressions;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Configuration;

/// <summary>Checks rules in the read more link's own stylesheet, which no component test renders.</summary>
public partial class ReadMoreLinkStylesheetTests
{
    // Fails before the fix: the button variant set white text at the same specificity as the dark scheme's link color
    // rule, which comes later in the file, so under a dark system preference the button text took the link color on the
    // primary fill: #9090d6 on #5c5cb0, 1.97:1.
    [Fact]
    public void ButtonVariant_TextColor_OutranksTheDarkSchemeLinkColor()
    {
        // Act
        var css = Squash(ReadMoreLinkCss());
        var button = ButtonRule().Match(css).Value;
        var hover = ButtonHoverRule().Match(css).Value;

        // Assert
        button.ShouldContain("color:var(--osirion-action-primary-text,#fff);");
        hover.ShouldContain("color:var(--osirion-action-primary-text,#fff);");
        css.ShouldNotContain("color:white");
    }

    // Fails before the fix: every focus indicator keyed off :focus, so a mouse click on the link drew the outline and the
    // button ring, which only keyboard users need.
    [Fact]
    public void FocusIndicators_UseFocusVisible()
    {
        // Act
        var css = Squash(ReadMoreLinkCss());

        // Assert
        css.ShouldContain(".osirion-read-more:focus-visible{outline:");
        css.ShouldContain(".osirion-read-more-button:focus-visible{box-shadow:");
        Regex.IsMatch(css, @":focus(?![-\w])").ShouldBeFalse();
    }

    private static string Squash(string css) => WhitespaceRun().Replace(CssComment().Replace(css, string.Empty), string.Empty);

    private static string ReadMoreLinkCss()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return File.ReadAllText(Path.Combine(
                    directory.FullName, "src", "Osirion.Blazor.Core", "Components", "Navigation", "OsirionReadMoreLink.razor.css"));
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }

    [GeneratedRegex(@"(?:^|\})\.osirion-read-more\.osirion-read-more-button\{[^}]*\}")]
    private static partial Regex ButtonRule();

    [GeneratedRegex(@"(?:^|\})\.osirion-read-more\.osirion-read-more-button:hover\{[^}]*\}")]
    private static partial Regex ButtonHoverRule();

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex CssComment();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();
}
