using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Cards;

public class OsirionMetricCardTests : OsirionComponentTestBase
{
    public OsirionMetricCardTests()
    {
        Services.AddJSComponents();
    }

    private IRenderedComponent<OsirionMetricCard> RenderCard(
        string value,
        Action<ComponentParameterCollectionBuilder<OsirionMetricCard>>? parameterBuilder = null) =>
        RenderStatic<OsirionMetricCard>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameterBuilder?.Invoke(parameters);
        });

    [Fact]
    public void MetricCard_Defaults_RenderValueWithDefaultClasses()
    {
        var cut = RenderCard("42");

        var card = cut.Find("article.osirion-metric-card");
        card.ClassList.ShouldContain("osirion-metric-card-green");
        card.ClassList.ShouldContain("osirion-metric-card-border-accent");
        card.ClassList.ShouldContain("osirion-metric-card-shadow-medium");
        cut.Find(".osirion-metric-card-value").TextContent.ShouldBe("42");
        cut.Find(".osirion-metric-card-value").GetAttribute("data-osirion-metric").ShouldBe("false");
    }

    [Fact]
    public void MetricCard_OptionalTexts_RenderWhenProvided()
    {
        var cut = RenderCard("42", p => p
            .Add(c => c.Kicker, "Uptime")
            .Add(c => c.Label, "Requests")
            .Add(c => c.Description, "Per second"));

        cut.Find(".osirion-metric-card-kicker").TextContent.ShouldBe("Uptime");
        cut.Find(".osirion-metric-card-label").TextContent.ShouldBe("Requests");
        cut.Find(".osirion-metric-card-description").TextContent.ShouldBe("Per second");
    }

    [Fact]
    public void MetricCard_BlankOptionalTexts_RenderNothing()
    {
        var cut = RenderCard("42", p => p
            .Add(c => c.Kicker, " ")
            .Add(c => c.Label, " ")
            .Add(c => c.Description, " "));

        cut.FindAll(".osirion-metric-card-kicker").ShouldBeEmpty();
        cut.FindAll(".osirion-metric-card-label").ShouldBeEmpty();
        cut.FindAll(".osirion-metric-card-description").ShouldBeEmpty();
    }

    [Fact]
    public void MetricCard_AccentBorderShadowAndCssClass_MapToClasses()
    {
        var cut = RenderCard("42", p => p
            .Add(c => c.Accent, MetricCardAccent.Indigo)
            .Add(c => c.Border, MetricCardBorder.None)
            .Add(c => c.Shadow, MetricCardShadow.SmallAccent)
            .Add(c => c.CssClass, "h-100")
            .AddUnmatched("data-test", "metric"));

        var card = cut.Find("article.osirion-metric-card");
        card.ClassList.ShouldContain("osirion-metric-card-indigo");
        card.ClassList.ShouldContain("osirion-metric-card-border-none");
        card.ClassList.ShouldContain("osirion-metric-card-shadow-smallaccent");
        card.ClassList.ShouldContain("h-100");
        card.GetAttribute("data-test").ShouldBe("metric");
    }

    [Theory]
    [InlineData(MetricCardShadow.None, "large")]
    [InlineData(MetricCardShadow.Small, "large")]
    [InlineData(MetricCardShadow.Medium, "large")]
    [InlineData(MetricCardShadow.Large, "large")]
    [InlineData(MetricCardShadow.SmallAccent, "largeaccent")]
    [InlineData(MetricCardShadow.MediumAccent, "largeaccent")]
    [InlineData(MetricCardShadow.LargeAccent, "largeaccent")]
    public void MetricCard_Elevated_UpgradesTheShadowToLarge(MetricCardShadow shadow, string expected)
    {
        var cut = RenderCard("42", p => p
            .Add(c => c.Shadow, shadow)
            .Add(c => c.Elevated, true));

        cut.Find("article").ClassList.ShouldContain($"osirion-metric-card-shadow-{expected}");
    }

    [Fact]
    public void MetricCard_Animate_FlagsTheValueForAnimation()
    {
        var cut = RenderCard("42", p => p.Add(c => c.Animate, true));

        cut.Find(".osirion-metric-card-value").GetAttribute("data-osirion-metric").ShouldBe("true");
    }

    [Theory]
    [InlineData("99.99%", "", "99.99", "%")]
    [InlineData("$1,200+", "$", "1,200", "+")]
    [InlineData("18-32%", "", "18-32", "%")]
    [InlineData("18–32 ms", "", "18–32", " ms")]
    [InlineData("  42  ", "", "42", "")]
    [InlineData(".5x", "", ".5", "x")]
    [InlineData("~5-", "~", "5-", "")]
    [InlineData("N/A", "N/A", "", "")]
    [InlineData("", "", "", "")]
    public void MetricCard_Value_IsSplitIntoPrefixNumberAndSuffix(string value, string prefix, string number, string suffix)
    {
        var cut = RenderCard(value);

        var element = cut.Find(".osirion-metric-card-value");
        element.GetAttribute("data-prefix").ShouldBe(prefix);
        element.GetAttribute("data-count").ShouldBe(number);
        element.GetAttribute("data-suffix").ShouldBe(suffix);
        element.TextContent.ShouldBe(value);
    }

    [Fact]
    public void MetricCard_ValueChange_RecomputesTheNumericParts()
    {
        var cut = RenderCard("10%");

        cut.Render(p => p.Add(c => c.Value, "$7k"));

        var element = cut.Find(".osirion-metric-card-value");
        element.GetAttribute("data-prefix").ShouldBe("$");
        element.GetAttribute("data-count").ShouldBe("7");
        element.GetAttribute("data-suffix").ShouldBe("k");
    }
}
