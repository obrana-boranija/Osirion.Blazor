---
title: "Introduction to Osirion.Blazor"
date: "2025-05-04"
author: "Dejan Demonjić"
description: "An overview of the Osirion.Blazor ecosystem, a modular, high-performance CMS and component library designed for modern Blazor applications with SSR compatibility."
tags: [blazor, components, ssr, dotnet]
categories: [overview]
slug: "introduction"
is_featured: true
featured_image: "https://th.bing.com/th/id/OIP.YZ0-gBP3yMKk7VZdCGwwgAHaEB"
seo_metadata:
  metaTitle: "Osirion.Blazor: Modular, High-Performance Components for Blazor Applications"
  metaDescription: "Discover Osirion.Blazor, a comprehensive component library for Blazor with Analytics, Navigation, CMS, and Theming modules designed for SSR compatibility."
  ogTitle: "Osirion.Blazor: Modular Components for Modern Blazor Applications"
  ogDescription: "A comprehensive library of high-performance Blazor components with SSR compatibility and minimal JavaScript dependencies."
  ogType: "website"
  ogImage: "https://th.bing.com/th/id/OIP.YZ0-gBP3yMKk7VZdCGwwgAHaEB"
  twitterTitle: "Osirion.Blazor: Modern Component Library for Blazor"
  twitterDescription: "Enhance your Blazor applications with SSR-compatible components for analytics, navigation, content management, and theming."
  twitterImage: "https://th.bing.com/th/id/OIP.YZ0-gBP3yMKk7VZdCGwwgAHaEB"
  canonicalUrl: "https://getosirion.com/introduction"
  schemaType: "SoftwareApplication"
---

Osirion.Blazor is a comprehensive, modular component library designed to enhance Blazor applications with high-performance, SSR-compatible components. Built with a focus on modern web development practices, Osirion embraces progressive enhancement principles to ensure your applications work seamlessly across all Blazor hosting models.

## Core Principles

Osirion.Blazor is built on five core principles:

1. **SSR-First Development**: All components are designed for Server-Side Rendering compatibility from the ground up
2. **Minimal JavaScript Dependencies**: Using JavaScript only when absolutely necessary
3. **Progressive Enhancement**: Core functionality works without JavaScript, enhanced when available
4. **Modular Architecture**: Use only what you need with independently packaged modules
5. **Framework Integration**: Seamless integration with popular CSS frameworks

## Modules

Osirion.Blazor consists of several specialized modules that can be used independently or together:

### Analytics

Implement analytics tracking in your Blazor applications with support for multiple providers:

- Microsoft Clarity
- Matomo
- Google Analytics 4
- Yandex Metrica

Key features:
- SSR-compatible tracking
- Provider pattern for extensibility
- Privacy-focused with consent management
- Configuration-driven setup

[Learn more about Osirion.Blazor.Analytics →](/analytics/getting-started)

### Navigation

Enhance your application's navigation experience with:

- Smooth scrolling
- Scroll restoration for navigation
- "Back to top" functionality
- Enhanced navigation interception

Key features:
- Zero JavaScript for core functionality
- Customizable through CSS variables
- Respects reduced motion preferences
- Works with all Blazor hosting models

[Learn more about Osirion.Blazor.Navigation →](/navigation/getting-started)

### Content Management

Build content-driven websites with integrated CMS capabilities:

- Multiple content providers (GitHub, FileSystem)
- Markdown rendering with frontmatter
- Content organization with tags and categories
- Directory-based navigation and search

Key features:
- Git-based content workflow
- SEO optimization built-in
- Provider pattern for custom sources
- Full-text search capabilities

[Learn more about Osirion.Blazor.Cms →](/cms/getting-started)

### Theming

Create consistent, themeable applications with:

- CSS framework integration
- Dark mode support
- CSS variables for theming
- Framework-specific adaptations

Key features:
- System preference detection
- User preference persistence
- Minimal JavaScript with progressive enhancement
- Integration with popular frameworks (Bootstrap, FluentUI, MudBlazor, Radzen)

[Learn more about Osirion.Blazor.Theming →](/theming/getting-started)

## Getting Started

### Installation

Install the complete package:

```bash
dotnet add package Osirion.Blazor
```

Or just the modules you need:

```bash
dotnet add package Osirion.Blazor.Core
dotnet add package Osirion.Blazor.Analytics
dotnet add package Osirion.Blazor.Navigation
dotnet add package Osirion.Blazor.Cms
dotnet add package Osirion.Blazor.Theming
```

### Basic Setup

Register all services at once:

```csharp
// In Program.cs
using Osirion.Blazor.Extensions;

builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .UseAnalytics(analytics => {
            analytics.AddClarity(options => {
                options.SiteId = "your-clarity-site-id";
            });
        })
        .UseNavigation(navigation => {
            navigation.AddScrollToTop();
            navigation.AddEnhancedNavigation();
        })
        .UseTheming(theming => {
            theming.UseFramework(CssFramework.Bootstrap);
            theming.EnableDarkMode();
        });
});
```

Or use configuration-based setup:

```csharp
// Register from appsettings.json
builder.Services.AddOsirionBlazor(builder.Configuration);
```

With corresponding configuration:

```json
{
  "Osirion": {
    "Analytics": {
      "Clarity": {
        "SiteId": "your-clarity-site-id",
        "Enabled": true
      }
    },
    "Navigation": {
      "Enhanced": {
        "Behavior": "Smooth",
        "ResetScrollOnNavigation": true
      },
      "ScrollToTop": {
        "Position": "BottomRight",
        "Behavior": "Smooth"
      }
    },
    "Theming": {
      "Framework": "Bootstrap",
      "EnableDarkMode": true,
      "UseSystemPreference": true
    }
  }
}
```

### Using Components

Use Osirion components in your Blazor application:

```razor
@using Osirion.Blazor.Components

<ClarityTracker />
<EnhancedNavigation Behavior="ScrollBehavior.Smooth" />
<ScrollToTop Position="Position.BottomRight" />

<ThemeProvider Framework="CssFramework.Bootstrap">
    <ThemeToggle />
    
    <main>
        <ContentList Directory="blog" />
    </main>
</ThemeProvider>
```

## Requirements

- .NET 8.0 or higher
- Blazor (Server, WebAssembly, or Auto)

## Browser Support

Osirion.Blazor supports all modern browsers:

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

Progressive enhancement ensures functionality even in environments with limited JavaScript support.

## Framework Integration

Osirion.Blazor integrates with popular CSS frameworks:

- Bootstrap 5+
- FluentUI for Blazor
- MudBlazor
- Radzen Blazor

Framework integration maps Osirion's CSS variables to the framework's native variables, ensuring consistent styling.

## SSR Compatibility

All Osirion components are designed for Server-Side Rendering (SSR) compatibility:

- Static rendering for build-time generation
- Hydration for interactive enhancements
- Progressive enhancement for JavaScript availability

This ensures optimal performance and SEO benefits while maintaining rich interactivity.

## Next Steps

- [Explore the Analytics module →](/analytics/getting-started)
- [Learn about Navigation components →](/navigation/getting-started)
- [Discover CMS capabilities →](/cms/getting-started)
- [Set up theming for your application →](/theming/getting-started)
- [View examples and demos →](/examples)
- [Read the API reference →](/api-reference)

Ready to enhance your Blazor applications? Let's get started!
