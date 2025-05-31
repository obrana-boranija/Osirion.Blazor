# Quick Reference for Osirion.Blazor

## Service Registration

### Option 1: Fluent API (Recommended)

```csharp
// Configure all services using the fluent API
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .AddGitHubCms(options => {
            options.Owner = "username";
            options.Repository = "repo";
        })
        .AddScrollToTop(ButtonPosition.BottomRight)
        .AddOsirionStyle(CssFramework.Bootstrap)
        .AddClarityTracker(options => {
            options.SiteId = "clarity-id";
        })
        .AddMatomoTracker(options => {
            options.SiteId = "matomo-id";
        });
});
```

### Option 2: Individual Service Registration

```csharp
// Add services individually
builder.Services.AddGitHubCms(options => {
    options.Owner = "username";
    options.Repository = "repo";
});

builder.Services.AddScrollToTop(options => {
    options.Position = ButtonPosition.BottomRight;
    options.Behavior = ScrollBehavior.Smooth;
});

builder.Services.AddOsirionStyle(CssFramework.Bootstrap);

builder.Services.AddClarityTracker(options => {
    options.SiteId = "clarity-id";
});

builder.Services.AddMatomoTracker(options => {
    options.SiteId = "matomo-id";
});
```

### Option 3: Configuration-Based Registration

```csharp
// Register all services from appsettings.json sections
builder.Services.AddOsirionBlazor(builder.Configuration);
```

With corresponding appsettings.json:

```json
{
  "GitHubCms": {
    "Owner": "username",
    "Repository": "repo",
    "ContentPath": "content",
    "Branch": "main"
  },
  "ScrollToTop": {
    "Position": "BottomRight",
    "Behavior": "Smooth",
    "VisibilityThreshold": 300,
    "Text": "Top"
  },
  "OsirionStyle": {
    "UseStyles": true,
    "FrameworkIntegration": "Bootstrap",
    "CustomVariables": "--osirion-primary-color: #0077cc;"
  },
  "Clarity": {
    "SiteId": "clarity-id",
    "Track": true
  },
  "Matomo": {
    "SiteId": "matomo-id",
    "Track": true
  }
}
```

## Navigation

### Enhanced Navigation Interceptor
```razor
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

### Scroll To Top Button
```razor
<!-- Basic usage -->
<ScrollToTop />

<!-- Customized -->
<ScrollToTop 
    Position="ButtonPosition.BottomRight" 
    Behavior="ScrollBehavior.Smooth"
    VisibilityThreshold="300" 
    Text="Top" />
```

### Scroll Behavior Options
- `ScrollBehavior.Auto` - Browser default
- `ScrollBehavior.Instant` - Immediate scroll
- `ScrollBehavior.Smooth` - Animated scroll

### Button Position Options
- `ButtonPosition.BottomRight` - Bottom right corner
- `ButtonPosition.BottomLeft` - Bottom left corner
- `ButtonPosition.TopRight` - Top right corner
- `ButtonPosition.TopLeft` - Top left corner

## Analytics

### Microsoft Clarity

```razor
<ClarityTracker Options="@(new ClarityOptions 
{ 
    TrackerUrl = "https://www.clarity.ms/tag/", 
    SiteId = "your-site-id", 
    Track = true 
})" />
```

### Matomo

```razor
<MatomoTracker Options="@(new MatomoOptions 
{ 
    TrackerUrl = "//analytics.example.com/", 
    SiteId = "1", 
    Track = true 
})" />
```

## GitHub CMS

### Configuration
```csharp
builder.Services.AddGitHubCms(options =>
{
    options.Owner = "username";
    options.Repository = "repo";
    options.ContentPath = "content";
});
```

### Basic Components
```razor
<!-- Content List -->
<ContentList Directory="blog" />

<!-- Content View -->
<ContentView Path="blog/post.md" />

<!-- Categories and Tags -->
<CategoriesList />
<TagCloud MaxTags="20" />
<SearchBox Placeholder="Search..." />
<DirectoryNavigation CurrentDirectory="blog" />
```

## Styling

### Include Default Styles
```html
<!-- In App.razor or _Host.cshtml -->
<link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion-cms.css" />
```

### Using OsirionStyles Component
```razor
<!-- In your layout -->
<OsirionStyles />

<!-- With custom variables -->
<OsirionStyles CustomVariables="
    --osirion-primary-color: #0077cc;
    --osirion-border-radius: 0.25rem;
" />
```

### Override CSS Variables
```css
/* In your app.css or custom style block */
:root {
    /* General styles */
    --osirion-primary-color: #0077cc;
    --osirion-color: #333333;
    --osirion-border-radius: 0.25rem;
    --osirion-font-size: 1.1rem;
    
    /* ScrollToTop specific styles */
    --osirion-scroll-background: rgba(0, 0, 0, 0.3);
    --osirion-scroll-color: #ffffff;
    --osirion-scroll-size: 40px;
    --osirion-scroll-margin: 20px;
}
```

### Configuration-Based Styling
```csharp
// In Program.cs
builder.Services.Configure<GitHubCmsOptions>(options => {
    options.UseStyles = true;
    options.CustomVariables = "--osirion-primary-color: #0077cc;";
});

// For ScrollToTop
builder.Services.AddScrollToTop(options => {
    options.CustomVariables = "--osirion-scroll-background: #0077cc;";
});
```

### Key CSS Variables
```css
/* Colors */
--osirion-primary-color: #2563eb;
--osirion-color: #374151;
--osirion-color-background: #ffffff;

/* Sizing */
--osirion-border-radius: 0.5rem;
--osirion-padding: 1.5rem;

/* Typography */
--osirion-font-size: 1rem;
--osirion-font-size-title: 1.25rem;

/* ScrollToTop Colors */
--osirion-scroll-background: rgba(0, 0, 0, 0.3);
--osirion-scroll-hover-background: rgba(0, 0, 0, 0.5);
--osirion-scroll-color: #ffffff;
--osirion-scroll-hover-color: #ffffff;

/* ScrollToTop Sizes */
--osirion-scroll-size: 40px;
--osirion-scroll-margin: 20px;
--osirion-scroll-border-radius: 4px;
--osirion-scroll-z-index: 1000;
```

## Complete Layout Example

```razor
@inherits LayoutComponentBase
@using Osirion.Blazor.Components.Navigation
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options
@using Osirion.Blazor.Components.GitHubCms
@inject IOptions<ClarityOptions>? ClarityOptions
@inject IOptions<MatomoOptions>? MatomoOptions

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="~/" />
    
    <!-- Include Osirion styles -->
    <link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion-cms.css" />
    
    <!-- App stylesheets -->
    <link rel="stylesheet" href="css/bootstrap/bootstrap.min.css" />
    <link rel="stylesheet" href="css/app.css" />
    <link rel="icon" type="image/png" href="favicon.png" />
</head>

<body>
    <!-- Navigation -->
    <EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
    <ScrollToTop />

    <!-- Analytics -->
    @if (ClarityOptions?.Value is not null)
    {
        <ClarityTracker Options="@ClarityOptions.Value" />
    }

    @if (MatomoOptions?.Value is not null)
    {
        <MatomoTracker Options="@MatomoOptions.Value" />
    }

    <!-- Main content -->
    <div class="page">
        @Body
    </div>

    <script src="_framework/blazor.web.js"></script>
</body>
```