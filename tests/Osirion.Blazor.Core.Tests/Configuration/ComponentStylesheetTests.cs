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

    // Fails before the fix: the grid passed the Bootstrap-only class h-100 to each card and had no rule of its own
    // that makes a card fill its cell.
    [Fact]
    public void MetricGrid_Cells_StretchTheirCardWithoutAFrameworkClass()
    {
        // Act
        var css = Squash(ComponentCss("Sections", "OsirionMetricGrid.razor.css"));

        // Assert
        css.ShouldContain(".osirion-metric-grid>li>*{");
        css.ShouldContain(".osirion-metric-grid>li>*{height:100%;}");
    }

    // Fails before the fix: rgba(var(--token, #hex), .05) is invalid when the token holds a color, so the browser
    // dropped the whole declaration and the tech wave and circuit backgrounds were not painted.
    [Fact]
    public void BackgroundPattern_Colors_AreNotBuiltByWrappingATokenInRgba()
    {
        // Act
        var css = Squash(ComponentCss("Layout", "OsirionBackgroundPattern.razor.css"));

        // Assert
        css.ShouldNotContain("rgba(var(");
        css.ShouldContain("color-mix(insrgb,var(--osirion-action-primary,#1e40af)5%,transparent)");
    }

    // Fails before the fix: the prefixed declaration had the stops 52% then 44%, which are out of order, while the
    // standard one faded out toward the bottom, as the page documents, so browsers using each rendered a different mask.
    [Fact]
    public void BackgroundPattern_ImageMask_IsTheSameSoftFadeInBothDeclarations()
    {
        // Act
        var css = ComponentCss("Layout", "OsirionBackgroundPattern.razor.css");
        var masks = Regex.Matches(css, @"\.osirion-bg-image-mask\s*\{(?<body>[^}]*)\}").Single().Groups["body"].Value;
        var prefixed = Regex.Match(masks, @"-webkit-mask-image:\s*(?<value>[^;]+);").Groups["value"].Value;
        var standard = Regex.Match(masks, @"(?<!-)mask-image:\s*(?<value>[^;]+);").Groups["value"].Value;

        // Assert
        prefixed.ShouldNotBeNullOrWhiteSpace();
        Squash(prefixed).ShouldBe(Squash(standard));
        Squash(standard).ShouldBe("linear-gradient(180deg,#0000%,transparent100%)");
    }

    // Fails before the fix: the animated patterns kept moving for visitors who ask for reduced motion.
    [Theory]
    [InlineData(".osirion-bg-grid-animated")]
    [InlineData(".osirion-bg-tech-wave::before")]
    public void BackgroundPattern_AnimatedPatterns_StopUnderReducedMotion(string selector)
    {
        // Act
        var css = Squash(ComponentCss("Layout", "OsirionBackgroundPattern.razor.css"));
        var block = Regex.Match(css, @"@media\(prefers-reduced-motion:reduce\)\{(?<body>.*?\}\})").Groups["body"].Value;

        // Assert
        block.ShouldContain(selector + "{animation:none;}");
    }

    // Fails before the fix: the spinner kept turning for visitors who ask for reduced motion.
    [Fact]
    public void PageLoading_Spinner_StopsUnderReducedMotion()
    {
        // Act
        var css = Squash(ComponentCss("States", "OsirionPageLoading.razor.css"));

        // Assert
        css.ShouldContain("@media(prefers-reduced-motion:reduce){.osirion-spinner{animation:none;}}");
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
