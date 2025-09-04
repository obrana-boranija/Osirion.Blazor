---
id: 'hero-section'
order: 1
layout: docs
title: HeroSection Component
permalink: /docs/components/core/hero-section
description: Complete documentation for the HeroSection component - a versatile hero section component for landing pages and article headers with multiple layout variants, background options, and call-to-action buttons.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Layout
tags:
- hero-section
- layout
- landing-page
- blazor
- components
is_featured: true
published: true
slug: hero-section
lang: en
custom_fields: {}
seo_properties:
  title: 'HeroSection Component - Osirion.Blazor Documentation'
  description: 'Complete guide to the HeroSection component with parameters, usage examples, and best practices for creating stunning hero sections in Blazor applications.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/hero-section'
  lang: en
  robots: index, follow
  og_title: 'HeroSection Component - Osirion.Blazor'
  og_description: 'Complete documentation for creating stunning hero sections in Blazor applications with the HeroSection component.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'HeroSection Component - Osirion.Blazor'
  twitter_description: 'Complete documentation for creating stunning hero sections in Blazor applications.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# HeroSection Component

The `HeroSection` component is a versatile and powerful component designed for creating stunning hero sections on landing pages, article headers, and promotional areas. It supports multiple layout variants, background options, call-to-action buttons, and metadata display.

## Overview

The HeroSection component provides a flexible foundation for creating impactful first impressions on your web pages. It supports both background images and side-by-side layouts, multiple variants for different use cases, and comprehensive customization options.

**Key Features:**
- Multiple layout variants (Hero, Jumbotron, Minimal)
- Background image or side image support
- Flexible content alignment options
- Call-to-action button integration
- Article metadata display
- Responsive design
- SSR compatibility
- Accessibility compliant

## Parameters

### Content Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | The main hero title text |
| `TitleContent` | `RenderFragment?` | `null` | Custom title content that overrides Title parameter |
| `Subtitle` | `string?` | `null` | The hero subtitle text |
| `SubtitleContent` | `RenderFragment?` | `null` | Custom subtitle content that overrides Subtitle parameter |
| `Summary` | `string?` | `null` | The hero summary/description text |
| `SummaryContent` | `RenderFragment?` | `null` | Custom summary content that overrides Summary parameter |
| `ChildContent` | `RenderFragment?` | `null` | Additional custom content |

### Image Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ImageUrl` | `string?` | `null` | The image URL for hero image |
| `ImageAlt` | `string?` | `null` | Alternative text for the image |
| `UseBackgroundImage` | `bool` | `false` | Whether to display image as background |
| `ImagePosition` | `Alignment` | `Right` | Position of side image (Left, Right) |
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | Background pattern overlay |

### Layout Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `HeroVariant` | `Hero` | Layout variant (Hero, Jumbotron, Minimal) |
| `Alignment` | `Alignment` | `Left` | Content text alignment |
| `MinHeight` | `string` | `"60vh"` | Minimum height of the hero section |
| `HasDivider` | `bool` | `true` | Whether to show bottom divider |

### Button Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowPrimaryButton` | `bool` | `true` | Whether to display primary button |
| `PrimaryButtonText` | `string?` | `null` | Primary button text |
| `PrimaryButtonUrl` | `string?` | `null` | Primary button URL |
| `ShowSecondaryButton` | `bool` | `true` | Whether to display secondary button |
| `SecondaryButtonText` | `string?` | `null` | Secondary button text |
| `SecondaryButtonUrl` | `string?` | `null` | Secondary button URL |

### Styling Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `BackgroundColor` | `string?` | `null` | Background color override |
| `TextColor` | `string?` | `null` | Text color override |

### Metadata Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowMetadata` | `bool` | `false` | Whether to display article metadata |
| `Author` | `string?` | `null` | Author name for metadata |
| `PublishDate` | `DateTime?` | `null` | Publication date for metadata |
| `ReadTime` | `string?` | `null` | Estimated read time |

## Layout Variants

### Hero Variant (Default)
The standard hero layout with balanced content and image sizing. Ideal for landing pages and main sections.

```razor
<HeroSection 
    Variant="HeroVariant.Hero"
    Title="Welcome to Our Platform"
    Subtitle="Build Amazing Applications"
    Summary="Discover the power of modern web development with our comprehensive component library."
    ImageUrl="/images/hero-image.jpg"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/getting-started"
    SecondaryButtonText="Learn More"
    SecondaryButtonUrl="/documentation" />
```

### Jumbotron Variant
Large, prominent hero section perfect for major announcements or product showcases.

```razor
<HeroSection 
    Variant="HeroVariant.Jumbotron"
    Title="Revolutionary New Features"
    Subtitle="Transform Your Workflow"
    Summary="Experience next-generation tools designed to accelerate your development process."
    UseBackgroundImage="true"
    ImageUrl="/images/jumbotron-bg.jpg"
    MinHeight="80vh"
    PrimaryButtonText="Explore Features"
    PrimaryButtonUrl="/features" />
```

### Minimal Variant
Clean, minimal hero section ideal for documentation pages and focused content.

```razor
<HeroSection 
    Variant="HeroVariant.Minimal"
    Title="Documentation"
    Summary="Comprehensive guides and references for developers."
    ShowPrimaryButton="false"
    ShowSecondaryButton="false"
    HasDivider="false" />
```

## Content Customization

### Using Custom Content Fragments

For advanced content customization, use the content fragment parameters:

```razor
<HeroSection>
    <TitleContent>
        <h1 class="custom-hero-title">
            <span class="highlight">Advanced</span> Component Library
        </h1>
    </TitleContent>
    
    <SubtitleContent>
        <p class="custom-subtitle">
            <strong>Blazor</strong> components for modern web applications
        </p>
    </SubtitleContent>
    
    <SummaryContent>
        <div class="custom-summary">
            <p>Build faster with our comprehensive set of UI components.</p>
            <ul class="feature-list">
                <li>SSR Compatible</li>
                <li>Accessibility First</li>
                <li>Performance Optimized</li>
            </ul>
        </div>
    </SummaryContent>
    
    <ChildContent>
        <div class="custom-content">
            <div class="stats-row">
                <div class="stat">
                    <span class="number">50+</span>
                    <span class="label">Components</span>
                </div>
                <div class="stat">
                    <span class="number">10k+</span>
                    <span class="label">Downloads</span>
                </div>
            </div>
        </div>
    </ChildContent>
</HeroSection>
```

## Image Configuration

### Background Image Setup

```razor
<HeroSection 
    Title="Stunning Visuals"
    Subtitle="Create Beautiful Experiences"
    UseBackgroundImage="true"
    ImageUrl="/images/hero-background.jpg"
    ImageAlt="Beautiful landscape background"
    BackgroundColor="#1a1a1a"
    TextColor="white"
    BackgroundPattern="BackgroundPatternType.Dots" />
```

### Side Image Layout

```razor
<HeroSection 
    Title="Feature Showcase"
    Summary="Detailed explanation of our key features."
    UseBackgroundImage="false"
    ImageUrl="/images/feature-screenshot.png"
    ImageAlt="Feature screenshot"
    ImagePosition="Alignment.Right" />
```

## Metadata Display

Enable metadata display for blog posts and articles:

```razor
<HeroSection 
    Title="@article.Title"
    Summary="@article.Summary"
    ShowMetadata="true"
    Author="@article.Author"
    PublishDate="@article.PublishDate"
    ReadTime="@article.ReadTime"
    ImageUrl="@article.FeaturedImage" />
```

## Responsive Behavior

The HeroSection component automatically adapts to different screen sizes:

- **Desktop**: Full layout with side-by-side content and images
- **Tablet**: Adjusted spacing and proportions
- **Mobile**: Stacked layout with optimized image sizes

## Styling and Customization

### CSS Custom Properties

```css
.osirion-hero-section {
    --hero-padding-top: 4rem;
    --hero-padding-bottom: 4rem;
    --hero-title-size: 3.5rem;
    --hero-subtitle-size: 1.5rem;
    --hero-summary-size: 1.125rem;
    --hero-button-spacing: 1rem;
}
```

### Custom CSS Classes

```css
.custom-hero {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.custom-hero .osirion-hero-title {
    font-weight: 800;
    letter-spacing: -0.025em;
}

.custom-hero .osirion-hero-summary {
    font-size: 1.25rem;
    opacity: 0.9;
}
```

Apply custom classes:

```razor
<HeroSection 
    Class="custom-hero"
    Title="Custom Styled Hero"
    Summary="This hero uses custom styling." />
```

## Accessibility Features

The HeroSection component includes comprehensive accessibility support:

- **Semantic HTML**: Uses proper heading hierarchy
- **Screen Reader Support**: All content is accessible to screen readers
- **Keyboard Navigation**: Buttons are keyboard accessible
- **Alt Text**: Image alt attributes for screen readers
- **Color Contrast**: Maintains WCAG AA compliance
- **Focus Management**: Proper focus indicators

## Best Practices

### Content Guidelines
- Keep titles concise and impactful (under 10 words)
- Limit summaries to 2-3 sentences
- Use clear, action-oriented button text
- Ensure sufficient color contrast

### Image Optimization
- Use high-quality images optimized for web
- Provide appropriate alt text for all images
- Consider different image sizes for responsive layouts
- Use modern image formats (WebP, AVIF) when possible

### Performance Considerations
- Optimize images for fast loading
- Use lazy loading for below-fold content
- Minimize the number of custom fonts
- Consider Critical CSS for above-fold content

### SEO Optimization
- Use descriptive, keyword-rich titles
- Include relevant meta descriptions
- Structure content with proper heading hierarchy
- Implement structured data when appropriate

## Common Use Cases

### Landing Page Hero
```razor
<HeroSection 
    Variant="HeroVariant.Hero"
    Title="Transform Your Business"
    Subtitle="With Modern Technology"
    Summary="Streamline operations and boost productivity with our innovative solutions."
    ImageUrl="/images/business-hero.jpg"
    PrimaryButtonText="Start Free Trial"
    PrimaryButtonUrl="/signup"
    SecondaryButtonText="View Demo"
    SecondaryButtonUrl="/demo" />
```

### Product Announcement
```razor
<HeroSection 
    Variant="HeroVariant.Jumbotron"
    Title="Introducing Version 2.0"
    Subtitle="More Powerful Than Ever"
    Summary="Experience enhanced performance, new features, and improved user experience."
    UseBackgroundImage="true"
    ImageUrl="/images/product-v2-bg.jpg"
    PrimaryButtonText="Download Now"
    PrimaryButtonUrl="/download" />
```

### Documentation Header
```razor
<HeroSection 
    Variant="HeroVariant.Minimal"
    Title="API Reference"
    Summary="Complete reference documentation for all available endpoints and methods."
    ShowMetadata="true"
    Author="Documentation Team"
    PublishDate="@DateTime.Now"
    ShowPrimaryButton="false"
    ShowSecondaryButton="false" />
```

### Article Header
```razor
<HeroSection 
    Title="@Model.Title"
    Summary="@Model.Summary"
    ShowMetadata="true"
    Author="@Model.Author"
    PublishDate="@Model.PublishDate"
    ReadTime="@Model.ReadTime"
    ImageUrl="@Model.FeaturedImage"
    ShowPrimaryButton="false"
    ShowSecondaryButton="false"
    HasDivider="true" />
```

## Integration Examples

### With CMS Content
```razor
@if (content != null)
{
    <HeroSection 
        Title="@content.Title"
        Summary="@content.Description"
        ImageUrl="@content.FeaturedImage"
        ShowMetadata="@(content.Type == "article")"
        Author="@content.Author"
        PublishDate="@content.DateCreated"
        ReadTime="@content.ReadTime" />
}
```

### With Analytics Tracking
```razor
<HeroSection 
    Title="Special Offer"
    Summary="Limited time promotion for new customers."
    PrimaryButtonText="Claim Offer"
    PrimaryButtonUrl="/offer"
    @onclick="TrackHeroButtonClick" />

@code {
    private void TrackHeroButtonClick()
    {
        // Analytics tracking logic
        AnalyticsService.TrackEvent("hero_button_click", new { section = "offer" });
    }
}
```

The HeroSection component provides a comprehensive solution for creating engaging hero sections that enhance user experience while maintaining performance and accessibility standards.
