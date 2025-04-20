# Quick Reference

## Navigation

### Basic Usage
```razor
<EnhancedNavigationInterceptor />
```

### With Options
```razor
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

### Scroll Behavior Options
- `ScrollBehavior.Auto` - Browser default
- `ScrollBehavior.Instant` - Immediate scroll
- `ScrollBehavior.Smooth` - Animated scroll

## Analytics

### Microsoft Clarity

#### With DI
```csharp
// Program.cs
builder.Services.AddClarityTracker(options =>
{
    options.TrackerUrl = "https://www.clarity.ms/tag/";
    options.SiteId = "your-site-id";
    options.Track = true;
});
```

```razor
@inject IOptions<ClarityOptions> ClarityOptions
<ClarityTracker Options="@ClarityOptions.Value" />
```

#### Without DI
```razor
<ClarityTracker Options="@(new ClarityOptions 
{ 
    TrackerUrl = "https://www.clarity.ms/tag/", 
    SiteId = "your-site-id", 
    Track = true 
})" />
```

### Matomo

#### With DI
```csharp
// Program.cs
builder.Services.AddMatomoTracker(options =>
{
    options.TrackerUrl = "//analytics.example.com/";
    options.SiteId = "1";
    options.Track = true;
});
```

```razor
@inject IOptions<MatomoOptions> MatomoOptions
<MatomoTracker Options="@MatomoOptions.Value" />
```

#### Without DI
```razor
<MatomoTracker Options="@(new MatomoOptions 
{ 
    TrackerUrl = "//analytics.example.com/", 
    SiteId = "1", 
    Track = true 
})" />
```

## Complete Layout Example

```razor
@inherits LayoutComponentBase
@using Osirion.Blazor.Components.Navigation
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options
@inject IOptions<ClarityOptions>? ClarityOptions
@inject IOptions<MatomoOptions>? MatomoOptions

<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />

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

## Configuration Example

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

## Environment-Based Configuration

```csharp
if (builder.Environment.IsProduction())
{
    builder.Services.AddClarityTracker(builder.Configuration);
    builder.Services.AddMatomoTracker(builder.Configuration);
}
else
{
    builder.Services.AddClarityTracker(options => options.Track = false);
    builder.Services.AddMatomoTracker(options => options.Track = false);
}
```

## Best Practices

1. Place navigation and analytics components in your main layout
2. Use configuration-based setup for production
3. Disable tracking in development environments
4. Use null checks when working with injected options
5. Consider user privacy and consent requirements