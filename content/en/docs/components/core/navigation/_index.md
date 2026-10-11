---
id: 'core-navigation-components-overview'
order: 1
layout: docs
title: Core Navigation Components
permalink: /docs/components/core/navigation
description: Breadcrumbs and read more links from Osirion.Blazor Core. Both render plain links under static SSR and need no script.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Navigation
tags:
- blazor
- navigation
- breadcrumbs
- links
- static-ssr
is_featured: true
published: true
slug: navigation
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Navigation Components - Osirion.Blazor'
  description: 'OsirionBreadcrumbs and OsirionReadMoreLink: breadcrumb trails and call-to-action links for Blazor that render without script.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/navigation'
  lang: en
  robots: index, follow
  og_title: 'Core Navigation Components - Osirion.Blazor'
  og_description: 'Breadcrumb trails and read more links for Blazor that render without script.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Navigation Components - Osirion.Blazor'
  twitter_description: 'Breadcrumb trails and read more links for Blazor that render without script.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Core Navigation Components

Osirion.Blazor Core has two small navigation components: a breadcrumb trail built from a URL path, and a link for
"read more" and other calls to action. Both render ordinary `<a>` elements, so they behave the same under static SSR,
Interactive Server and WebAssembly, and need no script.

They live in the `Osirion.Blazor.Components` namespace of the Osirion.Blazor.Core package:

```razor
@using Osirion.Blazor.Components
```

## Components

| Component | What it renders | Script | Available |
| --- | --- | --- | --- |
| [Breadcrumbs](/docs/components/core/navigation/breadcrumbs) | A `<nav>` with an ordered list of links, one per path segment | None | 4.0 |
| [Read more link](/docs/components/core/navigation/read-more-link) | A link with an optional icon, in several variants and sizes | None | 4.0 |

## Other navigation

These two cover links inside a page. Site navigation lives elsewhere:

- The Osirion.Blazor.Navigation package has the menu (`Menu`, `MenuItem`, `MenuGroup` and panels),
  `EnhancedNavigation` for scroll handling after enhanced navigation, and `ScrollToTop`.
- The CMS has components that read your content: `ContentBreadcrumbs`, `DirectoryNavigation`, `TableOfContents`,
  `TagCloud`, `CategoriesList`, `SearchBox` and previous and next links.

## Related

- [Navigation package](/docs/components/navigation)
- [CMS Web navigation](/docs/components/cms-web/navigation)
- [Core components](/docs/components/core)
