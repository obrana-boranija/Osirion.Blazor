---
id: 'osirion-page-loading'
order: 2
layout: docs
title: OsirionPageLoading
permalink: /components/page-loading
description: A centered CSS spinner with an optional line of text, for content that is still loading during streaming rendering or an interactive render.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Layout
- Loading States
tags:
- blazor
- loading
- spinner
- streaming-rendering
- css
is_featured: true
published: true
slug: page-loading
lang: en
redirect_from:
- /docs/components/core/layout/page-loading
custom_fields:
  group: layout
seo_properties:
  title: 'OsirionPageLoading: loading spinner for Blazor pages | Osirion.Blazor'
  description: 'A centered CSS spinner with optional text for Blazor content that is still loading, during streaming rendering or interactively.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/page-loading'
  lang: en
  robots: index, follow
  og_title: 'OsirionPageLoading - Osirion.Blazor'
  og_description: 'A centered CSS spinner with optional text for content that is still loading.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionPageLoading - Osirion.Blazor'
  twitter_description: 'A CSS loading spinner with optional text for Blazor pages.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionPageLoading

`OsirionPageLoading` renders a centered spinner with an optional line of text. Show it while a page or section
waits for data: during streaming rendering in static SSR, or while an interactive component loads.

## Usage

```razor
@using Osirion.Blazor.Components

<OsirionPageLoading />
```

A realistic case is a streamed page that loads content asynchronously:

```razor
@page "/articles"
@attribute [StreamRendering]
@using Osirion.Blazor.Components

@if (articles is null)
{
    <OsirionPageLoading Text="Loading articles..." />
}
else
{
    <ul>
        @foreach (var title in articles)
        {
            <li>@title</li>
        }
    </ul>
}

@code {
    private string[]? articles;

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(500); // stands in for your data call
        articles = ["Getting started", "Theming"];
    }
}
```

With `[StreamRendering]` the server sends the spinner first and replaces it with the list when the data arrives,
without any script of yours. The CMS components (`ArticlePage`, `DocumentPage`, `HomePage` and the content list
section) use it the same way while they load content.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `ShowText` | `bool` | `true` | Shows the text below the spinner. |
| `Text` | `string` | `"Loading content..."` | The text; when it is empty, no text element is rendered. |

The component does not apply `Class`, `Style`, `Theme` or unmatched attributes to its markup; wrap it in your own
element to position or restyle it.

## Behavior

- **Rendering.** The spinner is a CSS animation, so it works under static SSR, streaming rendering and interactive
  rendering alike.
- **Accessibility.** The text is a plain paragraph; the component sets no `role="status"` or live region. If screen
  reader users should hear that content is loading, wrap it in an element with `role="status"` yourself:

```razor
<div role="status">
    <OsirionPageLoading Text="Loading articles..." />
</div>
```

## Styling

| Class | Element |
| --- | --- |
| `osirion-loading` | The container: a centered column, at least 50% of the viewport high, up to 500 pixels wide. |
| `osirion-spinner` | The 40 pixel spinner. |

The spinner's moving edge uses `--osirion-action-primary`, the text uses `--osirion-text-secondary` and the
container background uses `--osirion-background-primary`, so the CSS framework adapters recolor it. The styles are
scoped to the component, which adds an attribute selector to each rule (one class plus one attribute), so override
them from a global stylesheet with a more specific selector, such as `.my-wrapper div.osirion-loading`.

## Related

- [OsirionContentNotFound](/components/content-not-found)
- [OsirionPageLayout](/components/page-layout)
- [Layout overview](/components/layout)
