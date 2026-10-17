using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Renders a responsive, accessible list of metric cards.</summary>
public partial class OsirionMetricGrid : OsirionComponentBase
{
    /// <summary>
    /// The metrics, one <see cref="OsirionMetricCard"/> each; with none, nothing is rendered.
    /// </summary>
    [Parameter] public IReadOnlyList<MetricGridItem> Metrics { get; set; } = [];

    /// <summary>
    /// The number of columns from 992px wide: <c>Two</c>, <c>Three</c> or <c>Four</c>. The grid has one column on
    /// phones and two from 576px.
    /// </summary>
    [Parameter] public MetricGridColumns Columns { get; set; } = MetricGridColumns.Four;

    /// <summary>
    /// The accessible name of the metric list.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "Key metrics";

    /// <summary>
    /// Extra CSS classes on the list. <see cref="OsirionComponentBase.Class"/> is not applied by this component.
    /// </summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Describes a metric card rendered by the grid.</summary>
    public record MetricGridItem(
        string Value,
        string? Label = null,
        string? Description = null,
        MetricCardAccent Accent = MetricCardAccent.Green,
        string? Kicker = null,
        bool Elevated = false,
        bool Animate = false);
}

/// <summary>Defines the large-screen column count for <see cref="OsirionMetricGrid" />.</summary>
public enum MetricGridColumns
{
    /// <summary>Two columns.</summary>
    Two = 2,

    /// <summary>Three columns.</summary>
    Three = 3,

    /// <summary>Four columns.</summary>
    Four = 4
}
