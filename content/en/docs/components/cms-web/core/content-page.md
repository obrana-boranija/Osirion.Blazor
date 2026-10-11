---
title: "ContentPage - Osirion Blazor"
description: "Load a content item by URL and lay it out with breadcrumbs, translation links, previous and next links, a sidebar, share links and related items."
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Core Components
tags:
- content-page
- cms
- article
- related-content
slug: "content-page"
layout: "component"
published: true
---

# ContentPage

`ContentPage` loads one content item by its URL and lays out a whole article: breadcrumbs, the item through
[LocalizedContentView](/docs/components/cms-web/core/localized-content-view), previous and next links, a sidebar of
categories and tags, optional share links and optional related items. Use it for a quick article page without
assembling those parts yourself.

## Usage

```razor
@page "/blog/{Slug}"
@using Osirion.Blazor.Cms.Web.Components

<ContentPage Path="@($"blog/{Slug}")" />

@code {
    [Parameter] public string? Slug { get; set; }
}
```

`Path` is the item's `Url` exactly as the provider stores it: no leading slash, no file extension, and for localized
content no locale folder (`blog/my-post`). The match is exact, including case.

`ContentPage` writes nothing to the page head. To add SEO metadata, keep the loaded item and render a
[SeoMetadataRenderer](/docs/components/cms-web/core/seo-metadata-renderer) for it:

```razor
@page "/blog/{Slug}"
@using Osirion.Blazor.Cms.Domain.Entities
@using Osirion.Blazor.Cms.Web.Components

@if (current is not null)
{
    <HeadContent>
        <SeoMetadataRenderer Content="@current" SchemaTypes="@(new[] { SchemaType.BlogPosting })" />
    </HeadContent>
}

<ContentPage Path="@($"blog/{Slug}")"
             ShowRelatedContent
             ShowShareLinks
             TagUrlFormatter="@(tag => $"/tag/{Uri.EscapeDataString(tag)}")"
             OnContentLoaded="@((ContentItem item) => current = item)" />

@code {
    [Parameter] public string? Slug { get; set; }

    private ContentItem? current;
}
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Path` | `string?` | `null` | URL of the item to load from the default provider. Required: without it the component stays on its loading text. |
| `ContentItem` | `ContentItem?` | `null` | The item shown. The component sets it when `Path` loads, replacing a value you pass; it is not two-way bound, so use `OnContentLoaded` to get the item. |
| `IgnorePathSegment` | `string?` | `null` | Prefixed to `Path` (as `segment/path`) when `Path` does not contain it, and removed from related item links. |
| `PathReplacePattern` | `string?` | `null` | A regular expression and its replacement, separated by a vertical bar, applied to `Path` before the lookup and to related item links. |
| `OnContentLoaded` | `EventCallback<ContentItem>` | none | Raised after the item loads. |
| `ShowBreadcrumbs` | `bool` | `true` | Renders `ContentBreadcrumbs` for the item. |
| `ShowBreadcrumbHome` | `bool` | `true` | Starts the breadcrumbs with a home link. |
| `BreadcrumbHomeUrl` | `string` | `"/"` | Home link URL. |
| `BreadcrumbHomeText` | `string` | `"Home"` | Home link text. |
| `ShowNavigationLinks` | `bool` | `true` | Shows previous and next links; missing neighbors are loaded with a query for the item's directory and locale, newest first. |
| `PreviousItem` | `ContentItem?` | `null` | Item for the previous link. |
| `NextItem` | `ContentItem?` | `null` | Item for the next link. |
| `ShowCategories` | `bool` | `true` | Shows the categories sidebar when the item has categories. |
| `CategoriesTitle` | `string` | `"Categories:"` | Sidebar heading. |
| `ShowTags` | `bool` | `true` | Shows the tag cloud when the item has tags. |
| `TagsTitle` | `string` | `"Tags:"` | Tag cloud heading. |
| `CategoryUrlFormatter` | `Func<string, string>?` | `null` | Builds category links in the article and the sidebar. |
| `TagUrlFormatter` | `Func<string, string>?` | `null` | Builds tag links in the article and the tag cloud. |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Builds previous, next and related item links. |
| `ShowShareLinks` | `bool` | `false` | Shows share links: Twitter, Facebook and LinkedIn unless you pass `ShareLinks`. |
| `ShareLinksTitle` | `string` | `"Share:"` | Share section heading. |
| `ShareLinks` | `List<ShareLink>` | empty | Custom share links (`Name`, `Label`, `Url`, `Icon`). |
| `ShowRelatedContent` | `bool` | `false` | Shows related items; when `RelatedItems` is empty, up to three are found by shared tags and categories. |
| `RelatedItems` | `IReadOnlyList<ContentItem>?` | `null` | Related items to show. |
| `RelatedContentTitle` | `string` | `"Related Content"` | Related section heading. |
| `FooterContent` | `string?` | `null` | HTML written after the article. Trusted markup only (see Behavior). |
| `SchemaType` | `string` | `"Article"` | schema.org type in the article's `itemtype` attribute. |
| `LoadingText` | `string` | `"Loading content..."` | Text of the loading state. |
| `NotFoundTitle` | `string` | `"Content Not Found"` | Heading when no item matches. |
| `NotFoundText` | `string` | `"The requested content could not be found."` | Message when no item matches. |
| `ShowBackLink` | `bool` | `true` | Shows a back link in the not-found state. |
| `BackLinkUrl` | `string` | `"/"` | Back link URL. |
| `BackLinkText` | `string` | `"Back to Home"` | Back link text. |
| `Class` | `string?` | `null` | Extra CSS classes on the root element. |

`ShowMetadata`, `FeaturedImageCaption`, `ShowFooter`, `PreviousItemLabel`, `NextItemLabel` and
`DirectoryUrlFormatter` are declared but have no effect in the current markup. `Style`, `Theme` and unmatched
attributes from the base class are not written to the markup either.

## Behavior

- **Rendering.** Static SSR renders the finished article on the server; nothing needs script. A new `Path` clears the
  neighbors, related items and default share links the component loaded for the previous one.
- **Body.** The item is rendered by `LocalizedContentView`, so the body is sanitized, a header shows the title,
  author, date, reading time and featured image, and translation links appear when the item has translations.
- **Sidebar.** `CategoriesList` and `TagCloud` list every category and tag of the site, not only the item's, without
  counts.
- **Share links.** They open in a new tab with `rel="noopener noreferrer"`. The default links carry the page's
  absolute URL without query string or fragment.
- **Related items.** The default link is the item's source path with `.md` removed; set `ContentUrlFormatter` when
  your routes differ.
- **Security.** `FooterContent` and each `ShareLink.Icon` are written as raw HTML without sanitizing. Never put
  user-supplied or content-supplied text in them.
- **Errors.** A provider failure is logged and the not-found state is shown.

## Styling

The root element has the class `osirion-content-page-container`; the article is `osirion-content-page` with
`osirion-content-main`, `osirion-content-primary` and `osirion-content-sidebar`. The other parts use
`osirion-content-share` (`osirion-content-share-link`, plus `osirion-content-share-{name}` per link),
`osirion-content-custom-footer`, `osirion-content-related` with `osirion-content-related-grid`, and
`osirion-content-not-found`. Colors and spacing come from theme tokens such as `--osirion-text-primary`,
`--osirion-text-secondary` and `--osirion-border-color`.

## Related

- [LocalizedContentView](/docs/components/cms-web/core/localized-content-view): the article inside the page
- [ContentView](/docs/components/cms-web/core/content-view): a single item without the page layout
- [ContentBreadcrumbs](/docs/components/cms-web/navigation/content-breadcrumbs): the breadcrumb trail
- [SeoMetadataRenderer](/docs/components/cms-web/core/seo-metadata-renderer): head metadata for the item
