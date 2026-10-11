---
id: 'cms-web-categories-list'
order: 1
layout: "component"
title: "CategoriesList - Osirion Blazor CMS"
permalink: /docs/components/cms-web/navigation/categories-list
description: "CategoriesList renders a link to every content category with its item count, loaded from the default CMS content provider."
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Navigation Components
tags:
- cms
- navigation
- categories
- taxonomy
published: true
slug: "categories-list"
lang: en
seo_properties:
  title: "CategoriesList Component - Osirion.Blazor CMS"
  description: "Render links to every CMS category with item counts, sorting, a limit, an active category and your own URL scheme."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/docs/components/cms-web/navigation/categories-list"
---

# CategoriesList

`CategoriesList` renders a list of links, one per content category, with the number of items in each. It loads the
categories itself from the default content provider, so use it in a sidebar, on a home page or on a category page.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<CategoriesList Title="Categories" />
```

A category page that lists the items of one category and marks it in the list:

```razor
@page "/category/{Category}"
@using Osirion.Blazor.Cms.Web.Components

<ContentList Category="@Category" />

<aside>
    <CategoriesList Title="Categories"
                    ActiveCategory="@Category"
                    SortByCount="false" />
</aside>

@code {
    [Parameter]
    public string Category { get; set; } = string.Empty;
}
```

The default links go to `/category/{slug}`, which matches this route. `ContentList` turns the hyphens of the slug
back into spaces and keeps items whose category contains that text, ignoring case, so this works for category names
made of letters, digits and spaces. For other names, pass `CategoryUrlFormatter` and read the value your own way.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Title` | `string?` | `null` | Heading rendered as an `h3` above the list. No heading when null. |
| `ShowCount` | `bool` | `true` | Shows each category's item count after its name. |
| `SortByCount` | `bool` | `true` | `true` sorts by count, highest first; `false` sorts by name. |
| `MaxCategories` | `int?` | `null` | Shows only the first N categories after sorting. |
| `ActiveCategory` | `string?` | `null` | Slug or name of the current category, compared ignoring case; its link gets the `osirion-active` class. |
| `CategoryUrlFormatter` | `Func<ContentCategory, string>?` | `null` | Builds each link from the category (`Name`, `Slug`, `Count`). Without it, links go to `/category/{Slug}`. |
| `LoadingText` | `string` | `"Loading categories..."` | Shown while the categories load. |
| `NoContentText` | `string` | `"No categories available."` | Shown when there are no categories or loading failed. |
| `Class` | `string?` | `null` | Extra CSS classes on the outer `div`. |

Other attributes on the tag are captured but not rendered, so use `Class` rather than `id` or `data-` attributes.

## Behavior

- Categories come from the `categories` front matter of every item the default provider holds, across all
  locales and including unpublished items. Names that differ only in case count as one category; the count is the number of items that carry it.
- The list is loaded again on every parameter change. Caching belongs to the content provider, not the component.
- Under static SSR the response is sent with the finished list. `LoadingText` can show briefly in interactive
  render modes.
- If the provider throws, the error is logged through `ILogger` and the empty-state text is shown. Exception
  details never reach the page.
- Links carry `rel="nofollow"`. Category names are written as text, so they are HTML-encoded.
- The current category is marked with a class only, not with `aria-current`.

## Styling

| Class | Element |
| --- | --- |
| `osirion-categories-list-container` | Outer `div`, plus your `Class` |
| `osirion-categories-title` | The `h3` heading |
| `osirion-categories-list` | The `ul` |
| `osirion-category-item` | Each `li` |
| `osirion-category-link` | Each link; `osirion-active` is added to the current one |
| `osirion-category-count` | The count inside a link |
| `osirion-loading`, `osirion-no-categories` | Loading and empty states |

Colors come from `--osirion-text-primary`, `--osirion-text-secondary`, `--osirion-action-primary`,
`--osirion-background-tertiary`, `--osirion-background-hover` and `--osirion-background-active`.

## Related

- [TagCloud](/docs/components/cms-web/navigation/tag-cloud), the same idea for tags
- [Content List](/docs/components/cms-web/core/content-list), for the category page itself
- [CMS navigation components](/docs/components/cms-web/navigation)
