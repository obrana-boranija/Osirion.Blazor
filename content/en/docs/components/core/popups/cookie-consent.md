---
id: 'cookie-consent'
order: 1
layout: docs
title: OsirionCookieConsent
permalink: /docs/components/core/popups/cookie-consent
description: A cookie consent banner that works without script, records the visitor's choice per category in a cookie and can gate analytics trackers.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Popups
- Privacy
- GDPR
tags:
- blazor
- cookie-consent
- privacy
- gdpr
- static-ssr
- antiforgery
is_featured: true
published: true
slug: cookie-consent
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionCookieConsent: cookie banner for Blazor static SSR | Osirion.Blazor'
  description: 'A Blazor cookie consent banner that works without script, stores consent per category and can gate analytics trackers.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/popups/cookie-consent'
  lang: en
  robots: index, follow
  og_title: 'OsirionCookieConsent - Osirion.Blazor'
  og_description: 'Cookie consent banner without script, with categories and an analytics consent gate.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionCookieConsent - Osirion.Blazor'
  twitter_description: 'Cookie consent banner for Blazor that works without script.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionCookieConsent

`OsirionCookieConsent` shows a cookie banner until the visitor accepts, declines or saves their own choice. The
buttons post a form to an endpoint in your app, which stores the decision in a cookie and redirects back, so the
banner works under static SSR without script. Analytics trackers can then wait for that consent.

## Setup

Register the services and map the endpoint in `Program.cs`:

```csharp
using Osirion.Blazor.Core.Extensions;

builder.Services.AddOsirionCookieConsent();

var app = builder.Build();

app.UseAntiforgery();
app.MapOsirionCookieConsent(); // POST /api/cookie-consent
```

`AddOsirionCookieConsent` registers `IHttpContextAccessor` and the antiforgery services. The endpoint requires an
antiforgery token, so `app.UseAntiforgery()` must run before the endpoints, as in the Blazor Web App template.

Then place the banner in a statically rendered layout:

```razor
@using Osirion.Blazor.Components

<OsirionCookieConsent PolicyLink="/privacy" />
```

## Usage

A banner with your own text and categories:

```razor
<OsirionCookieConsent Title="Cookies on this site"
                      Message="We use a cookie to remember your choice and, if you agree, privacy-friendly statistics."
                      PolicyLink="/privacy"
                      PolicyLinkText="Privacy policy"
                      ShowCustomizeButton="true"
                      Categories="categories"
                      ConsentExpiryDays="180" />

@code {
    private readonly IReadOnlyList<CookieCategory> categories =
    [
        new CookieCategory { Id = "necessary", Name = "Necessary", Description = "Keeps the site working.", IsRequired = true, IsEnabled = true },
        new CookieCategory { Id = "analytics", Name = "Statistics", Description = "Counts visits so we can improve the docs." }
    ];
}
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Title` | `string` | `"Cookie Consent"` | Banner heading (`h3`) and the region's accessible name. |
| `Message` | `string` | default notice | Banner text, rendered as encoded text. |
| `PolicyLink` | `string?` | `null` | Address of your privacy or cookie policy; no link when empty. |
| `PolicyLinkText` | `string` | `"Learn more"` | Text of that link. |
| `AcceptButtonText` | `string` | `"Accept All"` | Accept button text. |
| `DeclineButtonText` | `string` | `"Decline"` | Decline button text. |
| `ShowDeclineButton` | `bool` | `true` | Shows the decline button. |
| `CustomizeButtonText` | `string` | `"Customize"` | Customize button text. |
| `ShowCustomizeButton` | `bool` | `true` | Shows the customize button. |
| `ShowCustomizationPanel` | `bool` | `true` | Shows the category checkboxes after the visitor chose Customize. |
| `CustomizePanelTitle` | `string` | `"Cookie Preferences"` | Heading (`h4`) of the category panel. |
| `SavePreferencesButtonText` | `string` | `"Save Preferences"` | Button that saves the checked categories. |
| `Categories` | `IReadOnlyList<CookieCategory>` | necessary, analytics, marketing, preferences | Categories in the panel. |
| `Icon` | `RenderFragment?` | `null` | Icon next to the text. |
| `Position` | `string` | `"bottom"` | Adds `osirion-cookie-consent-{Position}`; the stylesheet supports `bottom` and `top`. |
| `ConsentExpiryDays` | `int` | `365` | Lifetime of the consent cookie, 1 to 3650 days. |
| `ConsentEndpoint` | `string` | `"/api/cookie-consent"` | Where the form posts; match the pattern given to `MapOsirionCookieConsent`. |

`CookieCategory` has `Id`, `Name`, `Description`, `IsRequired` (always on, checkbox disabled) and `IsEnabled`
(checked by default). `Class` goes on the banner element; unmatched attributes go on the `form`.

## Behavior

- **Flow without script.** Accept and Decline post the form, the endpoint writes the cookie and redirects to the
  same page, and the banner is no longer rendered. Customize redirects to the page with `?customize-cookies=true`,
  where the banner shows the category checkboxes; Save Preferences records the checked ones.
- **What is recorded.** Accept sets `necessary`, `analytics`, `marketing` and `preferences` to true; Decline sets
  `necessary` to true and the other three to false. Save Preferences records `necessary` plus each category the visitor checked; unchecked ones are left
  out, which counts as no consent. Custom category IDs are therefore recorded only through the panel.
- **The cookie.** `osirion_cookie_consent` holds JSON with `Version`, `ConsentDate`, `ConsentType` and
  `Categories`. It is `HttpOnly`, `Secure` and `SameSite=Lax`, so serve the site over HTTPS. Script cannot read it;
  decisions are made on the server.
- **Security.** The form carries an antiforgery token. A post without a valid token gets 400 and records nothing,
  and the endpoint redirects only to local paths.
- **Rendering.** The component reads the cookie from the current request. Without an HTTP request (an interactive
  render with no `HttpContext`), it renders nothing.
- **Accessibility.** The banner is a region named by its title, so its text is inside a landmark. Available from
  4.1. The buttons are real `button` elements in a form, usable with the keyboard.

## Reading consent on the server

`CookieConsentHandler` in `Osirion.Blazor.Core.Handlers` reads the cookie:

```csharp
using Osirion.Blazor.Core.Handlers;

bool statistics = CookieConsentHandler.IsCategoryConsented(httpContext, "analytics");
CookieConsentData? consent = CookieConsentHandler.GetConsentData(httpContext);
```

`ClearConsent(httpContext)` deletes the cookie, for example on a "change cookie settings" endpoint.
`app.UseOsirionCookieConsent()` adds middleware that puts the parsed data in `HttpContext.Items["CookieConsent"]`.

## Gating analytics

The analytics module can render trackers only after the visitor accepted the `analytics` category. Turn the gate on
in code:

```csharp
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Extensions;

builder.Services.AddOsirion(osirion => osirion
    .UseAnalytics(analytics => analytics
        .AddClarity(clarity => clarity.SiteId = "your-clarity-id")
        .RequireConsent()));
```

or in configuration:

```json
{
  "Osirion": {
    "Analytics": {
      "Consent": { "RequireConsent": true }
    }
  }
}
```

Without consent no tracker script is rendered at all; with GA4, Google Consent Mode starts with its defaults set to
denied. `RequireConsent(options => options.Category = "statistics")` uses another category ID. See
[Analytics](/docs/components/analytics).

## Styling

The form has `osirion-cookie-consent-form`; the banner has `osirion-cookie-consent` and
`osirion-cookie-consent-bottom` or `osirion-cookie-consent-top`, plus `osirion-cookie-consent-customizing` while the
panel is open. Inner parts include `osirion-cookie-consent-title`, `osirion-cookie-consent-description`,
`osirion-cookie-consent-link`, `osirion-cookie-consent-actions`, `osirion-cookie-consent-customize` and
`osirion-cookie-consent-category`. Colors come from the theme tokens (`--osirion-text-primary`,
`--osirion-border-color`, `--osirion-action-primary` and others), and the buttons use the active CSS framework's
classes.

## Related

- [OsirionAnnouncementBar](/docs/components/core/popups/announcement-bar)
- [Popups overview](/docs/components/core/popups)
- [Analytics](/docs/components/analytics)
