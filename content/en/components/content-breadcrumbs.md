---
id: 'cms-web-content-breadcrumbs'
order: 2
layout: "component"
title: ContentBreadcrumbs
permalink: /components/content-breadcrumbs
description: "ContentBreadcrumbs renders a breadcrumb trail from a CMS item's directory chain: home, each parent directory and the item title."
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
- breadcrumbs
- directories
- accessibility
published: true
slug: content-breadcrumbs
lang: en
redirect_from:
- /docs/components/cms-web/navigation/content-breadcrumbs
custom_fields:
  group: cms-navigation
seo_properties:
  title: "ContentBreadcrumbs Component - Osirion.Blazor CMS"
  description: "Render a breadcrumb trail from a CMS item's directories, with a home link, a current item and your own directory URLs."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/components/content-breadcrumbs"
---

# ContentBreadcrumbs

`ContentBreadcrumbs` renders a breadcrumb trail for a CMS item: a home link, a link for each directory from the
root down to the item's directory, and the item's title. Use it on article and documentation pages, where the
trail should follow the content tree rather than the URL.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<ContentBreadcrumbs Content="@Item" />
```

The component walks up from `Item.Directory` through each directory's `Parent`. A documentation page that sets
its own home link and directory URLs:

```razor
@using Osirion.Blazor.Cms.Domain.Entities
@using Osirion.Blazor.Cms.Web.Components

<ContentBreadcrumbs Content="@Item"
                    Directory="@Item?.Directory"
                    HomeText="Docs"
                    HomeUrl="/docs"
                    DirectoryUrlFormatter="@(directory => $"/{directory.Url.TrimStart('/')}")" />

@code {
    [Parameter]
    public ContentItem? Item { get; set; }
}
```

A directory's `Url` is the `permalink` from its `_index.md`, and is empty when there is none. Without a formatter,
links go to `/{directory.Path}`, the folder path in the content source, which can include the locale folder and is
not always a page on your site.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Content` | `ContentItem?` | `null` | The item being shown. Its title is the last crumb, and its `Directory` is used when `Directory` is not set. |
| `Directory` | `DirectoryItem?` | `null` | The deepest directory of the trail. Parents are added up to the root. |
| `ShowHome` | `bool` | `true` | Renders the home link first. |
| `HomeText` | `string` | `"Home"` | Text of the home link. |
| `HomeUrl` | `string` | `"/"` | Target of the home link. |
| `HideCurrentItem` | `bool` | `false` | Leaves out the item's title, for example when the page heading already shows it. |
| `DirectoryUrlFormatter` | `Func<DirectoryItem, string>?` | `null` | Builds each directory link. Without it, links go to `/{directory.Path}`. |
| `Class` | `string?` | `null` | Extra CSS classes on the `nav`. |

Other attributes on the tag are captured but not rendered.

## Behavior

- The component takes `Content.Directory` only once, when it is first initialized. If the item can change while
  the component stays on the page (an interactive page that loads another item), pass `Directory` as well, as the
  built-in `DocumentPage` does.
- A directory's name is its folder name, or the `title` from its `_index.md`.
- It renders `<nav aria-label="breadcrumb">` around an ordered list. The current item is plain text in a `span`,
  not a link. It has no `aria-current` attribute.
- The `/` separators are drawn with CSS (`::after`), not written into the markup.
- Titles and directory names are written as text, so they are HTML-encoded. No script is needed.

## Styling

| Class | Element |
| --- | --- |
| `osirion-breadcrumbs` | The `nav`, plus your `Class` |
| `osirion-breadcrumbs-list` | The `ol` |
| `osirion-breadcrumbs-item` | Each `li`; the current item's `li` also has `osirion-breadcrumbs-current` |
| `osirion-breadcrumbs-link` | The home and directory links |
| `osirion-breadcrumbs-text` | The current item's title |

Links use `--osirion-text-secondary` and turn `--osirion-action-primary` on hover; separators use
`--osirion-text-tertiary`.

In 4.0 the directory crumbs are built in code, so the component's scoped stylesheet does not reach them: they get
no separator and no link style, while the home and current crumbs do. Until that changes, add the rules to your own
stylesheet:

```css
.osirion-breadcrumbs .osirion-breadcrumbs-item {
    display: flex;
    align-items: center;
}

.osirion-breadcrumbs .osirion-breadcrumbs-item:not(:last-child)::after {
    content: "/";
    margin: 0 0.5rem;
    color: var(--osirion-text-tertiary, #9ca3af);
}

.osirion-breadcrumbs .osirion-breadcrumbs-link {
    color: var(--osirion-text-secondary, #6b7280);
    text-decoration: none;
}
```

## Related

- [OsirionBreadcrumbs](/components/breadcrumbs), breadcrumbs built from a URL path
- [DirectoryNavigation](/components/directory-navigation)
- [CMS navigation components](/components/cms-navigation)
