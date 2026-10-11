---
id: 'core-layout-components-overview'
order: 1
layout: docs
title: Layout
permalink: /docs/components/core/layout
description: 'Layout components in Osirion.Blazor.Core: page shell, footer, sticky sidebar, loading spinner and background patterns, all working under static SSR.'
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Layout
tags:
- blazor
- layout
- page-layout
- footer
- sidebar
- static-ssr
is_featured: true
published: true
slug: layout
lang: en
custom_fields: {}
seo_properties:
  title: 'Layout components - Osirion.Blazor'
  description: 'Page shell, footer, sticky sidebar, loading spinner and background patterns for Blazor sites, all working under static SSR.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/layout'
  lang: en
  robots: index, follow
  og_title: 'Layout components - Osirion.Blazor'
  og_description: 'Page shell, footer, sticky sidebar, loading spinner and background patterns for Blazor sites.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Layout components - Osirion.Blazor'
  twitter_description: 'Page shell, footer, sticky sidebar, loading spinner and background patterns.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Layout

These components build the frame of a page. All of them render their markup on the server and work under static
SSR; only `OsirionPageLayout` loads a small script, for the header that hides on scroll and for skip-link focus.

| Component | What it renders |
| --- | --- |
| [OsirionPageLayout](/docs/components/core/layout/page-layout) | `header`, `main` and `footer` with a sticky footer, a header that hides while scrolling down, and (from 4.1) a skip link. |
| [OsirionFooter](/docs/components/core/layout/footer) | Site footer with link columns, social links, copyright and legal links. |
| [OsirionStickySidebar](/docs/components/core/layout/sticky-sidebar) | An `aside` that stays in view and scrolls on its own, for a table of contents or docs navigation. |
| [OsirionPageLoading](/docs/components/core/layout/page-loading) | A centered CSS spinner with optional text. |
| [OsirionBackgroundPattern](/docs/components/core/layout/background-pattern) | A decorative CSS pattern layer behind a section. |

## A typical main layout

```razor
@inherits LayoutComponentBase
@using Osirion.Blazor.Components

<OsirionPageLayout>
    <Header>
        <NavMenu />
    </Header>
    <Body>
        @Body
    </Body>
    <Footer>
        <OsirionFooter CompanyName="Example Ltd" />
    </Footer>
</OsirionPageLayout>
```

`OsirionPageLayout` needs `builder.Services.AddJSComponents()` in `Program.cs` for its script. From 4.1, an
`OsirionFooter` inside the `Footer` slot renders a `div`, so the page has one footer landmark.

## Related

- [Popups](/docs/components/core/popups): cookie consent and the announcement bar, which usually sit in the layout too.
- [Sections](/docs/components/core/sections): hero and content sections that take a background pattern.
