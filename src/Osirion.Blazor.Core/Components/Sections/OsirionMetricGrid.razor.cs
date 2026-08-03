using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Renders a responsive, accessible list of metric cards.</summary>
public partial class OsirionMetricGrid : OsirionComponentBase
{
    /// <summary>Gets or sets the metrics to render.</summary>
    [Parameter] public IReadOnlyList<MetricGridItem> Metrics { get; set; } = [];

    /// <summary>Gets or sets the number of columns used at larger viewport widths.</summary>
    [Parameter] public MetricGridColumns Columns { get; set; } = MetricGridColumns.Four;

    /// <summary>Gets or sets the accessible name for the metric list.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Key metrics";

    /// <summary>Gets or sets additional classes for the grid.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Describes a metric card rendered by the grid.</summary>
    public record MetricGridItem(
        string Value,
        string? Label = null,
        string? Description = null,
        string Accent = "green",
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