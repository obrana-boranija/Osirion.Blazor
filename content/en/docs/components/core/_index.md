---
id: 'core-components-overview'
order: 1
layout: docs
title: Core components
permalink: /docs/components/core
description: Osirion.Blazor.Core holds the layout, content, card, section, navigation and form components. Setup steps and every category.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Documentation
tags:
- core-components
- osirion-blazor-core
- static-ssr
- blazor-components
is_featured: true
published: true
slug: core
lang: en
custom_fields: {}
seo_properties:
  title: 'Core components - Osirion.Blazor'
  description: 'Osirion.Blazor.Core holds the layout, content, card, section, navigation and form components. Setup steps and every category.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core'
  lang: en
  robots: index, follow
  og_title: 'Core components - Osirion.Blazor'
  og_description: 'Layout, content, card, section, navigation and form components that render under static SSR.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core components - Osirion.Blazor'
  twitter_description: 'Layout, content, card, section, navigation and form components that render under static SSR.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# Core components

Osirion.Blazor.Core is the package with the page building blocks: layout, content, cards, sections, navigation, forms
and states. Every component renders complete HTML under static SSR; where a component uses script, the script adds
to markup that already works (the one exception is `OsirionReveal`, see Rendering below). Version 4.0.0 targets
.NET 9 and .NET 10.

## Setup

Install the meta package, which includes Core, the theming package that loads its styles, and the `AddOsirion`
registration used below:

```bash
dotnet add package Osirion.Blazor
```

Register the services in `Program.cs`:

```csharp
using Osirion.Blazor.Extensions;

builder.Services.AddOsirion(builder.Configuration);

// Components that enhance themselves with a script (page layout, carousels, metric cards,
// image gallery, reveal, video embeds, the HTML renderer) load it through BlazorJSComponents.
builder.Services.AddJSComponents();
```

Add the namespaces to `_Imports.razor`:

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Theming.Components
```

Render `<OsirionStyles />` once in your layout. It links the Core stylesheet and the components' scoped CSS, and the
adapter for your CSS framework; see [OsirionStyles](/docs/components/theming/osirion-styles).

## Categories

| Category | Components |
| --- | --- |
| [Layout](/docs/components/core/layout) | [`OsirionPageLayout`](/docs/components/core/layout/page-layout) (header, body and footer slots, sticky footer, and from 4.0 a skip link to the main content), `OsirionFooter`, `OsirionStickySidebar`, `OsirionBackgroundPattern`, `OsirionPageLoading`. |
| [Content](/docs/components/core/content) | [`OsirionArticleMetdata`](/docs/components/core/content/article-metadata) (author, date, read time). From 4.0: [callouts](/docs/components/core/content/callout), [tabs](/docs/components/core/content/tabs), [accordion and FAQ](/docs/components/core/content/accordion-and-faq), [responsive image](/docs/components/core/content/responsive-image), [video embed](/docs/components/core/content/video-embed), [reading progress](/docs/components/core/content/reading-progress). |
| [Cards](/docs/components/core/cards) | Feature, testimonial, subscription and metric cards, and from 4.0 [`OsirionAuthorCard`](/docs/components/core/cards/author-card). |
| [Sections](/docs/components/core/sections) | `HeroSection`, `OsirionBaseSection`, `OsirionContactInfoSection`, `OsirionResponsiveShowcaseSection`, `OsirionMetricGrid`, `OsirionContextCardGrid`. From 4.0: `OsirionPricingTable`, `OsirionFeatureComparison`, `OsirionSteps`. |
| [Navigation](/docs/components/core/navigation) | `OsirionBreadcrumbs` and `OsirionReadMoreLink`. Menus and scroll helpers are in the [Navigation package](/docs/components/navigation). |
| [Carousels](/docs/components/core/carousels) | `InfiniteLogoCarousel` and `OsirionTestimonialCarousel`. |
| [Forms](/docs/components/core/forms) | `OsirionContactForm`, which posts under static SSR. The newsletter form is a card: `OsirionSubscriptionCard`. |
| [Popups](/docs/components/core/popups) | [`OsirionCookieConsent`](/docs/components/core/popups/cookie-consent), and from 4.0 [`OsirionAnnouncementBar`](/docs/components/core/popups/announcement-bar), a site-wide notice that closes without script. |
| [Rendering](/docs/components/core/rendering) | `OsirionHtmlRenderer`, which sanitizes HTML and highlights code, and `OsirionReveal`, which fades content in as it scrolls into view. With `Animate` on (the default) the content starts hidden and only its script shows it, so do not wrap content that must be visible without script. |
| [States](/docs/components/core/states) | `OsirionContentNotFound` for missing content. |

`OsirionImageGallery`, a responsive image grid with an optional lightbox, is also part of Core and has no page yet.

## Shared parameters

Core components derive from `OsirionComponentBase` (all except `OsirionResponsiveShowcaseSection`), which gives them
these parameters:

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Class` | `string?` | `null` | Extra CSS classes. Most components add them to their root element. |
| `Style` | `string?` | `null` | Inline style, on the components that render it. |
| `Theme` | `ThemeMode` | `System` | `Light`, `Dark` or `Inverted` writes that value in `data-theme` (`data-bs-theme` with Bootstrap) on the component; `System` writes nothing. |
| `Attributes` | `IDictionary<string, object>` | empty | Captures unmatched attributes such as `id` or `aria-label`. |

`SetInteractive` is obsolete and fails the build; components read the render mode from `RendererInfo` themselves.

## Related

- [Theming components](/docs/components/theming)
- [CMS web components](/docs/components/cms-web)
- [All components](/docs/components)
