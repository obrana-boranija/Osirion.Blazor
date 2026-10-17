---
order: 2
title: ContentView
permalink: /components/content-view
description: "Render one content item as an article: title, author, date, reading time, featured image, sanitized body, categories, tags and previous and next links."
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Core Components
tags:
- content-view
- cms
- article
- sanitization
slug: content-view
layout: "component"
published: true
redirect_from:
- /docs/components/cms-web/core/content-view
custom_fields:
  group: cms-pages
---

# ContentView

`ContentView` renders one content item as an article: a header with the title, author, date, reading time, tags and
featured image, the sanitized HTML body, and a footer with categories, tags and optional previous and next links. Use
it when you build your own page around a single item.

## Usage

Pass an item you already loaded:

```razor
@using Osirion.Blazor.Cms.Web.Components

<ContentView Item="@item" />
```

Or let the component load it by the item's source path (`ContentItem.Path`, as the provider reports it):

```razor
<ContentView Path="blog/getting-started.md"
             ShowNavigationLinks="true"
             ContentUrlFormatter="@(item => "/" + item.Url)"
             TagUrlFormatter="@(tag => $"/blog/tag/{Uri.EscapeDataString(tag)}")"
             AuthorUrlFormatter="@(name => $"/authors/{Uri.EscapeDataString(name.ToLowerInvariant().Replace(' ', '-'))}")" />
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Item` | `ContentItem?` | `null` | The item to render. An item set by the parent is never replaced by `Path` loading. |
| `Path` | `string` | `""` | Source path to load from the default provider when `Item` is not set. |
| `ShowNavigationLinks` | `bool` | `false` | Shows previous and next links. When the component loads by `Path`, it looks the neighbors up with a directory query built from the folder part of `Path`, newest first. |
| `PreviousItem` | `ContentItem?` | `null` | Item for the previous link. |
| `NextItem` | `ContentItem?` | `null` | Item for the next link. |
| `CategoryUrlFormatter` | `Func<string, string>?` | `null` | Builds category links; default `/category/{slug}`. |
| `TagUrlFormatter` | `Func<string, string>?` | `null` | Builds tag links; default `/tag/{slug}`. |
| `AuthorUrlFormatter` | `Func<string, string>?` | `null` | When set, the author's name links to the returned URL with `rel="author"`. Available from 4.1. |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Builds previous and next links; default `/{item.Path}`. |
| `LoadingText` | `string` | `"Loading content..."` | Shown while loading, in interactive render modes only. |
| `NotFoundText` | `string` | `"Content not found."` | Message when no item is found. |
| `Class` | `string?` | `null` | Extra CSS classes on the root element. |

`Style`, `Theme` and unmatched attributes from the base class are not written to the markup.

The default previous and next link is the item's source path, which usually differs from its public URL. When you
show navigation links, set `ContentUrlFormatter`, for example `item => "/" + item.Url`.

## Behavior

- **Rendering.** Everything renders on the server under static SSR; there is no script. The loading text appears only
  in interactive render modes. When `Path` changes, the previous item and the neighbors the component loaded are
  cleared before the new one loads, so an old article never shows under a new address.
- **Not found.** Without an item the component renders a "Content Not Found" heading, `NotFoundText` and a link
  back to `/`.
- **Security.** The body is written as HTML after it passes through the registered `IHtmlContentSanitizer` (the
  default allow-list when none is registered), even when the item comes from a custom provider or from your code.
  Script, event handler attributes, `javascript:`, `vbscript:` and `data:` URLs, forms, iframes, inline SVG and
  inline styles (apart from table cell alignment) are removed. The result is cached until the content changes. To allow a custom element, register it with
  `AddOsirionHtmlSanitizer(options => options.AllowElement("chart", "type", "data"))`. Title, author and tags are
  encoded as text.
- **Images.** The featured image uses the item title as its `alt` text. From 4.0 it loads eagerly with
  `fetchpriority="high"` and async decoding, because it is usually the largest element on an article page.
- **Dates.** The date is `DateCreated` (the `date` front matter), formatted as `MMM d, yyyy`.
- **SEO.** `ContentView` writes nothing to the page head. Render a
  [SeoMetadataRenderer](/components/seo-metadata-renderer) inside `<HeadContent>` next to it.

## Styling

The root element has the class `osirion-content-view`; the article inside is `osirion-content-article`. The header
uses `osirion-content-header`, `osirion-content-title` and `osirion-content-meta` (items `osirion-content-author`,
`osirion-content-date`, `osirion-content-read-time`), the body `osirion-content-body`, and the footer
`osirion-content-footer`, `osirion-content-category`, `osirion-content-tag` and `osirion-content-navigation`
(`osirion-content-previous`, `osirion-content-next`). Tags are rendered twice, in the header
(`osirion-content-tags-inline`) and in the footer (`osirion-content-tags-mobile`), and the library styles neither
class, so both lists show at every width; hide one in your own CSS if you want only one. Colors and spacing
come from theme tokens such as `--osirion-text-primary`, `--osirion-text-secondary`, `--osirion-border-color` and
`--osirion-background-secondary`.

## Related

- [LocalizedContentView](/components/localized-content-view): the same with translation links
- [ContentRenderer](/components/content-renderer): the body only, with code highlighting
- [ContentPage](/components/content-page): a full page layout around an item
- [ContentList](/components/content-list): lists, including `Author` for author pages
- [OsirionAuthorCard](/components/author-card): the author introduction on an author page
