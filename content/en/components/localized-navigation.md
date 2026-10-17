---
id: 'cms-web-localized-navigation'
order: 4
layout: "component"
title: LocalizedNavigation
permalink: /components/localized-navigation
description: "LocalizedNavigation loads the CMS directory tree for one locale and renders it with links to directories and their content items."
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
- localization
- directories
- sidebar
published: true
slug: localized-navigation
lang: en
redirect_from:
- /docs/components/cms-web/navigation/localized-navigation
custom_fields:
  group: cms-navigation
seo_properties:
  title: "LocalizedNavigation Component - Osirion.Blazor CMS"
  description: "Load and render the CMS directory tree of one locale, with links to directories and content items and your own URL formatters."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/components/localized-navigation"
---

# LocalizedNavigation

`LocalizedNavigation` loads the directory tree of one locale from the default content provider and renders it as
nested lists: a link per directory and, under an expanded directory, a link per content item. Use it as the
sidebar of a documentation site whose content is organized in locale folders.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<LocalizedNavigation CurrentLocale="en" ExpandAllDirectories="true" />
```

A documentation sidebar that takes the locale from the route and links to the public URLs:

```razor
@page "/{Locale}/docs"
@using Osirion.Blazor.Cms.Web.Components

<LocalizedNavigation Title="Documentation"
                     CurrentLocale="@Locale"
                     ExpandAllDirectories="true"
                     DirectoryUrlFormatter="@(directory => $"/{directory.Url.TrimStart('/')}")"
                     ContentUrlFormatter="@(item => $"/{item.Url.TrimStart('/')}")" />

@code {
    [Parameter]
    public string Locale { get; set; } = "en";
}
```

Pass both formatters. Without them, directory links go to `/{directory.Path}` and content links to
`/{item.Path}`, the folder and file paths in the content source (a content path ends in `.md`), which are not
pages on your site. A directory's `Url` is the `permalink` from its `_index.md`; an item's `Url` is the address the
CMS gives it.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `CurrentLocale` | `string?` | `null` | Locale whose top-level directories are loaded. When null, the top-level directories of every locale are loaded. |
| `Title` | `string?` | `null` | Heading rendered as an `h3` above the tree. No heading when null. |
| `ExpandAllDirectories` | `bool` | `false` | Expands every directory, showing the whole tree. |
| `ExpandedDirectoryId` | `string?` | `null` | `Id` of the one directory to expand when `ExpandAllDirectories` is false. |
| `ShowContentItems` | `bool` | `true` | Lists the content items of expanded directories, sorted by title. |
| `DirectoryUrlFormatter` | `Func<DirectoryItem, string>?` | `null` | Builds directory links. Without it, links go to `/{directory.Path}`. |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Builds content links. Without it, links go to `/{item.Path}`. |
| `OnDirectorySelected` | `EventCallback<DirectoryItem>` | none | Raised when a directory link is clicked in an interactive render mode. |
| `OnContentSelected` | `EventCallback<ContentItem>` | none | Raised when a content link is clicked in an interactive render mode. |
| `EnableLocalization` | `bool` | `true` | When false, `CurrentLocale` is ignored and the top-level directories of every locale are loaded. |
| `LoadingText` | `string` | `"Loading navigation..."` | Shown while the tree loads. |
| `NoContentText` | `string` | `"No navigation available."` | Shown when there are no directories or loading failed. |
| `Class` | `string?` | `null` | Extra CSS classes on the `nav`. |

The component also declares `OnLocaleChanged` and `LocaleNameFormatter` for a locale switcher. In 4.0 it does not
load the list of available locales, so the switcher never renders and these two parameters have no effect. Use your
own language links and pass the chosen locale in `CurrentLocale`. Other attributes on the tag are captured but not
rendered.

## Behavior

- Directories are sorted by their `order` (from `_index.md`), content items by title. A directory's name is its
  folder name, or the `title` from its `_index.md`.
- An expanded directory shows its content items first, then its subdirectories. A subdirectory shows its own
  children only when it is expanded too, so `ExpandedDirectoryId` opens one level.
- If your content sits in locale folders such as `en/`, the locale folder is the only top-level directory of that
  locale, so the tree starts with a single link to it. Set `ExpandAllDirectories`, or pass the folder's `Id` in
  `ExpandedDirectoryId`, to show what is inside.
- The tree is loaded again on every parameter change. Under static SSR the response is sent with the finished tree.
- If the provider throws, the error is logged through `ILogger` and the empty-state text is shown.
- Under static SSR the links are plain links and the callbacks are not raised.
- The tree is a `nav` without an accessible name. Names and titles are written as text, so they are HTML-encoded.

## Styling

| Class | Element |
| --- | --- |
| `osirion-localized-navigation` | The `nav`, plus your `Class` |
| `osirion-nav-title` | The `h3` heading |
| `osirion-nav-list` | The top-level `ul` |
| `osirion-nav-item`, `osirion-nav-link` | A directory's `li` and link |
| `osirion-nav-sublist` | The `ul` under an expanded directory |
| `osirion-nav-content-item`, `osirion-nav-content-link` | A content item's `li` and link |
| `osirion-loading`, `osirion-no-directories` | Loading and empty states |

The component's scoped stylesheet styles the `nav`, the heading, the outer list and the loading and empty states.
In 4.0 the directory and content entries are built in code, so its rules for `osirion-nav-item`, `osirion-nav-link`,
`osirion-nav-sublist` and the content classes do not reach them. Style those classes from your own stylesheet, for
example:

```css
.osirion-localized-navigation .osirion-nav-sublist {
    list-style: none;
    padding-left: 1rem;
}

.osirion-localized-navigation .osirion-nav-link,
.osirion-localized-navigation .osirion-nav-content-link {
    color: var(--osirion-text-secondary, #6b7280);
    text-decoration: none;
}
```

## Related

- [DirectoryNavigation](/components/directory-navigation), for a list of directories you load yourself
- [Localized Content View](/components/localized-content-view)
- [CMS navigation components](/components/cms-navigation)
