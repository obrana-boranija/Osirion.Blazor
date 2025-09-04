---
id: 'core-components-overview'
order: 1
layout: docs
title: Core Components Overview
permalink: /docs/components/core/
description: Explore the Core Components module of Osirion.Blazor - essential building blocks for creating modern Blazor applications with consistent design and functionality.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Documentation
tags:
- blazor
- core-components
- cards
- forms
- layout
- states
- navigation
- ui-components
is_featured: true
published: true
slug: core
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Components Overview - Essential Blazor Components | Osirion.Blazor'
  description: 'Discover the Core Components module with essential building blocks for Blazor applications. Cards, Forms, Layout, States, and Navigation components.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/'
  lang: en
  robots: index, follow
  og_title: 'Core Components Overview - Osirion.Blazor'
  og_description: 'Essential building blocks for creating modern Blazor applications with consistent design and functionality.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Components Overview - Osirion.Blazor'
  twitter_description: 'Essential building blocks for creating modern Blazor applications.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Components Overview

The Core Components module provides essential building blocks for creating modern Blazor applications. These components offer consistent design patterns, robust functionality, and excellent user experience while maintaining flexibility for customization and theming.

## Module Architecture

The Core Components are organized into logical categories that address common UI needs:

### 🎴 [Cards Components](/docs/components/core/cards/)
Display information in structured, visually appealing containers:
- **[OsirionFeatureCard](/docs/components/core/cards/feature-card)** - Showcase features with icons, titles, and descriptions
- **OsirionSubscriptionCard** - Present pricing plans and subscription options
- **OsirionTestimonialCard** - Display customer testimonials and reviews

### 📝 [Forms Components](/docs/components/core/forms/)
Handle user input with validation and accessibility:
- **[OsirionContactForm](/docs/components/core/forms/contact-form)** - Complete contact form with validation and email integration

### 📐 [Layout Components](/docs/components/core/layout/)
Structure your application with flexible layouts:
- **[OsirionPageLayout](/docs/components/core/layout/page-layout)** - Flexible page layouts with sticky footer support
- **[OsirionPageLoading](/docs/components/core/layout/page-loading)** - Elegant loading states with animations
- **OsirionFooter** - Customizable footer component
- **OsirionStickySidebar** - Responsive sidebar with sticky positioning
- **OsirionBackgroundPattern** - Decorative background patterns

### 🔄 [States Components](/docs/components/core/states/)
Communicate application state to users:
- **[OsirionContentNotFound](/docs/components/core/states/content-not-found)** - Elegant 404 and error pages

### 🧭 [Navigation Components](/docs/components/core/navigation/)
Guide users through your application:
- **OsirionBreadcrumbs** - Hierarchical navigation breadcrumbs
- **OsirionReadMoreLink** - Expandable content links

### 🎠 [Carousel Components](/docs/components/core/carousels/)
Display content in interactive carousels:
- **OsirionTestimonialCarousel** - Rotating testimonials display
- **InfiniteLogoCarousel** - Continuous logo carousel

### 📄 [Sections Components](/docs/components/core/sections/)
Create structured page sections:
- **HeroSection** - Eye-catching hero sections
- **OsirionBaseSection** - Base section template
- **OsirionContactInfoSection** - Contact information display
- **OsirionResponsiveShowcaseSection** - Responsive content showcase

### 🔧 [Utility Components](/docs/components/core/utilities/)
Helper components for common tasks:
- **OsirionHtmlRenderer** - Safe HTML content rendering
- **OsirionArticleMetadata** - Article metadata display
- **OsirionCookieConsent** - GDPR-compliant cookie consent

## Key Features

### 🎨 **Framework Agnostic**
Compatible with popular CSS frameworks:
- Bootstrap 5.x
- Tailwind CSS 3.x
- Fluent UI
- Custom CSS frameworks

### ♿ **Accessibility First**
All components include:
- ARIA labels and roles
- Keyboard navigation support
- Screen reader compatibility
- High contrast mode support
- Focus management

### 📱 **Responsive Design**
Mobile-first approach with:
- Responsive breakpoints
- Touch-friendly interactions
- Adaptive layouts
- Performance optimization

### ⚡ **Performance Optimized**
Designed for optimal performance:
- Minimal bundle size
- Efficient rendering
- Memory management
- SSR compatibility

### 🛠️ **Developer Experience**
Built with developers in mind:
- Comprehensive documentation
- TypeScript support
- IntelliSense integration
- Extensive examples

## Getting Started

### Installation

```bash
dotnet add package Osirion.Blazor.Core
```

### Basic Setup

```csharp
// Program.cs or Startup.cs
builder.Services.AddOsirionBlazorCore();
```

### Import Namespace

```razor
@using Osirion.Blazor.Components
```

### Your First Component

```razor
<OsirionFeatureCard 
    Title="Welcome to Osirion.Blazor"
    Description="Build amazing Blazor applications with our component library."
    IconClass="fas fa-rocket"
    Variant="primary"
    Size="large" />
```

## Component Categories

### Cards & Content Display

Perfect for presenting information in visually appealing, structured formats. Cards are ideal for dashboards, product listings, feature showcases, and content previews.

```razor
<div class="row">
    <div class="col-md-4">
        <OsirionFeatureCard 
            Title="Fast Performance"
            Description="Optimized for speed and efficiency."
            IconClass="fas fa-bolt" />
    </div>
    <div class="col-md-4">
        <OsirionFeatureCard 
            Title="Mobile First"
            Description="Responsive design for all devices."
            IconClass="fas fa-mobile-alt" />
    </div>
    <div class="col-md-4">
        <OsirionFeatureCard 
            Title="Accessible"
            Description="Built with accessibility in mind."
            IconClass="fas fa-universal-access" />
    </div>
</div>
```

### Forms & User Input

Robust form components with built-in validation, accessibility features, and integration capabilities for handling user input effectively.

```razor
<OsirionContactForm 
    Title="Get in Touch"
    ShowSubject="true"
    ShowPhoneNumber="true"
    RequiredFields="@(new[] { "Name", "Email", "Message" })"
    OnSubmitSuccess="HandleFormSuccess"
    OnSubmitError="HandleFormError" />
```

### Layout & Structure

Flexible layout components that provide consistent structure and responsive behavior across your application.

```razor
<OsirionPageLayout StickyFooter="true">
    <Header>
        <nav class="navbar">
            <!-- Navigation content -->
        </nav>
    </Header>
    
    <Body>
        <main>
            <!-- Page content -->
        </main>
    </Body>
    
    <Footer>
        <footer>
            <!-- Footer content -->
        </footer>
    </Footer>
</OsirionPageLayout>
```

### States & Feedback

Communicate application state and provide user feedback with loading indicators, error pages, and status messages.

```razor
@if (isLoading)
{
    <OsirionPageLoading 
        Message="Loading awesome content..."
        AnimationType="Pulse"
        ShowProgress="true"
        Progress="loadingProgress" />
}
else if (hasError)
{
    <OsirionContentNotFound 
        Title="Something went wrong"
        Message="We're working to fix this issue."
        ShowSuggestions="true"
        Suggestions="@errorPageSuggestions" />
}
else
{
    <!-- Your content here -->
}
```

## Design Principles

### Consistency
All components follow consistent design patterns, naming conventions, and behavior patterns to ensure a cohesive development experience.

### Flexibility
Components are designed to be flexible and customizable while maintaining their core functionality and accessibility features.

### Composability
Components can be easily combined and composed to create complex UI patterns and custom solutions.

### Performance
Optimized for rendering performance, memory usage, and bundle size to ensure fast, responsive applications.

## Integration Examples

### With Bootstrap

```razor
<!-- Bootstrap-styled components -->
<OsirionFeatureCard 
    Class="shadow-sm border-0"
    Variant="primary"
    Title="Bootstrap Integration"
    Description="Seamless integration with Bootstrap utilities." />
```

### With Tailwind CSS

```razor
<!-- Tailwind-styled components -->
<OsirionFeatureCard 
    Class="bg-white rounded-lg shadow-lg hover:shadow-xl transition-shadow"
    Title="Tailwind Integration"
    Description="Perfect harmony with Tailwind CSS classes." />
```

### With Custom CSS

```razor
<!-- Custom-styled components -->
<OsirionFeatureCard 
    Class="custom-card theme-dark"
    Style="--card-bg: #1a1a1a; --card-color: #ffffff;"
    Title="Custom Styling"
    Description="Complete control over appearance." />
```

## Advanced Features

### Theming Support

```csharp
// Configure custom themes
builder.Services.Configure<OsirionThemeOptions>(options =>
{
    options.DefaultTheme = "dark";
    options.EnableThemeToggle = true;
    options.CustomThemes.Add("corporate", new ThemeDefinition
    {
        PrimaryColor = "#0066cc",
        SecondaryColor = "#6c757d",
        FontFamily = "Inter, sans-serif"
    });
});
```

### Localization

```razor
@inject IStringLocalizer<SharedResources> Localizer

<OsirionContactForm 
    Title="@Localizer["ContactForm.Title"]"
    SubmitButtonText="@Localizer["ContactForm.Submit"]"
    ValidationMessages="@GetLocalizedValidationMessages()" />
```

### Custom Validation

```csharp
public class CustomContactFormModel : ContactFormModel
{
    [CustomValidation(typeof(BusinessRules), nameof(BusinessRules.ValidateBusinessEmail))]
    public override string Email { get; set; } = "";
}
```

## Best Practices

### Component Usage

1. **Consistent Patterns**: Use components consistently across your application
2. **Proper Sizing**: Choose appropriate sizes for different contexts
3. **Accessibility**: Always provide proper labels and descriptions
4. **Performance**: Use loading states for better user experience
5. **Error Handling**: Implement proper error handling and recovery

### Styling Guidelines

1. **Theme Consistency**: Stick to your application's design system
2. **Responsive Design**: Test components across different screen sizes
3. **Color Contrast**: Ensure sufficient color contrast for accessibility
4. **Typography**: Use consistent typography hierarchy
5. **Spacing**: Maintain consistent spacing patterns

### Development Tips

1. **Component Composition**: Build complex UIs by composing simple components
2. **State Management**: Use appropriate state management patterns
3. **Event Handling**: Implement proper event handling and cleanup
4. **Testing**: Write tests for component behavior and accessibility
5. **Documentation**: Document custom configurations and usage patterns

## Browser Support

Core Components support all modern browsers:

- **Chrome**: 90+
- **Firefox**: 88+
- **Safari**: 14+
- **Edge**: 90+

## Next Steps

Ready to start building? Explore the individual component documentation:

1. **[Cards Components](/docs/components/core/cards/)** - Learn about all card variants and customization options
2. **[Forms Components](/docs/components/core/forms/)** - Master form creation and validation
3. **[Layout Components](/docs/components/core/layout/)** - Structure your application effectively
4. **[States Components](/docs/components/core/states/)** - Handle application states gracefully

### Quick Links

- 🚀 [Getting Started Guide](/docs/getting-started)
- 📖 [API Reference](/docs/api-reference)
- 🎨 [Theming Guide](/docs/theming)
- ♿ [Accessibility Guide](/docs/accessibility)
- 🧪 [Testing Guide](/docs/testing)
- 📱 [Examples & Demos](/examples)

The Core Components module provides everything you need to build modern, accessible, and performant Blazor applications with confidence.
