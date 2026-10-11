---
id: 'osirion-reading-progress'
order: 8
layout: docs
title: "Reading Progress - Osirion Blazor"
permalink: /docs/components/core/content/reading-progress
description: "OsirionReadingProgress shows a bar at the top of the window that fills as the reader scrolls, driven by a CSS scroll-driven animation with no script."
author: Dejan Demonjić
date: 2026-10-11
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Content
tags:
- reading-progress
- scroll
- css
- static-ssr
slug: reading-progress
lang: en
published: true
---

# Reading Progress

`OsirionReadingProgress` shows a thin bar at the top of the window that fills from left to right as the reader
scrolls through the page. Use it on long articles. It is a CSS scroll-driven animation, so it needs no script.

Available from 4.1.

## Usage

Place it once on the page, anywhere in the markup:

```razor
@using Osirion.Blazor.Components

<OsirionReadingProgress />
```

To change its color and thickness, set its two custom properties in your stylesheet:

```css
:root {
    --osirion-reading-progress-color: #16a34a;
    --osirion-reading-progress-height: 4px;
}
```

## Parameters

The component has no parameters of its own.

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Class` | `string?` | `null` | Extra classes on the bar. |

Unmatched attributes go on the bar `<div>`.

## Behavior

- The bar is `position: fixed` at the top of the viewport and its width follows the scroll position of the page's
  root scroller (`animation-timeline: scroll(root block)`). It does not follow a scrolling container inside the page.
- Browsers without scroll-driven animations do not show the bar at all, so it never sits empty or stuck. The CSS
  checks support with `@supports (animation-timeline: scroll())`.
- The bar is decorative: it has `aria-hidden="true"`, because the scroll position is already available to assistive
  technology, and it ignores the pointer, so it never blocks a click on a sticky header beneath it.
- It renders the same in static SSR and in interactive render modes.

## Styling

| Class or property | Purpose |
| --- | --- |
| `osirion-reading-progress` | The bar |
| `--osirion-reading-progress-color` | Bar color. Default: `--osirion-action-primary`, falling back to `#2563eb`. |
| `--osirion-reading-progress-height` | Bar thickness. Default: `3px`. |

The bar sits at `z-index: 1100`. If a fixed header in your layout uses a higher value, pass a class
(`Class="article-progress"`) and raise the bar's `z-index` with a selector such as
`body .osirion-reading-progress.article-progress`; the component's own styles are scoped (a class plus an attribute),
so a single class selector is not specific enough to override them.

## Related

- [Article metadata](/docs/components/core/content/article-metadata)
- [Content components](/docs/components/core/content)
