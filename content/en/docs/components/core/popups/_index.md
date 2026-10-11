---
id: 'core-popups-components-overview'
order: 1
layout: docs
title: Popups
permalink: /docs/components/core/popups
description: 'Banners in Osirion.Blazor.Core that work without script: cookie consent with categories and a dismissible site-wide announcement bar.'
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Popups
tags:
- blazor
- cookie-consent
- announcement
- privacy
- static-ssr
is_featured: true
published: true
slug: popups
lang: en
custom_fields: {}
seo_properties:
  title: 'Popups and banners - Osirion.Blazor'
  description: 'Cookie consent and a dismissible announcement bar for Blazor that work without script under static SSR.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/popups'
  lang: en
  robots: index, follow
  og_title: 'Popups and banners - Osirion.Blazor'
  og_description: 'Cookie consent and a dismissible announcement bar that work without script.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Popups and banners - Osirion.Blazor'
  twitter_description: 'Cookie consent and an announcement bar that work without script.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Popups

Both banners in this group work the same way: their buttons post a small form to an endpoint you map, the endpoint
sets a cookie and redirects back, and the next server render reads that cookie. No script is involved, so they work
under static SSR, and every post is protected by an antiforgery token.

| Component | What it does |
| --- | --- |
| [OsirionCookieConsent](/docs/components/core/popups/cookie-consent) | Cookie banner with accept, decline and per-category choices; analytics trackers can wait for its consent. |
| [OsirionAnnouncementBar](/docs/components/core/popups/announcement-bar) | Site-wide announcement that visitors can close; remembered per announcement ID. Available from 4.1. |

## Setup for both

```csharp
using Osirion.Blazor.Core.Extensions;

builder.Services.AddOsirionCookieConsent();

var app = builder.Build();

app.UseAntiforgery();
app.MapOsirionCookieConsent();  // POST /api/cookie-consent
app.MapOsirionAnnouncements();  // POST /api/announcement/dismiss (4.1)
```

```razor
@using Osirion.Blazor.Components

<OsirionAnnouncementBar Id="release-4-1">
    Osirion.Blazor 4.1 is out. <a href="/blog">Read what's new</a>
</OsirionAnnouncementBar>

@* page content *@

<OsirionCookieConsent PolicyLink="/privacy" />
```

Both components read cookies from the current HTTP request, so put them in a statically rendered layout. In an
interactive render without a request they render nothing.

## Related

- [Layout](/docs/components/core/layout)
- [Analytics](/docs/components/analytics)
