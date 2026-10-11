---
id: 'cms-web-navigation-components-overview'
order: 1
layout: docs
title: CMS Navigation Components
permalink: /docs/components/cms-web/navigation
description: 'Components that link CMS content: categories, tags, breadcrumbs, directories, previous and next links, series, table of contents and search.'
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Navigation Components
tags:
- blazor
- cms
- navigation
- categories
- tags
- search
- table-of-contents
is_featured: true
published: true
slug: navigation
lang: en
custom_fields: {}
seo_properties:
  title: 'CMS Navigation Components - Osirion.Blazor'
  description: 'Blazor components that link CMS content: categories, tags, breadcrumbs, directories, previous and next links, series, table of contents and search.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/cms-web/navigation'
  lang: en
  robots: index, follow
  og_title: 'CMS Navigation Components - Osirion.Blazor'
  og_description: 'Blazor components that link CMS content: categories, tags, breadcrumbs, directories, series, table of contents and search.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'CMS Navigation Components - Osirion.Blazor'
  twitter_description: 'Blazor components that link CMS content: categories, tags, breadcrumbs, directories, series, table of contents and search.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# CMS navigation components

These components help readers move through content served by the Osirion CMS: category and tag links,
breadcrumbs, directory lists, previous and next links, series links, a table of contents and a search form. They
ship in the Osirion.Blazor.Cms.Web package, in the `Osirion.Blazor.Cms.Web.Components` namespace.

```razor
@using Osirion.Blazor.Cms.Web.Components
```

## Components

| Component | What it renders | Where its data comes from |
| --- | --- | --- |
| [CategoriesList](/docs/components/cms-web/navigation/categories-list) | Links to every category, with item counts | Loaded from the default content provider |
| [TagCloud](/docs/components/cms-web/navigation/tag-cloud) | Links to every tag, with item counts | Loaded from the default content provider, or the tags you pass |
| [ContentBreadcrumbs](/docs/components/cms-web/navigation/content-breadcrumbs) | Home, the item's directories and the item title | The content item or directory you pass |
| [DirectoryNavigation](/docs/components/cms-web/navigation/directory-navigation) | A list of directories with one level of subdirectories | The directories you pass |
| [LocalizedNavigation](/docs/components/cms-web/navigation/localized-navigation) | A directory tree with links to content, for one locale | Loaded from the default content provider |
| [OsirionContentNavigation](/docs/components/cms-web/navigation/osirion-content-navigation) | Previous and next links between the items of a directory | Loaded around the item you pass, or the neighbours you pass |
| [ContentSeriesNavigation](/docs/components/cms-web/navigation/series-navigation) | "Part 2 of 5", the parts of a series and previous and next links (from 4.0) | Loaded from the items with the same `series` |
| [TableOfContents](/docs/components/cms-web/navigation/table-of-contents) | Links to the headings of an HTML string | The HTML you pass |
| [SearchBox](/docs/components/cms-web/navigation/search-box) | A search form that submits with GET | None; your results page reads the query |

## Before you start

Components that load data use `IContentProviderManager`, so a content provider must be registered. The example
apps register everything, including the CMS from the `Osirion:Cms` configuration section, with one call:

```csharp
using Osirion.Blazor.Extensions;

builder.Services.AddOsirion(builder.Configuration);
```

## Rendering

Every component here renders under static SSR. Links are plain anchors and the search box is a plain HTML form,
so none of them needs script. Components that load data do it in `OnParametersSetAsync`; without streaming
rendering, a statically rendered page is sent once the data is there. Click callbacks such as `DirectoryClicked`
run only in an interactive render mode.

## Styling

Each component ships a scoped stylesheet whose colors, spacing and fonts come from the Osirion theme tokens
(`--osirion-text-primary`, `--osirion-action-primary`, `--osirion-background-tertiary` and others), so setting the
tokens or using a framework adapter restyles them with the rest of the site. Scoped rules carry an extra attribute
selector, so an override in your own stylesheet needs one more class or element than the rule it replaces, for
example `.sidebar .osirion-categories-list .osirion-category-link`.

## Related

- [CMS Web components](/docs/components/cms-web)
- [Core CMS components](/docs/components/cms-web/core), such as `ContentList` for category, tag and search result pages
- [OsirionBreadcrumbs](/docs/components/core/navigation/breadcrumbs), breadcrumbs built from a URL path instead of the content tree
