---
id: 'analytics-components-overview'
order: 1
layout: docs
title: Analytics Components Overview
permalink: /docs/components/analytics
description: Complete overview of Osirion.Blazor Analytics components for Google Analytics 4, Microsoft Clarity, Matomo, and Yandex Metrica integration with privacy compliance and performance tracking.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
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
- privacy
- performance
is_featured: true
published: true
slug: analytics
lang: en
custom_fields: {}
seo_properties:
  title: 'Analytics Components - Osirion.Blazor Multi-Provider Analytics'
  description: 'Explore Osirion.Blazor Analytics components for multi-provider analytics integration with privacy compliance and performance tracking.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/analytics'
  lang: en
  robots: index, follow
  og_title: 'Analytics Components - Osirion.Blazor'
  og_description: 'Multi-provider analytics integration with privacy compliance for Blazor applications.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Analytics Components - Osirion.Blazor'
  twitter_description: 'Multi-provider analytics integration for Blazor applications.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Analytics Components Overview

The Osirion.Blazor Analytics module provides comprehensive analytics integration components supporting multiple analytics providers including Google Analytics 4, Microsoft Clarity, Matomo, and Yandex Metrica. These components are designed with privacy compliance, performance optimization, and ease of use as core principles.

## Module Features

**Multi-Provider Support**: Integration with major analytics platforms in a unified API
**Privacy Compliance**: Built-in GDPR and privacy regulation compliance features
**Performance Optimized**: Minimal impact on page load times and Core Web Vitals
**Consent Management**: Integration with cookie consent and privacy preferences
**Custom Event Tracking**: Comprehensive event tracking capabilities
**SSR Compatible**: Full Server-Side Rendering support with proper script loading

## Supported Analytics Providers

### Google Analytics 4 (GA4)
Next-generation Google Analytics with enhanced privacy features and event-based tracking.

### Microsoft Clarity
User behavior analytics with heatmaps, session recordings, and user interaction insights.

### Matomo
Privacy-focused, self-hosted analytics platform with GDPR compliance by design.

### Yandex Metrica
Comprehensive web analytics platform popular in Eastern European markets.

## Core Components

### Analytics Tracker Components

Individual components for each analytics provider with provider-specific configuration options.

#### GA4Tracker
Google Analytics 4 integration component with enhanced e-commerce and event tracking.

**Key Features:**
- Enhanced e-commerce tracking
- Custom event definitions
- User identification
- Privacy controls
- Debug mode support
- Conversion tracking

```razor
<GA4Tracker 
    MeasurementId="G-XXXXXXXXXX"
    Enabled="@analyticsEnabled"
    DebugMode="@isDevelopment" />
```

**Configuration:**
```csharp
// Program.cs
builder.Services.AddOsirionAnalytics(options =>
{
    options.AddGA4(ga4 =>
    {
        ga4.MeasurementId = "G-XXXXXXXXXX";
        ga4.EnableDebugMode = builder.Environment.IsDevelopment();
        ga4.TrackPageViews = true;
        ga4.TrackDownloads = true;
        ga4.TrackExternalLinks = true;
    });
});
```

#### ClarityTracker
Microsoft Clarity integration for user behavior analytics and session recordings.

**Key Features:**
- Session recording
- Heatmap generation
- User interaction tracking
- Privacy-friendly design
- Real-time insights
- Performance monitoring

```razor
<ClarityTracker 
    ProjectId="your-clarity-project-id"
    Enabled="@clarityEnabled" />
```

**Configuration:**
```csharp
builder.Services.AddOsirionAnalytics(options =>
{
    options.AddClarity(clarity =>
    {
        clarity.ProjectId = "your-clarity-project-id";
        clarity.EnableHeatmaps = true;
        clarity.EnableSessionRecordings = true;
        clarity.SampleRate = 100; // Percentage of sessions to track
    });
});
```

#### MatomoTracker
Matomo (formerly Piwik) integration for privacy-focused analytics.

**Key Features:**
- Privacy by design
- Self-hosted option
- GDPR compliance
- Custom dimensions
- Goal tracking
- E-commerce tracking

```razor
<MatomoTracker 
    SiteId="1"
    MatomoUrl="https://your-matomo-instance.com/"
    Enabled="@matomoEnabled" />
```

**Configuration:**
```csharp
builder.Services.AddOsirionAnalytics(options =>
{
    options.AddMatomo(matomo =>
    {
        matomo.SiteId = 1;
        matomo.MatomoUrl = "https://your-matomo-instance.com/";
        matomo.EnableLinkTracking = true;
        matomo.EnableDownloadTracking = true;
        matomo.RespectDoNotTrack = true;
    });
});
```

#### YandexMetricaTracker
Yandex Metrica integration for comprehensive web analytics.

**Key Features:**
- Detailed user behavior analytics
- Webvisor session recordings
- Form analytics
- Click mapping
- Conversion tracking
- Real-time statistics

```razor
<YandexMetricaTracker 
    CounterId="12345678"
    Enabled="@yandexEnabled" />
```

**Configuration:**
```csharp
builder.Services.AddOsirionAnalytics(options =>
{
    options.AddYandexMetrica(yandex =>
    {
        yandex.CounterId = 12345678;
        yandex.EnableWebvisor = true;
        yandex.EnableClickmap = true;
        yandex.EnableTrackLinks = true;
        yandex.AccurateTrackBounce = true;
    });
});
```

## Installation and Setup

### Package Installation

```bash
dotnet add package Osirion.Blazor.Analytics
```

### Service Registration

```csharp
// Program.cs
builder.Services.AddOsirionAnalytics(options =>
{
    // Global analytics settings
    options.EnableInDevelopment = false;
    options.RespectDoNotTrack = true;
    options.EnableConsentManagement = true;
    
    // Configure multiple providers
    options.AddGA4(ga4 =>
    {
        ga4.MeasurementId = builder.Configuration["Analytics:GA4:MeasurementId"];
        ga4.EnableDebugMode = builder.Environment.IsDevelopment();
    });
    
    options.AddClarity(clarity =>
    {
        clarity.ProjectId = builder.Configuration["Analytics:Clarity:ProjectId"];
    });
    
    options.AddMatomo(matomo =>
    {
        matomo.SiteId = int.Parse(builder.Configuration["Analytics:Matomo:SiteId"]);
        matomo.MatomoUrl = builder.Configuration["Analytics:Matomo:Url"];
    });
});
```

### Configuration

```json
{
  "Analytics": {
    "GA4": {
      "MeasurementId": "G-XXXXXXXXXX"
    },
    "Clarity": {
      "ProjectId": "your-clarity-project-id"
    },
    "Matomo": {
      "SiteId": "1",
      "Url": "https://your-matomo-instance.com/"
    },
    "YandexMetrica": {
      "CounterId": "12345678"
    }
  }
}
```

### Import Statements

```razor
@* _Imports.razor *@
@using Osirion.Blazor.Analytics.Components
@using Osirion.Blazor.Analytics.Services
@using Osirion.Blazor.Analytics.Options
```

## Usage Examples

### Basic Multi-Provider Setup

```razor
<!-- App.razor or MainLayout.razor -->
@if (analyticsEnabled)
{
    <!-- Google Analytics 4 -->
    <GA4Tracker 
        MeasurementId="G-XXXXXXXXXX"
        Enabled="@cookieConsent.AnalyticsAccepted" />
    
    <!-- Microsoft Clarity -->
    <ClarityTracker 
        ProjectId="your-clarity-project-id"
        Enabled="@cookieConsent.AnalyticsAccepted" />
    
    <!-- Matomo -->
    <MatomoTracker 
        SiteId="1"
        MatomoUrl="https://analytics.yoursite.com/"
        Enabled="@cookieConsent.AnalyticsAccepted" />
}

@code {
    private bool analyticsEnabled = true;
    private CookieConsent cookieConsent = new();
    
    protected override async Task OnInitializedAsync()
    {
        cookieConsent = await CookieService.GetConsentAsync();
        analyticsEnabled = !IsPrerendering && cookieConsent.AnalyticsAccepted;
    }
}
```

### Environment-Specific Configuration

```razor
<GA4Tracker 
    MeasurementId="@GetGA4MeasurementId()"
    DebugMode="@IsDevelopment"
    Enabled="@IsAnalyticsEnabled()" />

@code {
    [Inject] private IWebHostEnvironment Environment { get; set; } = default!;
    
    private bool IsDevelopment => Environment.IsDevelopment();
    
    private string GetGA4MeasurementId()
    {
        return Environment.IsDevelopment() 
            ? "G-XXXXXXXXXX" // Development ID
            : "G-YYYYYYYYYY"; // Production ID
    }
    
    private bool IsAnalyticsEnabled()
    {
        return !IsDevelopment || Configuration.GetValue<bool>("Analytics:EnableInDevelopment");
    }
}
```

### Privacy-Compliant Setup

```razor
<OsirionCookieConsent 
    @ref="cookieConsent"
    OnConsentChanged="OnCookieConsentChanged"
    ShowAnalyticsOption="true"
    ShowMarketingOption="true" />

@if (hasAnalyticsConsent)
{
    <GA4Tracker 
        MeasurementId="G-XXXXXXXXXX"
        Enabled="true" />
    
    <ClarityTracker 
        ProjectId="your-clarity-project-id"
        Enabled="true" />
}

@code {
    private bool hasAnalyticsConsent = false;
    private OsirionCookieConsent? cookieConsent;
    
    private async Task OnCookieConsentChanged(CookieConsentEventArgs args)
    {
        hasAnalyticsConsent = args.AnalyticsAccepted;
        StateHasChanged();
        
        if (!hasAnalyticsConsent)
        {
            // Clear analytics cookies and data
            await AnalyticsService.ClearDataAsync();
        }
    }
}
```

### Custom Event Tracking

```razor
@inject IAnalyticsService AnalyticsService

<button @onclick="TrackButtonClick" class="cta-button">
    Download Now
</button>

<form @onsubmit="TrackFormSubmit">
    <!-- Form content -->
    <button type="submit">Submit</button>
</form>

@code {
    private async Task TrackButtonClick()
    {
        await AnalyticsService.TrackEventAsync("button_click", new
        {
            button_name = "download_cta",
            page_location = NavigationManager.Uri,
            user_type = "visitor"
        });
    }
    
    private async Task TrackFormSubmit()
    {
        await AnalyticsService.TrackEventAsync("form_submit", new
        {
            form_name = "contact_form",
            page_title = "Contact Us",
            timestamp = DateTime.UtcNow
        });
    }
}
```

### E-commerce Tracking

```razor
@code {
    private async Task TrackPurchase(Order order)
    {
        await AnalyticsService.TrackEventAsync("purchase", new
        {
            transaction_id = order.Id,
            value = order.Total,
            currency = order.Currency,
            items = order.Items.Select(item => new
            {
                item_id = item.ProductId,
                item_name = item.ProductName,
                category = item.Category,
                quantity = item.Quantity,
                price = item.Price
            })
        });
    }
    
    private async Task TrackAddToCart(Product product, int quantity)
    {
        await AnalyticsService.TrackEventAsync("add_to_cart", new
        {
            currency = "USD",
            value = product.Price * quantity,
            items = new[]
            {
                new
                {
                    item_id = product.Id,
                    item_name = product.Name,
                    category = product.Category,
                    quantity = quantity,
                    price = product.Price
                }
            }
        });
    }
}
```

## Privacy and Compliance

### GDPR Compliance

The Analytics components include built-in GDPR compliance features:

**Consent Management**: Integration with cookie consent components
**Data Minimization**: Only collect necessary analytics data
**User Rights**: Support for data deletion and opt-out requests
**Anonymization**: IP address anonymization and user data protection

```csharp
// Configure privacy settings
builder.Services.AddOsirionAnalytics(options =>
{
    options.RespectDoNotTrack = true;
    options.AnonymizeIp = true;
    options.EnableConsentManagement = true;
    options.DataRetentionDays = 365; // Automatic data deletion
});
```

### Cookie Management

```razor
<OsirionCookieConsent 
    OnConsentChanged="HandleConsentChange"
    ShowAnalyticsDetails="true">
    
    <AnalyticsConsentText>
        We use analytics cookies to understand how you interact with our website.
        This helps us improve your experience and our services.
    </AnalyticsConsentText>
</OsirionCookieConsent>

@code {
    private async Task HandleConsentChange(CookieConsentEventArgs consent)
    {
        if (!consent.AnalyticsAccepted)
        {
            // Disable analytics tracking
            await AnalyticsService.DisableTrackingAsync();
            
            // Clear existing analytics cookies
            await AnalyticsService.ClearCookiesAsync();
        }
        else
        {
            // Enable analytics tracking
            await AnalyticsService.EnableTrackingAsync();
        }
    }
}
```

## Performance Optimization

### Lazy Loading

```csharp
// Configure lazy loading for analytics scripts
builder.Services.AddOsirionAnalytics(options =>
{
    options.LazyLoadScripts = true;
    options.LoadTimeout = TimeSpan.FromSeconds(10);
    options.PreloadScripts = false; // Don't preload in head
});
```

### Conditional Loading

```razor
@* Only load analytics on specific pages *@
@if (ShouldLoadAnalytics())
{
    <GA4Tracker MeasurementId="G-XXXXXXXXXX" />
}

@code {
    private bool ShouldLoadAnalytics()
    {
        var currentPath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        
        // Don't load on admin pages
        if (currentPath.StartsWith("admin/")) return false;
        
        // Don't load on internal tools
        if (currentPath.StartsWith("tools/")) return false;
        
        return true;
    }
}
```

### Script Optimization

```html
<!-- Analytics scripts are automatically optimized -->
<script async src="https://www.googletagmanager.com/gtag/js?id=G-XXXXXXXXXX"></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());
  gtag('config', 'G-XXXXXXXXXX', {
    'anonymize_ip': true,
    'cookie_flags': 'SameSite=None;Secure',
    'send_page_view': false // Handled by component
  });
</script>
```

## Advanced Configuration

### Custom Analytics Provider

```csharp
// Create custom analytics provider
public class CustomAnalyticsProvider : IAnalyticsProvider
{
    public string Name => "CustomAnalytics";
    public bool ShouldRender { get; set; } = true;
    
    public async Task TrackEventAsync(string eventName, object? parameters = null)
    {
        // Custom event tracking implementation
    }
    
    public async Task TrackPageViewAsync(string? pagePath = null, string? pageTitle = null)
    {
        // Custom page view tracking implementation
    }
}

// Register custom provider
builder.Services.AddOsirionAnalytics(options =>
{
    options.AddCustomProvider<CustomAnalyticsProvider>();
});
```

### Multiple Site Tracking

```razor
<!-- Track multiple sites with different configurations -->
<GA4Tracker 
    MeasurementId="G-SITE1-ID"
    Enabled="@IsSite1" />

<GA4Tracker 
    MeasurementId="G-SITE2-ID"
    Enabled="@IsSite2" />

<MatomoTracker 
    SiteId="1"
    MatomoUrl="https://analytics.site1.com/"
    Enabled="@IsSite1" />

<MatomoTracker 
    SiteId="2"
    MatomoUrl="https://analytics.site2.com/"
    Enabled="@IsSite2" />

@code {
    private bool IsSite1 => Host.Contains("site1.com");
    private bool IsSite2 => Host.Contains("site2.com");
    
    [Inject] private IHttpContextAccessor HttpContext { get; set; } = default!;
    
    private string Host => HttpContext.HttpContext?.Request.Host.Value ?? "";
}
```

## Testing and Debugging

### Debug Mode

```razor
<GA4Tracker 
    MeasurementId="G-XXXXXXXXXX"
    DebugMode="@IsDevelopment"
    Enabled="true" />

@code {
    [Inject] private IWebHostEnvironment Environment { get; set; } = default!;
    
    private bool IsDevelopment => Environment.IsDevelopment();
}
```

### Analytics Validation

```csharp
// Validate analytics configuration
public class AnalyticsValidator
{
    public async Task<ValidationResult> ValidateConfigurationAsync()
    {
        var result = new ValidationResult();
        
        // Check if GA4 is properly configured
        if (string.IsNullOrEmpty(ga4Options.MeasurementId))
        {
            result.Errors.Add("GA4 Measurement ID is not configured");
        }
        
        // Validate Matomo configuration
        if (matomoOptions.SiteId <= 0)
        {
            result.Errors.Add("Matomo Site ID must be greater than 0");
        }
        
        return result;
    }
}
```

## Best Practices

### Performance
- Load analytics scripts asynchronously
- Use lazy loading for non-critical analytics
- Minimize the number of active providers
- Implement proper error handling

### Privacy
- Always obtain user consent before tracking
- Provide clear privacy policies
- Implement data retention policies
- Support user data deletion requests

### Data Quality
- Validate tracking implementation
- Use consistent event naming conventions
- Implement proper error tracking
- Monitor data accuracy regularly

### Compliance
- Stay updated with privacy regulations
- Implement proper consent management
- Document data collection practices
- Regular compliance audits

The Analytics components provide a comprehensive, privacy-compliant solution for multi-provider analytics integration while maintaining excellent performance and user experience standards.
