---
id: 'cms-web-tag-cloud'
order: 9
title: "TagCloud - Osirion Blazor CMS"
permalink: /docs/components/cms-web/navigation/tag-cloud
description: "TagCloud renders a link to every content tag with its item count, loaded from the default CMS provider or from a list you pass."
author: Dejan Demonjić
date: "2024-12-29"
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Navigation Components
tags:
- cms
- navigation
- tags
- taxonomy
published: true
slug: "tag-cloud"
lang: en
seo_properties:
  title: "TagCloud Component - Osirion.Blazor CMS"
  description: "Render links to every CMS tag with item counts, sorting, a limit, an active tag and your own URL scheme."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/docs/components/cms-web/navigation/tag-cloud"
---

# TagCloud

`TagCloud` renders a wrapping row of tag links, each with the number of items that carry the tag. It loads the
tags from the default content provider, or shows the tags you pass. Every tag is drawn at the same size; the
component does not scale tags by count.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<TagCloud Title="Tags" MaxTags="20" />
```

A tag page that keeps the tag name in the query string, so tags with spaces or symbols work:

```razor
@page "/tags"
@using Osirion.Blazor.Cms.Web.Components

<TagCloud Title="Tags"
          ActiveTag="@Tag"
          TagUrlFormatter="@(tag => $"/tags?tag={Uri.EscapeDataString(tag.Name)}")" />

@if (!string.IsNullOrWhiteSpace(Tag))
{
    <ContentList Tag="@Tag" />
}

@code {
    [SupplyParameterFromQuery(Name = "tag")]
    public string? Tag { get; set; }
}
```

`ContentList` matches its `Tag` against tag names exactly, ignoring case, which is why this page passes the name
rather than the slug of the default `/tag/{slug}` links.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Tags` | `IReadOnlyList<ContentTag>?` | `null` | Tags to show. When null or empty, the component loads them from the default provider. |
| `MaxTags` | `int?` | `null` | Shows only the first N loaded tags after sorting. |
| `SortByCount` | `bool` | `true` | For loaded tags: `true` sorts by count, highest first; `false` sorts by name. |
| `ShowCount` | `bool` | `true` | Shows each tag's item count after its name. |
| `ActiveTag` | `string?` | `null` | Slug or name of the current tag, compared ignoring case; its link gets the `osirion-tag-active` class. |
| `TagUrlFormatter` | `Func<ContentTag, string>?` | `null` | Builds each link from the tag (`Name`, `Slug`, `Count`). Without it, links go to `/tag/{Slug}`. |
| `Title` | `string?` | `null` | Heading rendered as an `h3` above the tags. No heading when null. |
| `LoadingText` | `string` | `"Loading tags..."` | Shown while the tags load, in interactive render modes only. |
| `NoContentText` | `string` | `"No tags available."` | Shown when there are no tags or loading failed. |
| `Class` | `string?` | `null` | Extra CSS classes on the outer `div`. |

`MaxTags` and `SortByCount` apply only to tags the component loads; tags you pass in `Tags` are shown as given.
Other attributes on the tag are captured but not rendered.

## Behavior

- Loaded tags come from the `tags` front matter of every item the default provider holds, across all locales and
  including unpublished items. Names that differ only in case count as one tag.
- Under static SSR the response is sent with the finished list, and the loading text is never rendered.
- If the provider throws, the error is logged through `ILogger` and the empty-state text is shown. Exception
  details never reach the page.
- Links carry `rel="nofollow"`. Tag names are written as text, so they are HTML-encoded.
- The current tag is marked with a class only, not with `aria-current`.

## Styling

| Class | Element |
| --- | --- |
| `osirion-tag-cloud` | Outer `div` (it also carries `osirion-tags-container`), plus your `Class` |
| `osirion-tag-cloud-title` | The `h3` heading |
| `osirion-tags-container` | The `div` around the links |
| `osirion-tag-link` | Each link; `osirion-tag-active` is added to the current one |
| `osirion-tag-count` | The count inside a link |
| `osirion-loading`, `osirion-no-tags` | Loading and empty states |

Tag colors use `--osirion-tag-background-color`, `--osirion-tag-text-color` and
`--osirion-tag-hover-background-color`, which fall back to the theme's background and text tokens. The active tag
uses `--osirion-background-active` (falling back to `--osirion-action-primary`) and `--osirion-action-primary-text`.

## Related

- [CategoriesList](/docs/components/cms-web/navigation/categories-list), the same idea for categories
- [Content List](/docs/components/cms-web/core/content-list), for the tag page itself
- [CMS navigation components](/docs/components/cms-web/navigation)
