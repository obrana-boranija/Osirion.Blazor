---
id: 'core-content-components-overview'
order: 1
layout: docs
title: Core Content Components
permalink: /docs/components/core/content
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
custom_fields: {}
seo_properties:
  title: 'Core Content Components - Osirion.Blazor'
  description: 'Callouts, tabs, accordions, FAQs, responsive images, video embeds and article metadata for Blazor pages, working under static SSR.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/content'
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

# Core Content Components

These components go inside the body of an article or a documentation page. Each one renders complete HTML under
static server-side rendering; where a script is involved, it only adds to what is already on the page.

They live in the `Osirion.Blazor.Components` namespace of the Osirion.Blazor.Core package:

```razor
@using Osirion.Blazor.Components
```

## Components

| Component | What it renders | Script | Available |
| --- | --- | --- | --- |
| [Article metadata](/docs/components/core/content/article-metadata) | Author, publish date and read time in one line | None | 4.0 |
| [Callouts](/docs/components/core/content/callout) | A highlighted note, tip, important point, warning or caution, from Razor or Markdown | None | 4.1 |
| [Tabs](/docs/components/core/content/tabs) | Alternative content, such as one install command per package manager, behind tabs | None | 4.1 |
| [Accordion and FAQ](/docs/components/core/content/accordion-and-faq) | Collapsible sections on `<details>`, and questions with FAQPage structured data | None | 4.1 |
| [Responsive image](/docs/components/core/content/responsive-image) | An `<img>` with intrinsic size, `srcset`, `sizes` and lazy loading | None | 4.1 |
| [Video embed](/docs/components/core/content/video-embed) | A YouTube or Vimeo thumbnail that loads the player when pressed | Optional | 4.1 |
| [Reading progress](/docs/components/core/content/reading-progress) | A bar at the top of the window that fills as the reader scrolls | None | 4.1 |

"Optional" means the component works without script and the script adds behavior: the video embed is a link to the
video until its script swaps in the player.

## Razor or Markdown

Markdown pages rendered by the CMS cannot contain Razor components. Callouts are the exception: write them with
GitHub's alert syntax (`> [!NOTE]`) and they get the same styling as `OsirionCallout`. The other components are for
Razor pages and layouts.

## Related

- [Carousels](/docs/components/core/carousels)
- [Core components](/docs/components/core)
