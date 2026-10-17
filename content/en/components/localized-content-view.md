---
order: 3
title: LocalizedContentView
permalink: /components/localized-content-view
description: "Render one content item with links to its translations, locale-aware category and tag links, a sanitized body and optional previous and next links."
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-17
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
- static-ssr
slug: localized-content-view
lang: en
layout: "component"
published: true
redirect_from:
- /docs/components/cms-web/core/localized-content-view
custom_fields:
  group: cms-pages
seo_properties:
  title: 'LocalizedContentView: translated articles for Blazor'
  description: 'LocalizedContentView renders one CMS content item with links to its translations and locale-aware category and tag links.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/localized-content-view'
  lang: en
  robots: index, follow
  og_title: 'LocalizedContentView - Osirion.Blazor'
  og_description: 'One content item with links to its translations, locale-aware category and tag links and a sanitized body.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'LocalizedContentView - Osirion.Blazor'
  twitter_description: 'One content item with links to its translations, locale-aware category and tag links and a sanitized body.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# LocalizedContentView

`LocalizedContentView` renders one content item, like [ContentView](/components/content-view),
and adds links to the same item in other languages. Use it on multilingual sites where each translation is a
separate Markdown file.

Package `Osirion.Blazor.Cms.Web`, namespace `Osirion.Blazor.Cms.Web.Components`. [Source on GitHub](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Cms.Web/Components/LocalizedContentView.razor).

```demo
localized-content-view
```

The demo loads this site's own page for the 1.5 release notes and shows it in a scrolling frame. This site's content is
written in English only, so the item has no translations and the language links do not appear; they show above the
article once a second language exists. `LocalizedContentView` writes the title as an `h1`, so the demo adds a second
`h1` to this documentation page.

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

<!-- osirion:parameters LocalizedContentView -->

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `LocalizationId` | `string?` | `null` | The id an item shares with its translations. The component sets it from the item's `ContentId`; leave it unset. |
| `Path` | `string?` | `null` | The source path of the item to load from the default provider while `Item` is not set. A later change of the path is ignored once an item is shown, so pass `Item` on a page that changes address while it is interactive. |
| `CurrentLocale` | `string` | `"en"` | The locale of the page. It is left out of the translation links, it is the locale of the default category and tag links (`/{locale}/category/{slug}`) and of the neighbor query. |
| `OnLocaleChanged` | `EventCallback<string>` | - | Has no effect: translation links are plain links and nothing in the markup raises it. |
| `LoadingText` | `string` | `"Loading content..."` | The text shown while the item loads, in interactive render modes only. A statically rendered response is sent once the item has loaded. |
| `NotFoundText` | `string` | `"Content not found."` | The message shown when no item is found, when loading fails (the error is logged) and when neither `Item` nor `Path` is set. |
| `LocaleNameFormatter` | `Func<string, string>?` | `null` | Builds the link text of a translation from its locale code. Without it the text is the code in upper case, such as `SR`. |
| `TranslationUrlFormatter` | `Func<string, string, string>?` | `null` | Builds a translation link from the localization id and the target locale. Without it the link is the translation's `Url` under a leading slash, which the built-in providers store without the locale folder, so add the locale here. |
| `CategoryUrlFormatter` | `Func<string, string>?` | `null` | Builds a category link from the category name. Without it the link is `/{CurrentLocale}/category/{slug}`. |
| `TagUrlFormatter` | `Func<string, string>?` | `null` | Builds a tag link from the tag name. Without it the link is `/{CurrentLocale}/tag/{slug}`. |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Builds the links of the previous and next items from the item. Without it a link is `/{item.Path}`, the source path, which is usually not the public address. |
| `Item` | `ContentItem?` | `null` | The item to render. When it is set, the component loads its translations (and the neighbors when `ShowNavigationLinks` is on) and never loads by `Path`. |
| `PreviousItem` | `ContentItem?` | `null` | The item the previous link points to. Loading with `ShowNavigationLinks` on replaces it when the neighbor query finds the item. |
| `NextItem` | `ContentItem?` | `null` | The item the next link points to. See `PreviousItem`. |
| `ShowNavigationLinks` | `bool` | `false` | Whether to show previous and next links to items in the same directory and locale, newest first. The component looks the neighbors up whenever it loads, also for an item you pass. |
| `ShowJumbotron` | `bool` | `true` | Whether to render the header: the title as the page's `h1`, the author ("By ..."), the date, the reading time and the featured image. When it is on, the body leaves out its own leading `h1` so the title appears once; when it is off, the body is written as it is. |
| `EnableLocalization` | `bool` | `true` | Whether to look up the item's translations and list them. The links appear only when the item has more than one translation, and the current locale is left out. |
| `Theme` | `ThemeMode` | `ThemeMode.System` | The component's color mode. `Light` or `Dark` writes it to `data-theme` (`data-bs-theme` under Bootstrap) on the root element; `System` writes nothing. |
| `Class` | `string?` | `null` | Extra CSS classes for the component's root element. |
| `Style` | `string?` | `null` | An inline style for the component's root element. |

<!-- /osirion:parameters -->

The `Theme` and `Style` rows come from the base class. `LocalizedContentView` does not write them, or unmatched attributes, so only `Class` reaches the root element.

## Behavior

- **Rendering.** Static SSR renders the item, its translation links and its footer on the server; there is no
  script. Translation, category, tag and navigation links are ordinary links.
- **Loading.** The component loads by `Path` only while `Item` is null, so in an interactive page that changes
  address, pass `Item` rather than a new `Path`.
- **Header.** With `ShowJumbotron` on, the header shows the title as the page's `h1`, and the body leaves out its
  own leading `h1` so the title appears once. With it off, the body is written as it is, leading `h1` included.
- **Default URLs.** The built-in providers drop the locale folder from an item's `Url`, so the default translation
  link is the same path for every language. Add the locale with `TranslationUrlFormatter`. The default previous and
  next link is the item's source path; set `ContentUrlFormatter` when you show navigation links.
- **Not found.** Without an item the component renders `NotFoundText` in a plain block. The response keeps its status
  code.
- **Security.** The body passes through the registered `IHtmlContentSanitizer` before it is written as HTML, the
  same allow-list `ContentView` uses. Titles, names and locale labels are encoded as text.
- **Images.** The featured image uses the title as `alt` text and, from 4.0, loads eagerly with
  `fetchpriority="high"`.
- **SEO.** Nothing is written to the head. For `hreflang` links, pass the translations to
  [SeoMetadataRenderer](/components/seo-metadata-renderer) as `AlternateLanguageUrls`.

## Styling

The root element has the class `osirion-localized-content-view`. Inside are `osirion-translations` (the language
links), `osirion-content-article`, `osirion-content-header` with `osirion-content-meta`, `osirion-content-body`,
`osirion-content-footer` with `osirion-content-categories` and `osirion-content-tags`, and
`osirion-content-navigation`. The not-found and loading states use `osirion-not-found` and `osirion-loading`. Colors
and spacing come from theme tokens such as `--osirion-text-primary`, `--osirion-text-secondary` and
`--osirion-action-primary`.

## Related

- [CMS pages](/components/cms-pages): the group overview
- [ContentView](/components/content-view): the single-language version
- [ContentPage](/components/content-page): renders this component inside a page layout
- [LocalizedNavigation](/components/localized-navigation): language-aware navigation
- [Localization for Blazor content sites](/blog/localization-for-blazor-content-sites)
