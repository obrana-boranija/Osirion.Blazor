# Osirion.Blazor.Analytics

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Analytics)](https://www.nuget.org/packages/Osirion.Blazor.Analytics)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Analytics integration for Blazor applications, supporting multiple providers with SSR compatibility.

## Features

- **Multiple Providers**: Microsoft Clarity, Matomo, Google Analytics 4, Yandex Metrica, Plausible and Umami out of the box
- **Provider Pattern**: Easily extend with your own analytics providers
- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Configuration-Driven**: Simple setup through dependency injection
- **Privacy-Focused**: Opt-in tracking with consent options

## Installation

```bash
dotnet add package Osirion.Blazor.Analytics
```

## Usage

### Quick Start

```csharp
// In Program.cs
using Osirion.Blazor.Analytics.Extensions;

builder.Services.AddOsirionAnalytics(analytics => {
    analytics
        .AddClarity(options => {
            options.SiteId = "your-clarity-site-id";
        })
        .AddMatomo(options => {
            options.SiteId = "1";
            options.TrackerUrl = "//analytics.example.com/";
        });
});
```

```razor
@using Osirion.Blazor.Analytics.Components

<!-- In your layout -->
<ClarityTracker />
<MatomoTracker />
```

### With Configuration

```json
// In appsettings.json
{
  "Osirion": {
    "Analytics": {
      "Clarity": {
        "SiteId": "your-clarity-site-id",
        "Enabled": true
      },
      "Matomo": {
        "SiteId": "1",
        "TrackerUrl": "//analytics.example.com/",
        "TrackLinks": true,
        "RequireConsent": false
      }
    }
  }
}
```

```csharp
// In Program.cs
builder.Services.AddOsirion(builder.Configuration);
```

### Consent (recommended for EU sites)

By default trackers render as soon as they are configured. Turn on the consent gate and no tracker
script is rendered until the visitor accepted the `analytics` category in `OsirionCookieConsent`:

```csharp
builder.Services.AddOsirionAnalytics(analytics => {
    analytics.AddGA4(o => o.MeasurementId = "G-XXXXXXXXXX").RequireConsent();
});
```

```json
{ "Osirion": { "Analytics": { "Consent": { "RequireConsent": true, "Category": "analytics" } } } }
```

Consent is read from the consent cookie on the server. When the gate is on and there is no `HttpContext`
(for example in an already interactive component) nothing renders. With GA4 the script also declares
Google Consent Mode defaults (all denied) before `config` and then grants `analytics_storage`; advertising
signals stay denied. Matomo's own `RequireConsent` option is not needed with the gate; without the gate it
keeps Matomo silent until your page pushes `['setConsentGiven']` or `['rememberConsentGiven']` onto `_paq`.

### Plausible and Umami

Both are cookie-free. Plausible gives every site its own script (Site installation in the site settings);
Umami needs the website ID, and the script URL for a self-hosted instance.

```csharp
builder.Services.AddOsirionAnalytics(analytics => {
    analytics
        .AddPlausible(o => o.ScriptUrl = "https://plausible.io/js/pa-XXXXX.js")
        .AddUmami(o => o.SiteId = "94db1cb1-74f4-4a40-ad6c-962362670409")
        .RequireConsent();
});
```

```razor
<head>
    ...
    <PlausibleTracker />
    <UmamiTracker />
</head>
```

```json
{
  "Osirion": {
    "Analytics": {
      "Plausible": { "ScriptUrl": "https://plausible.io/js/pa-XXXXX.js" },
      "Umami": { "SiteId": "94db1cb1-74f4-4a40-ad6c-962362670409", "ScriptUrl": "https://umami.example.com/script.js" }
    }
  }
}
```

The Plausible switches (`OutboundLinks`, `FileDownloads`, `FormSubmissions`, `HashBasedRouting`,
`CaptureOnLocalhost`) are written only when set, so the settings in your Plausible dashboard apply otherwise.
Plausible's snippet includes a short inline script that defines `plausible.init`; Umami is a single external
script configured by attributes. Both scripts follow `history.pushState`, so enhanced navigations are counted
without the page view module. These trackers do not set cookies; whether your site still asks for analytics
consent is your call, and the consent gate covers them like every other tracker.

### Tracker parameters

Every tracker accepts `Enabled` to override the configured option for that render. `ClarityTracker`,
`MatomoTracker` and `UmamiTracker` accept `SiteId`; `GA4Tracker` accepts `MeasurementId` and `DebugMode`;
`YandexMetricaTracker` accepts `CounterId`, `WebVisor`, `ClickMap`, `TrackLinks` and `AccurateTrackBounce`.

### Configuration is validated

Values are JSON/attribute encoded into the markup, and formats are checked at startup: GA4 measurement ID
(`G-XXXXXXXXXX`), numeric Yandex counter ID, http(s) tracker and CDN URLs. A bad value fails with a message
naming the configuration key; a tracker whose values are invalid renders nothing.

### Page views after enhanced navigation

Matomo and Yandex Metrica trackers add a static script (`_content/Osirion.Blazor.Analytics/js/osirion-analytics.js`,
no inline script) that listens for Blazor's `enhancedload` event and reports the new URL. GA4 relies on its
Enhanced Measurement setting "Page changes based on browser history events". Clarity, Plausible and Umami follow
history changes themselves. Set `AutoTrackPageViews = false` to leave the module out. Navigation inside an already interactive
router does not raise `enhancedload` and is not reported yet.

### Manual Tracking

```csharp
@inject IAnalyticsService Analytics

// Track a page view
await Analytics.TrackPageViewAsync("/custom-path");

// Track an event
await Analytics.TrackEventAsync("category", "action", "label", value);
```

### Creating Custom Providers

```csharp
public class CustomProvider : IAnalyticsProvider
{
    public string ProviderId => "custom";
    public bool IsEnabled => true;
    public bool ShouldRender => true;

    public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        // Custom implementation
        return Task.CompletedTask;
    }

    public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        // Custom implementation
        return Task.CompletedTask;
    }

    public string GetScript()
    {
        return "<script>/* Custom tracking script */</script>";
    }
}

// Register the provider
builder.Services.AddOsirionAnalytics(analytics => {
    analytics.AddProvider<CustomProvider>();
});
```

## Documentation

For more detailed documentation, see [Analytics Documentation](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/ANALYTICS.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.