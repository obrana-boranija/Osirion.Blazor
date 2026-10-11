---
id: 'cms-web-directory-navigation'
order: 3
layout: "component"
title: "DirectoryNavigation - Osirion Blazor CMS"
permalink: /docs/components/cms-web/navigation/directory-navigation
description: "DirectoryNavigation renders a list of CMS directories you pass, with one level of subdirectories, an active directory and click callbacks."
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
- directories
- sidebar
published: true
slug: "directory-navigation"
lang: en
seo_properties:
  title: "DirectoryNavigation Component - Osirion.Blazor CMS"
  description: "Render a sidebar list of CMS directories with one level of subdirectories, an active directory and your own directory URLs."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/docs/components/cms-web/navigation/directory-navigation"
---

# DirectoryNavigation

`DirectoryNavigation` renders a list of links to the CMS directories you give it, and can show one level of
subdirectories under each. It does not load anything itself: you pass the directories, which keeps it usable for
any part of the content tree.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<DirectoryNavigation Directories="@directories" />
```

A sidebar that loads the top-level directories of one locale, marks the current one and expands it:

```razor
@using Osirion.Blazor.Cms.Domain.Entities
@using Osirion.Blazor.Cms.Domain.Services
@using Osirion.Blazor.Cms.Web.Components
@inject IContentProviderManager ContentProviderManager

<DirectoryNavigation Title="Sections"
                     Directories="@directories"
                     IsLoading="@(directories is null)"
                     CurrentDirectory="@CurrentPath"
                     ExpandedDirectory="@CurrentPath"
                     ShowItemCount="false"
                     DirectoryUrlFormatter="@(directory => $"/{directory.Url.TrimStart('/')}")" />

@code {
    [Parameter]
    public string? CurrentPath { get; set; }

    private IReadOnlyList<DirectoryItem>? directories;

    protected override async Task OnInitializedAsync()
    {
        directories = await ContentProviderManager.GetDirectoryTreeFromDefaultAsync("en");
    }
}
```

`GetDirectoryTreeFromDefaultAsync` returns the directories without a parent; their subdirectories are in
`Children`. If your content sits in locale folders such as `en/`, the locale folder is the only directory without a
parent, so pass its `Children` to list the sections inside it. `CurrentDirectory` and `ExpandedDirectory` are compared with `DirectoryItem.Path`, the folder path in
the content source.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Directories` | `IReadOnlyList<DirectoryItem>?` | `null` | The directories to list. |
| `Title` | `string?` | `null` | Heading rendered as an `h3` above the list. No heading when null. |
| `CurrentDirectory` | `string?` | `null` | Path of the current directory; its link gets the `osirion-active` class. |
| `ExpandedDirectory` | `string?` | `null` | Path of the top-level directory whose subdirectories are shown. |
| `ExpandAllSubdirectories` | `bool` | `false` | Shows the subdirectories of every top-level directory. |
| `ShowSubdirectories` | `bool` | `true` | Allows subdirectories to be shown at all. |
| `ShowItemCount` | `bool` | `true` | Shows a count after each name. The count is the number of subdirectories (`Children.Count`), not of content items. |
| `IsLoading` | `bool` | `false` | Shows `LoadingText` instead of the list while you load the directories. |
| `LoadingText` | `string` | `"Loading navigation..."` | Text shown while `IsLoading` is true. |
| `NoContentText` | `string` | `"No directories available."` | Shown when `Directories` is null or empty. |
| `DirectoryUrlFormatter` | `Func<DirectoryItem, string>?` | `null` | Builds each link. Without it, links go to `/{directory.Url}`. |
| `DirectoryClicked` | `EventCallback<DirectoryItem>` | none | Called when a link is clicked in an interactive render mode. When set, the click no longer navigates. |

This component does not apply `Class` or other attributes in 4.0. Wrap it in your own element to style it.

## Behavior

- Only one level of subdirectories is rendered, even when they have children of their own.
- A directory's `Url` is the `permalink` from its `_index.md` and is empty when there is none, so the default
  link is `/` for directories without a permalink. A permalink that starts with a slash would produce `//...`;
  the formatter in the example trims it. A directory's name is its folder name, or the `title` from its `_index.md`.
- Under static SSR the links are plain links and `DirectoryClicked` is not called. In an interactive render mode
  with `DirectoryClicked` set, the click runs your callback instead of navigating, so you decide what happens.
- The list is wrapped in a `nav` without an accessible name. Directory names are written as text, so they are
  HTML-encoded.

## Styling

| Class | Element |
| --- | --- |
| `osirion-directory-navigation` | The `nav` |
| `osirion-directory-title` | The `h3` heading |
| `osirion-directory-list` | The top-level `ul` |
| `osirion-subdirectory-list` | The `ul` of subdirectories |
| `osirion-directory-item` | Each `li` |
| `osirion-directory-link` | Each link; `osirion-active` is added to the current one |
| `osirion-directory-count` | The count inside a link |
| `osirion-loading`, `osirion-no-directories` | Loading and empty states |

Colors come from `--osirion-text-primary`, `--osirion-text-secondary`, `--osirion-action-primary` and
`--osirion-background-secondary`.

## Related

- [LocalizedNavigation](/docs/components/cms-web/navigation/localized-navigation), which loads the tree itself and lists content items
- [ContentBreadcrumbs](/docs/components/cms-web/navigation/content-breadcrumbs)
- [CMS navigation components](/docs/components/cms-web/navigation)
