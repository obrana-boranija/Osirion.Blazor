---
id: 'osirion-announcement-bar'
order: 2
layout: docs
title: OsirionAnnouncementBar
permalink: /docs/components/core/popups/announcement-bar
description: A site-wide announcement above the page that visitors can close without script; the dismissal is remembered in a cookie per announcement.
author: Dejan Demonjić
date: 2026-10-11
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Popups
tags:
- blazor
- announcement
- banner
- static-ssr
- antiforgery
- accessibility
is_featured: true
published: true
slug: announcement-bar
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionAnnouncementBar: dismissible site announcement for Blazor | Osirion.Blazor'
  description: 'A site-wide Blazor announcement bar that visitors can close without script, remembered per announcement in a cookie.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/popups/announcement-bar'
  lang: en
  robots: index, follow
  og_title: 'OsirionAnnouncementBar - Osirion.Blazor'
  og_description: 'A dismissible site-wide announcement that closes without script.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionAnnouncementBar - Osirion.Blazor'
  twitter_description: 'A dismissible site-wide announcement that closes without script.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionAnnouncementBar

`OsirionAnnouncementBar` shows a one-line announcement across the top of every page, such as a release or an event,
with a close button. Closing posts a small form; an endpoint in your app remembers the choice in a cookie and
redirects back, so it works under static SSR without script.

Available from 4.1.

## Setup

Map the dismissal endpoint in `Program.cs`:

```csharp
using Osirion.Blazor.Core.Extensions;

var app = builder.Build();

app.UseAntiforgery();
app.MapOsirionAnnouncements(); // POST /api/announcement/dismiss
```

The endpoint requires an antiforgery token, so `app.UseAntiforgery()` must run before the endpoints, as in the
Blazor Web App template.

## Usage

Put the bar at the top of a statically rendered layout:

```razor
@using Osirion.Blazor.Components

<OsirionAnnouncementBar Id="release-4-1">
    Osirion.Blazor 4.1 is out. <a href="/blog">Read what's new</a>
</OsirionAnnouncementBar>
```

With [OsirionPageLayout](/docs/components/core/layout/page-layout), place it in the `Header` slot above your menu,
so it scrolls away with the header:

```razor
<OsirionPageLayout>
    <Header>
        <OsirionAnnouncementBar Id="docs-survey" RememberDays="7" RegionLabel="Site notice">
            Tell us what is missing from the docs. <a href="/survey">Take the two-minute survey</a>
        </OsirionAnnouncementBar>
        <NavMenu />
    </Header>
    <Body>
        @Body
    </Body>
</OsirionPageLayout>
```

To announce something new, change `Id`. Visitors who closed the previous announcement then see the new one.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Id` | `string` | empty | Required. 1 to 40 lowercase letters, digits or hyphens, such as `release-4-1`. A dismissal is remembered per ID. |
| `ChildContent` | `RenderFragment?` | `null` | The announcement; it may contain links. |
| `Dismissible` | `bool` | `true` | Shows the close button. |
| `RememberDays` | `int` | `30` | How long a dismissal lasts, 1 to 365 days. |
| `DismissText` | `string` | `"Dismiss announcement"` | Accessible name of the close button. |
| `RegionLabel` | `string` | `"Announcement"` | Accessible name of the region. |
| `DismissEndpoint` | `string` | `"/api/announcement/dismiss"` | Where the close form posts; match the pattern given to `MapOsirionAnnouncements`. |

`Class` and unmatched attributes go on the bar element.

An invalid `Id` throws `ArgumentException`, and a `RememberDays` outside 1 to 365 throws
`ArgumentOutOfRangeException`, so a typo fails during development instead of writing a bad cookie name.

## Behavior

- **Closing without script.** The close button submits a form with the ID, the lifetime and the current path. The
  endpoint sets the cookie `osirion_announcement_{Id}` and redirects back; the next render leaves the bar out.
- **The cookie.** `HttpOnly`, `SameSite=Lax`, path `/`, and `Secure` when the request is HTTPS, so local development
  over HTTP also works.
- **Security.** The form carries an antiforgery token. A post without a valid token or with an invalid ID gets 400,
  the lifetime is bounded to 365 days, and the endpoint redirects only to local paths.
- **Rendering.** The bar reads the cookie from the request, so it renders under static SSR and while prerendering.
  Without an HTTP request (an interactive render) it renders nothing rather than show a dismissed announcement again,
  so keep it in a statically rendered layout.
- **Accessibility.** The bar is a region named by `RegionLabel`. The close button has an accessible name from
  `DismissText` and a visible focus outline; its "x" glyph is hidden from assistive technology.
- **Content.** `ChildContent` is a normal Razor fragment and is encoded like any other Razor output.

## Styling

| Class | Element |
| --- | --- |
| `osirion-announcement` | The bar: a centered flex row. |
| `osirion-announcement-content` | The announcement text; links in it inherit the bar's color and are underlined. |
| `osirion-announcement-form` | The close form. |
| `osirion-announcement-dismiss` | The close button. |

The background is `--osirion-action-primary` and the text `--osirion-action-primary-text`, so the bar follows the
theme and the CSS framework adapter. Override them on the bar to use other colors:

```css
.osirion-announcement {
    --osirion-action-primary: #14532d;
    --osirion-action-primary-text: #ffffff;
}
```

## Related

- [OsirionCookieConsent](/docs/components/core/popups/cookie-consent)
- [OsirionPageLayout](/docs/components/core/layout/page-layout)
- [Popups overview](/docs/components/core/popups)
