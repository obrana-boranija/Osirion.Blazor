---
title: "ContentRenderer - Osirion Blazor"
description: "Render only the sanitized HTML body of a content item, with code highlighting from OsirionHtmlRenderer, and build the rest of the page yourself."
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Core Components
tags:
- content-renderer
- html
- sanitization
- syntax-highlighting
slug: "content-renderer"
layout: "component"
published: true
---

# ContentRenderer

`ContentRenderer` renders the body of a content item and nothing else: no title, metadata or links. Use it when you
lay out the header, sidebar and footer of an article yourself and only need the Markdown output in the middle.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<ContentRenderer Item="@item" />
```

A custom article layout around it:

```razor
@if (item is not null)
{
    <article class="post">
        <h1>@item.Title</h1>
        <p class="post-meta">@item.Author, @item.DateCreated.ToString("MMMM d, yyyy")</p>
        <ContentRenderer Item="@item" Class="post-body" />
    </article>
}
```

`ContentRenderer` does not load anything; get the item from `IContentProviderManager` or a page component first.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Item` | `ContentItem?` | `null` | The item whose `Content` (HTML) is rendered. Only the empty wrapper renders when it or its content is null. |
| `Class` | `string?` | `null` | Extra CSS classes on the root element. |

`Style`, `Theme` and unmatched attributes from the base class are not written to the markup.

## Behavior

- **Rendering.** The HTML is passed to [OsirionHtmlRenderer](/docs/components/core/rendering/html-renderer) with its
  default settings and rendered on the server under static SSR.
- **Security.** `OsirionHtmlRenderer` sanitizes the HTML with the registered `IHtmlContentSanitizer` before writing
  it, so script, event handler attributes, `javascript:` and `data:` URLs, forms, iframes and inline SVG never reach
  the page, whatever provider the item came from.
- **Code blocks.** When the HTML contains code blocks, the renderer adds Prism 1.29.0 from cdnjs (verified with
  Subresource Integrity) to highlight them. The code is readable without script; highlighting is an enhancement.
  Line numbers and the copy button are off. To change these settings, or to self-host Prism with `PrismBaseUrl`, use
  `OsirionHtmlRenderer` directly with `HtmlContent="@item.Content"`.
- **SEO.** Nothing is written to the head.

## Styling

The root element is a `div` with the class `osirion-content-view` (the same class as `ContentView`), containing
`osirion-content-body` and the renderer's `article`. The body takes its line height and text color from the theme
tokens `--osirion-line-height-relaxed` and `--osirion-text-primary`. The renderer also links its own stylesheet,
`_content/Osirion.Blazor.Core/css/osirion-html-renderer.min.css`.

## Related

- [ContentView](/docs/components/cms-web/core/content-view): the body plus header, metadata and footer
- [HTML renderer](/docs/components/core/rendering/html-renderer): highlighting options and Prism settings
- [CMS Web core components](/docs/components/cms-web/core)
