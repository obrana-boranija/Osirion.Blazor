---
id: 'osirion-styles-component'
order: 3
layout: docs
title: OsirionStyles
permalink: /docs/components/theming/osirion-styles
description: OsirionStyles links the Osirion stylesheets and the CSS framework adapter, and applies the visitor's light or dark theme as the page loads.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Theming Components
- Styling
tags:
- blazor
- osirion-styles
- theming
- dark-mode
- css-custom-properties
- css-framework
is_featured: true
published: true
slug: osirion-styles
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionStyles Component - Osirion.Blazor'
  description: 'Link the Osirion stylesheets and framework adapter and apply the stored light or dark theme with OsirionStyles.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/theming/osirion-styles'
  lang: en
  robots: index, follow
  og_title: 'OsirionStyles Component - Osirion.Blazor'
  og_description: 'Links the Osirion stylesheets and the framework adapter, and applies the visitor theme as the page loads.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionStyles Component - Osirion.Blazor'
  twitter_description: 'Links the Osirion stylesheets and the framework adapter, and applies the visitor theme.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# OsirionStyles

`OsirionStyles` links the stylesheets that Osirion components need, adds the adapter for your CSS framework, and
applies the visitor's light or dark theme while the page loads. Render it once per page, in your main layout.

## Usage

With theming configured (see [Theming](/docs/components/theming)), the component needs no parameters:

```razor
@using Osirion.Blazor.Theming.Components
@inherits LayoutComponentBase

@Body

<OsirionStyles />
```

A layout that makes dark the default theme and sets the base font:

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Theming.Components
@inherits LayoutComponentBase

@Body

<OsirionStyles Mode="ThemeMode.Dark"
               CustomVariables="--osirion-font-family-base: Inter, system-ui, sans-serif;" />
```

`Mode` only sets the default. A visitor who already picked a theme with
[`ThemeToggle`](/docs/components/theming/theme-toggle) keeps it. The font variable takes effect when no framework
adapter is used: every adapter sets `--osirion-font-family-base` from the framework's own font.

## Parameters

`Mode`, `Framework` and `UseStyles` override the matching `ThemingOptions` value for this component; left unset, the
configured value applies. `CustomVariables` does not replace the configured value: both are written (see Behavior).

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Mode` | `ThemeMode?` | `null` | The theme used when the visitor has not picked one: `Light`, `Dark` or `System`. Overrides `DefaultMode`. |
| `Framework` | `CssFramework?` | `null` | Which adapter stylesheet to link. Overrides `Framework`; see the note below. |
| `UseStyles` | `bool?` | `null` | When `true`, only the Core stylesheets are linked; the adapter and the theme stylesheet are left out. Overrides `UseDefaultStyles`. |
| `CustomVariables` | `string?` | `null` | CSS declarations written into a `:root` block. |

The inherited `Class`, `Style` and `Theme` parameters and any extra attributes have no effect: the component renders
no wrapper element.

Set the framework in configuration rather than only through the `Framework` parameter. The adapter's rules apply under
a class on `<html>` (such as `osirion-bootstrap-integration`) that comes from the configured framework, so the
parameter on its own links the adapter stylesheet without the class it needs.

## Behavior

The component renders, in this order:

1. `_content/Osirion.Blazor.Core/css/index.min.css` and `_content/Osirion.Blazor.Core/Osirion.Blazor.Core.bundle.scp.css`.
2. Unless `UseStyles` (or `UseDefaultStyles`) is `true`, the framework adapter from
   `_content/Osirion.Blazor.Theming/css/adapters/` and `_content/Osirion.Blazor.Theming/css/index.min.css`.
3. A `<style>` block with the `CustomVariables` parameter, then one with the configured `CustomVariables`. Both are
   `:root` rules, so for the same property the configured value, written last, wins.
4. `osirion-styles.js`, loaded through the script loader that `builder.Services.AddJSComponents()` registers.
5. A short inline script that sets `data-osirion-theme`, `data-osirion-framework`, the framework class and the
   framework's own dark-mode marker on `<html>` while the page is parsed.

The theme is resolved in this order: the `osirion-preferred-theme` cookie, then `Mode` or `DefaultMode`. `System`
follows the operating system's `prefers-color-scheme` and reacts when it changes, as long as the visitor has not
picked a theme. `ThemeMode.Inverted` is not handled by the script: the page takes the operating system's theme, as
with `System`, but does not react when it changes.

The script applies the theme again after every Blazor enhanced navigation. Under Static SSR this is all that runs; no
interactive render mode is needed. When `IThemeService` raises `ThemeChanged`, the component renders again, and it
unsubscribes when disposed.

With Fluent UI Blazor v5, the component also passes the mode to `Blazor.theme.setThemeMode`, so Fluent components
follow the Osirion theme.

### Security

`CustomVariables` is written into the page as raw CSS without encoding. Pass only values from your code or
configuration, never text a visitor supplied. The component writes an inline script and does not add a nonce, so a
Content Security Policy must allow it.

## Styling

After the script runs, `<html>` carries:

| Marker | Values |
| --- | --- |
| `data-osirion-theme` | `light` or `dark` |
| `data-osirion-framework` | `None`, `Bootstrap`, `FluentUI`, `MudBlazor` or `Radzen` |
| Framework class | `osirion-bootstrap-integration`, `osirion-fluent-integration`, `osirion-mudblazor-integration` or `osirion-radzen-integration` |
| Dark-mode marker | `data-bs-theme` (Bootstrap), `mud-theme-dark` (MudBlazor), `fluent-dark-theme` or `fluent-light-theme` (Fluent UI), `rz-dark-theme` (Radzen) |

Use these in your own CSS to style per theme or per framework.

The `CustomVariables` blocks are plain `:root` rules. They come after the Core stylesheet, so they change the tokens it
defines in `:root`, such as `--osirion-font-family-base`. They do not change the theme colors, which the theme
stylesheet sets under the more specific `:root[data-osirion-theme=light]` and `:root[data-osirion-theme=dark]`, nor
anything an adapter sets under `html.osirion-*-integration`. To change a theme color, use a more specific selector in
your own stylesheet; to change a color an adapter provides, change the framework's variable. The
[Theming](/docs/components/theming) page shows both and lists the main tokens.

## Related

- [Theming](/docs/components/theming): setup, options and adapters.
- [ThemeToggle](/docs/components/theming/theme-toggle): the button that switches the theme.
