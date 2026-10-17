---
id: 'theme-picker-component'
order: 4
layout: docs
title: ThemePicker
permalink: /components/theme-picker
description: ThemePicker renders a labelled list from which visitors pick a colour palette; the choice applies at once and is remembered in a cookie.
author: Dejan Demonjić
date: 2026-10-16
last_modified: 2026-10-16
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Theming Components
- Styling
tags:
- blazor
- theme-picker
- theming
- palettes
- accessibility
is_featured: false
published: true
slug: theme-picker
lang: en
redirect_from:
- /docs/components/theming/theme-picker
custom_fields:
  group: theming
seo_properties:
  title: 'ThemePicker Component - Osirion.Blazor'
  description: 'Let visitors pick a colour palette with a labelled list that works under static SSR and on static sites.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/theme-picker'
  lang: en
  robots: index, follow
  og_title: 'ThemePicker Component - Osirion.Blazor'
  og_description: 'A labelled list from which visitors pick a colour palette, applied at once and remembered.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: TechArticle
  twitter_card: summary_large_image
  twitter_title: 'ThemePicker Component - Osirion.Blazor'
  twitter_description: 'A labelled list from which visitors pick a colour palette, applied at once and remembered.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# ThemePicker

`ThemePicker` renders a labelled list of [palettes](/components/palettes). Choosing one applies it at once
and keeps it in the `osirion-preferred-palette` cookie, so the next page, and the next visit, use it too. Light and dark
stay with [`ThemeToggle`](/components/theme-toggle); the two sit well side by side.

## Usage

Allow palette selection, so every page links the palette stylesheet and a chosen palette is painted with the first
frame:

```json
{
  "Osirion": {
    "Theming": {
      "AllowPaletteSelection": true
    }
  }
}
```

Then place the picker, for example next to the toggle in a menu:

```razor
@using Osirion.Blazor.Theming.Components

<ThemePicker />
<ThemeToggle />
```

Offer fewer palettes, in your order, and rename the option that applies none:

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Theming.Components

<ThemePicker Label="Colours"
             FrameworkLabel="Site colours"
             Palettes="@([ThemePalette.Framework, ThemePalette.Editor, ThemePalette.Contrast])" />
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Label` | `string` | `"Palette"` | The visible label of the list. |
| `Palettes` | `IReadOnlyList<ThemePalette>?` | `null` (every palette) | The palettes offered, in this order. Duplicates and values outside the enum are dropped. The palette the visitor already uses is always offered, first when it is not in the list. |
| `FrameworkLabel` | `string` | `"Default"` | The text of the `Framework` option, which applies no palette and shows the site's own colours. |

`Class`, `Style` and any extra attributes go on the outer `<label>`.

## Behavior

- The server renders the list with the visitor's palette selected (`IThemeService.CurrentPalette`). On a page exported to
  static files, or one cached before the visitor chose, the script selects the palette from the cookie when the page
  loads and after every enhanced navigation.
- Choosing an option sets `data-osirion-palette` on `<html>` (or removes it for `Framework`), writes the cookie for one
  year (`SameSite=Lax`, `Secure` on HTTPS) and keeps every picker on the page in step. When the page has no palette
  stylesheet (`AllowPaletteSelection` off), the script adds it.
- No interactive render mode is needed. The script, `theme-picker.js`, is loaded once through the script loader that
  `builder.Services.AddJSComponents()` registers, and listens on the document, so pickers that enhanced navigation adds
  work without further setup. It writes no inline script and no event handler attributes.
- Without JavaScript the list renders but choosing does nothing, as with `ThemeToggle`.

### Accessibility

The list is a native `<select>` inside its `<label>`, so it has an accessible name, works with the keyboard (arrow keys
change the palette) and needs no id, which keeps several pickers on one page valid. Its focus outline uses
`--osirion-focus-ring-color`. The label takes the colour of its container, so it stays readable in a menu with its own
background, such as the Studio Blue frame.

## Styling

| Class | Element |
| --- | --- |
| `osirion-theme-picker` | The outer `<label>` |
| `osirion-theme-picker-label` | The label text |
| `osirion-theme-picker-select` | The `<select>`; it reads `--osirion-input-background`, `--osirion-input-text`, `--osirion-input-border` and `--osirion-input-radius` |

To hide the label visually but keep it for screen readers, pass a `Class` and give
`.your-class .osirion-theme-picker-label` the same rules as Osirion's `osirion-visually-hidden` class in your stylesheet.

## Related

- [Palettes](/components/palettes): the palettes, their contrast checks and how they are applied.
- [ThemeToggle](/components/theme-toggle): switches between light and dark.
- [OsirionStyles](/components/osirion-styles): links the palette stylesheet.
