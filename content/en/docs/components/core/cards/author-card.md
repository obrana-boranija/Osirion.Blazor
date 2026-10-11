---
id: 'osirion-author-card'
order: 4
layout: docs
title: OsirionAuthorCard
permalink: /docs/components/core/cards/author-card
description: OsirionAuthorCard introduces an author with avatar or initials, name, role, bio and profile links. Renders without script. Available from 4.1.
author: Dejan Demonjić
date: 2026-10-11
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Cards
tags:
- author-card
- authors
- cards
- osirion-blazor-core
is_featured: false
published: true
slug: author-card
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionAuthorCard - Osirion.Blazor'
  description: 'OsirionAuthorCard introduces an author with avatar or initials, name, role, bio and profile links. Renders without script. Available from 4.1.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/cards/author-card'
  lang: en
  robots: index, follow
  og_title: 'OsirionAuthorCard - Osirion.Blazor'
  og_description: 'An author card with avatar or initials, name, role, bio and rel="me" links.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionAuthorCard - Osirion.Blazor'
  twitter_description: 'An author card with avatar or initials, name, role, bio and profile links.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# OsirionAuthorCard

`OsirionAuthorCard` introduces a person: an avatar (or their initials), the name, a role, a short bio and links to
their profiles. Put it under an article or at the top of an author's page. It renders without script.

Available from 4.1.

## Usage

```razor
<OsirionAuthorCard Name="Jane Developer" />
```

With every detail, and links in an `AuthorLink` list:

```razor
@using Osirion.Blazor.Core.Models

<OsirionAuthorCard Name="Jane Developer"
                   Role="Principal engineer"
                   Bio="Writes about Blazor and the web platform."
                   AvatarUrl="/img/authors/jane.jpg"
                   ProfileUrl="/authors/jane-developer"
                   Links="links" />

@code {
    private readonly AuthorLink[] links =
    [
        new("GitHub", "https://github.com/jane"),
        new("Email", "mailto:jane@example.com")
    ];
}
```

### An author's page

With Osirion.Blazor.Cms.Web, combine the card with `ContentList`, which filters by the `author` front matter field
(the whole name, ignoring case):

```razor
@page "/authors/jane-developer"
@using Osirion.Blazor.Cms.Web.Components

<OsirionAuthorCard Name="Jane Developer" Role="Principal engineer" />
<ContentList Author="Jane Developer" Directory="blog" />
```

To link bylines on article pages to that page, set `AuthorUrlFormatter` on `ContentView`; the author's name then links
there with `rel="author"`.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Name` | `string` | `""` | Required. The author's name; the initials are taken from its first two words. |
| `Role` | `string?` | `null` | Role or title, shown under the name. |
| `Bio` | `string?` | `null` | Short biography, plain text. |
| `AvatarUrl` | `string?` | `null` | Avatar image. Without a usable URL the initials are shown. |
| `ProfileUrl` | `string?` | `null` | The author's page. The name links there with `rel="author"`. |
| `Links` | `IReadOnlyList<AuthorLink>?` | `null` | Profile links, rendered as a list with `rel="me"`. Links with an empty label are left out. |
| `Class` | `string?` | `null` | Extra CSS classes on the card. |

`AuthorLink` is a record in `Osirion.Blazor.Core.Models`: `AuthorLink(string Label, string Url)`. Unmatched
attributes go on the card's root element.

## Behavior

- URLs are checked before they are written. `AvatarUrl`, `ProfileUrl` and each link's `Url` are rendered only when
  they are a path on the site (starting with a single `/`) or an absolute `http`, `https` or `mailto` URL. Other
  schemes such as `javascript:` and `data:`, protocol-relative URLs (`//host`) and relative paths without a leading
  `/` are left out, so author details taken from content cannot inject script.
- The avatar is decorative (the name is next to it): it has an empty `alt`, a 64 by 64 size, lazy loading and
  async decoding. The initials are hidden from assistive technology for the same reason.
- `rel="me"` lets services that verify profile links, such as Mastodon, match the author's site to the profile.
- All text is HTML-encoded.

## Styling

The card uses the classes `osirion-author-card`, `osirion-author-card-avatar`, `osirion-author-card-initials`,
`osirion-author-card-body`, `osirion-author-card-name`, `osirion-author-card-role`, `osirion-author-card-bio` and
`osirion-author-card-links`. It reads the theme tokens `--osirion-border-color`, `--osirion-background-primary`,
`--osirion-text-primary`, `--osirion-text-secondary`, and `--osirion-action-primary` with
`--osirion-action-primary-text` for the initials. On viewports 30rem wide or narrower the avatar stacks above the text.

## Related

- [Cards overview](/docs/components/core/cards)
- [Article metadata](/docs/components/core/content/article-metadata)
- [CMS web components](/docs/components/cms-web)
