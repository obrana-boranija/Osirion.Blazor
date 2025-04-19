# Osirion.Blazor

![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor)
![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)

Modern, high-performance Blazor components and utilities that work with SSR, Server, and WebAssembly hosting models.

## Features

- 🚀 SSR Compatible (works with Server-Side Rendering)
- 🔒 Zero-JS Dependencies for core functionality
- 🎯 Multi-Platform (.NET 8, .NET 9+)
- 📊 Analytics Integration (Microsoft Clarity, Matomo)
- 🧭 Enhanced Navigation Support

## Installation

```bash
dotnet add package Osirion.Blazor
```

## Getting Started

1. Add to your `_Imports.razor`:
```razor
@using Osirion.Blazor.Components.Navigation
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options
```

2. Configure services in `Program.cs`:
```csharp
using Osirion.Blazor.Extensions;

// Basic setup
builder.Services.AddOsirionBlazor();

// Analytics (choose your preferred method)

// Option 1: Configuration-based
builder.Services.AddClarityTracker(builder.Configuration);
builder.Services.AddMatomoTracker(builder.Configuration);

// Option 2: Programmatic
builder.Services.AddClarityTracker(options =>
{
    options.TrackerUrl = "https://www.clarity.ms/tag/";
    options.SiteId = "your-site-id";
    options.Track = true;
});
```

3. Add configuration to `appsettings.json` (if using configuration-based):
```json
{
  "Clarity": {
    "TrackerUrl": "https://www.clarity.ms/tag/",
    "SiteId": "your-site-id",
    "Track": true
  },
  "Matomo": {
    "TrackerUrl": "//analytics.example.com/",
    "SiteId": "1",
    "Track": true
  }
}
```

4. Add components to your layout (`MainLayout.razor` or `App.razor`):
```razor
@inherits LayoutComponentBase
@inject IOptions<ClarityOptions>? ClarityOptions
@inject IOptions<MatomoOptions>? MatomoOptions

<!-- Enhanced navigation with scroll behavior -->
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />

<!-- Analytics tracking -->
@if (ClarityOptions?.Value != null)
{
    <ClarityTracker Options="@ClarityOptions.Value" />
}

@if (MatomoOptions?.Value != null)
{
    <MatomoTracker Options="@MatomoOptions.Value" />
}

<div class="page">
    @Body
</div>
```

## Components

### Navigation

**EnhancedNavigationInterceptor** - Automatically scrolls to top after navigation:
```razor
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

Options: `ScrollBehavior.Auto`, `ScrollBehavior.Instant`, `ScrollBehavior.Smooth`

### Analytics

**ClarityTracker** - Microsoft Clarity integration:
```razor
<ClarityTracker Options="@clarityOptions" />
```

**MatomoTracker** - Matomo analytics integration:
```razor
<MatomoTracker Options="@matomoOptions" />
```

## Documentation

- [Navigation Components](./docs/NAVIGATION.md)
- [Analytics Components](./docs/ANALYTICS.md)
- [Examples](./examples/)

## License

MIT License - see [LICENSE](LICENSE.txt)