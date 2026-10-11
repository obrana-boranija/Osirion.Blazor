# OsirionResponsiveImage

Purpose
An image that keeps its space while it loads, offers the browser files sized for the screen (`srcset`), and loads
lazily unless it is the page's main image. It renders a plain `<img>` (inside `<figure>` when it has a caption), so
it works under static server-side rendering without script.

Key parameters
- `Src` (required): the image URL, used by browsers without `srcset` support and passed to `SrcFormatter`
- `Alt` (required): the text alternative; an empty string marks a decorative image
- `Width`, `Height`: intrinsic size in pixels; set both so the page does not shift while the image loads
- `SrcSet`: the `srcset` candidates as written, or
- `Widths` + `SrcFormatter`: the widths to offer and how each file's URL is built, for an image service or CDN
- `Sizes`: how wide the image is shown; defaults to `auto, 100vw` for lazy images and `100vw` for priority images
  when there is a `srcset`
- `Priority`: the page's main image; loads at once with `fetchpriority="high"` instead of lazily
- `Caption`: wraps the image in `<figure>` with a `<figcaption>`
- `Class` and unmatched attributes go on the `<img>`

Usage
```razor
<OsirionResponsiveImage Src="@Photo(1600)"
                        Alt="A laptop showing charts on a desk"
                        Width="1600" Height="1066"
                        Widths="[640, 960, 1280, 1600]"
                        SrcFormatter="(_, width) => Photo(width)"
                        Sizes="(min-width: 992px) 720px, 100vw"
                        Priority="true" />

@code {
    private static string Photo(int width) =>
        $"https://images.example.com/laptop.jpg?w={width}&h={width * 2 / 3}&fit=crop";
}
```

Notes
- Every file in the `srcset` should have the aspect ratio of `Width` and `Height`; the CSS keeps that ratio
  (`height: auto`) while the image scales down.
- `sizes="auto"` lets the browser use the laid-out width of a lazy image; browsers without support skip it and use
  the next value (`100vw`).
- Mark only one image per page as `Priority`, the one in the first view. Lazy loading that image would delay
  Largest Contentful Paint.
- Width and Height must be positive when set; other values throw `ArgumentOutOfRangeException`.
