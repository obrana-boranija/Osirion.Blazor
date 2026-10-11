---
id: 'core-rendering-components-overview'
order: 1
layout: docs
title: Core Rendering Components
permalink: /docs/components/core/rendering
description: OsirionHtmlRenderer writes sanitized HTML, such as rendered Markdown, into a Blazor page, with optional Prism syntax highlighting for code blocks.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Rendering
tags:
- blazor
- html
- sanitization
- syntax-highlighting
- static-ssr
is_featured: true
published: true
slug: rendering
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Rendering Components - Osirion.Blazor'
  description: 'Render sanitized HTML in Blazor with OsirionHtmlRenderer, with optional Prism syntax highlighting, line numbers and copy buttons.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/rendering'
  lang: en
  robots: index, follow
  og_title: 'Core Rendering Components - Osirion.Blazor'
  og_description: 'Render sanitized HTML in Blazor, with optional syntax highlighting for code blocks.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Rendering Components - Osirion.Blazor'
  twitter_description: 'Render sanitized HTML in Blazor, with optional syntax highlighting for code blocks.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Core Rendering Components

The rendering components put HTML you did not write in Razor onto a page: the output of a Markdown processor, a
CMS field or an API response. They live in the `Osirion.Blazor.Components` namespace of the Osirion.Blazor.Core
package:

```razor
@using Osirion.Blazor.Components
```

## Components

| Component | What it renders | Script | Available |
| --- | --- | --- | --- |
| [HTML renderer](/docs/components/core/rendering/html-renderer) | Sanitized HTML in an `<article>`, with Prism highlighting, line numbers and copy buttons for code blocks | Only for code blocks | 4.0 |

The folder also holds `OsirionReveal`, which animates content as it scrolls into view with a small script. It does
not have a page yet.

## Sanitizing is the default

Since 4.0, `OsirionHtmlRenderer` sanitizes its input with the registered `IHtmlContentSanitizer`, an allow-list
built on HtmlSanitizer. Script, event handler attributes, `javascript:` and `data:` URLs, forms, iframes, inline
styles and SVG are removed before the markup reaches the page. The CMS content components (`ContentView`,
`DocumentPage` and the others) sanitize the same way, so you only reach for the renderer directly when you hold
HTML outside the CMS.

## Related

- [Core content components](/docs/components/core/content)
- [CMS Web components](/docs/components/cms-web)
- [Core components](/docs/components/core)
