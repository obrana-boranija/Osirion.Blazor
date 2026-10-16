# ThemePicker

Purpose
A labelled list from which visitors pick a palette (`ThemePalette`). The choice applies at once and is kept in the
`osirion-preferred-palette` cookie, so later pages and visits use it too. Light and dark stay with `ThemeToggle`.

Key parameters
- Label: `string`, default `"Palette"`. The visible label of the list
- Palettes: `IReadOnlyList<ThemePalette>?`, default every palette. The palettes offered, in this order. Duplicates and
  values outside the enum are dropped; the palette the visitor already uses is always offered, first when it is not in
  the list
- FrameworkLabel: `string`, default `"Default"`. The text of the `Framework` option, which applies no palette and shows
  the site's own colours
- Class, Style and other attributes: rendered on the outer `label`

Usage
```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Theming.Components

<ThemePicker Label="Colours"
             FrameworkLabel="Site colours"
             Palettes="@([ThemePalette.Framework, ThemePalette.Editor, ThemePalette.Contrast])" />
<ThemeToggle />
```

Notes
- Emits `label.osirion-theme-picker` holding `span.osirion-theme-picker-label` and
  `select.osirion-theme-picker-select`. A native `select` inside its `label` has an accessible name, works from the
  keyboard and needs no id, so several pickers can share a page.
- Set `Osirion:Theming:AllowPaletteSelection` to `true` so every page links the palette stylesheet and a chosen palette
  is painted with the first frame. Without it, the script adds the stylesheet when a visitor first picks a palette.
- No interactive render mode is needed. `theme-picker.js` loads once through the script loader that
  `AddJSComponents()` registers and listens on the document, so pickers added by enhanced navigation work too. Without
  JavaScript the list renders but choosing does nothing.
- The server selects the visitor's palette (`IThemeService.CurrentPalette`); on a page exported to static files, the
  script selects it from the cookie when the page loads.
- The site documentation has the full reference:
  [theme-picker.md](https://github.com/demonjic/Osirion.Blazor/blob/master/content/en/docs/components/theming/theme-picker.md).
