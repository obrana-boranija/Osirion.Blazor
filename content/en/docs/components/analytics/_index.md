---
id: 'analytics-components-overview'
order: 1
layout: docs
title: Analytics
permalink: /docs/components/analytics
description: Add Clarity, Matomo, Google Analytics 4 or Yandex Metrica to a Blazor site from configuration, with an optional cookie consent gate.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Analytics Components
- Performance Tracking
tags:
- blazor
- analytics
- google-analytics
- microsoft-clarity
- matomo
- yandex-metrica
- plausible
- umami
- cookie-consent
is_featured: true
published: true
slug: analytics
lang: en
custom_fields: {}
seo_properties:
  title: 'Analytics Trackers for Blazor - Osirion.Blazor'
  description: 'Render Clarity, Matomo, GA4 and Yandex Metrica tracking scripts from configuration in Blazor, with an optional consent gate.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/analytics'
  lang: en
  robots: index, follow
  og_title: 'Analytics Trackers for Blazor - Osirion.Blazor'
  og_description: 'Tracker components for Clarity, Matomo, GA4 and Yandex Metrica, configured from appsettings.json and gated on consent.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Analytics Trackers for Blazor - Osirion.Blazor'
  twitter_description: 'Tracker components for Clarity, Matomo, GA4 and Yandex Metrica, configured from appsettings.json.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# Analytics

`Osirion.Blazor.Analytics` writes the tracking script of an analytics service into your pages. You configure the
service once, then place its tracker component in the page head. Trackers render on the server, so they work under
Static SSR with no interactive render mode.

| Service | Tracker component | Registration | Configuration section |
| --- | --- | --- | --- |
| Microsoft Clarity | `ClarityTracker` | `AddClarity` | `Osirion:Analytics:Clarity` |
| Matomo | `MatomoTracker` | `AddMatomo` | `Osirion:Analytics:Matomo` |
| Google Analytics 4 | `GA4Tracker` | `AddGA4` | `Osirion:Analytics:GA4` |
| Yandex Metrica | `YandexMetricaTracker` | `AddYandexMetrica` | `Osirion:Analytics:YandexMetrica` |
| Plausible | `PlausibleTracker` | `AddPlausible` | `Osirion:Analytics:Plausible` |
| Umami | `UmamiTracker` | `AddUmami` | `Osirion:Analytics:Umami` |

Plausible and Umami are available from 4.1.

## Usage

Install the package, or the `Osirion.Blazor` meta-package, which includes it:

```bash
dotnet add package Osirion.Blazor.Analytics
```

Register the services you use in `Program.cs`:

```csharp
using Osirion.Blazor.Analytics.Extensions;

builder.Services.AddOsirionAnalytics(analytics => analytics
    .AddGA4(options => options.MeasurementId = "G-XXXXXXXXXX")
    .AddClarity(options => options.SiteId = "your-clarity-project-id"));
```

Then render the trackers in the `<head>` of `App.razor`:

```razor
@using Osirion.Blazor.Analytics.Components

<head>
    <meta charset="utf-8" />
    <base href="/" />
    <HeadOutlet />
    <GA4Tracker />
    <ClarityTracker />
</head>
```

### From configuration

`AddOsirionAnalytics(builder.Configuration)` registers every service whose section exists under
`Osirion:Analytics`. `AddOsirion(builder.Configuration)` from the meta-package does the same.

```json
{
  "Osirion": {
    "Analytics": {
      "GA4": { "MeasurementId": "G-XXXXXXXXXX" },
      "Matomo": { "SiteId": "1", "TrackerUrl": "https://analytics.example.com/" }
    }
  }
}
```

```csharp
builder.Services.AddOsirionAnalytics(builder.Configuration);
```

To keep tracking off while you develop, set `"Enabled": false` for the service in `appsettings.Development.json`.

> [!WARNING]
> In 4.0.0, a tracker whose service is not registered fails the page. Render only the trackers you configured.
> From 4.1 such a tracker renders nothing.

## Parameters

Tracker parameters override the configured options for that render. A parameter you leave unset keeps the configured
value.

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Enabled` | `bool?` | `null` | Every tracker. `false` turns the tracker off for this render. |
| `SiteId` | `string?` | `null` | `ClarityTracker`, `MatomoTracker` and `UmamiTracker`: the site or website ID. |
| `MeasurementId` | `string?` | `null` | `GA4Tracker`: the measurement ID (`G-XXXXXXXXXX`). |
| `DebugMode` | `bool?` | `null` | `GA4Tracker`: turns on GA4 debug mode. |
| `CounterId` | `string?` | `null` | `YandexMetricaTracker`: the numeric counter ID. |
| `WebVisor` | `bool?` | `null` | `YandexMetricaTracker`: session replay. |
| `ClickMap` | `bool?` | `null` | `YandexMetricaTracker`: click map. |
| `TrackLinks` | `bool?` | `null` | `YandexMetricaTracker`: outbound link clicks. |
| `AccurateTrackBounce` | `bool?` | `null` | `YandexMetricaTracker`: accurate bounce rate. |

## Options

Every service has `Enabled` (default `true`) and `AutoTrackPageViews` (default `true`). The main options per service:

- **Clarity**: `SiteId` (the project ID), `TrackerUrl` (default `https://www.clarity.ms/tag/`).
- **Matomo**: `SiteId`, `TrackerUrl` (your Matomo host, required), `TrackLinks` (default `true`), `RequireConsent`
  (default `false`; Matomo's own consent mode, which waits until your page pushes `setConsentGiven`).
- **GA4**: `MeasurementId`, `AnonymizeIp` (default `true`), `SendPageView` (default `true`), `TrackOutboundLinks`
  (default `true`), `DebugMode`, `CookieDomain`, `CookieExpires`, `CookieFlags`, `RestrictDataProcessing`.
- **Yandex Metrica**: `CounterId`, `WebVisor` (default `false`), `ClickMap` (default `true`), `TrackLinks`
  (default `true`), `AccurateTrackBounce` (default `true`), `AlternativeCdn`, `EcommerceEnabled`.
- **Plausible**: `ScriptUrl` (your site's script from the Plausible site settings, required), `Endpoint`, and the
  switches `OutboundLinks`, `FileDownloads`, `FormSubmissions`, `HashBasedRouting` and `CaptureOnLocalhost`, written
  only when set so the dashboard settings apply otherwise.
- **Umami**: `SiteId` (the website ID), `ScriptUrl` (default Umami Cloud; set it for a self-hosted instance),
  `HostUrl`, `Domains`, `Tag`, `ExcludeSearch`, `ExcludeHash`, `DoNotTrack`.

The full list is in [ANALYTICS.md](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/ANALYTICS.md).

## Behavior

### Consent gate

The gate is off by default. Turn it on and no tracker script is rendered until the visitor accepted the `analytics`
category in [`OsirionCookieConsent`](/docs/components/core/popups/cookie-consent):

```csharp
builder.Services.AddOsirionAnalytics(analytics => analytics
    .AddGA4(options => options.MeasurementId = "G-XXXXXXXXXX")
    .RequireConsent());
```

or in configuration:

```json
{ "Osirion": { "Analytics": { "Consent": { "RequireConsent": true, "Category": "analytics" } } } }
```

Pass a configure action, such as `RequireConsent(options => options.Category = "...")`, to check a different
category. Consent is read from the
consent cookie on the server. The check fails closed: with the gate on and no `HttpContext`, for example inside a
component that already runs interactively, the tracker renders nothing. That is why trackers belong in `App.razor`.

With the gate on, the GA4 script first sets Google Consent Mode defaults to denied and then grants
`analytics_storage` only. Advertising signals stay denied. Plausible and Umami set no cookies; whether you still ask
for consent is your decision, and the gate covers them like the other trackers.

### Page views after enhanced navigation

Blazor enhanced navigation patches the page without a full load, so some scripts never see the new page. Matomo and
Yandex Metrica trackers add a static script (`_content/Osirion.Blazor.Analytics/js/osirion-analytics.js`) that
listens for Blazor's `enhancedload` event and reports the new URL. GA4 relies on its Enhanced Measurement setting
"Page changes based on browser history events". Clarity, Plausible and Umami follow history changes themselves. Set
`AutoTrackPageViews` to `false` to leave the page view script out. Navigation inside a router that already runs
interactively does not raise `enhancedload` and is not reported.

### Security

Configured values are JSON or attribute encoded before they are written into scripts and attributes. Formats are
checked at startup for every enabled service: a GA4 measurement ID must look like `G-XXXXXXXXXX`, a Yandex counter
ID must be numeric, an Umami website ID must be a GUID, and script and tracker URLs must be http(s) or
protocol-relative URLs (Plausible and Umami also accept a site path such as `/js/script.js`; the Yandex
`AlternativeCdn` must be an absolute http(s) URL). A bad value stops the app at startup with a message that names the
configuration key.

The Clarity, Matomo, GA4, Yandex Metrica and Plausible trackers write inline scripts and do not add a nonce, so a
Content Security Policy must allow them. Umami renders only an external script tag.

## Related

- [Cookie consent](/docs/components/core/popups/cookie-consent): the banner that records the visitor's choice.
- [Navigation](/docs/components/navigation): enhanced navigation and scroll behavior.
- [Osirion.Blazor.Analytics on NuGet](https://www.nuget.org/packages/Osirion.Blazor.Analytics)
