---
id: 'base-section'
order: 1
layout: docs
title: OsirionBaseSection
permalink: /docs/components/core/sections/base-section
description: OsirionBaseSection renders a page section with an id, heading, description, framework container, padding and an optional background image or pattern.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Sections
tags:
- blazor
- section
- layout
- background
- static-ssr
is_featured: true
published: true
slug: base-section
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionBaseSection - Osirion.Blazor'
  description: 'A Blazor page section with heading, description, framework container, padding and an optional background image or pattern.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/sections/base-section'
  lang: en
  robots: index, follow
  og_title: 'OsirionBaseSection - Osirion.Blazor'
  og_description: 'A page section with heading, container, padding and an optional background image or pattern.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionBaseSection - Osirion.Blazor'
  twitter_description: 'A page section with heading, container, padding and an optional background image or pattern.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionBaseSection

`OsirionBaseSection` renders a `<section>` with an id, an optional heading and description, your content inside the
CSS framework's container, and consistent vertical padding. Use it for the bands of a landing page so they share
spacing and alignment.

## Usage

```razor
<OsirionBaseSection Id="why" Title="Why static SSR" Description="Pages arrive as HTML and work without script.">
    <p>Search engines and readers get the full page on the first request.</p>
</OsirionBaseSection>
```

A left-aligned section with more padding, a background image and a shadow underneath:

```razor
<OsirionBaseSection Id="newsletter"
                    Title="Release notes by email"
                    TextAlignment="Alignment.Left"
                    Padding="SectionPadding.Large"
                    BackgroundImageUrl="/assets/images/blazor-cover.png"
                    HasDivider="true">
    <a href="/subscribe">Subscribe</a>
</OsirionBaseSection>
```

With an image, the text turns white and a dark overlay (`ShowOverlay`, on by default) keeps it readable.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Id` | `string?` | `null` | The section's `id`, for in-page links. Required. |
| `Title` | `string?` | `null` | Heading, rendered as an `<h2>`. Written as raw HTML; see Behavior. |
| `TitleContent` | `RenderFragment?` | `null` | Your own heading markup in place of `Title`. |
| `Description` | `string?` | `null` | Paragraph under the heading. Written as raw HTML. |
| `DescriptionContent` | `RenderFragment?` | `null` | Your own markup in place of `Description`. |
| `ChildContent` | `RenderFragment?` | `null` | The section's content. |
| `TextAlignment` | `Alignment` | `Center` | `Left`, `Center`, `Right` or `Justify`, for the heading and content. |
| `Padding` | `SectionPadding` | `Medium` | Vertical padding: `None`, `Small` (2rem), `Medium` (3rem) or `Large` (4rem). |
| `ContainerClass` | `string?` | `null` | Class of the inner container. When empty, the framework's container is used. |
| `BackgroundImageUrl` | `string?` | `null` | Background image, set as a CSS `url()`. |
| `ShowOverlay` | `bool` | `true` | Darkens a background image. Has no effect without one. |
| `BackgroundColor` | `string?` | `null` | A CSS color for the background. |
| `TextColor` | `string?` | `null` | A CSS color for the text. |
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | A decorative pattern, shown when `ShowPattern` is on. |
| `ShowPattern` | `bool` | `false` | Renders `BackgroundPattern` behind the content. |
| `HasDivider` | `bool` | `false` | Adds a shadow under the section to separate it from the next. |
| `Class` | `string?` | `null` | Extra CSS classes on the `<section>`. |
| `Style` | `string?` | `null` | Inline style, added after the generated background and color declarations. |

Unmatched attributes are written to the `<section>`. The pattern values are `Dots`, `Grid`, `Diagonal`,
`Honeycomb`, `GradientMesh`, `AnimatedGrid`, `TechWave`, `Circuit` and `DotsFade`; see
[background pattern](/docs/components/core/layout/background-pattern).

## Behavior

- The component renders plain HTML and needs no script.
- The heading block appears only when `Title` or `TitleContent` is set; a `Description` on its own is not shown.
- `Title` and `Description` are written as raw HTML without sanitizing, so you can use inline markup such as
  `<em>`. Pass only text you control; render user or content-provided text with `TitleContent` and
  `DescriptionContent`, where Blazor encodes it.
- `BackgroundImageUrl` is CSS-escaped before it is written (since 4.0), so a value cannot close the `url()` and add
  declarations. `BackgroundColor` and `TextColor` are written as given: use your own values.
- The section has `scroll-margin-top: 3rem`, so a link to `#id` does not land under a sticky header.

## Styling

The `<section>` has `osirion-section`, `osirion-section-align-{left|center|right|justify}` and
`osirion-section-padding-{none|small|medium|large}`, plus `osirion-section-with-background`,
`osirion-section-with-pattern` and `osirion-section-divider` when those options are on. Inside are
`osirion-section-overlay`, the container, `osirion-section-header` (with `osirion-section-header-{alignment}`),
`osirion-section-title`, `osirion-section-description` and `osirion-section-content`.

The container class follows the configured CSS framework:

| Framework | Container class |
| --- | --- |
| Bootstrap | `container-xxl` |
| Fluent UI | `osirion-container` |
| MudBlazor | `mud-container` |
| Radzen | `rz-container` |
| None | `osirion-container` |

Colors come from `--osirion-background-primary` and `--osirion-text-primary`, and spacing from the
`--osirion-spacing-*` tokens, so a framework adapter or your theme restyles every section at once.

## Related

- [Hero section](/docs/components/core/sections/hero-section)
- [Background pattern](/docs/components/core/layout/background-pattern)
- [Core section components](/docs/components/core/sections)
