---
id: 'cms-web-core-components-overview'
order: 1
layout: docs
title: CMS pages
permalink: /components/cms-pages
description: Components that list, render and describe CMS content in Osirion.Blazor, plus the sitemap and feed endpoints.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-17
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
slug: cms-pages
lang: en
redirect_from:
- /docs/components/cms-web/core
custom_fields:
  group: cms-pages
seo_properties:
  title: 'Blazor CMS page components'
  description: 'Components that list, render and describe CMS content in Osirion.Blazor, plus the sitemap and feed endpoints.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/cms-pages'
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

# CMS pages

These components turn the items of a content provider into pages: lists, single articles, whole page layouts and the
metadata search engines read. They are in the `Osirion.Blazor.Cms.Web` package and read from the default provider
through `IContentProviderManager`, so register a content provider first (see
[CMS Web components](/docs/packages/cms-web)).

Most live in the `Osirion.Blazor.Cms.Web.Components` namespace. `DocumentPage`, `HomePage` and `LandingPage` live in
`Osirion.Blazor.Cms.Components`, next to the section components.

```razor
@using Osirion.Blazor.Cms.Web.Components
@using Osirion.Blazor.Cms.Components
```

## Components

| Component | Use it for |
| --- | --- |
| [ContentList](/components/content-list) | Blog indexes and category, tag, author and search pages, with pagination and list SEO. |
| [ContentView](/components/content-view) | One item as an article: header, sanitized body, categories, tags, previous and next links. |
| [LocalizedContentView](/components/localized-content-view) | The same, with links to the item's translations. |
| [ContentPage](/components/content-page) | A whole article page loaded by URL: breadcrumbs, sidebar, share links, related items. |
| [ContentRenderer](/components/content-renderer) | Only the body, with code highlighting, inside a layout you build. |
| [ArticlePage](/components/article-page) | A whole article page from one item: head metadata, hero, breadcrumbs, related articles and a sidebar. |
| [DocumentPage](/components/document-page) | A whole documentation page: side navigation, table of contents, edit link and previous and next links. |
| [HomePage](/components/home-page) | A home page from one item: hero, body, a section of content items and a browse section. |
| [LandingPage](/components/landing-page) | A marketing page: a tall hero, the body in a section you size and style, and your own content. |
| [AboutPage](/components/about-page) | An about page from the item with the slug `about`, with room for author cards. |
| [ContactPage](/components/contact-page) | A contact page: a heading, an optional Markdown introduction and a contact form. |
| [NotFoundPage](/components/not-found-page) | A page not found page with a way home, a search box and suggested pages. Answers 404. |
| [SeoMetadataRenderer](/components/seo-metadata-renderer) | Title, description, canonical URL, Open Graph, Twitter Card and JSON-LD in the page head. |
| [Feeds and sitemap](/components/feeds-and-sitemap) | `/sitemap.xml`, `/feed.xml` and `/atom.xml` endpoints. Available from 4.1. |

## Choosing one

- Need a list? Use `ContentList`; it also writes the list page's head metadata.
- Need an article page with little code? [ArticlePage](/components/article-page) (used by the example site) loads an
  item by slug or URL and renders the hero, breadcrumbs, table of contents and head metadata. `ContentPage` is the
  older, simpler layout and writes no head metadata.
- Need a documentation page? Use [DocumentPage](/components/document-page). For the home, about, contact, landing and
  not found pages there are [HomePage](/components/home-page), [AboutPage](/components/about-page),
  [ContactPage](/components/contact-page), [LandingPage](/components/landing-page) and
  [NotFoundPage](/components/not-found-page).
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
