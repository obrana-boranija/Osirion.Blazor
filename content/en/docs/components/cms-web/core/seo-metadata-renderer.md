---
title: "SeoMetadataRenderer - Osirion Blazor"
description: "Write the page title, meta description, canonical URL, robots, Open Graph, Twitter Card and JSON-LD tags for a content item or a list of items."
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-17
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Core Components
tags:
- seo
- metadata
- open-graph
- json-ld
- structured-data
slug: "seo-metadata-renderer"
layout: "component"
published: true
---

# SeoMetadataRenderer

`SeoMetadataRenderer` writes the head metadata for a content page: the `<title>`, description, canonical URL,
robots directives, Open Graph and Twitter Card tags, and schema.org JSON-LD. Give it one item (`Content`) for an
article or several (`ContentItems`) for a list page. `ArticlePage` and `ContentList` already render it; add it
yourself when you build a page from `ContentView` or `ContentPage`.

## Usage

Place it inside `<HeadContent>`, and keep `<HeadOutlet />` in your `App.razor`:

```razor
@using Osirion.Blazor.Cms.Web.Components

@if (item is not null)
{
    <HeadContent>
        <SeoMetadataRenderer Content="@item"
                             SchemaTypes="@(new[] { SchemaType.BlogPosting })" />
    </HeadContent>
}
```

Render it only when you have content: with neither `Content` nor `ContentItems` it writes only an empty title.

Site-wide defaults come from `SeoMetadataOptions`, either in code:

```csharp
using Osirion.Blazor.Cms.Web.DependencyInjection;

builder.Services.AddOsirionSeoMetadata(seo =>
{
    seo.SiteName = "Contoso Docs";
    seo.OrganizationName = "Contoso";
    seo.SiteLogoUrl = "/logo.png";
    seo.DefaultImageUrl = "/assets/images/cover.png";
    seo.TwitterSite = "@contoso";
});
```

or in `appsettings.json` under `Osirion:Seo`, which `AddOsirion(builder.Configuration)` reads:

```json
{
  "Osirion": {
    "Seo": {
      "SiteName": "Contoso Docs",
      "SiteUrl": "https://docs.contoso.com",
      "SiteLogoUrl": "/logo.png",
      "DefaultImageUrl": "/assets/images/cover.png"
    }
  }
}
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Content` | `ContentItem?` | `null` | The item for a single content page. |
| `ContentItems` | `IReadOnlyList<ContentItem>?` | `null` | The items of a list page. |
| `CollectionTitle` | `string?` | `null` | Title of a list page. |
| `CollectionDescription` | `string?` | `null` | Description of a list page. |
| `SchemaTypes` | `SchemaType[]?` | `null` | JSON-LD types to generate, for example `Article`, `BlogPosting`, `TechArticle`, `WebPage`, `HowTo`, `BreadcrumbList` or `CollectionPage`, and for whole pages `SoftwareApplication`, `ProfilePage`, `AboutPage` and `ContactPage`. `FAQ` generates no data; use `EnableGeneratedFaq` instead. Google no longer shows FAQ or HowTo rich results. The first type also sets `og:type` when the front matter has no `og_type`. |
| `ApplicationCategory` | `string` | `"DeveloperApplication"` | `applicationCategory` of the SoftwareApplication schema, one of the app types search engines support. |
| `GenerateMultipleSchemas` | `bool` | `true` | Adds a BreadcrumbList when exactly one type is generated. |
| `SiteNameOverride` | `string?` | `null` | Site name in titles and Open Graph; otherwise `OrganizationName`, then the host name. |
| `OrganizationName` | `string?` | `null` | Publisher name in structured data. |
| `SiteLogoUrl` | `string?` | `null` | Publisher logo; default `/logo.png`. |
| `DefaultImageUrl` | `string?` | `null` | Social image when the item has none. |
| `DefaultImageWidth` | `int` | `1200` | `og:image:width`. |
| `DefaultImageHeight` | `int` | `630` | `og:image:height`. |
| `TwitterSite` | `string?` | `null` | `twitter:site` handle. |
| `TwitterCreator` | `string?` | `null` | `twitter:creator` handle. |
| `FacebookAppId` | `string?` | `null` | `fb:app_id`. |
| `AllowSearchIndexing` | `bool` | `true` | `false` writes `noindex, nofollow`. |
| `CanonicalQueryParameters` | `IReadOnlyCollection<string>?` | `null` | Query parameters kept in the canonical URL, for example `page`. |
| `AlternateLanguageUrls` | `List<string>?` | `null` | `hreflang` links, each a language code and a URL joined by a vertical bar; malformed entries are skipped. |
| `PrevPageUrl` | `string?` | `null` | `rel="prev"` link. |
| `NextPageUrl` | `string?` | `null` | `rel="next"` link. |
| `BreadcrumbHomeText` | `string` | `"Home"` | First BreadcrumbList entry. |
| `CustomBreadcrumbs` | `List<(string Name, string Url)>?` | `null` | Breadcrumb entries after Home, in place of the ones built from the item's URL. |
| `EmitMobileMetaTags` | `bool` | `false` | Also writes viewport, theme-color and mobile web app tags. |
| `EnableGeneratedFaq` | `bool` | `false` | Builds FAQPage data and answer tags from the title and description. |
| `EnableAeoOptimization` | `bool` | `true` | Must be on for `EnableGeneratedFaq`. |
| `EnableGeoOptimization` | `bool` | `true` | Writes summary meta tags (`entity-summary`, `content-type`, `primary-category`, `content-author`, `content-expertise-level`, `content-freshness`) when the item or list has a description. |
| `AllowAiDiscovery` | `bool` | `true` | Chooses between crawler opt-in and opt-out meta tags. |
| `AllowAiTraining` | `bool` | `true` | Sets the training opt-in tag among them. |
| `SiteDescription` | `string?` | `null` | Accepted but not written to the output. |

The crawler tags controlled by the last parameters use meta names that are not a standard; use `robots.txt` to
control crawlers reliably.

## What is read from the item

| Output | Source, first match wins |
| --- | --- |
| `<title>` | `seo_properties.title`, then `title`; the site name follows after a vertical bar, and the whole is cut to 60 characters |
| description | `seo_properties.description`, then `description`, cut to 160 characters |
| canonical | `seo_properties.canonical`, then the current URL without query string and fragment, on `SiteUrl` when it is set |
| robots | `AllowSearchIndexing`, then `seo_properties.robots` (front matter default `index, follow`) |
| `og:title`, `og:description` | `seo_properties.og_title` and `og_description`, then the title and description above |
| `og:type` | `seo_properties.og_type` (front matter default `article`), then `article` when the first schema type is Article, BlogPosting or TechArticle, `product` for Product, `video.other` for VideoObject, `profile` for ProfilePage, otherwise `website` |
| `og:image` | `seo_properties.og_image_url`, then `featured_image`, then `DefaultImageUrl` |
| Twitter Card | `seo_properties.twitter_card`, `twitter_title`, `twitter_description`, `twitter_image_url`, then Open Graph |
| keywords, `article:tag` | `tags` |
| `article:section` | the first of `categories` |
| JSON-LD | `seo_properties.json_ld`, then `SchemaTypes`, then `seo_properties.type` (front matter default `Article`), then `WebPage` |

Relative image URLs become absolute with the site's base URL.

## Page types

| Type | Use it for | What it holds |
| --- | --- | --- |
| `TechArticle` | Documentation and technical guides | The Article schema with the more specific type. |
| `SoftwareApplication` | A product page about an app or a library | Name, description, image, `ApplicationCategory`, last modified date and keywords. Search engines show software rich results only with a price and a rating or review; put the full schema in `json_ld` when you have them. |
| `ProfilePage` | An author's page | The person as `mainEntity`: the `author` (or the title), description and image, with tags that are absolute links as `sameAs`. |
| `AboutPage` | The about page | A WebPage with the site's organization as `mainEntity`. |
| `ContactPage` | The contact page | A WebPage with the more specific type. |

## Behavior

- **Rendering.** It renders on the server as part of the head, with no script, and recomputes everything when its
  parameters change, so a reused instance never keeps the previous page's canonical URL or structured data.
- **List pages.** With `ContentItems`, the title is `{CollectionTitle} | {site name}`, `og:type` is `website`, and
  JSON-LD holds a CollectionPage (when listed in `SchemaTypes`) plus a BlogPosting per item.
- **Security.** Meta values are HTML-encoded. JSON-LD is serialized with HTML-sensitive characters escaped; custom
  `json_ld` from front matter is parsed and re-serialized the same way, and ignored with a logged warning when it is
  not a JSON object or array.
- **Fixed values.** It also writes `generator`, `referrer`, `language` and `content-language`, the robots value
  again as `googlebot` and `bingbot`, publish date, last-modified and reading time meta tags, `dns-prefetch` and
  `preconnect` hints for the site's own address. The Article schema gives the author a `url` only when `AuthorUrl` is
  set.
- **Options.** `SeoMetadataOptions` fills the parameters you leave unset: `SiteName` (as `SiteNameOverride`),
  `OrganizationName`, `SiteLogoUrl`, `DefaultImageUrl`, `DefaultImageWidth`, `DefaultImageHeight`, `TwitterSite`,
  `TwitterCreator`, `FacebookAppId`, `AuthorUrl`, `EmitMobileMetaTags`, `EnableGeneratedFaq` and
  `DefaultSchemaTypes`. The switches `AllowSearchIndexing`, `AllowAiDiscovery`, `AllowAiTraining`,
  `EnableGeoOptimization`, `EnableAeoOptimization` and `GenerateMultipleSchemas` come from the options unless the
  page sets them as parameters. `BreadcrumbHomeText` applies when the parameter is empty.
- **Public address.** `SiteUrl` (for example `https://example.com`) makes the canonical URL, `og:url` and the
  addresses in structured data use the site's address with the page's path, whatever host served the request: a
  proxy, a preview host or a second domain. Only an absolute `http` or `https` address is used; otherwise they follow
  the request.

## Related

- [ContentList](/docs/components/cms-web/core/content-list): renders this for list pages
- [Feeds and sitemap](/docs/components/cms-web/core/feeds-and-sitemap): `sitemap.xml`, RSS and Atom
- [ContentView](/docs/components/cms-web/core/content-view): the article body to pair it with
- [Blazor SEO checklist for SSR and SSG](/blog/blazor-seo-ssr-ssg-checklist)
