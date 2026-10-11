---
title: "LocalizedContentView - Osirion Blazor"
description: "Render one content item with links to its translations, locale-aware category and tag links, a sanitized body and optional previous and next links."
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Core Components
tags:
- localized-content-view
- localization
- translations
- cms
slug: "localized-content-view"
layout: "component"
published: true
---

# LocalizedContentView

`LocalizedContentView` renders one content item, like [ContentView](/docs/components/cms-web/core/content-view),
and adds links to the same item in other languages. Use it on multilingual sites where each translation is a
separate Markdown file.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<LocalizedContentView Item="@item" CurrentLocale="@Locale" />
```

A page routed as `/{locale}/blog/{slug}`, with readable language names and translation links that keep the locale
in the address:

```razor
<LocalizedContentView Item="@item"
                      CurrentLocale="@Locale"
                      LocaleNameFormatter="@(code => code switch { "en" => "English", "sr" => "Srpski", _ => code })"
                      TranslationUrlFormatter="@((id, locale) => $"/{locale}/{item!.Url}")" />
```

This formatter assumes each translation has the same slug; build the URL another way if yours differ.

## How translations are found

Translations are the items that share the current item's `ContentId`. Give each translation the same `id` in its
front matter, or let the provider pair them: with `EnableLocalization` on, an item without an `id` gets a stable id
built from its path without the locale folder, so `en/blog/post.md` and `sr/blog/post.md` belong together. The links
appear only when localization is enabled on the component and more than one translation exists; the current locale
is left out.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Item` | `ContentItem?` | `null` | The item to render. |
| `Path` | `string?` | `null` | Source path to load from the default provider while `Item` is not set. |
| `CurrentLocale` | `string` | `"en"` | The page's locale: left out of the translation links and used in default category and tag URLs and the neighbor query. |
| `EnableLocalization` | `bool` | `true` | Looks up and shows translation links. |
| `LocaleNameFormatter` | `Func<string, string>?` | `null` | Link text for a locale code; default is the code in upper case. |
| `TranslationUrlFormatter` | `Func<string, string, string>?` | `null` | Builds a translation link from the localization id and the target locale; default `/{translation.Url}`. |
| `ShowJumbotron` | `bool` | `true` | Renders the header: title, "By" author, date, reading time and featured image. |
| `ShowNavigationLinks` | `bool` | `false` | Shows previous and next links to items in the same directory and locale, newest first. |
| `PreviousItem` | `ContentItem?` | `null` | Item for the previous link. |
| `NextItem` | `ContentItem?` | `null` | Item for the next link. |
| `CategoryUrlFormatter` | `Func<string, string>?` | `null` | Builds category links; default `/{CurrentLocale}/category/{slug}`. |
| `TagUrlFormatter` | `Func<string, string>?` | `null` | Builds tag links; default `/{CurrentLocale}/tag/{slug}`. |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Builds previous and next links; default `/{item.Path}`. |
| `LoadingText` | `string` | `"Loading content..."` | Shown while loading, in interactive render modes only. |
| `NotFoundText` | `string` | `"Content not found."` | Shown when no item is found. |
| `LocalizationId` | `string?` | `null` | Set from the item's `ContentId`; leave it unset. |
| `OnLocaleChanged` | `EventCallback<string>` | none | Not raised by the current markup, because translation links are plain links. |
| `Class` | `string?` | `null` | Extra CSS classes on the root element. |

`Style`, `Theme` and unmatched attributes from the base class are not written to the markup.

## Behavior

- **Rendering.** Static SSR renders the item, its translation links and its footer on the server; there is no
  script. Translation, category, tag and navigation links are ordinary links.
- **Loading.** The component loads by `Path` only while `Item` is null, so in an interactive page that changes
  address, pass `Item` rather than a new `Path`.
- **Default URLs.** The built-in providers drop the locale folder from an item's `Url`, so the default translation
  link is the same path for every language. Add the locale with `TranslationUrlFormatter`. The default previous and
  next link is the item's source path; set `ContentUrlFormatter` when you show navigation links.
- **Security.** The body passes through the registered `IHtmlContentSanitizer` before it is written as HTML, the
  same allow-list `ContentView` uses. Titles, names and locale labels are encoded as text.
- **Images.** The featured image uses the title as `alt` text and, from 4.0, loads eagerly with
  `fetchpriority="high"`.
- **SEO.** Nothing is written to the head. For `hreflang` links, pass the translations to
  [SeoMetadataRenderer](/docs/components/cms-web/core/seo-metadata-renderer) as `AlternateLanguageUrls`.

## Styling

The root element has the class `osirion-localized-content-view`. Inside are `osirion-translations` (the language
links), `osirion-content-article`, `osirion-content-header` with `osirion-content-meta`, `osirion-content-body`,
`osirion-content-footer` with `osirion-content-categories` and `osirion-content-tags`, and
`osirion-content-navigation`. The not-found and loading states use `osirion-not-found` and `osirion-loading`. Colors
and spacing come from theme tokens such as `--osirion-text-primary`, `--osirion-text-secondary` and
`--osirion-action-primary`.

## Related

- [ContentView](/docs/components/cms-web/core/content-view): the single-language version
- [ContentPage](/docs/components/cms-web/core/content-page): renders this component inside a page layout
- [LocalizedNavigation](/docs/components/cms-web/navigation/localized-navigation): language-aware navigation
- [Localization for Blazor content sites](/blog/localization-for-blazor-content-sites)
