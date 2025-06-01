---
id: 'getting-started'
order: 2
layout: documentation
title: Getting Started with Osirion.Blazor
permalink: /getting-started
description: Complete guide to setting up Osirion.Blazor with all modules - from GitHub CMS integration to analytics, navigation, and theming. Build your first Blazor site in minutes.
author: Dejan Demonjić
date: 2025-05-05
featured_image: https://repository-images.githubusercontent.com/389755180/4e2b8f74-43c4-4b01-96f2-f6bffa3de985
categories:
- Documentation
- Getting Started
tags:
- setup
- installation
- github-cms
- blazor
- modules
- configuration
is_featured: true
published: true
slug: getting-started
lang: en
custom_fields: {}
seo_properties:
  title: 'Getting Started with Osirion.Blazor - Complete Setup Guide'
  description: 'Learn how to set up Osirion.Blazor with GitHub CMS, analytics, navigation, and theming modules. Complete implementation guide with code examples.'
  image: https://repository-images.githubusercontent.com/389755180/4e2b8f74-43c4-4b01-96f2-f6bffa3de985
  canonical: 'https://getosirion.com/getting-started'
  lang: en
  robots: index, follow
  og_title: 'Getting Started with Osirion.Blazor'
  og_description: 'Complete setup guide for Osirion.Blazor modules including GitHub CMS integration'
  og_image_url: 'https://repository-images.githubusercontent.com/389755180/4e2b8f74-43c4-4b01-96f2-f6bffa3de985'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Getting Started with Osirion.Blazor'
  twitter_description: 'Complete setup guide for all Osirion.Blazor modules'
  twitter_image_url: 'https://repository-images.githubusercontent.com/389755180/4e2b8f74-43c4-4b01-96f2-f6bffa3de985'
  meta_tags: {}
---

# Getting Started with Osirion.Blazor

Welcome to Osirion.Blazor! This guide will walk you through setting up a complete Blazor application with GitHub as your CMS, analytics tracking, enhanced navigation, and beautiful theming - all in under 10 minutes.

![Osirion.Blazor Architecture](https://www.planttext.com/api/plantuml/png/SoWkIImgAStDuNBAJrBGjLDmpCbCJbMmKiX8pSd9vt98pKi1IW80)

## Prerequisites

- .NET 8.0 or .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- A GitHub account (for CMS features)
- Basic Blazor knowledge

## Quick Installation

### Option 1: Install Everything (Recommended)

```bash
dotnet add package Osirion.Blazor
```

This meta-package includes all modules and is perfect for getting started quickly.

### Option 2: Install Individual Modules

```bash
# Core components (required by other modules)
dotnet add package Osirion.Blazor.Core

# Content Management
dotnet add package Osirion.Blazor.Cms

# Analytics
dotnet add package Osirion.Blazor.Analytics

# Navigation enhancements
dotnet add package Osirion.Blazor.Navigation

# Theming system
dotnet add package Osirion.Blazor.Theming
```

## Understanding the Modules

![Osirion.Blazor Modules](https://mermaid.ink/img/pako:eNplkMFuwjAQRH9l5TMg8QM-IFEqDlUr0V5COVi7E2LVsU3sBIFQ_r1OAqLlNLPz3sxqT5BZhRCAL-S2IDa1haIwCtu2xJfY7UwmP1Kj3WiUJNRq8ppQZ2Qmm83bxrTW1fJQ7Q0dbbrO6H_LRWONuaR-1cjKdP_GjBGTa2vOkfcR47TwBl5HzKqrOqJFOcDk01k5mDdz6UqFj2zp8AzPcB5OYRpGuYqgCqEQMSR4TjCPQKPXXzHqT1dCJqm8Z5BCnmtMLTpqGXJRoIMShFQWtyIqVQ5TKLRE9ymdQcXqEiLfnHa7n-7XDz0FcSo?type=png)

### 📦 **Osirion.Blazor.Core**
The foundation that all other modules build upon.

**Key Features:**
- SSR-compatible base components
- Shared enums and utilities
- Framework detection
- CSS variable system
- Common component patterns

### 📝 **Osirion.Blazor.Cms**
Transform GitHub (or file system) into a powerful headless CMS.

**What's Included:**
- **Cms.Core**: Content models and provider abstractions
- **Cms.Web**: Display components (ContentView, ContentList, etc.)
- **Cms.Admin**: Optional admin interface for content management

**Key Components:**
- `ContentList` - Display content collections
- `ContentView` - Render individual content items
- `DirectoryNavigation` - Browse content by directory
- `CategoriesList` & `TagCloud` - Content organization
- `SearchBox` - Full-text content search
- `LocalizedContentView` - Multi-language support

### 📊 **Osirion.Blazor.Analytics**
Privacy-friendly analytics with multiple provider support.

**Supported Providers:**
- Microsoft Clarity
- Matomo
- Google Analytics 4
- Yandex Metrica

### 🧭 **Osirion.Blazor.Navigation**
Enhanced navigation with scroll management.

**Components:**
- `EnhancedNavigation` - Smooth scroll behavior
- `ScrollToTop` - Customizable back-to-top button

### 🎨 **Osirion.Blazor.Theming**
Adaptive theming with CSS framework integration.

**Supported Frameworks:**
- Bootstrap
- MudBlazor
- Fluent UI
- Radzen
- Custom themes

## Basic Implementation

### 1. Configure Services in Program.cs

```csharp
using Osirion.Blazor.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add all Osirion services with fluent configuration
builder.Services.AddOsirion(osirion =>
{
    // Configure GitHub CMS
    osirion.UseContent(content =>
    {
        content.AddGitHub(github =>
        {
            github.Owner = "your-username";
            github.Repository = "your-content-repo";
            github.ContentPath = "content";
            github.Branch = "main";
        });
    });

    // Add analytics
    osirion.UseAnalytics(analytics =>
    {
        analytics.AddClarity(opt => opt.SiteId = "your-clarity-id");
        analytics.AddGA4(opt => opt.MeasurementId = "G-XXXXXXXXXX");
    });

    // Configure navigation
    osirion.UseNavigation(nav =>
    {
        nav.AddEnhancedNavigation();
        nav.AddScrollToTop();
    });

    // Set up theming
    osirion.UseTheming(theme =>
    {
        theme.UseFramework(CssFramework.Bootstrap);
        theme.EnableDarkMode();
    });
});

var app = builder.Build();

// Configure pipeline
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### 2. Update Your App.razor

```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    
    <!-- Osirion.Blazor styles -->
    <link rel="stylesheet" href="_content/Osirion.Blazor.Core/css/index.css" />
    
    <!-- Your CSS framework (if using) -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    
    <HeadOutlet />
</head>
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

### 3. Create Your Layout Component

```razor
@inherits LayoutComponentBase
@using Osirion.Blazor.Navigation.Components
@using Osirion.Blazor.Analytics.Components
@using Osirion.Blazor.Theming.Components

<!-- Theme Provider -->
<ThemeProvider>
    <!-- Analytics Trackers -->
    <ClarityTracker />
    <GA4Tracker />
    
    <!-- Enhanced Navigation -->
    <EnhancedNavigation Behavior="ScrollBehavior.Smooth" />
    
    <!-- Main Layout -->
    <div class="layout">
        <header>
            <nav class="navbar">
                <!-- Your navigation here -->
            </nav>
        </header>
        
        <main>
            @Body
        </main>
        
        <footer>
            <!-- Your footer here -->
        </footer>
    </div>
    
    <!-- Scroll to Top Button -->
    <ScrollToTop Position="Position.BottomRight" />
</ThemeProvider>
```

### 4. Create Your First Content Page

```razor
@page "/"
@using Osirion.Blazor.Cms.Components

<div class="container">
    <!-- Hero Section -->
    <div class="hero">
        <h1>Welcome to My Site</h1>
        <p>Powered by Osirion.Blazor and GitHub</p>
    </div>
    
    <!-- Featured Content -->
    <ContentList 
        IsFeatured="true" 
        Count="3" 
        SortBy="SortField.Date" 
        SortDirection="SortDirection.Descending">
        <ItemTemplate Context="item">
            <div class="content-card">
                <h3>@item.Title</h3>
                <p>@item.Description</p>
                <a href="/content/@item.Path">Read More</a>
            </div>
        </ItemTemplate>
    </ContentList>
    
    <!-- Recent Blog Posts -->
    <h2>Recent Posts</h2>
    <ContentList 
        Directory="blog" 
        Count="5" />
    
    <!-- Categories -->
    <div class="sidebar">
        <h3>Categories</h3>
        <CategoriesList ShowItemCount="true" />
        
        <h3>Tags</h3>
        <TagCloud MaxTags="20" />
    </div>
</div>
```

### 5. Display Individual Content

```razor
@page "/content/{*path}"
@using Osirion.Blazor.Cms.Components

<ContentView Path="@Path" ShowMetadata="true">
    <NotFoundTemplate>
        <div class="alert alert-warning">
            <h4>Content Not Found</h4>
            <p>The requested content could not be found.</p>
            <a href="/">Return to Home</a>
        </div>
    </NotFoundTemplate>
</ContentView>

@code {
    [Parameter]
    public string? Path { get; set; }
}
```

## Configuration with appsettings.json

For production environments, use configuration files:

```json
{
  "Osirion": {
    "Cms": {
      "GitHub": {
        "Owner": "your-username",
        "Repository": "your-content-repo",
        "ContentPath": "content",
        "Branch": "main",
        "Token": "" // Optional, for private repos
      }
    },
    "Analytics": {
      "Clarity": {
        "SiteId": "your-clarity-id",
        "Enabled": true
      },
      "GA4": {
        "MeasurementId": "G-XXXXXXXXXX",
        "Enabled": true,
        "DebugMode": false
      }
    },
    "Navigation": {
      "Enhanced": {
        "Behavior": "Smooth",
        "ResetScrollOnNavigation": true
      },
      "ScrollToTop": {
        "Position": "BottomRight",
        "VisibilityThreshold": 300
      }
    },
    "Theming": {
      "Framework": "Bootstrap",
      "EnableDarkMode": true,
      "DefaultMode": "System"
    }
  }
}
```

Then simplify your Program.cs:

```csharp
// Load all configuration from appsettings.json
builder.Services.AddOsirion(builder.Configuration);
```

## Content Repository Structure

Organize your GitHub content repository like this:

```markdown
your-content-repo/
├── content/
│   ├── blog/
│   │   ├── 2025-01-15-getting-started.md
│   │   ├── 2025-01-20-advanced-features.md
│   │   └── 2025-01-25-best-practices.md
│   ├── docs/
│   │   ├── installation.md
│   │   ├── configuration.md
│   │   └── api-reference.md
│   ├── portfolio/
│   │   ├── project-1.md
│   │   └── project-2.md
│   └── pages/
│       ├── about.md
│       └── contact.md
├── media/
│   └── images/
│       ├── blog/
│       └── portfolio/
└── README.md
```

### Content File Format

```markdown
---
title: "My First Blog Post"
description: "Learn how to use Osirion.Blazor"
author: "Your Name"
date: "2025-01-15"
categories: [tutorial, blazor]
tags: [osirion, cms, github]
featured_image: "/media/images/blog/hero.jpg"
is_featured: true
---

# Your Markdown Content Here

Write your content using standard Markdown...
```

## Multi-Language Support

Enable localization for international sites:

```csharp
osirion.UseContent(content =>
{
    content.AddGitHub(github =>
    {
        github.EnableLocalization = true;
        github.DefaultLocale = "en";
    });
});
```

Content structure with localization:

```
content/
├── en/
│   ├── blog/
│   └── pages/
├── es/
│   ├── blog/
│   └── pages/
└── fr/
    ├── blog/
    └── pages/
```

## Admin Interface (Optional)

Add the CMS Admin module for a visual content editor:

```csharp
osirion.UseCmsAdmin(admin =>
{
    admin.UseGitHubProvider();
    admin.ConfigureAuthentication(auth =>
    {
        auth.UseGitHubAuthentication();
    });
});
```

Access the admin panel at `/admin` after configuration.

![Admin Interface](https://raw.githubusercontent.com/sanity-io/sanity/next/packages/%40sanity/base/assets/sanity-studio-poster.jpg)

## Next Steps

1. **[Content Management Guide](/docs/content-management)** - Deep dive into GitHub CMS features
2. **[Theming & Styling](/docs/theming)** - Customize the look and feel
3. **[Analytics Setup](/docs/analytics)** - Configure tracking and insights
4. **[Deployment Guide](/docs/deployment)** - Deploy to Azure, AWS, or Netlify
5. **[API Reference](/docs/api)** - Complete component documentation

## Getting Help

- 📚 **Documentation**: [getosirion.com/docs](https://getosirion.com/docs)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/obrana-boranija/Osirion.Blazor/discussions)
- 🐛 **Issues**: [GitHub Issues](https://github.com/obrana-boranija/Osirion.Blazor/issues)
- 📧 **Email**: support@getosirion.com

Ready to build something amazing? You now have everything you need to create beautiful, content-driven Blazor applications with GitHub as your CMS!
