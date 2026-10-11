---
id: 'homepage'
order: 1
layout: landing
title: 'Osirion.Blazor'
permalink: /
description: 'SSR-first Blazor components for content sites. Render Markdown from GitHub or a local folder as pages, lists and articles. For .NET 9 and .NET 10.'
author: 'Dejan Demonjić'
date: '2024-12-29'
last_modified: 2026-10-11
featured_image: '/images/osirion-hero.png'
categories:
  - 'Blazor'
  - 'CMS'
  - 'Components'
tags:
  - 'blazor'
  - 'ssr'
  - 'markdown'
  - 'cms'
  - 'github'
  - 'components'
is_featured: true
published: true
slug: 'homepage'
lang: 'en'
seo_properties:
  title: 'Osirion.Blazor - SSR-first Blazor components for content sites'
  description: 'Open-source Blazor components that render Markdown from GitHub or a local folder as pages, lists and articles, with static SSR.'
  canonical: 'https://getosirion.com'
  robots: 'index, follow'
---

Osirion.Blazor is an open-source (MIT) component library for Blazor sites built from content: blogs, documentation
and marketing pages. It reads Markdown with front matter from a GitHub repository or a local folder and renders it as
lists and articles. Every component renders its HTML on the server, so pages work under static server-side rendering
and add script only where a component needs the browser.

## What it does

- **Content from Markdown.** Lists with paging, sorting and filters by folder, category or tag; article pages
  with breadcrumbs, a table of contents and tags; search, a tag cloud and a category list. Localized content lives in
  language folders.
- **SEO.** Title, description, canonical URL, Open Graph and Twitter tags and schema.org structured data for each
  page, and a missing article answers with HTTP 404.
- **Safe output.** Rendered Markdown is cleaned with an allow-list, so script and event handler attributes in content
  never reach the page.
- **Page building blocks.** A page layout with header and footer, hero and other sections, cards, carousels, a
  contact form and a cookie consent banner.
- **Navigation.** Menus up to mega menus, scroll handling for Blazor's enhanced navigation and a scroll to top
  button.
- **Analytics.** Microsoft Clarity, Matomo, Google Analytics 4 and Yandex Metrica, optionally held back until the
  visitor accepts analytics cookies.
- **Theming.** Light and dark themes with a toggle, and adapters that make the components follow your CSS
  framework.

## Packages

| Package | What it gives you |
| --- | --- |
| `Osirion.Blazor` | All modules below and `AddOsirion`, which configures them from `appsettings.json`. Start here. |
| `Osirion.Blazor.Core` | Layout, sections, cards, carousels, the contact form, cookie consent, loading and not-found states, and the HTML sanitizer. |
| `Osirion.Blazor.Cms.Web` | Content components: `ContentList`, `ContentView`, `ArticlePage`, `HomePage`, search, tags, categories, table of contents and SEO metadata. |
| `Osirion.Blazor.Cms.Admin` | Admin pages under `/osirion` for editing content in a GitHub repository, behind an authorization policy. |
| `Osirion.Blazor.Cms` | CMS web and CMS admin together. |
| `Osirion.Blazor.Navigation` | `Menu`, `EnhancedNavigation` and `ScrollToTop`. |
| `Osirion.Blazor.Analytics` | Tracker components for the analytics providers above. |
| `Osirion.Blazor.Theming` | `OsirionStyles`, `ThemeToggle` and the CSS framework adapters. |

The CMS packages also bring in their layers (`Cms.Domain`, `Cms.Application`, `Cms.Infrastructure` and `Cms.Core`);
the GitHub and local folder content providers live in `Osirion.Blazor.Cms.Infrastructure`.

## Quick start

Add the package to a Blazor Web App:

```bash
dotnet add package Osirion.Blazor
```

Register it in `Program.cs`:

```csharp
using Osirion.Blazor.Extensions;

builder.Services.AddOsirion(builder.Configuration);
builder.Services.AddJSComponents();
```

Point it at a folder of Markdown files in `appsettings.json`:

```json
{
  "Osirion": {
    "Cms": {
      "Web": {
        "FileSystem": {
          "BasePath": "content",
          "IsDefault": true
        }
      }
    }
  }
}
```

Then put `<OsirionStyles />` in your layout and list the posts in `content/blog` on a page:

```razor
@page "/blog"
@using Osirion.Blazor.Cms.Web.Components

<ContentList Directory="blog" ShowPagination />
```

[Getting started](/docs/getting-started) walks through every step, including the article page and a GitHub
repository as the source.

## CSS frameworks

Set `Osirion:Theming:Framework` to `Bootstrap`, `FluentUI`, `MudBlazor` or `Radzen` and `OsirionStyles` loads an
adapter that maps the framework's colors onto Osirion's, so the components match the rest of the site in light and
dark mode. Your app still loads the framework itself. Without a framework, the components use Osirion's own palette.

The repository has complete example sites on Bootstrap, MudBlazor and Fluent UI.

## Documentation

- [Getting started](/docs/getting-started)
- [Components overview](/docs/components)
- [CMS web components](/docs/components/cms-web)
- [Core components](/docs/components/core)
- [Theming](/docs/components/theming)
- [Blog](/blog)

## Coming in 4.1

Available from 4.1: callouts, tabs, accordions and FAQ, steps, pricing tables, responsive images, video embeds, an
announcement bar, a reading progress bar, a sitemap with RSS and Atom feeds, and Plausible and Umami analytics. The
[component showcase](/showcase) renders the new content components with static SSR.

## Source

The source code, issues and examples are on [GitHub](https://github.com/obrana-boranija/Osirion.Blazor), and the
packages are on [NuGet](https://www.nuget.org/packages/Osirion.Blazor).
