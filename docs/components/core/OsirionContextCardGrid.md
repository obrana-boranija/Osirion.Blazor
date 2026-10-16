# OsirionContextCardGrid

Purpose
Shows a section with a heading, optional intro text and a grid of labeled detail cards. Each card has a label, a short
description and an accent color on its top border.

Key parameters
- Title: string, default "Context by category". The section heading (`h2`)
- Description: string, optional. Supporting text under the heading
- Items: `OsirionContextCardGrid.ContextCardItem[]`, default empty. The cards; the card grid is omitted when the array
  is empty
- HeadingId: string, default "osirion-context-card-grid-heading". The id of the heading, referenced by the section's
  `aria-labelledby`. Set a unique value when more than one grid is on a page
- Id: string, optional. The section's `id`, for in-page links
- SectionClass: string, optional. Extra CSS classes on the section
- ContainerClass: string, default "container". CSS classes on the inner content container
- Theme: `ThemeMode` (default `System`), inherited from `OsirionComponentBase`. `Light`, `Dark` and `Inverted` add a
  `data-theme` attribute (`data-bs-theme` when the Bootstrap framework is configured)
- Class and Style: strings, inherited from `OsirionComponentBase`. The component's markup does not read them
- Attributes: captures unmatched attributes (`CaptureUnmatchedValues`) and renders them on the section

`ContextCardItem` is a record: `ContextCardItem(string Label, string Description, string Accent = "green")`.

Usage
```razor
<OsirionContextCardGrid Title="Context by role"
                        Description="Each role sees the same data through its own priorities."
                        Id="roles"
                        HeadingId="roles-heading"
                        Items="@roleItems" />

@code {
    private readonly OsirionContextCardGrid.ContextCardItem[] roleItems =
    [
        new("Finance", "Cost variance and forecast predictability.", "blue"),
        new("Operations", "Coverage reliability and exception response."),
        new("Security", "Access reviews and audit evidence.", "purple")
    ];
}
```

Notes
- Emits `section.osirion-context-card-grid` with `aria-labelledby` pointing at the `h2` inside
  `div.osirion-context-card-grid-heading`. The cards sit in `div.osirion-context-card-grid-items`, and each is an
  `article.osirion-context-card-grid-item.osirion-context-card-grid-item-{accent}` with an `h3` (the label) and a `p`
  (the description).
- The accent string is used as written in the class name. The stylesheet defines `blue`, `purple`, `indigo`, `teal`
  and `amber`; any other value, including the default `green`, uses `--osirion-context-card-grid-accent`
  (default `#176b45`).
- Works in static SSR; the component has no script.
- The cards fill the width with `repeat(auto-fit, minmax(min(100%, 15rem), 1fr))`.
- Colors and spacing can be overridden with CSS variables such as `--osirion-context-card-grid-background`,
  `--osirion-context-card-grid-card-background` and `--osirion-context-card-grid-card-shadow`.
- There is no reduced-motion handling because the component has no animation.
