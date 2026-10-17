using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;
using static Osirion.Blazor.Components.OsirionMetricGrid;

namespace Osirion.Blazor.Core.Tests.Components.Sections;

public class OsirionMetricGridTests : OsirionComponentTestBase
{
    public OsirionMetricGridTests()
    {
        Services.AddJSComponents();
    }

    private IRenderedComponent<OsirionMetricGrid> RenderGrid(
        IReadOnlyList<MetricGridItem> metrics,
        Action<ComponentParameterCollectionBuilder<OsirionMetricGrid>>? parameterBuilder = null) =>
        RenderStatic<OsirionMetricGrid>(parameters =>
        {
            parameters.Add(p => p.Metrics, metrics);
            parameterBuilder?.Invoke(parameters);
        });

    [Fact]
    public void MetricGrid_NoMetrics_RendersNothing()
    {
        var cut = RenderGrid([]);

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void MetricGrid_Defaults_RenderAFourColumnListWithTheDefaultLabel()
    {
        var cut = RenderGrid([new MetricGridItem("42")]);

        var list = cut.Find("ul.osirion-metric-grid");
        list.ClassList.ShouldContain("osirion-metric-grid-columns-4");
        list.GetAttribute("aria-label").ShouldBe("Key metrics");
    }

    [Theory]
    [InlineData(MetricGridColumns.Two, "osirion-metric-grid-columns-2")]
    [InlineData(MetricGridColumns.Three, "osirion-metric-grid-columns-3")]
    public void MetricGrid_Columns_MapToTheColumnClass(MetricGridColumns columns, string expected)
    {
        var cut = RenderGrid([new MetricGridItem("42")], p => p.Add(c => c.Columns, columns));

        cut.Find("ul").ClassList.ShouldContain(expected);
    }

    [Fact]
    public void MetricGrid_LabelClassAndAttributes_AreAppliedToTheList()
    {
        var cut = RenderGrid([new MetricGridItem("42")], p => p
            .Add(c => c.AriaLabel, "Results")
            .Add(c => c.CssClass, "my-grid")
            .AddUnmatched("data-test", "grid"));

        var list = cut.Find("ul");
        list.GetAttribute("aria-label").ShouldBe("Results");
        list.ClassList.ShouldContain("my-grid");
        list.GetAttribute("data-test").ShouldBe("grid");
    }

    // Fails before the fix: Class and Style were ignored, unlike every other component.
    [Fact]
    public void MetricGrid_ClassAndStyle_AreAppliedToTheList()
    {
        var cut = RenderGrid([new MetricGridItem("42")], p => p
            .Add(c => c.Class, "wide")
            .Add(c => c.Style, "margin-top: 2rem"));

        var list = cut.Find("ul");
        list.ClassList.ShouldContain("wide");
        list.GetAttribute("style").ShouldBe("margin-top: 2rem");
    }

    // Fails before the fix: the list style is removed in CSS, which drops the list semantics in Safari, and the
    // list had no role to restore them.
    [Fact]
    public void MetricGrid_List_KeepsItsListRole()
    {
        var cut = RenderGrid([new MetricGridItem("42")]);

        cut.Find("ul").GetAttribute("role").ShouldBe("list");
    }

    [Fact]
    public void MetricGrid_Metrics_RenderOneFullHeightCardPerItemInOrder()
    {
        var cut = RenderGrid(
        [
            new MetricGridItem("99%", "Uptime", "Last year", MetricCardAccent.Blue, "SLA", Elevated: true, Animate: true),
            new MetricGridItem("3x"),
        ]);

        var items = cut.FindAll("ul > li > article.osirion-metric-card");
        items.Count.ShouldBe(2);
        items[0].ClassList.ShouldNotContain("h-100");
        items[0].ClassList.ShouldContain("osirion-metric-card-blue");
        items[0].ClassList.ShouldContain("osirion-metric-card-shadow-large");
        items[0].QuerySelector(".osirion-metric-card-kicker")!.TextContent.ShouldBe("SLA");
        items[0].QuerySelector(".osirion-metric-card-value")!.TextContent.ShouldBe("99%");
        items[0].QuerySelector(".osirion-metric-card-value")!.GetAttribute("data-osirion-metric").ShouldBe("true");
        items[0].QuerySelector(".osirion-metric-card-label")!.TextContent.ShouldBe("Uptime");
        items[0].QuerySelector(".osirion-metric-card-description")!.TextContent.ShouldBe("Last year");
        items[1].ClassList.ShouldContain("osirion-metric-card-green");
        items[1].ClassList.ShouldContain("osirion-metric-card-shadow-medium");
        items[1].QuerySelector(".osirion-metric-card-value")!.TextContent.ShouldBe("3x");
    }
}
