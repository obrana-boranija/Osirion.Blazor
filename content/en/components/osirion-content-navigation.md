---
id: 'cms-web-osirion-content-navigation'
order: 5
title: OsirionContentNavigation
permalink: /components/osirion-content-navigation
description: "OsirionContentNavigation renders previous and next links between CMS items of the same directory, found by front matter order."
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
- previous-next
- pagination
published: true
slug: osirion-content-navigation
lang: en
redirect_from:
- /docs/components/cms-web/navigation/osirion-content-navigation
custom_fields:
  group: cms-navigation
seo_properties:
  title: "OsirionContentNavigation Component - Osirion.Blazor CMS"
  description: "Render previous and next links between CMS items of a directory, with descriptions, icons, three layouts and your own URLs."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/components/osirion-content-navigation"
---

# OsirionContentNavigation

`OsirionContentNavigation` renders a pair of links to the previous and next item, for example at the end of an
article or a documentation page. Give it the current item and it finds the neighbours in the same directory, or
pass the neighbours yourself.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<OsirionContentNavigation Item="@Item" />
```

The built-in `ArticlePage` uses `<OsirionContentNavigation Item="Item" ShowDescription ShowPlaceholder />`. A
documentation page with card links, root-relative URLs and its own labels:

```razor
@using Osirion.Blazor.Cms.Domain.Entities
@using Osirion.Blazor.Cms.Web.Components

<OsirionContentNavigation Item="@Item"
                          Variant="ContentNavigationVariant.Card"
                          ShowDescription="true"
                          PreviousLabel="Previous page"
                          NextLabel="Next page"
                          AriaLabel="Documentation pages"
                          UrlGenerator="@(item => $"/{item.Url.TrimStart('/')}")" />

@code {
    [Parameter]
    public ContentItem? Item { get; set; }
}
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Item` | `ContentItem?` | `null` | The current item. Required: nothing renders without it, even when you pass both neighbours. |
| `PreviousItem` | `ContentItem?` | `null` | The previous item. When null, it is looked up from `Item`. |
| `NextItem` | `ContentItem?` | `null` | The next item. When null, it is looked up from `Item`. |
| `PreviousLabel` | `string` | `"Previous"` | Label above the previous item's title. |
| `NextLabel` | `string` | `"Next"` | Label above the next item's title. |
| `AriaLabel` | `string` | `"Content navigation"` | Accessible name of the `nav`. |
| `ShowDescription` | `bool` | `false` | Shows each neighbour's description under its title. |
| `ShowPlaceholder` | `bool` | `false` | Renders an empty placeholder where a neighbour is missing, so the other link keeps its side. |
| `ShowIcons` | `bool` | `true` | Shows the arrow icons. |
| `PreviousIcon` | `RenderFragment?` | a left chevron SVG | Icon of the previous link. |
| `NextIcon` | `RenderFragment?` | a right chevron SVG | Icon of the next link. |
| `Variant` | `ContentNavigationVariant` | `Default` | `Default`, `Compact` or `Card` layout. |
| `UrlGenerator` | `Func<ContentItem, string>?` | `null` | Builds each link. Without it, the link is the item's `Url`. |
| `TitleExtractor` | `Func<ContentItem, string>?` | `null` | Text shown as the title. Without it, the item's `Title`, or `Untitled`. |
| `DescriptionExtractor` | `Func<ContentItem, string?>?` | `null` | Text shown as the description. Without it, the item's `Description`. |
| `ShowNavigation` | `bool` | `true` | Set to false to render nothing and skip the lookup. |
| `PreventDefault` | `bool` | `false` | Stops the links from navigating in interactive render modes. Leave it false: the click callbacks below are not raised, so a prevented link does nothing. |
| `OnPreviousClick`, `OnNextClick` | `EventCallback<ContentItem>` | none | Declared, but not raised by the component in 4.0. |
| `Class` | `string?` | `null` | Extra CSS classes on the `nav`. |

Other attributes on the tag are added to the `nav`.

## Behavior

- When a neighbour is missing, the component queries the published items with the same locale whose directory has
  the same name as `Item.Directory`, sorted by the `order` front matter ascending. The items right before and after
  `Item` (matched by path) become the neighbours. An item without a directory is compared with all items of its
  locale. Give the items of a directory distinct `order` values so the sequence is predictable.
- The `nav` renders only when `Item` is set and at least one neighbour exists. With one neighbour it gets the
  `osirion-content-navigation-single` class.
- If the lookup fails, the error is logged through `ILogger` and only the neighbours you passed are shown.
- An item's `Url` is a path without a leading slash, so the default link is relative to the page's base address.
  The `UrlGenerator` in the example makes it root-relative.
- Titles and descriptions are written as text, so they are HTML-encoded. The links are plain anchors and need no
  script.

## Styling

| Class | Element |
| --- | --- |
| `osirion-content-navigation` | The `nav`, plus your `Class` |
| `osirion-content-navigation-default`, `-compact`, `-card` | The variant, on the `nav` |
| `osirion-content-navigation-single` | Added when only one neighbour is shown |
| `osirion-content-previous`, `osirion-content-next` | The two links |
| `osirion-content-navigation-label` | The label and icon |
| `osirion-content-navigation-icon` | The icon wrapper |
| `osirion-content-navigation-title` | The neighbour's title |
| `osirion-content-navigation-description` | The neighbour's description |
| `osirion-content-navigation-placeholder` | The empty placeholder |

The links use `--osirion-border-color`, `--osirion-background-primary`, `--osirion-text-primary` and
`--osirion-action-primary`, with `--osirion-focus-ring-color` for keyboard focus. The card variant uses
`--osirion-card-background`, `--osirion-card-border-color` and `--osirion-card-shadow`.

## Related

- [ContentSeriesNavigation](/components/series-navigation), previous and next links inside a series
- [ContentBreadcrumbs](/components/content-breadcrumbs)
- [CMS navigation components](/components/cms-navigation)
