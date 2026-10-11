---
id: 'osirion-page-layout'
order: 1
layout: docs
title: OsirionPageLayout
permalink: /docs/components/core/layout/page-layout
description: A header, main and footer page shell with a sticky footer, a header that hides on scroll and a skip link to the main content.
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
- page-layout
- sticky-footer
- skip-link
- accessibility
- static-ssr
is_featured: true
published: true
slug: page-layout
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionPageLayout: Blazor page shell with sticky footer and skip link | Osirion.Blazor'
  description: 'A header, main and footer shell for Blazor layouts with a sticky footer, a header that hides on scroll and a skip link.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/layout/page-layout'
  lang: en
  robots: index, follow
  og_title: 'OsirionPageLayout - Osirion.Blazor'
  og_description: 'Header, main and footer shell with a sticky footer, a hide-on-scroll header and a skip link.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionPageLayout - Osirion.Blazor'
  twitter_description: 'Header, main and footer shell for Blazor layouts.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionPageLayout

`OsirionPageLayout` renders the shell of a page: a `header`, a `main` and a `footer`, stacked so the footer sits at
the bottom of the viewport on short pages. Use it in your `MainLayout.razor`.

## Usage

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

The layout loads a small collocated script through BlazorJSComponents, so register its services in `Program.cs`:

```csharp
builder.Services.AddJSComponents();
```

The Bootstrap example site in the repository uses the layout this way, with an
[announcement bar](/docs/components/core/popups/announcement-bar) above the menu in the `Header` slot:

```razor
<OsirionPageLayout StickyFooter EnableHeaderScrollBehavior>
    <Header>
        <OsirionAnnouncementBar Id="release-4-1">
            New in 4.1: tabs, FAQ and feeds. <a href="/blog">Read more</a>
        </OsirionAnnouncementBar>
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

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Header` | `RenderFragment?` | `null` | Content of the `header` element; no header is rendered when it is null. |
| `Body` | `RenderFragment?` | `null` | Content of the `main` element. |
| `Footer` | `RenderFragment?` | `null` | Content of the `footer` element; no footer is rendered when it is null. |
| `StickyFooter` | `bool` | `true` | Keeps the footer at the bottom of the viewport when the content is short. |
| `EnableHeaderScrollBehavior` | `bool` | `true` | Makes the header sticky; it slides away while scrolling down and returns while scrolling up. |
| `MinHeightStrategy` | `string` | `"viewport"` | `"viewport"` sets `min-height: 100vh`; `"content"` sets `min-height: 100%`. |
| `ShowSkipLink` | `bool` | `true` | Renders a "skip to main content" link as the first focusable element. Available from 4.1. |
| `SkipLinkText` | `string` | `"Skip to main content"` | Text of the skip link; translate it for other languages. Available from 4.1. |
| `MainContentId` | `string` | `"osirion-main-content"` | `id` of the `main` element, which the skip link targets. Available from 4.1. |

`Class` and unmatched attributes go on the outer `div`.

## Behavior

- **Static SSR.** The header, main and footer render on the server. The sticky footer is pure CSS (a flex column).
- **Header on scroll.** With `EnableHeaderScrollBehavior` the header is `position: sticky`; the script hides it while
  you scroll down past the first 100 pixels and shows it again when you scroll up or return to the top. When the
  visitor prefers reduced motion the script leaves the header in place. Without script the header simply stays sticky.
- **Skip link (4.1).** The link stays off screen until it receives keyboard focus, then appears at the top left. Its
  address is the current page plus `#` and `MainContentId`, because a bare `#id` would resolve against
  `<base href="/">` and lead to the home page. The `main` element has `tabindex="-1"`, and the script moves focus to
  it after the jump, which Blazor's enhanced navigation would otherwise skip.
- **One footer landmark (4.1).** Inside the `Footer` slot, [OsirionFooter](/docs/components/core/layout/footer)
  renders a `div` instead of its own `footer`, so the page keeps a single contentinfo landmark. In 4.0 it rendered a
  nested `footer`.

## Styling

| Class | Element |
| --- | --- |
| `osirion-page-layout` | Outer `div`, plus `osirion-sticky-footer-layout` and `osirion-min-height-viewport` or `osirion-min-height-content`. |
| `osirion-page-header` | The `header`; `osirion-page-header-scroll` when the scroll behavior is on. |
| `osirion-page-main` | The `main` element. |
| `osirion-page-footer` | The `footer`. |
| `osirion-skip-link` | The skip link (4.1). |

The scrolling header has `z-index: 1000`. The skip link uses the page's own text and background colors, inverted,
and the theme's focus ring (`--osirion-focus-ring-color`, `--osirion-focus-ring-width`, `--osirion-focus-ring-offset`).

## Related

- [OsirionFooter](/docs/components/core/layout/footer)
- [OsirionAnnouncementBar](/docs/components/core/popups/announcement-bar)
- [OsirionStickySidebar](/docs/components/core/layout/sticky-sidebar)
- [Layout overview](/docs/components/core/layout)
