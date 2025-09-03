---
id: 'components-overview'
order: 1
layout: docs
title: Osirion.Blazor Components Overview
permalink: /docs/components
description: Complete overview of all Osirion.Blazor components organized by modules - Core, CMS Web, CMS Admin, Navigation, Analytics, and Theming components with usage examples and best practices.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Documentation
- Blazor
tags:
- blazor
- components
- core
- cms
- navigation
- analytics
- theming
is_featured: true
published: true
slug: components
lang: en
custom_fields: {}
seo_properties:
  title: 'Osirion.Blazor Components - Complete Component Library Documentation'
  description: 'Explore all Osirion.Blazor components organized by modules. Comprehensive documentation with examples for Core, CMS, Navigation, Analytics and Theming components.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components'
  lang: en
  robots: index, follow
  og_title: 'Osirion.Blazor Components - Complete Library'
  og_description: 'Complete documentation for all Osirion.Blazor components with usage examples and best practices.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Osirion.Blazor Components Documentation'
  twitter_description: 'Complete documentation for all Osirion.Blazor components with usage examples.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Osirion.Blazor Components Overview

Osirion.Blazor provides a comprehensive set of modular components designed for modern Blazor applications with Server-Side Rendering (SSR) compatibility. The component library is organized into six main modules, each serving specific functionality requirements.

## Module Architecture

The Osirion.Blazor component library is structured around the following modules:

### Core Components
Foundation components for layout, navigation, forms, cards, sections, and UI states. These components are framework-agnostic and provide essential building blocks for any Blazor application.

**Key Features:**
- SSR-compatible components
- Framework-agnostic design
- Responsive layouts
- Accessibility support
- Performance optimized

### CMS Web Components
Content management components for displaying and rendering GitHub-based CMS content. These components handle content rendering, SEO metadata, and page-level layouts.

**Key Features:**
- GitHub CMS integration
- SEO optimization
- Content rendering
- Localization support
- Dynamic page generation

### CMS Admin Components
Administrative interface components for managing CMS content through a web-based admin panel. These components provide content editing, workflow management, and administrative controls.

**Key Features:**
- Content editor interface
- Workflow management
- User management
- Dashboard components
- Administrative controls

### Navigation Components
Advanced navigation and menu components with enhanced functionality for complex navigation scenarios.

**Key Features:**
- Multi-level menus
- Responsive navigation
- Scroll-to-top functionality
- Breadcrumb navigation
- Mobile-optimized

### Analytics Components
Analytics integration components supporting multiple providers including Google Analytics 4, Microsoft Clarity, Matomo, and Yandex Metrica.

**Key Features:**
- Multi-provider support
- Privacy compliance
- Performance tracking
- Custom event tracking
- GDPR compliance

### Theming Components
Theme management and styling components for consistent design systems and dark/light mode support.

**Key Features:**
- Theme switching
- CSS custom properties
- Design tokens
- Dark/light mode
- Brand customization

## Component Categories

### Layout Components
- **HeroSection**: Hero banners with background options
- **OsirionPageLayout**: Main page layout wrapper
- **OsirionFooter**: Footer component with links and branding
- **OsirionStickySidebar**: Sticky sidebar for documentation
- **OsirionBackgroundPattern**: Decorative background patterns

### Content Components
- **ContentView**: GitHub CMS content display
- **ContentRenderer**: Markdown content rendering
- **ContentList**: Content listing with pagination
- **OsirionHtmlRenderer**: Safe HTML rendering
- **SeoMetadataRenderer**: SEO metadata injection

### Navigation Components
- **EnhancedNavigation**: Advanced navigation menu
- **Menu**: Flexible menu component
- **MenuItem**: Individual menu items
- **MenuGroup**: Menu grouping and organization
- **OsirionBreadcrumbs**: Breadcrumb navigation
- **ScrollToTop**: Scroll-to-top functionality

### UI Components
- **OsirionFeatureCard**: Feature showcase cards
- **OsirionTestimonialCard**: Customer testimonial cards
- **OsirionSubscriptionCard**: Subscription/pricing cards
- **OsirionContactForm**: Contact form with validation
- **OsirionCookieConsent**: GDPR cookie consent

### State Components
- **OsirionPageLoading**: Loading state indicators
- **OsirionContentNotFound**: 404 error pages
- **ThemeToggle**: Dark/light theme switching

### Analytics Components
- **GA4Tracker**: Google Analytics 4 integration
- **ClarityTracker**: Microsoft Clarity integration
- **MatomoTracker**: Matomo analytics integration
- **YandexMetricaTracker**: Yandex Metrica integration

## Getting Started with Components

To use Osirion.Blazor components in your application:

1. **Install the packages** you need based on your requirements
2. **Configure services** in your `Program.cs` or `Startup.cs`
3. **Add component imports** to your `_Imports.razor`
4. **Use components** in your Razor pages and components

### Basic Setup Example

```csharp
// Program.cs
builder.Services.AddOsirionCore();
builder.Services.AddOsirionCms();
builder.Services.AddOsirionNavigation();
builder.Services.AddOsirionAnalytics();
builder.Services.AddOsirionTheming();
```

```razor
@* _Imports.razor *@
@using Osirion.Blazor.Components
@using Osirion.Blazor.Cms.Components
@using Osirion.Blazor.Navigation.Components
@using Osirion.Blazor.Analytics.Components
@using Osirion.Blazor.Theming.Components
```

### Component Usage Example

```razor
<HeroSection 
    Title="Welcome to Osirion.Blazor"
    Subtitle="Modern Blazor Components"
    Summary="Build beautiful, responsive applications with our component library"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/docs/getting-started"
    UseBackgroundImage="true"
    ImageUrl="/images/hero-bg.jpg" />

<ContentView ContentPath="/docs/introduction" />

<EnhancedNavigation>
    <MenuItem Text="Home" Url="/" />
    <MenuItem Text="Documentation" Url="/docs" />
    <MenuItem Text="Components" Url="/docs/components" />
</EnhancedNavigation>
```

## Best Practices

### Performance Optimization
- Use SSR-compatible components for better initial load times
- Implement lazy loading for heavy components
- Optimize images and assets used in components
- Use CSS containment where appropriate

### Accessibility
- All components follow WCAG 2.1 AA guidelines
- Semantic HTML structure is maintained
- Keyboard navigation is fully supported
- Screen reader compatibility is ensured

### Customization
- Components support CSS custom properties for theming
- Override component styles using CSS classes
- Use component parameters for behavioral customization
- Implement custom render fragments for advanced scenarios

### SEO Optimization
- Use semantic HTML elements
- Implement proper heading hierarchy
- Include meta tags and structured data
- Optimize for Core Web Vitals

## Documentation Structure

Each component module has detailed documentation including:

- **Component Overview**: Purpose and use cases
- **Parameters**: All available configuration options
- **Examples**: Code samples and live demos
- **Styling**: CSS customization options
- **Best Practices**: Implementation recommendations
- **Accessibility**: WCAG compliance information

## Module Documentation

- [Core Components](/docs/components/core) - Foundation UI components
- [CMS Web Components](/docs/components/cms-web) - Content management components
- [CMS Admin Components](/docs/components/cms-admin) - Administrative interface components
- [Navigation Components](/docs/components/navigation) - Advanced navigation components
- [Analytics Components](/docs/components/analytics) - Analytics integration components
- [Theming Components](/docs/components/theming) - Theme and styling components

## Support and Contribution

For questions, issues, or contributions:

- **GitHub Repository**: [Osirion.Blazor](https://github.com/obrana-boranija/Osirion.Blazor)
- **Documentation**: [getosirion.com](https://getosirion.com)
- **Issues**: Report bugs and feature requests on GitHub
- **Discussions**: Join community discussions on GitHub

The Osirion.Blazor component library is designed to accelerate Blazor application development while maintaining high standards for performance, accessibility, and maintainability.
