---
id: 'cms-web-table-of-contents'
order: 8
title: "TableOfContents - Osirion Blazor CMS"
permalink: /docs/components/cms-web/navigation/table-of-contents
description: "TableOfContents reads the headings of an HTML string, such as a rendered CMS item, and renders links to them as a nested list."
author: Dejan Demonjić
date: "2024-12-29"
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Navigation Components
tags:
- cms
- navigation
- table-of-contents
- headings
published: true
slug: "table-of-contents"
lang: en
seo_properties:
  title: "TableOfContents Component - Osirion.Blazor CMS"
  description: "Build a table of contents from the headings of rendered Markdown or any HTML string, with heading level limits and no script."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/docs/components/cms-web/navigation/table-of-contents"
---

# TableOfContents

`TableOfContents` reads the `h1` to `h6` headings of an HTML string and renders a nested list of links to them. Pass
it the rendered content of a CMS item to give long articles and documentation pages an outline.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<TableOfContents Content="@Item?.Content" />
```

A sidebar outline limited to the second and third heading levels:

```razor
@using Osirion.Blazor.Cms.Domain.Entities
@using Osirion.Blazor.Cms.Web.Components

<aside>
    <h2>On this page</h2>
    <TableOfContents Content="@Item?.Content"
                     MinLevel="2"
                     MaxLevel="3"
                     EmptyText="This page has no sections." />
</aside>

@code {
    [Parameter]
    public ContentItem? Item { get; set; }
}
```

The built-in `ArticlePage` and `DocumentPage` render it the same way, under their own `TableOfContentsTitle`
heading.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Content` | `string?` | `null` | The HTML to read headings from. |
| `MinLevel` | `int` | `2` | Lowest heading level included, so the page's `h1` is left out by default. |
| `MaxLevel` | `int` | `6` | Highest heading level included. |
| `MaxDepth` | `int` | `3` | Nesting limit. `1` lists only top-level headings; the list renders at most two levels, so values above `2` change nothing. |
| `EmptyText` | `string` | `"No headings found."` | Shown when no heading matches. |
| `Class` | `string?` | `null` | Extra CSS classes on the `nav`. |

Other attributes on the tag are captured but not rendered.

## Behavior

- Each link points to `{current path}#{heading id}`. The CMS Markdown renderer gives every heading an `id` and the
  HTML sanitizer keeps it, so the links work for CMS content. For a heading without an `id`, the component makes one
  from the text (lower case, letters and digits, spaces turned into hyphens), and the link only works if your page
  has an element with that id.
- The heading's `id` is read only when it is the first attribute of the tag, as in the renderer's output.
- Headings are nested by level: an `h3` goes under the `h2` before it. The list shows two levels, the top-level
  headings and the headings directly under them. Deeper headings are not shown.
- Tags inside a heading are stripped and the remaining text is written as text, so it is HTML-encoded. Character
  references are not decoded first, so a heading such as `Q&A`, which the Markdown renderer writes as `Q&amp;A`,
  shows as `Q&amp;A` in the outline.
- The component always renders a `nav` with the accessible name "Table of Contents" (fixed English text), and the
  `EmptyText` paragraph when there is nothing to list. Headings are read on every parameter change, with no
  script, so the outline is part of the static SSR response.

## Styling

| Class | Element |
| --- | --- |
| `osirion-table-of-contents` | The `nav`, plus your `Class` |
| `osirion-toc-list` | The top-level `ol` |
| `osirion-toc-sublist` | The nested `ol` |
| `osirion-toc-item` | Each `li`, together with `osirion-toc-level-{n}` for the heading level |
| `osirion-toc-link` | Each link |
| `osirion-toc-empty` | The empty-state paragraph |

Links use `--osirion-text-secondary` and `--osirion-action-primary`, with `--osirion-focus-ring-color` for keyboard
focus. The stylesheet also has print rules.

## Related

- [OsirionStickySidebar](/docs/components/core/layout/sticky-sidebar), to keep the outline in view while reading
- [Content View](/docs/components/cms-web/core/content-view)
- [CMS navigation components](/docs/components/cms-web/navigation)
