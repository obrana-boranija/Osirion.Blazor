---
id: 'osirion-feature-card'
order: 1
layout: docs
title: OsirionFeatureCard
permalink: /docs/components/core/cards/feature-card
description: OsirionFeatureCard renders an image, a title, a description and optional links in one card. Parameters, behavior and styling.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Cards
tags:
- feature-card
- cards
- osirion-blazor-core
- static-ssr
is_featured: true
published: true
slug: feature-card
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionFeatureCard - Osirion.Blazor'
  description: 'OsirionFeatureCard renders an image, a title, a description and optional links in one card. Parameters, behavior and styling.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/cards/feature-card'
  lang: en
  robots: index, follow
  og_title: 'OsirionFeatureCard - Osirion.Blazor'
  og_description: 'A card with an image, title, description, a read more link and category links. Renders without script.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionFeatureCard - Osirion.Blazor'
  twitter_description: 'A card with an image, title, description, a read more link and category links.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# OsirionFeatureCard

`OsirionFeatureCard` renders an image next to (or above or below) a title, a description and optional links. Use it
for feature lists, service overviews and article teasers. It is plain markup and CSS, so it renders the same under
static SSR and in interactive modes.

## Usage

The smallest card is a title and a description:

```razor
<OsirionFeatureCard Title="Static SSR first"
                    Description="Every component renders meaningful HTML before any script runs." />
```

A teaser that links to an article, with the image on top and category links under the text:

```razor
@using Osirion.Blazor.Components

<OsirionFeatureCard Title="Content from GitHub"
                    Description="Publish Markdown from a repository and refresh it with a webhook."
                    ImageUrl="/images/github-cms.png"
                    ImageAlt="Markdown files in a GitHub repository"
                    ImagePosition="ImagePosition.Top"
                    Url="/docs/components/cms-web"
                    ShowReadMoreLink="true"
                    ReadMoreText="Read the guide"
                    ShowPillowButtons="true"
                    PillowButtons="categories" />

@code {
    private readonly List<PillowButton> categories =
    [
        new("CMS", "/blog/category/cms"),
        new("Static SSR", "/blog/category/ssr", ThemeColor.Secondary)
    ];
}
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Title` | `string?` | `null` | Card title, rendered as a paragraph with the class `osirion-feature-card-title`. |
| `Description` | `string?` | `null` | Text under the title. |
| `ShowDescription` | `bool` | `true` | Set to `false` to hide `Description`. |
| `ChildContent` | `RenderFragment?` | `null` | Extra content between the description and the links. |
| `Url` | `string?` | `null` | Wraps the image in a link, and is the read more link's target when `ReadMoreHref` is not set. |
| `ImageUrl` | `string?` | `null` | Image source. Without it the card shows a placeholder icon. |
| `ImageAlt` | `string?` | `null` | Alternative text; when not set, `Title` is used, then `Feature image`. |
| `ImageLazyLoading` | `bool` | `true` | Renders `loading="lazy"`; set to `false` for a card above the fold. |
| `ImageTransformOnHover` | `bool` | `false` | Zooms the image on hover. |
| `ImagePosition` | `ImagePosition` | `Left` | `Top`, `Bottom`, `Left` or `Right`. |
| `CardSize` | `CardSize` | `Normal` | `ExtraExtraSmall` to `ExtraExtraLarge`; sets the card dimensions. |
| `ContentAlignment` | `ContentAlignment` | `CenterLeft` | One of nine positions, from `TopLeft` to `BottomRight`. |
| `ContentFontSize` | `FontSizeVariant` | `Normal` | `ExtraExtraSmall` to `ExtraExtraLarge`. |
| `BorderTiming` | `EffectTiming` | `Always` | When the border shows: `Never`, `Always` or `OnHover`. |
| `BorderColor` | `ThemeColor` | `Neutral` | `Primary`, `Secondary`, `Tertiary` or `Neutral`. |
| `ShadowTiming` | `EffectTiming` | `OnHover` | When the shadow shows. |
| `ShadowColor` | `ThemeColor` | `Neutral` | Shadow color. |
| `TransformTiming` | `EffectTiming` | `OnHover` | When the card lifts. |
| `ShowReadMoreLink` | `bool` | `false` | Shows a read more link when `ReadMoreHref` or `Url` is set. |
| `ReadMoreText` | `string` | `"Read more"` | Link text. |
| `ReadMoreHref` | `string?` | `null` | Link target; falls back to `Url`. |
| `ReadMoreStretched` | `bool` | `true` | With pillow buttons shown, ends the link's clickable area 50px above the bottom of the text area so the buttons stay clickable. |
| `ShowPillowButtons` | `bool` | `false` | Shows `PillowButtons`. |
| `PillowButtons` | `List<PillowButton>?` | `null` | Small category-style links. |
| `Class` | `string?` | `null` | Extra CSS classes on the card. |
| `Style` | `string?` | `null` | Inline style on the card. |

`PillowButton` has `Text`, `Href` (default `#`), `Color` (`ThemeColor`, default `Primary`), `Target` and `AriaLabel`,
and a constructor `PillowButton(text, href, color)`. Unmatched attributes are passed to the card's root element.

## Behavior

- No script is needed. Hover effects are CSS, and `prefers-reduced-motion: reduce` turns off the lift and zoom.
- The read more link is rendered with `OsirionReadMoreLink` as a stretched link: it covers the card's text area, so
  that whole area is one click target. The image is a separate link only when `Url` is set.
- When `Url` is set, the image link has no text of its own; its accessible name is the image's alternative text, so
  set `ImageAlt` to something that describes the destination.
- If the image fails to load, an inline `onerror` attribute swaps in a placeholder from `placehold.co`. A Content
  Security Policy that blocks inline event handlers, or that host in `img-src`, leaves the broken image instead.
- On viewports 768px wide or narrower, `Left` and `Right` layouts stack into one column: the image above the
  content for `Left`, below it for `Right`.
- Text is HTML-encoded. URLs (`Url`, `ReadMoreHref`, `PillowButton.Href`) are written as given, so pass values your
  application controls.

## Styling

The root element carries `osirion-feature-card` plus one class per setting, for example
`osirion-feature-card-image-top`, `osirion-feature-card-md`, `osirion-feature-card-content-center-left`,
`osirion-feature-card-border-always`, `osirion-feature-card-shadow-hover` and `osirion-feature-card-transform-hover`.
The card reads the theme tokens `--osirion-card-background`, `--osirion-border-color`, `--osirion-action-primary`,
`--osirion-action-secondary` and `--osirion-heading-color`, and `--osirion-card-border-width` (default `2px`) for the
border, so a framework adapter or your own `:root` values restyle it.

## Related

- [Cards overview](/docs/components/core/cards)
- [OsirionReadMoreLink](/docs/components/core/navigation/read-more-link)
- [OsirionTestimonialCard](/docs/components/core/cards/testimonial-card)
