---
id: 'responsive-showcase-section'
order: 6
layout: docs
title: OsirionResponsiveShowcaseSection
permalink: /components/responsive-showcase-section
description: OsirionResponsiveShowcaseSection previews content in a frame that switches between desktop, tablet and mobile widths, for demo and docs pages.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Sections
- Responsive Design
tags:
- blazor
- responsive
- showcase
- preview
- interactive
is_featured: true
published: true
slug: responsive-showcase-section
lang: en
redirect_from:
- /docs/components/core/sections/responsive-showcase-section
custom_fields:
  group: sections
seo_properties:
  title: 'OsirionResponsiveShowcaseSection - Osirion.Blazor'
  description: 'Preview Blazor content in a frame that switches between desktop, tablet and mobile widths, with an optional code sample.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/responsive-showcase-section'
  lang: en
  robots: index, follow
  og_title: 'OsirionResponsiveShowcaseSection - Osirion.Blazor'
  og_description: 'A preview frame that switches between desktop, tablet and mobile widths.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionResponsiveShowcaseSection - Osirion.Blazor'
  twitter_description: 'A preview frame that switches between desktop, tablet and mobile widths.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionResponsiveShowcaseSection

`OsirionResponsiveShowcaseSection` shows content inside a browser-like frame with buttons that switch the frame
between desktop, tablet and mobile widths, and an optional code sample under it. It is a tool for component demos
and design reviews, not for a production page.

## Usage

The component is in its own namespace, and switching widths needs an interactive render mode on the page:

```razor
@page "/demo/cards"
@rendermode InteractiveServer
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Components.Sections

<OsirionResponsiveShowcaseSection Title="Feature card">
    <OsirionFeatureCard Title="Static SSR" Description="Every component renders on the server." />
</OsirionResponsiveShowcaseSection>
```

Start in the mobile width and offer the markup under the preview:

```razor
<OsirionResponsiveShowcaseSection Title="Hero"
                                  InitialViewport="OsirionResponsiveShowcaseSection.ViewportMode.Mobile"
                                  CodeSnippet="@heroMarkup">
    <HeroSection Title="Release notes" Subtitle="What changed in each version" />
</OsirionResponsiveShowcaseSection>

@code {
    private const string heroMarkup =
        "<HeroSection Title=\"Release notes\" Subtitle=\"What changed in each version\" />";
}
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `ChildContent` | `RenderFragment?` | `null` | The content to preview. Without it, a placeholder is shown. |
| `Title` | `string` | `"Component Preview"` | Heading above the frame, rendered as an `<h3>`. |
| `InitialViewport` | `ViewportMode` | `Desktop` | The width shown first: `Desktop`, `Tablet` or `Mobile`. |
| `ShowDimensions` | `bool` | `true` | Shows the current width under the frame. |
| `ShowCode` | `bool` | `true` | Shows a Show Code button when `CodeSnippet` is set. |
| `CodeSnippet` | `string?` | `null` | Code shown under the preview when the button is pressed. Written as encoded text. |
| `Style` | `string?` | `null` | Inline style on the frame, added after its width declarations. |

`ViewportMode` is nested in the component: write `OsirionResponsiveShowcaseSection.ViewportMode.Mobile`. The
component also declares `Class`, `ShowBrowserChrome` and unmatched attributes, but 4.0 does not apply them: the
frame always has its browser bar, and the root element takes no extra classes or attributes.

## Behavior

- The frame widths are 1200px (desktop), 768px (tablet) and 375px (mobile), each capped at 100% of the available
  width.
- The frame is a `<div>`, not an `<iframe>`. Layouts that depend on the width of their container (wrapping flex
  rows, `auto-fit` grids, container queries) change with the frame; CSS media queries still follow the browser
  window.
- Switching widths and showing the code are Blazor event handlers. Under static SSR the component renders in its
  `InitialViewport` with the code hidden, and the buttons do nothing. Because `ChildContent` cannot cross a render
  mode boundary, make the whole page interactive rather than setting `@rendermode` on this component.
- The width buttons report their state with `aria-pressed`, but in 4.0 they contain only icons hidden from assistive
  technology and have no accessible name.

## Styling

The root is `osirion-responsive-showcase`, with `showcase-header`, `showcase-title`, `viewport-controls` and
`viewport-btn` (`active` on the current width) for the header, and `showcase-body`, `viewport-container`
(`viewport-desktop`, `viewport-tablet` or `viewport-mobile`), `viewport-chrome`, `component-wrapper`,
`dimensions-info` and `code-section` for the rest.

## Related

- [Core section components](/components/sections)
- [Feature card](/components/feature-card)
- [Hero section](/components/hero-section)
