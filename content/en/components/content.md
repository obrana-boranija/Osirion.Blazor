---
id: 'core-content-components-overview'
order: 1
layout: docs
title: Content
permalink: /components/content
description: Components for the body of articles and docs pages in Osirion.Blazor Core, from callouts and tabs to video embeds, rendering under static SSR.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Content
tags:
- blazor
- content
- articles
- static-ssr
- accessibility
is_featured: true
published: true
slug: content
lang: en
redirect_from:
- /docs/components/core/content
- /docs/components/core/rendering
custom_fields:
  group: content
seo_properties:
  title: 'Core Content Components - Osirion.Blazor'
  description: 'Callouts, tabs, accordions, FAQs, responsive images, video embeds and article metadata for Blazor pages, working under static SSR.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/content'
  lang: en
  robots: index, follow
  og_title: 'Core Content Components - Osirion.Blazor'
  og_description: 'Components for articles and docs pages that render under static SSR.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Content Components - Osirion.Blazor'
  twitter_description: 'Components for articles and docs pages that render under static SSR.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Content

These components go inside the body of an article or a documentation page. Each one renders complete HTML under
static server-side rendering; where a script is involved, it only adds to what is already on the page.

They live in the `Osirion.Blazor.Components` namespace of the Osirion.Blazor.Core package:

```razor
@using Osirion.Blazor.Components
```

## Components

| Component | What it renders | Script | Available |
| --- | --- | --- | --- |
| [Article metadata](/components/article-metadata) | Author, publish date and read time in one line | None | 4.0 |
| [Callouts](/components/callout) | A highlighted note, tip, important point, warning or caution, from Razor or Markdown | None | 4.0 |
| [Tabs](/components/tabs) | Alternative content, such as one install command per package manager, behind tabs | None | 4.0 |
| [Accordion and FAQ](/components/accordion-and-faq) | Collapsible sections on `<details>`, and questions with FAQPage structured data | None | 4.0 |
| [Responsive image](/components/responsive-image) | An `<img>` with intrinsic size, `srcset`, `sizes` and lazy loading | None | 4.0 |
| [Video embed](/components/video-embed) | A YouTube or Vimeo thumbnail that loads the player when pressed | Optional | 4.0 |
| [Reading progress](/components/reading-progress) | A bar at the top of the window that fills as the reader scrolls | None | 4.0 |
| [HTML renderer](/components/html-renderer) | Sanitized HTML in an `<article>`, with Prism highlighting, line numbers and copy buttons for code blocks | Only for code blocks | 4.0 |

"Optional" means the component works without script and the script adds behavior: the video embed is a link to the
video until its script swaps in the player.

## Razor or Markdown

Markdown pages rendered by the CMS cannot contain Razor components. Callouts are the exception: write them with
GitHub's alert syntax (`> [!NOTE]`) and they get the same styling as `OsirionCallout`. The other components are for
Razor pages and layouts.

## HTML you did not write in Razor

`OsirionHtmlRenderer` puts HTML you did not write in Razor onto a page: the output of a Markdown processor, a CMS
field or an API response. The same folder holds `OsirionReveal`, which animates content as it scrolls into view with
a small script. It does not have a page yet.

Since 4.0, `OsirionHtmlRenderer` sanitizes its input with the registered `IHtmlContentSanitizer`, an allow-list
built on HtmlSanitizer. Script, event handler attributes, `javascript:` and `data:` URLs, forms, iframes, inline
styles and SVG are removed before the markup reaches the page. The CMS content components (`ContentView`,
`DocumentPage` and the others) sanitize the same way, so you only reach for the renderer directly when you hold
HTML outside the CMS.

## Related

- [Carousels](/components/sections#carousels)
- [CMS Web components](/docs/packages/cms-web)
- [Core components](/docs/packages/core)
