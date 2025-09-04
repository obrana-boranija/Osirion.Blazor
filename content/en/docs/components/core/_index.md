---
id: 'core-components-overview'
order: 1
layout: docs
title: Core Components Overview
permalink: /docs/components/core
description: Complete overview of Osirion.Blazor Core components - foundation UI components including layout, navigation, forms, cards, sections, and states for building modern Blazor applications.
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
- layout
- navigation
- forms
- ui-components
is_featured: true
published: true
slug: core
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Components - Osirion.Blazor Foundation UI Components'
  description: 'Explore Osirion.Blazor Core components for layout, navigation, forms, cards, and UI states. Foundation components for modern Blazor applications.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core'
  lang: en
  robots: index, follow
  og_title: 'Core Components - Osirion.Blazor'
  og_description: 'Foundation UI components for building modern Blazor applications with SSR compatibility.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Components - Osirion.Blazor'
  twitter_description: 'Foundation UI components for building modern Blazor applications.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Components Overview

The Osirion.Blazor Core module provides essential UI components that serve as the foundation for building modern Blazor applications. These components are designed to be framework-agnostic, SSR-compatible, and highly customizable while maintaining excellent performance and accessibility standards.

## Module Features

**Framework Agnostic**: Works with Bootstrap, Tailwind CSS, Fluent UI, and custom CSS frameworks
**SSR Compatible**: Full Server-Side Rendering support for optimal performance
**Accessibility First**: WCAG 2.1 AA compliant with comprehensive screen reader support
**Performance Optimized**: Minimal bundle size with efficient rendering
**Responsive Design**: Mobile-first responsive layouts across all components
**Customizable Theming**: CSS custom properties and comprehensive styling options

## Component Categories

### Layout Components

Essential components for page structure and layout organization.

#### [HeroSection](/docs/components/core/hero-section)
Versatile hero section component for landing pages and article headers.

**Key Features:**
- Multiple layout variants (Hero, Jumbotron, Minimal)
- Background image or side image support
- Call-to-action button integration
- Article metadata display
- Responsive design with mobile optimization

```razor
<HeroSection 
    Title="Welcome to Our Platform"
    Subtitle="Build Amazing Applications"
    Summary="Discover the power of modern web development."
    ImageUrl="/images/hero.jpg"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/getting-started" />
```

#### OsirionPageLayout
Main page layout wrapper with header, content, and footer areas.

**Key Features:**
- Flexible layout structure
- Header and footer integration
- Sidebar support
- Responsive breakpoints
- Content area optimization

```razor
<OsirionPageLayout>
    <HeaderContent>
        <nav><!-- Navigation content --></nav>
    </HeaderContent>
    
    <MainContent>
        <!-- Page content -->
    </MainContent>
    
    <FooterContent>
        <OsirionFooter />
    </FooterContent>
</OsirionPageLayout>
```

#### OsirionFooter
Comprehensive footer component with links, branding, and social media.

**Key Features:**
- Multi-column layout support
- Social media link integration
- Copyright and legal links
- Newsletter signup integration
- Responsive design

```razor
<OsirionFooter 
    CompanyName="Your Company"
    CopyrightYear="2025"
    ShowSocialLinks="true"
    ShowNewsletter="true" />
```

#### OsirionStickySidebar
Sticky sidebar component ideal for documentation and content navigation.

**Key Features:**
- Sticky positioning
- Scroll-aware behavior
- Responsive collapse
- Content synchronization
- Accessibility support

```razor
<OsirionStickySidebar>
    <nav class="sidebar-nav">
        <!-- Sidebar navigation content -->
    </nav>
</OsirionStickySidebar>
```

#### OsirionBackgroundPattern
Decorative background patterns for visual enhancement.

**Key Features:**
- Multiple pattern types (Dots, Grid, Diagonal, Wave)
- Customizable colors and opacity
- Performance optimized SVG patterns
- Responsive scaling

```razor
<OsirionBackgroundPattern 
    BackgroundPattern="BackgroundPatternType.Dots"
    OpacityLevel="0.1" />
```

### Content Components

Components for displaying and managing content presentation.

#### [OsirionArticleMetadata](/docs/components/core/article-metadata)
Displays article metadata including author, date, and read time.

**Key Features:**
- Author information with icon
- Customizable date formatting
- Read time estimation
- Conditional rendering
- Cultural date support

```razor
<OsirionArticleMetadata 
    Author="John Doe"
    PublishDate="@DateTime.Now"
    ReadTime="5"
    DateFormat="MMM dd, yyyy" />
```

#### OsirionHtmlRenderer
Safe HTML content rendering with sanitization and security features.

**Key Features:**
- XSS protection
- HTML sanitization
- Markdown rendering support
- Code syntax highlighting
- Performance optimization

```razor
<OsirionHtmlRenderer 
    Content="@htmlContent"
    SanitizeHtml="true"
    EnableSyntaxHighlighting="true" />
```

### Card Components

Flexible card components for displaying structured content.

#### OsirionFeatureCard
Feature showcase cards with extensive customization options.

**Key Features:**
- Image and icon support
- Call-to-action buttons
- Hover effects and animations
- Border and shadow customization
- Content alignment options

```razor
<OsirionFeatureCard 
    Title="Advanced Analytics"
    Description="Track performance with detailed insights."
    ImageUrl="/images/analytics-icon.svg"
    ActionButtonText="Learn More"
    ActionButtonUrl="/analytics" />
```

#### OsirionTestimonialCard
Customer testimonial and review display cards.

**Key Features:**
- Customer photo integration
- Star rating display
- Quote formatting
- Company information
- Responsive layout

```razor
<OsirionTestimonialCard 
    CustomerName="Sarah Johnson"
    CustomerTitle="CEO, TechCorp"
    CustomerPhoto="/images/customers/sarah.jpg"
    TestimonialText="Outstanding service and results!"
    Rating="5" />
```

#### OsirionSubscriptionCard
Pricing and subscription plan display cards.

**Key Features:**
- Pricing information display
- Feature list management
- Popular plan highlighting
- Action button integration
- Comparison support

```razor
<OsirionSubscriptionCard 
    PlanName="Professional"
    Price="$29"
    PriceFrequency="per month"
    Features="@proFeatures"
    IsPopular="true"
    ActionButtonText="Choose Plan" />
```

### Form Components

Form elements with validation and accessibility features.

#### OsirionContactForm
Comprehensive contact form with validation and email integration.

**Key Features:**
- Built-in form validation
- Email service integration
- Anti-spam protection
- Accessibility compliance
- Customizable fields

```razor
<OsirionContactForm 
    EmailService="@emailService"
    RequiredFields="@requiredFields"
    ShowCaptcha="true"
    SuccessMessage="Thank you for your message!" />
```

### Navigation Components

Navigation and wayfinding components.

#### OsirionBreadcrumbs
Breadcrumb navigation for hierarchical site structure.

**Key Features:**
- Automatic path generation
- Custom breadcrumb items
- SEO-friendly markup
- Accessibility support
- Mobile responsive

```razor
<OsirionBreadcrumbs>
    <BreadcrumbItem Text="Home" Url="/" />
    <BreadcrumbItem Text="Documentation" Url="/docs" />
    <BreadcrumbItem Text="Components" IsActive="true" />
</OsirionBreadcrumbs>
```

#### OsirionReadMoreLink
Smart read more/less functionality for content truncation.

**Key Features:**
- Content length detection
- Smooth expand/collapse
- Customizable trigger text
- Accessibility support
- Performance optimized

```razor
<OsirionReadMoreLink 
    Content="@longContent"
    MaxLength="200"
    ReadMoreText="Continue reading"
    ReadLessText="Show less" />
```

### State Components

Components for handling different application states.

#### OsirionPageLoading
Loading state indicators and progress displays.

**Key Features:**
- Multiple loading animations
- Progress percentage display
- Custom loading messages
- Accessible loading states
- Performance optimized

```razor
<OsirionPageLoading 
    IsVisible="@isLoading"
    LoadingText="Loading content..."
    ShowProgress="true"
    ProgressPercentage="@loadingProgress" />
```

#### OsirionContentNotFound
404 error pages and content not found states.

**Key Features:**
- Customizable error messages
- Navigation suggestions
- Search integration
- Brand consistency
- SEO considerations

```razor
<OsirionContentNotFound 
    Title="Page Not Found"
    Message="The requested page could not be found."
    ShowHomeLink="true"
    ShowSearchBox="true" />
```

### Popup Components

Modal and popup components for user interactions.

#### OsirionCookieConsent
GDPR-compliant cookie consent management.

**Key Features:**
- GDPR compliance
- Cookie categorization
- Preference management
- Customizable messaging
- Legal compliance tracking

```razor
<OsirionCookieConsent 
    PrivacyPolicyUrl="/privacy"
    CookiePolicyUrl="/cookies"
    ShowPreferences="true"
    AllowAnalytics="true" />
```

### Section Components

Large-scale section components for page organization.

#### OsirionBaseSection
Base section component for consistent section styling.

**Key Features:**
- Consistent spacing and layout
- Background and pattern support
- Container width management
- Responsive behavior
- Theme integration

```razor
<OsirionBaseSection 
    BackgroundColor="#f8f9fa"
    HasPattern="true"
    PatternType="Dots"
    ContainerSize="Large">
    <!-- Section content -->
</OsirionBaseSection>
```

#### OsirionContactInfoSection
Contact information display section.

**Key Features:**
- Contact details display
- Map integration
- Social media links
- Office hours display
- Multi-location support

```razor
<OsirionContactInfoSection 
    CompanyName="Your Company"
    Address="123 Main St, City, State"
    Phone="+1 (555) 123-4567"
    Email="contact@company.com"
    ShowMap="true" />
```

#### OsirionResponsiveShowcaseSection
Responsive showcase section for products or features.

**Key Features:**
- Responsive grid layouts
- Image optimization
- Content overlay support
- Interactive elements
- Performance optimization

```razor
<OsirionResponsiveShowcaseSection 
    Items="@showcaseItems"
    ColumnsLarge="3"
    ColumnsMedium="2"
    ColumnsSmall="1"
    ShowOverlay="true" />
```

### Carousel Components

Interactive carousel and slider components.

#### InfiniteLogoCarousel
Infinite scrolling logo carousel for brand showcases.

**Key Features:**
- Infinite scroll animation
- Auto-pause on hover
- Responsive sizing
- Logo optimization
- Performance efficient

```razor
<InfiniteLogoCarousel 
    Logos="@clientLogos"
    AnimationSpeed="Medium"
    PauseOnHover="true"
    ShowGrayscale="true" />
```

#### OsirionTestimonialCarousel
Rotating testimonial display carousel.

**Key Features:**
- Auto-rotation
- Navigation controls
- Touch/swipe support
- Responsive design
- Accessibility compliant

```razor
<OsirionTestimonialCarousel 
    Testimonials="@customerTestimonials"
    AutoRotate="true"
    RotationSpeed="5000"
    ShowNavigation="true" />
```

## Installation and Setup

### Package Installation

```bash
dotnet add package Osirion.Blazor.Core
```

### Service Registration

```csharp
// Program.cs
builder.Services.AddOsirionCore(options =>
{
    options.EnableAnalytics = true;
    options.DefaultTheme = "light";
    options.EnablePerformanceOptimizations = true;
});
```

### Import Statements

```razor
@* _Imports.razor *@
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models
@using Osirion.Blazor.Core.Enums
```

### CSS Integration

```html
<!-- Bootstrap Theme -->
<link href="_content/Osirion.Blazor.Core/css/osirion-bootstrap.css" rel="stylesheet" />

<!-- Tailwind Theme -->
<link href="_content/Osirion.Blazor.Core/css/osirion-tailwind.css" rel="stylesheet" />

<!-- Custom Theme -->
<link href="_content/Osirion.Blazor.Core/css/osirion-core.css" rel="stylesheet" />
```

## Design Principles

### Consistency
All Core components follow consistent design patterns, naming conventions, and behavior expectations across the entire library.

### Flexibility
Components provide extensive customization options while maintaining ease of use for common scenarios.

### Performance
Optimized for minimal bundle size, efficient rendering, and excellent Core Web Vitals scores.

### Accessibility
Built with accessibility as a first-class concern, ensuring all components meet WCAG 2.1 AA standards.

### Responsiveness
Mobile-first responsive design ensures components work seamlessly across all device sizes.

## Best Practices

### Component Usage
- Use semantic HTML structure provided by components
- Leverage built-in accessibility features
- Customize through CSS custom properties when possible
- Follow responsive design patterns

### Performance Optimization
- Use lazy loading for below-fold components
- Optimize images used in components
- Implement proper caching strategies
- Monitor bundle size impact

### Styling Guidelines
- Use CSS custom properties for theming
- Maintain consistent spacing using design tokens
- Implement proper focus indicators
- Support dark/light theme variations

### Accessibility Implementation
- Provide alternative text for images
- Ensure proper keyboard navigation
- Maintain color contrast ratios
- Test with screen readers

## Component Documentation

Each component includes comprehensive documentation with:

- **Parameter Reference**: Complete parameter listing with types and descriptions
- **Usage Examples**: Practical code examples for common scenarios
- **Styling Guide**: CSS customization options and best practices
- **Accessibility Notes**: Accessibility features and considerations
- **Performance Tips**: Optimization recommendations

## Browser Support

The Core components support:

- **Modern Browsers**: Chrome 90+, Firefox 88+, Safari 14+, Edge 90+
- **Mobile Browsers**: iOS Safari 14+, Chrome Mobile 90+
- **Legacy Support**: IE11 support available with polyfills

## Migration and Compatibility

- **Blazor Server**: Full compatibility with Blazor Server applications
- **Blazor WebAssembly**: Full compatibility with Blazor WASM applications
- **Blazor SSR**: Optimized for Server-Side Rendering scenarios
- **Hybrid Applications**: Compatible with MAUI Blazor Hybrid apps

The Core components module provides the essential building blocks for creating professional, accessible, and performant Blazor applications while maintaining flexibility for diverse use cases and design requirements.
