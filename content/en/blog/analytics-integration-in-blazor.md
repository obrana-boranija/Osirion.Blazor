---
id: ""
title: Analytics Integration for Blazor Applications
description: How to integrate analytics tools like Microsoft Clarity and Matomo into your Blazor applications using Osirion.Blazor components.
author: Dejan Demonjić
date: 2025-04-12
featured_image: "https://images.pexels.com/photos/97080/pexels-photo-97080.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
categories: 
- Analytics
- Integration
tags: 
- Analytics
- Tracking
- Blazor
- Privacy
is_featured: true
published: true
layout: null
slug: analytics-integration-in-blazor
custom_fields: {}
seo: 
  meta_title: ""
  meta_description: ""
  canonical_url: ""
  robots: "index, follow"
  og_title: ""
  og_description: ""
  og_image_url: ""
  og_type: article
  json_ld: ""
  schema_type: Article
  twitter_card: summary_large_image
  twitter_title: ""
  twitter_description: ""
  twitter_image_url: ""

---
[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Osirion.Blazor.svg)](https://www.nuget.org/packages/Osirion.Blazor)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

# Analytics Integration for Blazor Applications

Adding analytics to your Blazor application can provide valuable insights into user behavior. Osirion.Blazor offers easy integration with popular analytics platforms like Microsoft Clarity and Matomo.

## Why Use Analytics in Blazor?

Analytics tools can help you:

1. Understand user engagement
2. Identify usability issues
3. Track conversion rates
4. Optimize performance
5. Make data-driven decisions

## Available Analytics Components

Osirion.Blazor provides components for two popular analytics platforms:

### Microsoft Clarity

Clarity is Microsoft's free analytics tool that provides heatmaps, session recordings, and insights.

```razor
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options

<ClarityTracker Options="new ClarityOptions { 
    TrackerUrl = 'https://www.clarity.ms/tag/', 
    SiteId = 'your-clarity-id',
    Track = true 
}" />
```

### Matomo (formerly Piwik)

Matomo is an open-source analytics platform that gives you full control over your data.

```razor
<MatomoTracker Options="new MatomoOptions {
    TrackerUrl = '//analytics.example.com/',
    SiteId = '1',
    Track = true
}" />
```

## Configuration Options

Both trackers support the following options:

- `TrackerUrl`: The URL of the analytics script
- `SiteId`: Your site ID for the analytics platform
- `Track`: Controls whether tracking is enabled

## Setting Up with Dependency Injection

You can also configure analytics via dependency injection:

```csharp
// Program.cs
using Osirion.Blazor.Extensions;

// Using configuration
builder.Services.AddClarityTracker(builder.Configuration);
builder.Services.AddMatomoTracker(builder.Configuration);

// Or programmatically
builder.Services.AddClarityTracker(options =>
{
    options.TrackerUrl = "https://www.clarity.ms/tag/";
    options.SiteId = "your-clarity-id";
    options.Track = true;
});
```

Then in your layout:

```razor
@inject IOptions<ClarityOptions> ClarityOptions
@inject IOptions<MatomoOptions> MatomoOptions

@if (ClarityOptions?.Value != null)
{
    <ClarityTracker Options="@ClarityOptions.Value" />
}

@if (MatomoOptions?.Value != null)
{
    <MatomoTracker Options="@MatomoOptions.Value" />
}
```

## Environment-Specific Configuration

It's a good practice to disable analytics in development environments:

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

## Privacy Considerations

When implementing analytics, consider these privacy best practices:

1. Obtain user consent before enabling tracking
2. Provide a clear privacy policy
3. Allow users to opt out
4. Consider data retention policies
5. Be transparent about what data is collected

## Conditional Tracking

You can conditionally enable tracking based on user consent:

```razor
@if (ConsentGiven)
{
    <ClarityTracker Options="@ClarityOptions" />
}
```

## Conclusion

Osirion.Blazor makes it easy to integrate analytics into your Blazor applications. By following the examples in this article, you can start tracking user behavior and gaining valuable insights to improve your application.

Remember to respect user privacy and comply with regulations like GDPR when implementing analytics.
