---
id: 'osirion-video-embed'
order: 6
layout: docs
title: Video Embed
permalink: /components/video-embed
description: "OsirionVideoEmbed shows a YouTube or Vimeo thumbnail and loads the player only when pressed, privacy-enhanced by default; without script it links to the video."
author: Dejan Demonjić
date: 2026-10-11
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Content
tags:
- video
- youtube
- vimeo
- privacy
- performance
slug: video-embed
lang: en
published: true
redirect_from:
- /docs/components/core/content/video-embed
custom_fields:
  group: content
---

# Video Embed

`OsirionVideoEmbed` shows a YouTube or Vimeo video as a thumbnail with a play button and loads the player only when
the visitor presses it. Until then, nothing but the thumbnail is requested from the video host. Without script, the
thumbnail is a link that opens the video on its own site.

Available from 4.1.

## Usage

The component loads its script through the BlazorJSComponents package, which Osirion.Blazor.Core references.
Register its services once in `Program.cs`, as the example apps do:

```csharp
builder.Services.AddJSComponents();
```

Then:

```razor
@using Osirion.Blazor.Components

<OsirionVideoEmbed VideoId="V0Af7y7aMBE" Title="Build better web apps with Blazor in .NET 10" />
```

A Vimeo video that starts at 30 seconds, with a thumbnail served from your own site:

```razor
<OsirionVideoEmbed Provider="VideoProvider.Vimeo"
                   VideoId="76979871"
                   Title="Product tour"
                   ThumbnailUrl="/img/tour-poster.jpg"
                   StartSeconds="30" />
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `VideoId` | `string` | `""` | Marked `[EditorRequired]`. YouTube: the 11 characters after `v=` in the watch URL. Vimeo: the number in the video's URL. |
| `Title` | `string` | `""` | Marked `[EditorRequired]`. The video's title, shown on the thumbnail and used to name the play link and the player for screen readers. |
| `Provider` | `VideoProvider` | `YouTube` | `YouTube` or `Vimeo`. |
| `ThumbnailUrl` | `string?` | `null` | Thumbnail image. Without it, YouTube videos use `https://i.ytimg.com/vi/{VideoId}/hqdefault.jpg` and Vimeo videos show a plain black frame. |
| `StartSeconds` | `int?` | `null` | Where playback starts, in seconds. |
| `PrivacyEnhanced` | `bool` | `true` | YouTube plays from `www.youtube-nocookie.com`; Vimeo gets `dnt=1`. |
| `AspectRatio` | `VideoAspectRatio` | `Widescreen` | `Widescreen` (16:9), `Standard` (4:3), `Square` (1:1) or `Vertical` (9:16, at most 22.5rem wide). |
| `PlayLabelText` | `string?` | `null` | Accessible name of the play link. Defaults to "Play video: " followed by `Title`; set it for other languages. |
| `Class` | `string?` | `null` | Extra classes on the container. |

Unmatched attributes go on the container `<div>`.

## Behavior

- Static SSR renders the thumbnail inside a link to the video's page (`youtube.com/watch` or `vimeo.com`), plus the
  start time if set. That link works with script off.
- With script, a plain click replaces the thumbnail with an `<iframe>` player that starts playing and receives focus.
  A click with Ctrl, Cmd, Shift or Alt, or with a button other than the main one, still follows the link, so "open in
  new tab" keeps working.
- The player URL autoplays after the click. With `PrivacyEnhanced` on, YouTube uses `youtube-nocookie.com` and Vimeo
  gets `dnt=1`. The iframe has `referrerpolicy="strict-origin-when-cross-origin"`.
- The default YouTube thumbnail comes from `i.ytimg.com`, a Google host. Set `ThumbnailUrl` to an image on your own
  site when no request may reach the video host before the visitor chooses to play.
- `VideoId` is validated: YouTube IDs must be 11 letters, digits, `-` or `_`, and Vimeo IDs digits only. Anything else
  throws `ArgumentException`, so no other text reaches the player URL. A negative `StartSeconds` throws
  `ArgumentOutOfRangeException`.
- The frame shape comes from CSS classes (`aspect-ratio`), not inline styles. With a Content-Security-Policy, allow
  `frame-src https://www.youtube-nocookie.com https://player.vimeo.com` (add `https://www.youtube.com` if you turn
  `PrivacyEnhanced` off) and `img-src https://i.ytimg.com` for the default YouTube thumbnails.

## Styling

| Class | Element |
| --- | --- |
| `osirion-video-embed` | The container, black background with rounded corners |
| `osirion-video-embed-widescreen`, `-standard`, `-square`, `-vertical` | The aspect ratio, from `AspectRatio` |
| `osirion-video-embed-link` | The link covering the thumbnail |
| `osirion-video-embed-thumbnail` | The thumbnail `<img>` (lazy loaded, `object-fit: cover`) |
| `osirion-video-embed-play` | The play button, drawn in CSS |
| `osirion-video-embed-title` | The title over the top of the thumbnail |
| `osirion-video-embed-player` | The `<iframe>` the script adds |

The corner radius uses `--osirion-radius-md` and the focus ring `--osirion-focus-ring-width` and
`--osirion-focus-ring-color`, each with a fallback value. The play button's color change on hover is not animated
under `prefers-reduced-motion: reduce`.

## Related

- [Responsive image](/components/responsive-image)
- [Content components](/components/content)
