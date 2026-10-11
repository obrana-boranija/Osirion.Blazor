# OsirionPricingTable and OsirionFeatureComparison

Purpose
`OsirionPricingTable` shows plans side by side: name, price, period, description, features and a call to action,
with the recommended plan set apart. `OsirionFeatureComparison` is the detailed table that usually follows it,
with check marks, crosses and values per plan. Both render without script; the buttons take the classes of the
configured CSS framework.

Key parameters
- OsirionPricingTable: `Plans` (`IReadOnlyList<PricingPlan>`, in `Osirion.Blazor.Core.Models`), `Class`
- `PricingPlan`: `Name` and `Price` (required), `Period`, `Description`, `Features`, `CallToActionText`,
  `CallToActionUrl`, `Highlighted` (primary button and accent border), `Badge`
- OsirionFeatureComparison: `PlanNames`, `Rows` (`FeatureComparisonRow(Feature, Values)`), `Caption`
  (default "Compare plans"), `ShowCaption`, `FeatureColumnText`, `IncludedText`, `NotIncludedText`
- `ComparisonCell`: `true` or `false` (check or cross) or text, converted implicitly

Usage
```razor
<OsirionPricingTable Plans="plans" />
<OsirionFeatureComparison PlanNames="@(["Starter", "Team"])" Rows="rows" />

@code {
    private readonly PricingPlan[] plans =
    [
        new() { Name = "Starter", Price = "Free", Features = ["1 site"], CallToActionText = "Get started", CallToActionUrl = "/start" },
        new() { Name = "Team", Price = "$29", Period = "per month", Features = ["5 sites", "Analytics"],
                CallToActionText = "Start trial", CallToActionUrl = "/trial", Highlighted = true, Badge = "Most popular" },
    ];

    private readonly FeatureComparisonRow[] rows =
    [
        new("Static SSR", [true, true]),
        new("Single sign-on", [false, "Add-on"]),
    ];
}
```

Notes
- Plans are a list (`ul`), so screen readers announce how many there are. Each button's accessible name includes
  the plan ("Start trial: Team").
- Only http(s), `mailto:` and site-relative call-to-action URLs are rendered; a plan with another URL shows no button.
- Check marks and crosses are drawn in CSS and read as `IncludedText` and `NotIncludedText`. Rows shorter than the
  number of plans leave the remaining cells empty.
- The comparison scrolls sideways on narrow screens; the scroll region is focusable and named by the caption.
