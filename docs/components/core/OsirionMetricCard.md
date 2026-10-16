# OsirionMetricCard

Purpose
Shows one prominent metric (a value, an optional label and description, an optional kicker above it) in a card. The
value can count up when it scrolls into view. The full value text is always rendered by the server, so the card is
readable without script.

Key parameters
- Value: string, required. The complete display value, such as `99.99%` or `18-32%`. The first run of digits and dots
  (with anything between them, such as the dash of a range) is treated as the number to animate; text before it is the
  prefix and text after it is the suffix
- Label: string, optional. Text below the value
- Description: string, optional. Supporting text below the label
- Kicker: string, optional. Text above the value
- Accent: `MetricCardAccent` (default `Green`): `Primary`, `Secondary`, `Success`, `Warning`, `Danger`, `Info`, `Blue`,
  `Teal`, `Green`, `Amber`, `Purple` or `Indigo`
- Border: `MetricCardBorder` (default `Accent`): `None`, `Default` or `Accent`
- Shadow: `MetricCardShadow` (default `Medium`): `None`, `Small`, `Medium`, `Large`, `SmallAccent`, `MediumAccent` or
  `LargeAccent`
- Elevated: bool, default false. Upgrades a shadow smaller than `Large` to `Large` (an accent shadow becomes
  `LargeAccent`). It never downgrades a shadow you chose explicitly
- Animate: bool, default false. Enables the count-up animation of the numeric part
- CssClass: string, optional. Extra CSS classes added to the root element
- Theme: `ThemeMode` (default `System`), inherited from `OsirionComponentBase`. `Light`, `Dark` and `Inverted` add a
  `data-theme` attribute (`data-bs-theme` when the Bootstrap framework is configured)
- Class and Style: strings, inherited from `OsirionComponentBase`. The card's markup does not read them
- Attributes: captures unmatched attributes (`CaptureUnmatchedValues`) and renders them on the root element

Usage
```razor
<OsirionMetricCard Kicker="Reliability"
                   Value="99.99%"
                   Label="Availability"
                   Description="Measured over the last twelve months"
                   Accent="MetricCardAccent.Teal"
                   Border="MetricCardBorder.Default"
                   Shadow="MetricCardShadow.SmallAccent"
                   Animate="true" />
```

Notes
- Emits `article.osirion-metric-card` with the modifier classes `osirion-metric-card-{accent}`,
  `osirion-metric-card-border-{border}` and `osirion-metric-card-shadow-{shadow}` (lower case), followed by `CssClass`.
  Inside are `span.osirion-metric-card-kicker`, `div.osirion-metric-card-value`, `div.osirion-metric-card-label` and
  `div.osirion-metric-card-description`. The kicker, label and description are omitted when empty or whitespace.
- The value element carries `data-osirion-metric` (`true` or `false` from `Animate`), `data-count` (the numeric part),
  `data-prefix` and `data-suffix`. The component adds no ARIA roles.
- Works in static SSR: the `Value` text is rendered as is. A collocated script (loaded through `BlazorJSComponents`)
  adds the count-up only when `Animate` is true. It starts when at least 30% of the value is visible and runs for
  1200 ms. A range such as `18-32%` counts both ends, and decimal places in the source value are kept.
- Reduced motion: the script does not animate when the browser reports `prefers-reduced-motion: reduce`, so the
  final value stays.
- If the numeric part cannot be parsed, the script leaves the value unchanged.
- The card has a hover lift (`translateY`) on every shadow except `None`; the stylesheet has no reduced-motion rule for
  that transition.
- Accent colors come from `--osirion-metric-card-accent`, which each accent class sets from theme variables (for
  example `--osirion-status-success` or `--osirion-color-teal-600`) with fixed fallbacks.
