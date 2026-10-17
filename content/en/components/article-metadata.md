---
id: 'osirion-article-metadata'
order: 1
layout: "component"
title: Article Metadata
permalink: /components/article-metadata
description: "OsirionArticleMetdata shows an article's author, publish date and read time in one line with icons, and renders nothing when all three are empty."
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Content
tags:
- metadata
- article
- author
- date
- read-time
- blog
slug: article-metadata
lang: en
published: true
redirect_from:
- /docs/components/core/article-metadata
- /docs/components/core/content/article-metadata
custom_fields:
  group: content
seo_properties:
  title: "Article Metadata Component - Osirion.Blazor"
  description: "Show an article's author, publish date and read time with OsirionArticleMetdata, a script-free Blazor component."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/components/article-metadata"
---

# Article Metadata

`OsirionArticleMetdata` shows an article's author, publish date and read time on one line, each with a small icon.
Use it under an article title or in a list of posts. It renders only the fields you set, and nothing at all when all
three are empty.

> [!NOTE]
> The component name is spelled `OsirionArticleMetdata` (without the second "a") in the library. Use that spelling in
> Razor.

## Usage

```razor
@using Osirion.Blazor.Components

<OsirionArticleMetdata Author="Dejan Demonjić" PublishDate="@publishDate" ReadTime="5" />

@code {
    private readonly DateTime publishDate = new(2026, 10, 11);
}
```

This renders the author, the date in the current culture's short date format, and "5 min".

A realistic case is a post header fed from your own model, with a long date format:

```razor
<h1>@post.Title</h1>
<OsirionArticleMetdata Author="@post.Author"
                       PublishDate="@post.PublishedOn"
                       ReadTime="@post.ReadMinutes.ToString(CultureInfo.InvariantCulture)"
                       DateFormat="MMMM d, yyyy" />
```

`post` stands for your own type here; `PublishedOn` is a `DateTime?` and `ReadMinutes` an `int`. Add
`@using System.Globalization` for `CultureInfo`.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Author` | `string?` | `null` | Author name. Hidden when empty or whitespace. |
| `PublishDate` | `DateTime?` | `null` | Publication date. Hidden when `null`. |
| `DateFormat` | `string?` | `"d"` | .NET date format string for `PublishDate`, applied with `CultureInfo.CurrentCulture`. |
| `ReadTime` | `string?` | `null` | Read time in minutes. The component appends " min", so pass the number only (`"5"`, not `"5 min"`). |

The component inherits `Class`, `Style`, `Theme` and unmatched attributes from `OsirionComponentBase`, but its
markup does not apply them: the root element always renders as `<div class="osirion-article-metadata">`. To restyle
it, wrap the component in an element of your own and target `.your-wrapper .osirion-article-metadata` from your
stylesheet. The component's own styles are scoped (a class plus an attribute), so a single class selector loses to
them; a parent class matches their specificity, and your rule wins when your stylesheet loads after the library's.

## Behavior

- Renders plain HTML under static SSR and needs no script.
- The date is formatted on the server with the request's current culture, so set up request localization if your
  readers expect their own date format.
- Values are written as text, so Razor encodes them; HTML in `Author` shows as text.
- The date is plain text, not a `<time>` element.
- `HeroSection` uses this component when its `ShowMetadata` parameter is `true`, passing its own `Author`,
  `PublishDate` and `ReadTime`.

## Styling

The markup is a flex row:

| Class | Element |
| --- | --- |
| `osirion-article-metadata` | The wrapping `<div>` (wraps onto several lines when needed, 80% opacity) |
| `osirion-article-metadata-author` | The author item |
| `osirion-article-metadata-date` | The date item |
| `osirion-article-metadata-read-time` | The read time item |
| `osirion-article-metadata-icon` | The date and read time icons |

The styles are scoped to the component and use the theme tokens `--osirion-spacing-2`, `--osirion-spacing-4`,
`--osirion-spacing-6` and `--osirion-font-size-sm`, so a theme or framework adapter that sets them changes the spacing
and size. Icons draw with `currentColor` and follow the text color.

## Related

- [Hero section](/components/hero-section)
- [Content components](/components/content)
