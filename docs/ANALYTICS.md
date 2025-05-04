# Osirion.Blazor.Analytics

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Analytics)](https://www.nuget.org/packages/Osirion.Blazor.Analytics)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor.Analytics)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Analytics integration for Blazor applications, supporting multiple providers with SSR compatibility.

## Features

- **Multiple Providers**: Microsoft Clarity and Matomo support out of the box
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
builder.Services.AddOsirionAnalytics(builder.Configuration);

// Or when using the full Osirion.Blazor package:
builder.Services.AddOsirion(builder.Configuration);
```

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

## Configuration Options

### Clarity Options

- `SiteId`: Your Microsoft Clarity site ID
- `Enabled`: Whether the tracker is enabled (default: true)
- `TrackerUrl`: The URL for the Clarity script (default: "https://www.clarity.ms/tag/")
- `TrackUserAttributes`: Whether to track user attributes (default: true)
- `AutoTrackPageViews`: Whether to automatically track page views (default: true)

### Matomo Options

- `SiteId`: Your Matomo site ID
- `Enabled`: Whether the tracker is enabled (default: true)
- `TrackerUrl`: The URL to your Matomo installation
- `TrackLinks`: Whether to track link clicks (default: true)
- `TrackDownloads`: Whether to track downloads (default: true)
- `RequireConsent`: Whether to require cookie consent (default: false)
- `AutoTrackPageViews`: Whether to automatically track page views (default: true)

## Integration with Privacy Modules

Osirion.Blazor.Analytics can integrate with privacy consent modules to provide GDPR-compliant analytics:

```csharp
builder.Services.AddOsirionAnalytics(analytics => {
    analytics.AddMatomo(options => {
        options.RequireConsent = true;
    });
});

builder.Services.AddOsirionPrivacy();
```

## SSR Considerations

The analytics components are designed to work seamlessly in Server-Side Rendering (SSR) environments:

- Scripts are only rendered when needed
- No JavaScript interop is required for basic functionality
- Components properly handle non-interactive rendering scenarios

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.