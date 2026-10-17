---
id: 'osirion-responsive-image'
order: 5
layout: docs
title: Responsive Image
permalink: /components/responsive-image
description: "OsirionResponsiveImage renders an img with intrinsic size, srcset, sizes and lazy loading, so images neither shift the layout nor load too large."
author: Dejan Demonjić
date: 2026-10-11
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Content
tags:
- images
- srcset
- lazy-loading
- core-web-vitals
- static-ssr
slug: responsive-image
lang: en
published: true
redirect_from:
- /docs/components/core/content/responsive-image
custom_fields:
  group: content
---

# Responsive Image

`OsirionResponsiveImage` renders an `<img>` that keeps its space while it loads, offers the browser files sized for
the screen, and loads lazily unless it is the page's main image. It is plain HTML, so it needs no script.

Available from 4.1.

## Usage

```razor
@using Osirion.Blazor.Components

<OsirionResponsiveImage Src="/images/team.jpg" Alt="The team at the 2026 meetup" Width="1200" Height="800" />
```

This renders a lazily loaded, asynchronously decoded image with `width` and `height`, so the browser reserves its
space before the file arrives.

With an image service or CDN that resizes on request, give the widths to offer and how to build each file's URL:

```razor
<OsirionResponsiveImage Src="@PhotoAt(1600)"
                        Alt="A laptop showing charts on a desk"
                        Width="1600"
                        Height="1066"
                        Widths="imageWidths"
                        SrcFormatter="(_, width) => PhotoAt(width)"
                        Sizes="(min-width: 992px) 720px, 100vw"
                        Priority="true"
                        Caption="Monthly figures for the second quarter." />

@code {
    private readonly int[] imageWidths = [640, 960, 1280, 1600];

    private static string PhotoAt(int width) =>
        $"https://images.example.com/laptop.jpg?w={width}&h={width * 2 / 3}&fit=crop";
}
```

The `srcset` becomes four candidates from 640 to 1600 pixels wide. `Priority` marks this as the main image of the
page, so it loads at once instead of lazily. `images.example.com` stands for your own image service.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Src` | `string` | `""` | Image URL, used by browsers that ignore `srcset` and passed to `SrcFormatter`. Marked `[EditorRequired]`. |
| `Alt` | `string` | `""` | Marked `[EditorRequired]`. Text alternative. Describe what the image shows; an empty string marks a purely decorative image. |
| `Width` | `int?` | `null` | Intrinsic width in pixels. Set it with `Height` to reserve the space and avoid layout shift. |
| `Height` | `int?` | `null` | Intrinsic height in pixels. |
| `SrcSet` | `string?` | `null` | The `srcset` candidates as written, for example `/img/a-640.jpg 640w, /img/a-1280.jpg 1280w`. Takes precedence over `Widths`. |
| `Widths` | `IReadOnlyList<int>?` | `null` | Widths in pixels to offer in `srcset`. Ignored without `SrcFormatter`. |
| `SrcFormatter` | `Func<string, int, string>?` | `null` | Builds the URL of the file of a given width from `Src`, for example `(src, width) => $"{src}?w={width}"`. |
| `Sizes` | `string?` | see below | The `sizes` attribute: how wide the image is shown. |
| `Priority` | `bool` | `false` | The page's main image: loads eagerly with `fetchpriority="high"` instead of lazily. |
| `Caption` | `string?` | `null` | Wraps the image in `<figure>` with this text in a `<figcaption>`. |
| `Class` | `string?` | `null` | Extra classes on the `<img>`. |

Unmatched attributes go on the `<img>`, not on the `<figure>`.

When there is a `srcset` and `Sizes` is not set, lazily loaded images get `sizes="auto, 100vw"` (the browser uses the
laid-out width where it supports `auto`) and priority images get `100vw`. Without a `srcset`, no `sizes` attribute is
written.

## Behavior

- Every image gets `decoding="async"`. Without `Priority` it also gets `loading="lazy"`; with `Priority` it gets
  `loading="eager"` and `fetchpriority="high"`. Mark only one image per page as `Priority`, the one in the first view:
  lazy loading that image would delay Largest Contentful Paint.
- `Widths` are sorted, duplicates and values of zero or less are dropped. If nothing is left, no `srcset` is written.
- Every file in the `srcset` should have the aspect ratio of `Width` and `Height`. The stylesheet sets
  `height: auto` and `max-width: 100%`, so the image scales down inside its container and keeps that ratio.
- `Width` or `Height` set to zero or a negative number throws `ArgumentOutOfRangeException`.
- URLs are written as attributes, so Razor encodes them. The component does not check where they point; pass URLs
  you control or have validated.

## Styling

| Class | Element |
| --- | --- |
| `osirion-responsive-image` | The `<img>`: `display: block`, `max-width: 100%`, `height: auto` |
| `osirion-responsive-image-figure` | The `<figure>` when there is a caption |
| `osirion-responsive-image-caption` | The `<figcaption>` |

The figure's bottom margin uses `--osirion-spacing-4`, and the caption uses `--osirion-spacing-2`,
`--osirion-font-size-sm` and `--osirion-text-secondary`, each with a fallback value.

## Related

- [Video embed](/components/video-embed)
- [Content components](/components/content)
