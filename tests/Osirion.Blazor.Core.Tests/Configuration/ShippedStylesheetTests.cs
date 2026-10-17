using Shouldly;

namespace Osirion.Blazor.Core.Tests.Configuration;

/// <summary>Checks rules in the stylesheets the package ships, which no component test renders.</summary>
public class ShippedStylesheetTests
{
    private const string FrameworkNoneBody =
        ":where(html[data-osirion-framework=None]) body{background-color:var(--osirion-background-primary,#fff);color:var(--osirion-text-primary,#111827);font-family:var(--osirion-font-family-base,system-ui,sans-serif)}";

    // Fails before the fix: no shipped stylesheet painted the body, so a site without a CSS framework kept the browser's
    // white page in the dark theme and drew light text on it.
    [Theory]
    [InlineData("index.min.css")]
    [InlineData("base/base-styles.min.css")]
    public void MinifiedStylesheet_FrameworkNone_PaintsTheBodyInTheThemeColors(string file)
    {
        // Act
        var css = File.ReadAllText(CoreCss(file));

        // Assert
        css.ShouldContain(FrameworkNoneBody);
    }

    [Fact]
    public void BodyRule_IsScopedToFrameworkNoneAtZeroSpecificity()
    {
        // Act
        var css = File.ReadAllText(CoreCss("index.min.css"));

        // Assert: a framework paints its own body, and any rule a site writes for body must win
        css.ShouldNotContain("}body{background", Case.Sensitive);
        css.ShouldContain(":where(html[data-osirion-framework=None]) body{");
    }

    // Fails before the fix: every text link in rendered Markdown carried the external-link icon, so a link to
    // /blog/another-post looked as if it left the site.
    [Fact]
    public void HtmlRenderer_ExternalLinkIcon_IsOnlyOnAddressesThatLeaveTheSite()
    {
        // Act
        var css = File.ReadAllText(CoreCss("osirion-html-renderer.min.css"));
        var iconRules = System.Text.RegularExpressions.Regex.Matches(css, @"([^{}]*)::after\{content:")
            .Select(match => match.Groups[1].Value)
            .Where(selector => selector.Contains(".osirion-html-renderer a", StringComparison.Ordinal))
            .ToList();

        // Assert
        iconRules.ShouldNotBeEmpty();
        iconRules.ShouldAllBe(selector => selector.Contains("a:is([href^=\"http:\"],[href^=\"https:\"],[href^=\"//\"])", StringComparison.Ordinal));
    }

    // Fails before the fix: the medium card was 450 pixels wide whatever its column, so on a 375 pixel screen it pushed
    // the page 116 pixels wider than the screen.
    [Fact]
    public void FeatureCard_FixedSizes_NeverGrowPastTheirContainer()
    {
        // Act
        var css = Squash(File.ReadAllText(ComponentCss("Cards", "OsirionFeatureCard.razor.css")));

        // Assert
        css.ShouldContain(".osirion-feature-card,.osirion-feature-card-image-container,.osirion-feature-card-image{max-width:100%;}");
    }

    // Fails before the fix: the value took the accent as it is; green on the Bootstrap adapter's light card read 2.78:1
    // and indigo on its dark card 1.83:1, below the 3:1 large text needs.
    [Fact]
    public void MetricCard_Value_MovesTheAccentTowardTheTextColor()
    {
        // Act
        var css = Squash(File.ReadAllText(ComponentCss("Cards", "OsirionMetricCard.razor.css")));

        // Assert
        css.ShouldContain(".osirion-metric-card-value{margin-bottom:0.5rem;color:color-mix(insrgb,var(--osirion-metric-card-accent)65%,var(--osirion-text-primary,#111827));");
    }

    // Fails before the fix: the content of an OsirionReveal started at opacity 0 and only the script revealed it, and the
    // Down, Left and Right rules moved it 1.25rem with !important even when the script never ran, so with script blocked,
    // Animate off or reduced motion the content stayed invisible or offset.
    [Fact]
    public void Reveal_HiddenStateAppliesOnlyAfterTheScriptArmsTheElement()
    {
        // Act
        var css = Squash(File.ReadAllText(ComponentCss("Rendering", "OsirionReveal.razor.css")));
        var script = File.ReadAllText(Path.Combine(Path.GetDirectoryName(ComponentCss("Rendering", "OsirionReveal.razor.css"))!, "OsirionReveal.razor.js"));

        // Assert
        css.ShouldNotContain("data-osirion-reveal");
        css.ShouldNotContain("!important");
        css.ShouldContain(".osirion-reveal.osirion-reveal-armed{opacity:0;");
        css.ShouldContain(".osirion-reveal-armed.osirion-reveal-left{transform:translateX(1.25rem);}");
        script.ShouldContain("classList.add('osirion-reveal-armed')");
    }

    private static string Squash(string css) => System.Text.RegularExpressions.Regex.Replace(
        System.Text.RegularExpressions.Regex.Replace(css, @"/\*.*?\*/", string.Empty, System.Text.RegularExpressions.RegexOptions.Singleline),
        @"\s+",
        string.Empty);

    private static string ComponentCss(string folder, string file) =>
        Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(CoreCss(file))!)!, "..", "Components", folder, file);

    private static string CoreCss(string file)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return Path.Combine(directory.FullName, "src", "Osirion.Blazor.Core", "wwwroot", "css", file);
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }
}
