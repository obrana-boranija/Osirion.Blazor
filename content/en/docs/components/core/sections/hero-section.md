---
id: 'hero-section'
order: 1
layout: docs
title: HeroSection
permalink: /docs/components/core/sections/hero-section
description: HeroSection renders a page header with a title, subtitle, buttons, article metadata and a side image, background image or pattern.
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
- hero
- landing-page
- header
- static-ssr
is_featured: true
published: true
slug: hero-section
lang: en
custom_fields: {}
seo_properties:
  title: 'HeroSection - Osirion.Blazor'
  description: 'A Blazor page header with title, subtitle, buttons, article metadata and a side image, background image or pattern, rendered without script.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/sections/hero-section'
  lang: en
  robots: index, follow
  og_title: 'HeroSection - Osirion.Blazor'
  og_description: 'A page header with title, subtitle, buttons and a side or background image, rendered without script.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'HeroSection - Osirion.Blazor'
  twitter_description: 'A page header with title, subtitle, buttons and a side or background image, rendered without script.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# HeroSection

`HeroSection` renders the header of a page: a title, a subtitle, a summary, up to two buttons and an image, either
beside the text or behind it. The CMS page components (`ArticlePage`, `HomePage`, `LandingPage`) use it for their
headers, and you can use it on your own Razor pages.

## Usage

```razor
<HeroSection Title="Release notes" Subtitle="What changed in each version" />
```

A landing page header with a side image, two buttons and a background pattern:

```razor
<HeroSection Title="Osirion.Blazor"
             Subtitle="SSR-first Blazor components for content sites"
             ImageUrl="/assets/images/blazor-cover.png"
             ImageAlt=""
             BackgroundPattern="BackgroundPatternType.Grid"
             PrimaryButtonText="Read the docs"
             PrimaryButtonUrl="/docs"
             SecondaryButtonText="Source on GitHub"
             SecondaryButtonUrl="https://github.com/obrana-boranija/Osirion.Blazor" />
```

An article header with the image behind the text and the author, date and reading time:

```razor
<HeroSection Title="Static SSR with Blazor"
             Subtitle="What renders on the server, and what needs interactivity"
             ImageUrl="/assets/images/blazor-cover.png"
             UseBackgroundImage="true"
             HasOverlay="true"
             Alignment="Alignment.Center"
             ShowMetadata="true"
             Author="Dejan Demonjić"
             PublishDate="@(new DateTime(2026, 10, 11))"
             ReadTime="6"
             MinHeight="40vh" />
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Title` | `string?` | `null` | Main heading, rendered as an `<h1>`. |
| `TitleContent` | `RenderFragment?` | `null` | Your own heading markup in place of `Title`. |
| `Subtitle` | `string?` | `null` | Line under the title. |
| `SubtitleContent` | `RenderFragment?` | `null` | Your own markup in place of `Subtitle`. |
| `Summary` | `string?` | `null` | A longer paragraph under the subtitle. |
| `SummaryContent` | `RenderFragment?` | `null` | Your own markup in place of `Summary`. |
| `ImageUrl` | `string?` | `null` | The image, shown beside the text or, with `UseBackgroundImage`, behind it. |
| `ImageAlt` | `string?` | `null` | Alternative text of the side image. Falls back to `Title`, then to "Hero image"; pass `""` for a decorative image. |
| `UseBackgroundImage` | `bool` | `false` | Uses `ImageUrl` as the section's background instead of a side image. |
| `HasOverlay` | `bool` | `false` | Darkens a background image so the text stays readable. |
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | A decorative pattern, shown when there is no background image. |
| `Alignment` | `Alignment` | `Left` | Text alignment: `Left`, `Center` or `Right`. `Justify` is treated as `Left`. |
| `ImagePosition` | `Alignment` | `Right` | Side of the image. In 4.0, `Left` and `Justify` are rendered as `Right`, so the image is always on the right. |
| `Variant` | `HeroVariant` | `Hero` | `Hero`, `Jumbotron` or `Minimal`; changes spacing and the side image's size. |
| `MinHeight` | `string` | `"60vh"` | Minimum height, as a CSS length. |
| `PrimaryButtonText` | `string?` | `null` | Text of the primary button. |
| `PrimaryButtonUrl` | `string?` | `null` | Address of the primary button. |
| `ShowPrimaryButton` | `bool` | `true` | Renders the primary button when its text and address are set. |
| `SecondaryButtonText` | `string?` | `null` | Text of the secondary (outlined) button. |
| `SecondaryButtonUrl` | `string?` | `null` | Address of the secondary button. |
| `ShowSecondaryButton` | `bool` | `true` | Renders the secondary button when its text and address are set. |
| `ShowMetadata` | `bool` | `false` | Shows `Author`, `PublishDate` and `ReadTime` with the article metadata component. |
| `Author` | `string?` | `null` | Author name for the metadata line. |
| `PublishDate` | `DateTime?` | `null` | Publication date for the metadata line. |
| `ReadTime` | `string?` | `null` | Reading time in minutes; "6" is shown as "6 min". |
| `BackgroundColor` | `string?` | `null` | A CSS color for the background. |
| `TextColor` | `string?` | `null` | A CSS color for the text. |
| `HasDivider` | `bool` | `true` | Adds a shadow in the primary color along the top edge. |
| `ChildContent` | `RenderFragment?` | `null` | Extra content under the buttons. |
| `Class` | `string?` | `null` | Extra CSS classes on the `<section>`. |
| `Style` | `string?` | `null` | Inline style, added after the generated declarations. |
| `Theme` | `ThemeMode` | `System` | `Light` or `Dark` forces the section's color scheme. |

Unmatched attributes are written to the `<section>`.

## Behavior

- The component renders plain HTML and needs no script.
- `Title` is an `<h1>`. A page should have one, so when the page already has its main heading, pass `TitleContent`
  with the level you need.
- Text parameters are HTML-encoded. Button addresses are written as given, so pass addresses you control.
- `ImageUrl` is CSS-escaped when it becomes a background image (since 4.0), so a front matter `featured_image`
  cannot close the `url()` and add declarations.
- The side image carries `width` and `height` attributes for its variant (600 by 315 for `Hero`, 600 by 600 for
  `Jumbotron`, 315 by 315 for `Minimal`) to avoid layout shift, and `fetchpriority="high"` because it is usually the
  largest element in view.
- The buttons take the configured CSS framework's large primary and outlined primary button classes.

## Styling

The `<section>` has `osirion-hero-section`, `osirion-hero-variant-{hero|jumbotron|minimal}` and
`osirion-hero-align-{left|center|right}`, plus `osirion-hero-with-background` and `osirion-hero-with-overlay`, or
`osirion-hero-with-side-image`, depending on the image. Inside are `osirion-hero-title`, `osirion-hero-subtitle`,
`osirion-hero-summary`, `osirion-hero-buttons`, `osirion-hero-additional` and `osirion-hero-image-container`.

The text sizes scale with the window between a minimum and a maximum, set as custom properties on the section:
`--hero-title-min`, `--hero-title-pref`, `--hero-title-max` and the same three for `subtitle` and `summary`. Set them
through `Style`:

```razor
<HeroSection Title="Changelog" Style="--hero-title-max: 3rem; --hero-subtitle-max: 1.125rem;" />
```

Colors come from `--osirion-background-primary` and `--osirion-text-primary`, the divider from
`--osirion-color-primary-600`.

## Related

- [Base section](/docs/components/core/sections/base-section)
- [Article metadata](/docs/components/core/content/article-metadata)
- [Background pattern](/docs/components/core/layout/background-pattern)
- [Core section components](/docs/components/core/sections)
