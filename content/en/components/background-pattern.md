---
id: 'osirion-background-pattern'
order: 5
layout: docs
title: OsirionBackgroundPattern
permalink: /components/background-pattern
description: A decorative CSS background layer (dots, grid, diagonal lines, honeycomb and more) drawn in the theme's primary color behind a section.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Layout
tags:
- blazor
- background
- pattern
- css
- layout
is_featured: true
published: true
slug: background-pattern
lang: en
redirect_from:
- /docs/components/core/layout/background-pattern
custom_fields:
  group: layout
seo_properties:
  title: 'OsirionBackgroundPattern: CSS background patterns for Blazor | Osirion.Blazor'
  description: 'A decorative CSS background layer for Blazor sections: dots, grid, diagonal lines, honeycomb, circuit and more, in the theme color.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/background-pattern'
  lang: en
  robots: index, follow
  og_title: 'OsirionBackgroundPattern - Osirion.Blazor'
  og_description: 'Decorative CSS background patterns drawn in the theme primary color.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionBackgroundPattern - Osirion.Blazor'
  twitter_description: 'Decorative CSS background patterns for Blazor sections.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionBackgroundPattern

`OsirionBackgroundPattern` renders an empty, absolutely positioned `div` that fills its parent with a decorative
pattern made of CSS gradients. Use it behind a hero or section; it has no content and needs no images or script.

Most of the time you do not place it yourself: [HeroSection](/components/hero-section),
[OsirionBaseSection](/components/base-section) and `OsirionContentNotFound` take a
`BackgroundPattern` parameter and render it for you.

## Usage

Through a section:

```razor
@using Osirion.Blazor.Components

<OsirionBaseSection Title="Categories" ShowPattern="true" BackgroundPattern="BackgroundPatternType.Grid">
    <p>Browse the documentation by topic.</p>
</OsirionBaseSection>
```

`OsirionBaseSection` draws the pattern only when `ShowPattern` is `true` (its default is `false`).

On its own, inside a positioned container, with the content raised above it:

```razor
<div style="position: relative;">
    <OsirionBackgroundPattern BackgroundPattern="BackgroundPatternType.Dots" />
    <div style="position: relative; z-index: 1;">
        <h2>Release notes</h2>
    </div>
</div>
```

The pattern layer has `position: absolute; inset: 0; z-index: 0`, so its parent needs a `position` other than
`static`, and your content needs `position: relative` with a higher `z-index` to stay above it. The stylesheet's
`osirion-bg-content` class is scoped to the component, so it does not apply to markup in your own pages; set those
two properties yourself, as above.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | The pattern. When null, nothing is rendered. |
| `MaskImage` | `bool` | `true` | Adds `osirion-bg-image-mask`, a vertical gradient mask that fades the pattern out. |
| `Class` | `string?` | `null` | Extra CSS classes on the pattern `div`. |

`Style`, `Theme` and unmatched attributes are not applied to the markup.

## Patterns

| `BackgroundPatternType` | CSS class | Look |
| --- | --- | --- |
| `Dots` | `osirion-bg-dots` | Dots on a 24 px grid. |
| `Grid` | `osirion-bg-grid` | Thin lines on a 24 px grid. |
| `Diagonal` | `osirion-bg-diagonal` | 45 degree lines. |
| `Honeycomb` | `osirion-bg-honeycomb` | Hexagon tiling. |
| `GradientMesh` | `osirion-bg-gradient-mesh` | Blurred color glows in the corners. |
| `AnimatedGrid` | `osirion-bg-grid-animated` | Grid with a slow pulse animation. |
| `TechWave` | `osirion-bg-tech-wave` | Slowly rotating light overlay. |
| `Circuit` | `osirion-bg-circuit` | Dots and short lines like a circuit board. |
| `DotsFade` | `osirion-bg-dots-fade` | Dots that fade out toward the edges. |

`TechWave` and `Circuit` wrap the theme color in `rgba()`, which browsers reject when the token holds a color value
(the default tokens do), so these two currently show little or nothing. Prefer the other patterns.

Under a dark theme (`data-theme="dark"` on a parent), the stylesheet draws every pattern except `GradientMesh` and
`TechWave` as dots at 0.15 opacity.

## Behavior

- **Rendering.** Pure CSS; the same markup under static SSR and interactive rendering.
- **Accessibility.** The layer is an empty `div` with no text, so assistive technology has nothing to announce. Keep
  enough contrast between your text and the pattern; the patterns use low opacity (0.15 to 0.2 for the line and dot
  patterns) for that reason.
- **Motion.** `AnimatedGrid` and `TechWave` animate continuously, and the component's stylesheet has no
  reduced-motion rule of its own. Choose a static pattern where motion is a concern.

## Styling

Patterns are drawn in `--osirion-action-primary`, so they follow the theme and the CSS framework adapter. Change the
color for one area by setting the token on a parent:

```css
.release-notes {
    --osirion-action-primary: #0e7490;
}
```

## Related

- [HeroSection](/components/hero-section)
- [OsirionBaseSection](/components/base-section)
- [Layout overview](/components/layout)
