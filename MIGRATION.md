# Migration Guide

## Upgrading to v1.2.0

### Breaking Changes

None. Version 1.2.0 is fully backward compatible with v1.1.0.

### New Features

This version introduces analytics components:

1. **ClarityTracker**: Microsoft Clarity integration
2. **MatomoTracker**: Matomo analytics integration
3. **AnalyticsServiceCollectionExtensions**: DI configuration helpers

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.2.0
   ```

2. Add new using statements to `_Imports.razor`:
   ```razor
   @using Osirion.Blazor.Components.Analytics
   @using Osirion.Blazor.Components.Analytics.Options
   ```

3. Configure analytics services in `Program.cs` (optional):
   ```csharp
   builder.Services.AddClarityTracker(builder.Configuration);
   builder.Services.AddMatomoTracker(builder.Configuration);
   ```

4. Add analytics components to your layout:
   ```razor
   @inject IOptions<ClarityOptions>? ClarityOptions
   @inject IOptions<MatomoOptions>? MatomoOptions

   @if (ClarityOptions?.Value != null)
   {
       <ClarityTracker Options="@ClarityOptions.Value" />
   }

   @if (MatomoOptions?.Value != null)
   {
       <MatomoTracker Options="@MatomoOptions.Value" />
   }
   ```

### Configuration Options

For analytics, you can now use either configuration-based or programmatic setup:

#### Configuration-based (appsettings.json)
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

#### Programmatic
```csharp
builder.Services.AddClarityTracker(options =>
{
    options.TrackerUrl = "https://www.clarity.ms/tag/";
    options.SiteId = "your-site-id";
    options.Track = true;
});
```

## Upgrading from v1.0.0 to v1.1.0

### Breaking Changes

None. Version 1.1.0 is fully backward compatible with v1.0.0.

### New Features

This version introduces the EnhancedNavigationInterceptor component.

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.1.0
   ```

2. Add the navigation component to your layout:
   ```razor
   <EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
   ```

## Common Issues

### Analytics Components Not Rendering

Ensure that:
1. Configuration is properly set up in appsettings.json or Program.cs
2. All required properties (TrackerUrl, SiteId) are provided
3. Track property is set to true
4. Components are properly injected in your layout

### EnhancedNavigationInterceptor Not Working

Verify that:
1. You're using Blazor with enhanced navigation enabled
2. The component is placed in your main layout
3. Only one instance of the component exists in your application