---
id: 'cms-web-core-components-overview'
order: 1
layout: docs
title: CMS Web Core Components
permalink: /docs/components/cms-web/core
description: Components that list, render and describe CMS content in Osirion.Blazor, plus the sitemap and feed endpoints.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Core Components
tags:
- cms
- content-list
- content-view
- seo
- feeds
is_featured: true
published: true
slug: core
lang: en
custom_fields: {}
seo_properties:
  title: 'CMS Web Core Components | Osirion.Blazor'
  description: 'Components that list, render and describe CMS content in Osirion.Blazor, plus the sitemap and feed endpoints.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/cms-web/core'
  lang: en
  robots: index, follow
  og_title: 'CMS Web Core Components | Osirion.Blazor'
  og_description: 'List, render and describe CMS content in Blazor with Osirion.Blazor.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'CMS Web Core Components | Osirion.Blazor'
  twitter_description: 'List, render and describe CMS content in Blazor with Osirion.Blazor.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# CMS Web core components

These components turn the items of a content provider into pages: lists, single articles and the metadata search
engines read. They live in the `Osirion.Blazor.Cms.Web.Components` namespace and read from the default provider
through `IContentProviderManager`, so register a content provider first (see
[CMS Web components](/docs/components/cms-web)).

```razor
@using Osirion.Blazor.Cms.Web.Components
```

## Components

| Component | Use it for |
| --- | --- |
| [ContentList](/docs/components/cms-web/core/content-list) | Blog indexes and category, tag, author and search pages, with pagination and list SEO. |
| [ContentView](/docs/components/cms-web/core/content-view) | One item as an article: header, sanitized body, categories, tags, previous and next links. |
| [LocalizedContentView](/docs/components/cms-web/core/localized-content-view) | The same, with links to the item's translations. |
| [ContentPage](/docs/components/cms-web/core/content-page) | A whole article page loaded by URL: breadcrumbs, sidebar, share links, related items. |
| [ContentRenderer](/docs/components/cms-web/core/content-renderer) | Only the body, with code highlighting, inside a layout you build. |
| [SeoMetadataRenderer](/docs/components/cms-web/core/seo-metadata-renderer) | Title, description, canonical URL, Open Graph, Twitter Card and JSON-LD in the page head. |
| [Feeds and sitemap](/docs/components/cms-web/core/feeds-and-sitemap) | `/sitemap.xml`, `/feed.xml` and `/atom.xml` endpoints. Available from 4.1. |

## Choosing one

- Need a list? Use `ContentList`; it also writes the list page's head metadata.
- Need an article page with little code? `ArticlePage` (used by the example site) loads an item by slug or URL and
  renders the hero, breadcrumbs, table of contents and head metadata. `ContentPage` is the older, simpler layout and
  writes no head metadata.
- Building your own layout? Combine `ContentView` or `ContentRenderer` with a `SeoMetadataRenderer` in
  `<HeadContent>`.

## Shared behavior

- Everything renders under static SSR. Links are ordinary links, and no component here needs script; only code
  highlighting in `ContentRenderer` loads Prism when the content has code blocks.
- Content HTML is sanitized before it is written: the providers sanitize Markdown output when they load it, and
  `ContentView`, `LocalizedContentView` and `ContentRenderer` sanitize again, which covers custom providers and items
  you create in code. Widen the allow-list with `AddOsirionHtmlSanitizer` when your content needs a custom element.
- Category and tag links default to `/category/{slug}` and `/tag/{slug}`; `LocalizedContentView`, and therefore
  `ContentPage`, puts the locale in front (`/en/category/{slug}`). Each component takes formatter parameters when your
  routes differ.
