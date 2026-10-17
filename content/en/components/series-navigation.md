---
id: 'cms-web-series-navigation'
order: 7
title: ContentSeriesNavigation
permalink: /components/series-navigation
description: "ContentSeriesNavigation shows an item's place in a series, lists the parts and links the previous and next part, from series front matter."
author: Dejan Demonjić
date: 2026-10-11
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Navigation Components
tags:
- cms
- navigation
- series
- front-matter
- previous-next
published: true
slug: series-navigation
lang: en
redirect_from:
- /docs/components/cms-web/navigation/series-navigation
custom_fields:
  group: cms-navigation
seo_properties:
  title: "ContentSeriesNavigation Component - Osirion.Blazor CMS"
  description: "Link the parts of a multi-part article with series and series_order front matter: Part 2 of 5, a list of parts and previous and next links."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/components/series-navigation"
---

# ContentSeriesNavigation

Available from 4.1.

`ContentSeriesNavigation` shows where an item stands in a series ("Part 2 of 5"), lists every part and links the
previous and next part. Use it on multi-part tutorials and article series. The series comes from front matter, so
adding a part is a matter of writing its Markdown file.

## Usage

Give every part the same `series` name and number it with `series_order`:

```yaml
---
title: "Deploy the blog"
series: "Build a blog with Osirion"
series_order: 3
---
```

Then render the component with the item being shown:

```razor
@using Osirion.Blazor.Cms.Web.Components

<ContentSeriesNavigation Item="@Item" />
```

A tutorial page that shows the position and part list at the top and only the previous and next links at the
bottom, with its own wording:

```razor
@using Osirion.Blazor.Cms.Domain.Entities
@using Osirion.Blazor.Cms.Web.Components

<ContentSeriesNavigation Item="@Item" PositionFormat="Lesson {0} of {1}" />

@* the article body *@

<ContentSeriesNavigation Item="@Item"
                         ShowPartList="false"
                         PositionFormat="Lesson {0} of {1}"
                         PreviousText="Previous lesson"
                         NextText="Next lesson" />

@code {
    [Parameter]
    public ContentItem? Item { get; set; }
}
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Item` | `ContentItem?` | `null` | The item being shown. Required; nothing renders without it. |
| `ShowPartList` | `bool` | `true` | Lists every part, not only the previous and next one. |
| `PositionFormat` | `string` | `"Part {0} of {1}"` | Position text; `{0}` is the part number, `{1}` the number of parts. Formatted with the current culture. |
| `PreviousText` | `string` | `"Previous"` | Label of the previous-part link. |
| `NextText` | `string` | `"Next"` | Label of the next-part link. |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Builds each part's link. Without it, links go to `/{item.Url}`, the address the content lists use. |
| `Class` | `string?` | `null` | Extra CSS classes on the `nav`. |

Other attributes on the tag are added to the `nav`.

## Behavior

- The parts are the published items in the item's locale whose `series` matches, ignoring case. They are ordered
  by `series_order`; parts without a number come after the numbered ones, oldest first.
- Nothing renders when the item has no `series`, when the series has only one published part, or when the item
  itself is not among the parts. The content is queried only for items with a series, and only again when the item
  or its series changes.
- If the content provider fails, the error is logged through `ILogger` and the component renders nothing, so the
  article still shows.
- The `nav` is labelled with the series name and position, for example "Build a blog with Osirion: Part 3 of 5".
  The current part in the list carries `aria-current="page"`, and the previous and next links carry `rel="prev"`
  and `rel="next"`.
- Titles and the series name are written as text, so they are HTML-encoded. The links are plain anchors, so it
  works under static SSR without script.

## Styling

| Class | Element |
| --- | --- |
| `osirion-series-nav` | The `nav`, plus your `Class` |
| `osirion-series-nav-heading` | The paragraph with the position and series name |
| `osirion-series-nav-position`, `osirion-series-nav-name` | The position text and the series name |
| `osirion-series-nav-parts` | The `ol` of parts |
| `osirion-series-nav-current` | The current part's `li` |
| `osirion-series-nav-pager` | The `div` around the previous and next links |
| `osirion-series-nav-previous`, `osirion-series-nav-next` | The previous and next links |
| `osirion-series-nav-direction`, `osirion-series-nav-title` | A link's label and the part's title |

The box uses `--osirion-border-color` and `--osirion-background-secondary`; text uses `--osirion-text-primary` and
`--osirion-text-secondary`.

## Related

- [OsirionContentNavigation](/components/osirion-content-navigation), previous and next links by directory order
- [Content List](/components/content-list)
- [CMS navigation components](/components/cms-navigation)
