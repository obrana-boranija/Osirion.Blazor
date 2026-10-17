---
id: 'osirion-breadcrumbs'
order: 1
layout: docs
title: OsirionBreadcrumbs
permalink: /components/breadcrumbs
description: OsirionBreadcrumbs turns a URL path into an accessible breadcrumb trail, with a home link, custom segment text and the page title as the last crumb.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Navigation
tags:
- blazor
- breadcrumbs
- navigation
- accessibility
- static-ssr
is_featured: true
published: true
slug: breadcrumbs
lang: en
redirect_from:
- /docs/components/core/navigation/breadcrumbs
custom_fields:
  group: navigation
seo_properties:
  title: 'OsirionBreadcrumbs - Osirion.Blazor'
  description: 'Build an accessible breadcrumb trail from a URL path in Blazor, with a home link, custom segment text and the page title as the last crumb.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/breadcrumbs'
  lang: en
  robots: index, follow
  og_title: 'OsirionBreadcrumbs - Osirion.Blazor'
  og_description: 'An accessible breadcrumb trail built from a URL path, rendered without script.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionBreadcrumbs - Osirion.Blazor'
  twitter_description: 'An accessible breadcrumb trail built from a URL path, rendered without script.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionBreadcrumbs

`OsirionBreadcrumbs` renders a breadcrumb trail from a URL path: one link per segment, an optional home link first,
and the current page last. Use it at the top of articles and docs pages so readers can move up the hierarchy.

## Usage

Pass the path. Each segment becomes a crumb whose text is the slug turned into words:

```razor
<OsirionBreadcrumbs Path="/docs/components/navigation" />
```

This renders **Home / Docs / Components / Navigation**, with links to `/`, `/docs` and `/docs/components`, and the
last crumb as text.

Slugs lose punctuation and casing, so pass the page title for the last crumb with `CurrentPageText`:

```razor
<OsirionBreadcrumbs Path="/blog/pre-mvp-make-it-right"
                    CurrentPageText="Pre-MVP: make it right" />
```

The trail reads **Home / Blog / Pre-MVP: make it right** instead of ending in "Pre Mvp Make It Right". The CMS
`ArticlePage` does this for you: it passes the item's URL and title.

To build the path from the current address, take the path part of the URI so a query string does not end up in the
last segment:

```razor
@inject NavigationManager Navigation

<OsirionBreadcrumbs Path="@(new Uri(Navigation.Uri).AbsolutePath)" />
```

To change how every segment is written, pass a `SegmentFormatter`. It receives the raw segment and returns the text:

```razor
<OsirionBreadcrumbs Path="/docs/api/cms-web"
                    SegmentFormatter="@(segment => segment == "api" ? "API" : segment.Replace('-', ' '))" />
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Path` | `string` | `""` | The path to split into crumbs. Leading and trailing slashes are ignored. |
| `CurrentPageText` | `string?` | `null` | Text of the last crumb. When empty, the last segment is formatted like the others. Available since 4.0. |
| `ShowHome` | `bool` | `true` | Renders a home link before the path segments. |
| `HomeText` | `string` | `"Home"` | Text of the home link. |
| `HomeUrl` | `string` | `"/"` | Address of the home link. |
| `LinkLastItem` | `bool` | `false` | Renders the last crumb as a link instead of text. |
| `UrlPrefix` | `string` | `"/"` | Put before each segment's cumulative path to form its link, for example `/en/` for a localized site. |
| `SegmentFormatter` | `Func<string, string>?` | `null` | Formats a segment's text. Without it, hyphens become spaces and each word starts with a capital letter. |
| `Class` | `string?` | `null` | Extra CSS classes on the `<nav>` element. |

Each link is `UrlPrefix` followed by the path up to that segment: with `Path="/a/b/c"` and the default prefix, the
crumbs link to `/a` and `/a/b`, and to `/a/b/c` as well when `LinkLastItem` is set. The component renders only these parameters; additional attributes and
`Style` are not written to the markup.

## Behavior

- The component renders plain HTML and needs no script, so it works under static SSR, Interactive Server and
  WebAssembly.
- The trail is a `<nav aria-label="breadcrumb">` containing an `<ol>`: a navigation landmark named "breadcrumb"
  holding an ordered list.
- The last crumb carries `aria-current="page"`, whether it is text or a link (`LinkLastItem`).
- Segment text and `CurrentPageText` are written as text and HTML-encoded.
- The component writes no structured data. On CMS pages, `SeoMetadataRenderer` adds the `BreadcrumbList` JSON-LD
  and names its last item with the content title.

## Styling

| Class | Element |
| --- | --- |
| `osirion-breadcrumbs` | The `<nav>` |
| `osirion-breadcrumbs-list` | The `<ol>` |
| `osirion-breadcrumbs-item` | Each `<li>`; every item but the last is followed by a `/` separator drawn in CSS |
| `osirion-breadcrumbs-current` | The last `<li>` |
| `osirion-breadcrumbs-link` | Each link |
| `osirion-breadcrumbs-text` | The last crumb when it is not a link |

The colors come from the theme tokens `--osirion-text-primary` (links), `--osirion-action-primary` (link hover),
`--osirion-text-secondary` (current page) and `--osirion-text-tertiary` (separator), so a framework adapter recolors
the trail with the rest of the site. Spacing uses `--osirion-spacing-2` and `--osirion-spacing-6`. At 768px wide and
narrower, the text drops to `--osirion-font-size-sm` and the separator margin narrows.

## Related

- [Read more link](/components/read-more-link)
- [Core navigation components](/components/navigation)
- [CMS Web navigation](/components/cms-navigation)
