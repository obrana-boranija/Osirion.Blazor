# Analytics Components

Osirion.Blazor provides SSR-compatible analytics components that work with popular analytics platforms. These components are designed to be simple to use, configurable, and secure.

## Features

- **SSR Compatible**: Works with Server-Side Rendering and Static SSR
- **No JavaScript Interop**: Pure Blazor implementation without JS dependencies
- **Configuration-Driven**: Configure via DI container or directly through parameters
- **Type-Safe**: Strongly-typed options with intellisense support

## Supported Analytics Platforms

### Microsoft Clarity

Microsoft Clarity is a user behavior analytics tool that helps you understand how users interact with your website.

#### Usage

```razor
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options

<!-- Using with parameters -->
<ClarityTracker Options="@(new ClarityOptions 
{ 
    TrackerUrl = "https://www.clarity.ms/tag/", 
    SiteId = "your-site-id", 
    Track = true 
})" />

<!-- Using with injected options -->
@inject IOptions<ClarityOptions> ClarityOptions
<ClarityTracker Options="@ClarityOptions.Value" />
```

#### Configuration

```csharp
// In Program.cs
builder.Services.AddClarityTracker(builder.Configuration);

// Or programmatically
builder.Services.AddClarityTracker(options =>
{
    options.TrackerUrl = "https://www.clarity.ms/tag/";
    options.SiteId = "your-site-id";
    options.Track = true;
});
```

```json
// In appsettings.json
{
  "Clarity": {
    "TrackerUrl": "https://www.clarity.ms/tag/",
    "SiteId": "your-site-id",
    "Track": true
  }
}
```

### Matomo

Matomo (formerly Piwik) is an open-source analytics platform that gives you full control over your data.

#### Usage

```razor
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options

<!-- Using with parameters -->
<MatomoTracker Options="@(new MatomoOptions 
{ 
    TrackerUrl = "//analytics.example.com/", 
    SiteId = "1", 
    Track = true 
})" />

<!-- Using with injected options -->
@inject IOptions<MatomoOptions> MatomoOptions
<MatomoTracker Options="@MatomoOptions.Value" />
```

#### Configuration

```csharp
// In Program.cs
builder.Services.AddMatomoTracker(builder.Configuration);

// Or programmatically
builder.Services.AddMatomoTracker(options =>
{
    options.TrackerUrl = "//analytics.example.com/";
    options.SiteId = "1";
    options.Track = true;
});
```

```json
// In appsettings.json
{
  "Matomo": {
    "TrackerUrl": "//analytics.example.com/",
    "SiteId": "1",
    "Track": true
  }
}
```

## Common Options

Both analytics components share common properties:

- `TrackerUrl` (string): The URL of the analytics script
- `SiteId` (string): Your site ID for the analytics platform
- `Track` (bool): Controls whether tracking is enabled

## Best Practices

1. **Environment-Specific Configuration**: Use different configuration values for development, staging, and production
2. **Security**: Ensure tracker URLs are from trusted sources
3. **Performance**: Place analytics components at the bottom of your layout to avoid blocking critical content
4. **Privacy**: Consider user consent before enabling tracking
5. **Conditional Rendering**: Use the `Track` property to conditionally enable analytics based on user preferences or environment

## Example: Using with Environment Configuration

```csharp
// Program.cs
var environment = builder.Environment;

if (environment.IsProduction())
{
    builder.Services.AddClarityTracker(builder.Configuration);
    builder.Services.AddMatomoTracker(builder.Configuration);
}
else
{
    // Disable tracking in non-production environments
    builder.Services.AddClarityTracker(options => options.Track = false);
    builder.Services.AddMatomoTracker(options => options.Track = false);
}
```

## Example: Using with Feature Flags

```razor
@inject IFeatureManager FeatureManager

@if (await FeatureManager.IsEnabledAsync("Analytics"))
{
    <ClarityTracker Options="@clarityOptions" />
    <MatomoTracker Options="@matomoOptions" />
}
```

## Troubleshooting

1. **Components not rendering**: Check that all required options (`TrackerUrl`, `SiteId`) are set and `Track` is true
2. **Analytics not capturing data**: Verify that the tracker URLs are correct and accessible
3. **Configuration not working**: Ensure configuration section names match (`Clarity`, `Matomo`) in your configuration source