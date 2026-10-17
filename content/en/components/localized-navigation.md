---
id: 'cms-web-localized-navigation'
order: 4
layout: "component"
title: LocalizedNavigation
permalink: /components/localized-navigation
description: "LocalizedNavigation loads the CMS directory tree for one locale and renders it with links to directories and their content items."
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-17
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
  title: 'LocalizedNavigation: a content tree sidebar for Blazor'
  description: 'Load and render the CMS directory tree of one locale, with links to directories and content items and your own URL formatters.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/localized-navigation'
  lang: en
  robots: index, follow
  og_title: 'LocalizedNavigation - Osirion.Blazor'
  og_description: 'Loads the CMS directory tree for one locale and renders it with links to directories and content items.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'LocalizedNavigation - Osirion.Blazor'
  twitter_description: 'Loads the CMS directory tree for one locale and renders it with links to directories and content items.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# LocalizedNavigation

`LocalizedNavigation` loads the directory tree of one locale from the default content provider and renders it as
nested lists: a link per directory and, under an expanded directory, a link per content item. Use it as the
sidebar of a documentation site whose content is organized in locale folders.

Package `Osirion.Blazor.Cms.Web`, namespace `Osirion.Blazor.Cms.Web.Components`. [Source on GitHub](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Cms.Web/Components/Navigation/LocalizedNavigation.razor).

```demo
localized-navigation
```

The demo loads the English directories of this site's own content and expands the `docs` directory, which lists
its pages and the `packages` directory under it. It sets no URL formatter: the default links go to pages of this site.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<LocalizedNavigation CurrentLocale="en" ExpandAllDirectories="true" />
```

A documentation sidebar that takes the locale from the route and opens one directory:

```razor
@page "/{Locale}/docs"
@using Osirion.Blazor.Cms.Web.Components

<LocalizedNavigation Title="Documentation"
                     CurrentLocale="@Locale"
                     ExpandedDirectoryId="@OpenDirectoryId" />

@code {
    [Parameter]
    public string Locale { get; set; } = "en";

    private string? OpenDirectoryId { get; set; }
}
```

The default links go to the public pages: a content link to the item's `Url` and a directory link to the address its
`_index.md` declares in `permalink`, otherwise to its path without the locale folder, as `ContentBreadcrumbs` and
`DirectoryNavigation` build them. Set `DirectoryUrlFormatter` and `ContentUrlFormatter` only to link somewhere else.

## Parameters

<!-- osirion:parameters LocalizedNavigation -->

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Title` | `string?` | `null` | The heading above the tree, rendered as an `h3`. No heading when `null`. |
| `LoadingText` | `string` | `"Loading navigation..."` | The text shown while the tree loads. |
| `NoContentText` | `string` | `"No navigation available."` | The text shown when there are no directories, when no default content provider is registered or when loading failed. |
| `CurrentLocale` | `string?` | `null` | The locale whose directories are shown: the top-level directories of that locale, directly below the locale folder. Their subdirectories are reached through each directory's children. When `null`, or when `EnableLocalization` is `false`, the root directories are shown, which are the locale folders when the content sits in them. |
| `OnLocaleChanged` | `EventCallback<string>` | - | The callback run when the locale changes. Not used in 4.0: the locale buttons never render, because the component does not load the available locales. |
| `OnDirectorySelected` | `EventCallback<DirectoryItem>` | - | The callback run with the directory when its link is clicked in an interactive render mode. When it is set, the click no longer navigates. Under static SSR it is not called. |
| `OnContentSelected` | `EventCallback<ContentItem>` | - | The callback run with the content item when its link is clicked in an interactive render mode. When it is set, the click no longer navigates. Under static SSR it is not called. |
| `ExpandAllDirectories` | `bool` | `false` | Whether every directory is expanded, which shows the whole tree. |
| `ExpandedDirectoryId` | `string?` | `null` | The `Id` of the directory to expand when `ExpandAllDirectories` is `false`. |
| `ShowContentItems` | `bool` | `true` | Whether the content items of an expanded directory are listed, sorted by title, before its subdirectories. |
| `DirectoryUrlFormatter` | `Func<DirectoryItem, string>?` | `null` | A function that builds each directory link. Without it, a link goes to the address the directory's `_index.md` declares in `permalink`, otherwise to the directory's path without the locale folder. |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | A function that builds each content link. Without it, a link goes to the item's URL from the site root, the address its page is served at. |
| `LocaleNameFormatter` | `Func<string, string>?` | `null` | A function that turns a locale code into the text of its button. Not used in 4.0: the locale buttons never render. |
| `EnableLocalization` | `bool` | `true` | Whether `CurrentLocale` is used. When `false`, the root directories are loaded. |
| `AriaLabel` | `string` | `"Localized navigation"` | The accessible name of the `nav` element. |
| `Theme` | `ThemeMode` | `ThemeMode.System` | The component's color mode. `Light` or `Dark` writes it to `data-theme` (`data-bs-theme` under Bootstrap) on the root element; `System` writes nothing. |
| `Class` | `string?` | `null` | Extra CSS classes for the component's root element. |
| `Style` | `string?` | `null` | An inline style for the component's root element. |

<!-- /osirion:parameters -->

`OnLocaleChanged` and `LocaleNameFormatter` are for a locale switcher. The component does not load the list of
available locales, because the content provider has no API that returns them, so the switcher never renders and these
two parameters have no effect. Use your own language
links and pass the chosen locale in `CurrentLocale`. `Class` goes on the `nav`. `Style` and `Theme` are accepted but
not written, and other attributes on the tag are captured but not rendered.

## Behavior

- Directories are sorted by their `order` (from `_index.md`), content items by title. A directory's name is its
  folder name, or the `title` from its `_index.md`.
- An expanded directory shows its content items first (unless `ShowContentItems` is off), then its subdirectories. A
  subdirectory shows its own children only when it is expanded too, so `ExpandedDirectoryId` opens one level.
- With `CurrentLocale` set, the component shows the top-level directories of that locale, the ones directly below
  the locale folder. The provider returns the locale's directories at every depth as one list; the component keeps
  the ones whose parent is not in that list and reaches the rest through each directory's `Children`, so a nested
  directory appears once, under its parent. Without a locale, the list holds the root directories, which are the
  locale folders when your content sits in folders such as `en/`.
- A directory's `_index.md` is not listed as a page under its content items: its address is the directory's own
  link.
- The tree is loaded again on every parameter change. Under static SSR the response is sent with the finished tree.
- With no default provider registered, or when the provider throws, `NoContentText` is shown. A provider error is
  logged through `ILogger`.
- Under static SSR the links are plain links and the callbacks are not raised. In an interactive render mode a link
  stops the browser from navigating only when its callback is set (`OnDirectorySelected` for directories,
  `OnContentSelected` for content items); without a callback the link navigates, and no handler attribute is written.
- The `nav` is named by `AriaLabel`, `"Localized navigation"` by default. Names and titles are written as text, so
  they are HTML-encoded.

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
The directory and content entries are built in code, so its rules for `osirion-nav-item`, `osirion-nav-link`,
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
