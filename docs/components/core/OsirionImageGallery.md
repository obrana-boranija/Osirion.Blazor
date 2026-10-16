# OsirionImageGallery

Purpose
Shows a responsive grid of images with captions. With script, clicking an image opens a modal viewer with previous and
next navigation. Without script, each image is a plain link to the full-size file.

Key parameters
- Items: `IReadOnlyList<OsirionImageGallery.GalleryItem>`, required. The images to show, in order
- Columns: int, default 2. Column count at large widths, clamped to 1 to 4
- GapSize: int, default 4. Gap between items in steps of 0.25rem, clamped to 0 to 5
- Dark: bool, default false. Uses a light caption color for dark surfaces
- Theme: `ThemeMode` (default `System`), inherited from `OsirionComponentBase`. `Light`, `Dark` and `Inverted` add a
  `data-theme` attribute (`data-bs-theme` when the Bootstrap framework is configured)
- Class and Style: strings, inherited from `OsirionComponentBase`. The gallery's markup does not read them
- Attributes: captures unmatched attributes (`CaptureUnmatchedValues`) and renders them on the root element

`GalleryItem` is a record: `GalleryItem(string Src, string Alt, string Caption = "", int Width = 1438,
int Height = 1136)`. `Width` and `Height` are written to the `img` element so the browser can reserve space.

Usage
```razor
<OsirionImageGallery Items="@images" Columns="3" GapSize="3" />

@code {
    private readonly IReadOnlyList<OsirionImageGallery.GalleryItem> images =
    [
        new("/images/overview.webp", "Dashboard overview", "Overview"),
        new("/images/reports.webp", "Report builder", "Reports", 1200, 900),
        new("/images/settings.webp", "Settings page")
    ];
}
```

Notes
- Emits `div.osirion-image-gallery` (plus `osirion-image-gallery-dark` when `Dark` is true) holding
  `div.osirion-image-gallery-grid` and a hidden lightbox. Each item is a `figure.osirion-image-gallery-item` with an
  `a.osirion-image-gallery-trigger` that links to `Src`, wraps an `img` (`loading="lazy"`) and has an `aria-label`
  of `View full size: {caption}` (the alt text is used when there is no caption, and `View full size image` when both
  are empty). The caption is a `figcaption` outside the link and is omitted when empty.
- The lightbox is `div.osirion-image-gallery-lightbox` with `role="dialog"`, `aria-modal="true"` and
  `aria-label="Image viewer"`, hidden until opened. It contains a backdrop button and a close button (both labelled
  "Close image viewer"), "Previous image" and "Next image" buttons, the image, the caption and a counter such as
  `2 / 5`.
- Works in static SSR: the grid and the links to the full-size images need no script. A collocated script (loaded
  through `BlazorJSComponents`) turns each link into a viewer trigger and sets `aria-haspopup="dialog"` on it.
- Script behavior: Escape closes the viewer, Left and Right arrow keys and the buttons move between images (wrapping
  at both ends), Tab and Shift+Tab stay inside the open viewer, focus moves to the close button on open and returns to
  the trigger on close, and page scrolling is disabled while it is open.
- Reduced motion: the stylesheet turns off the image hover transition and the zoom icon fade under
  `prefers-reduced-motion: reduce`.
- At viewport widths of 700px or less the grid becomes a single column regardless of `Columns`.
- The viewer shows the thumbnail's own `src` and `alt`, so the thumbnail and full-size image are the same file.
