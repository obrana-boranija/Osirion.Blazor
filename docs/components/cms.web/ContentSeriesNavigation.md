# ContentSeriesNavigation

Purpose
Shows where an article stands in its series ("Part 2 of 5"), lists the parts and links the previous and next part.
The series comes from the article's `series` and `series_order` front matter.

Key parameters
- `Item` (required): the article being shown
- `ContentUrlFormatter`: how a part's URL is built; defaults to `/{item.Url}`
- `ShowPartList`: list every part, not only the neighbours (default true)
- `PositionFormat`: `Part {0} of {1}` by default; `{0}` is the part number, `{1}` the number of parts
- `PreviousText`, `NextText`: link labels, `Previous` and `Next` by default

Usage
```razor
<ContentSeriesNavigation Item="@item" />
```

```yaml
---
title: "Deploying the blog"
series: "Build a blog with Osirion"
series_order: 3
---
```

Notes
- Parts are the published items in the article's locale with the same series name (ignoring case), ordered by
  `series_order`, then parts without a number, oldest first.
- Nothing renders for an article outside a series, for a series with one published part, or when the content
  provider fails (the error is logged).
- The links carry `rel="prev"` and `rel="next"`; the current part is marked `aria-current="page"`, and the `nav` is
  labelled with the series name and position.
