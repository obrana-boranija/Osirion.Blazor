# OsirionVideoEmbed

Purpose
A YouTube or Vimeo video that shows a thumbnail and loads the player only when the visitor presses play. Pages stay
fast, and nothing from the video host but the thumbnail loads before that click. Without script the thumbnail is a
link that opens the video on its own site.

Key parameters
- `VideoId` (required): the 11 characters after `v=` for YouTube, the number in the URL for Vimeo
- `Title` (required): names the play link and the player for screen readers and shows on the thumbnail
- `Provider`: `VideoProvider.YouTube` (default) or `VideoProvider.Vimeo`
- `ThumbnailUrl`: defaults to YouTube's `hqdefault.jpg`; Vimeo needs one, or shows a plain frame
- `StartSeconds`: where playback starts
- `PrivacyEnhanced`: `youtube-nocookie.com` for YouTube and `dnt=1` for Vimeo (default true)
- `AspectRatio`: `Widescreen` (16:9, default), `Standard` (4:3), `Square`, `Vertical` (9:16)
- `PlayLabelText`: the play link's accessible name; defaults to "Play video: {Title}"

Usage
```razor
<OsirionVideoEmbed VideoId="V0Af7y7aMBE" Title="Build better web apps with Blazor in .NET 10" />

<OsirionVideoEmbed Provider="VideoProvider.Vimeo"
                   VideoId="76979871"
                   Title="Product tour"
                   ThumbnailUrl="/img/tour-poster.jpg"
                   StartSeconds="30" />
```

Notes
- Requires `builder.Services.AddJSComponents()`, like the other Osirion components with collocated scripts.
- The player is an iframe with `autoplay`, so one click starts the video; focus moves to it.
- The default YouTube thumbnail comes from `i.ytimg.com`, a Google host. Set `ThumbnailUrl` to an image on your own
  site when no request may reach the video host before the visitor chooses to play.
- Video IDs are checked (YouTube: 11 letters, digits, `-` or `_`; Vimeo: digits) and an invalid one throws
  `ArgumentException`, so no other text reaches the player URL.
- The frame shape comes from CSS classes (`aspect-ratio`), with no inline styles, so a strict Content-Security-Policy
  works. Allow `frame-src https://www.youtube-nocookie.com https://player.vimeo.com` and `img-src https://i.ytimg.com`.
