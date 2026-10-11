---
id: 'core-states-components-overview'
order: 1
layout: docs
title: Core State Components
permalink: /docs/components/core/states
description: Components for pages without content in Osirion.Blazor Core, such as a 404 page with suggestions and a loading indicator.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- States
tags:
- blazor
- 404
- not-found
- empty-state
- static-ssr
is_featured: true
published: true
slug: states
lang: en
custom_fields: {}
seo_properties:
  title: 'Core State Components - Osirion.Blazor'
  description: 'Not found and empty states for Blazor pages with OsirionContentNotFound, rendered without script under static SSR.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/states'
  lang: en
  robots: index, follow
  og_title: 'Core State Components - Osirion.Blazor'
  og_description: 'Not found and empty states for Blazor pages, rendered without script.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core State Components - Osirion.Blazor'
  twitter_description: 'Not found and empty states for Blazor pages, rendered without script.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Core State Components

State components fill a page, or part of one, when there is no content to show: the address points nowhere, a list
is empty, or the content is still on its way. They live in the `Osirion.Blazor.Components` namespace of the
Osirion.Blazor.Core package:

```razor
@using Osirion.Blazor.Components
```

## Components

| Component | What it renders | Script | Available |
| --- | --- | --- | --- |
| [Content not found](/docs/components/core/states/content-not-found) | A 404 or empty state with a message, buttons, suggested links and a contact address | None | 4.0 |
| [Page loading](/docs/components/core/layout/page-loading) | A loading indicator; documented with the layout components | See its page | 4.0 |

## Status codes

These components only render markup; they do not set the HTTP status code. A not found page should answer with
404 so search engines do not index it as a real page. The CMS page components (`ArticlePage`, `DocumentPage`,
`HomePage`) do this for you; on your own pages, see
[content not found](/docs/components/core/states/content-not-found#status-code).

## Related

- [Core layout components](/docs/components/core/layout)
- [Core components](/docs/components/core)
