# OsirionMetricGrid

Purpose
Lays out a set of `OsirionMetricCard` components as a responsive list. Each metric is described by a
`MetricGridItem` record, so the grid needs no child markup.

Key parameters
- Metrics: `IReadOnlyList<OsirionMetricGrid.MetricGridItem>`, default empty. The metrics to render, one card each. When
  the list is empty the component renders nothing
- Columns: `MetricGridColumns` (default `Four`): `Two`, `Three` or `Four` columns at large widths
- AriaLabel: string, default "Key metrics". The accessible name of the list
- CssClass: string, optional. Extra CSS classes added to the list element
- Theme: `ThemeMode` (default `System`), inherited from `OsirionComponentBase`. `Light`, `Dark` and `Inverted` add a
  `data-theme` attribute (`data-bs-theme` when the Bootstrap framework is configured)
- Class and Style: strings, inherited from `OsirionComponentBase`. Both are applied to the list element
- Attributes: captures unmatched attributes (`CaptureUnmatchedValues`) and renders them on the list element

`MetricGridItem` is a record: `MetricGridItem(string Value, string? Label = null, string? Description = null,
MetricCardAccent Accent = MetricCardAccent.Green, string? Kicker = null, bool Elevated = false, bool Animate = false)`.
Each member is passed to the matching `OsirionMetricCard` parameter. The card's `Border` and `Shadow` are not
available through the item and keep their defaults.

Usage
```razor
<OsirionMetricGrid Columns="MetricGridColumns.Three"
                   AriaLabel="Service reliability metrics"
                   Metrics="@metrics" />

@code {
    private readonly IReadOnlyList<OsirionMetricGrid.MetricGridItem> metrics =
    [
        new("99.9%", "Availability", "Measured over twelve months", MetricCardAccent.Teal, Animate: true),
        new("24/7", "Monitoring", Kicker: "Operations"),
        new("15 min", "Response target", Accent: MetricCardAccent.Amber, Elevated: true)
    ];
}
```

Notes
- Emits `ul.osirion-metric-grid.osirion-metric-grid-columns-{2|3|4}` with `role="list"` and `aria-label`, followed by
  `CssClass` and `Class`. Each metric is an `li` (keyed by the item record) containing an `OsirionMetricCard`.
- The list is one column by default, two columns from 576px, and three or four columns from 992px when `Columns` asks
  for them (`Two` stays at two). The gap is `--osirion-metric-grid-gap` (default 1rem).
- Works in static SSR. Only the cards with `Animate: true` load their count-up script; see `OsirionMetricCard` for
  that script and its reduced-motion handling.
