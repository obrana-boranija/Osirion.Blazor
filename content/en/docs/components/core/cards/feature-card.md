---
id: 'osirion-feature-card'
order: 1
layout: docs
title: OsirionFeatureCard Component
permalink: /docs/components/core/cards/feature-card
description: Learn how to use the OsirionFeatureCard component to create flexible, visually appealing feature cards with images, content, and interactive elements.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Cards
tags:
- blazor
- feature-card
- ui-components
- cards
- responsive-design
- content-display
is_featured: true
published: true
slug: feature-card
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionFeatureCard Component - Flexible Content Cards | Osirion.Blazor'
  description: 'Create stunning feature cards with the OsirionFeatureCard component. Supports images, content alignment, visual effects, and comprehensive customization.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/cards/feature-card'
  lang: en
  robots: index, follow
  og_title: 'OsirionFeatureCard Component - Osirion.Blazor'
  og_description: 'Create stunning feature cards with flexible layouts, visual effects, and comprehensive customization options.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionFeatureCard Component - Osirion.Blazor'
  twitter_description: 'Create stunning feature cards with flexible layouts and visual effects.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionFeatureCard Component

The OsirionFeatureCard component is a highly flexible and customizable card component designed to showcase features, services, or content with rich visual styling. It supports multiple layout configurations, visual effects, image positioning, and interactive elements while maintaining excellent accessibility and performance.

## Component Overview

OsirionFeatureCard provides a comprehensive solution for displaying content in an attractive card format. It combines image display, text content, action buttons, and visual effects into a cohesive, responsive component that adapts to different screen sizes and design requirements.

### Key Features

**Flexible Layout**: Multiple image positions (top, bottom, left, right) with responsive behavior
**Visual Effects**: Configurable borders, shadows, and hover transformations
**Content Alignment**: Multiple alignment options for title, description, and content
**Action Support**: Built-in support for links, buttons, and read-more functionality
**Image Handling**: Lazy loading, alt text, and transform effects for images
**Accessibility**: Full keyboard navigation, ARIA support, and screen reader compatibility
**Framework Agnostic**: Works with Bootstrap, Tailwind CSS, and custom frameworks

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | The feature card title text. |
| `Description` | `string?` | `null` | The feature card description text. |
| `ShowDescription` | `bool` | `true` | Whether to display the description text. |
| `ChildContent` | `RenderFragment?` | `null` | Additional content to render within the card. |
| `Url` | `string?` | `null` | General URL for the entire card (makes card clickable). |
| `ImageUrl` | `string?` | `null` | URL of the feature card image. |
| `ImageAlt` | `string?` | `null` | Alt text for the image. Defaults to Title if not specified. |
| `ImageLazyLoading` | `bool` | `true` | Whether image should use lazy loading. |
| `ImageTransformOnHover` | `bool` | `false` | Whether image should transform on hover. |
| `ImagePosition` | `ImagePosition` | `Left` | Position of image relative to content (Top, Bottom, Left, Right). |
| `CardSize` | `CardSize` | `Normal` | Size variant of the card (Small, Normal, Large). |
| `ContentAlignment` | `ContentAlignment` | `CenterLeft` | Content alignment within the card. |
| `ContentFontSize` | `FontSizeVariant` | `Normal` | Font size variant for content text. |
| `BorderTiming` | `EffectTiming` | `Always` | When border should be visible (Never, Always, OnHover). |
| `BorderColor` | `ThemeColor` | `Neutral` | Color theme for the border. |
| `ShadowTiming` | `EffectTiming` | `OnHover` | When shadow should be visible (Never, Always, OnHover). |
| `ShadowColor` | `ThemeColor` | `Neutral` | Color theme for the shadow. |
| `TransformTiming` | `EffectTiming` | `OnHover` | When transform effect should be applied. |
| `ShowReadMoreLink` | `bool` | `false` | Whether to show read more link. |
| `ReadMoreText` | `string` | `"Read more"` | Text for the read more link. |
| `ReadMoreUrl` | `string?` | `null` | URL for the read more link. |
| `PillowButtons` | `List<PillowButton>?` | `null` | Collection of pillow-style action buttons. |

## Basic Usage

### Simple Feature Card

```razor
@using Osirion.Blazor.Components

<OsirionFeatureCard 
    Title="Advanced Analytics"
    Description="Get detailed insights into your application performance with our comprehensive analytics dashboard."
    ImageUrl="/images/analytics-feature.jpg"
    ImageAlt="Analytics Dashboard Screenshot" />
```

### Card with Image Positioning

```razor
<!-- Image on top -->
<OsirionFeatureCard 
    Title="Mobile Responsive"
    Description="Fully responsive design that works perfectly on all devices and screen sizes."
    ImageUrl="/images/mobile-responsive.jpg"
    ImagePosition="ImagePosition.Top"
    CardSize="CardSize.Large" />

<!-- Image on right -->
<OsirionFeatureCard 
    Title="Security First"
    Description="Enterprise-grade security with encryption, authentication, and compliance standards."
    ImageUrl="/images/security-feature.jpg"
    ImagePosition="ImagePosition.Right"
    ContentAlignment="ContentAlignment.CenterLeft" />
```

### Interactive Card with Visual Effects

```razor
<OsirionFeatureCard 
    Title="Real-time Collaboration"
    Description="Work together seamlessly with real-time updates and collaborative editing features."
    ImageUrl="/images/collaboration.jpg"
    Url="/features/collaboration"
    BorderTiming="EffectTiming.OnHover"
    BorderColor="ThemeColor.Primary"
    ShadowTiming="EffectTiming.OnHover"
    ShadowColor="ThemeColor.Primary"
    TransformTiming="EffectTiming.OnHover"
    ImageTransformOnHover="true"
    ShowReadMoreLink="true"
    ReadMoreText="Learn More"
    ReadMoreUrl="/features/collaboration" />
```

## Advanced Usage

### Feature Grid Layout

```razor
<div class="feature-grid">
    <OsirionFeatureCard 
        Title="Performance Optimized"
        Description="Lightning-fast performance with optimized rendering and smart caching strategies."
        ImageUrl="/images/performance.jpg"
        ImagePosition="ImagePosition.Top"
        CardSize="CardSize.Normal"
        ShadowTiming="EffectTiming.Always"
        TransformTiming="EffectTiming.OnHover" />
    
    <OsirionFeatureCard 
        Title="Accessibility First"
        Description="WCAG compliant with full keyboard navigation and screen reader support."
        ImageUrl="/images/accessibility.jpg"
        ImagePosition="ImagePosition.Top"
        CardSize="CardSize.Normal"
        ShadowTiming="EffectTiming.Always"
        TransformTiming="EffectTiming.OnHover" />
    
    <OsirionFeatureCard 
        Title="Developer Friendly"
        Description="Rich API, comprehensive documentation, and excellent developer experience."
        ImageUrl="/images/developer-tools.jpg"
        ImagePosition="ImagePosition.Top"
        CardSize="CardSize.Normal"
        ShadowTiming="EffectTiming.Always"
        TransformTiming="EffectTiming.OnHover" />
</div>

<style>
.feature-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 2rem;
    margin: 2rem 0;
}

@media (max-width: 768px) {
    .feature-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
}
</style>
```

### Card with Custom Content and Actions

```razor
<OsirionFeatureCard 
    Title="Premium Features"
    Description="Unlock advanced capabilities with our premium plan."
    ImageUrl="/images/premium-features.jpg"
    ImagePosition="ImagePosition.Left"
    CardSize="CardSize.Large"
    BorderTiming="EffectTiming.Always"
    BorderColor="ThemeColor.Primary"
    PillowButtons="@premiumPillowButtons">
    
    <div class="custom-content">
        <ul class="feature-list">
            <li>Advanced Analytics Dashboard</li>
            <li>Priority Customer Support</li>
            <li>Custom Integrations</li>
            <li>White-label Options</li>
        </ul>
        
        <div class="pricing-info">
            <span class="price">$29</span>
            <span class="period">/month</span>
        </div>
    </div>
</OsirionFeatureCard>

@code {
    private List<PillowButton> premiumPillowButtons = new()
    {
        new PillowButton
        {
            Text = "Start Free Trial",
            Url = "/trial",
            Color = ThemeColor.Primary,
            Size = ButtonSize.Normal
        },
        new PillowButton
        {
            Text = "View Pricing",
            Url = "/pricing",
            Color = ThemeColor.Secondary,
            Size = ButtonSize.Normal
        }
    };
}

<style>
.custom-content {
    margin: 1rem 0;
}

.feature-list {
    list-style: none;
    padding: 0;
    margin: 1rem 0;
}

.feature-list li {
    padding: 0.5rem 0;
    border-bottom: 1px solid var(--border-color, #e5e7eb);
    position: relative;
    padding-left: 1.5rem;
}

.feature-list li::before {
    content: "✓";
    position: absolute;
    left: 0;
    color: var(--success-color, #10b981);
    font-weight: bold;
}

.pricing-info {
    display: flex;
    align-items: baseline;
    gap: 0.25rem;
    margin-top: 1rem;
}

.price {
    font-size: 2rem;
    font-weight: bold;
    color: var(--primary-color, #3b82f6);
}

.period {
    color: var(--text-muted, #6b7280);
    font-size: 1rem;
}
</style>
```

### Testimonial-Style Card

```razor
<OsirionFeatureCard 
    Title="Sarah Johnson, CTO"
    Description="This platform has transformed how our team collaborates. The real-time features are incredible!"
    ImageUrl="/images/testimonial-sarah.jpg"
    ImagePosition="ImagePosition.Left"
    CardSize="CardSize.Large"
    ContentAlignment="ContentAlignment.CenterLeft"
    BorderTiming="EffectTiming.Always"
    BorderColor="ThemeColor.Neutral"
    ShadowTiming="EffectTiming.OnHover"
    ContentFontSize="FontSizeVariant.Large">
    
    <div class="testimonial-footer">
        <div class="company-info">
            <strong>TechCorp Solutions</strong>
            <div class="rating">
                ⭐⭐⭐⭐⭐
            </div>
        </div>
    </div>
</OsirionFeatureCard>

<style>
.testimonial-footer {
    margin-top: 1rem;
    padding-top: 1rem;
    border-top: 1px solid var(--border-color, #e5e7eb);
}

.company-info strong {
    color: var(--primary-color, #3b82f6);
}

.rating {
    margin-top: 0.5rem;
    font-size: 1.1rem;
}
</style>
```

### Product Showcase Grid

```razor
<div class="product-showcase">
    @foreach (var product in products)
    {
        <OsirionFeatureCard 
            Title="@product.Name"
            Description="@product.Description"
            ImageUrl="@product.ImageUrl"
            ImagePosition="ImagePosition.Top"
            CardSize="CardSize.Normal"
            Url="@($"/products/{product.Slug}")"
            ShadowTiming="EffectTiming.OnHover"
            TransformTiming="EffectTiming.OnHover"
            ShowReadMoreLink="true"
            ReadMoreText="View Product"
            ReadMoreUrl="@($"/products/{product.Slug}")"
            PillowButtons="@GetProductButtons(product)" />
    }
</div>

@code {
    private List<Product> products = new()
    {
        new Product
        {
            Name = "Analytics Pro",
            Description = "Advanced analytics platform with real-time dashboards and reporting capabilities.",
            ImageUrl = "/images/products/analytics-pro.jpg",
            Slug = "analytics-pro",
            Price = 49.99m
        },
        new Product
        {
            Name = "Security Suite",
            Description = "Comprehensive security solution with threat detection and automated responses.",
            ImageUrl = "/images/products/security-suite.jpg",
            Slug = "security-suite",
            Price = 79.99m
        },
        new Product
        {
            Name = "Collaboration Hub",
            Description = "Team collaboration platform with chat, video calls, and project management.",
            ImageUrl = "/images/products/collaboration-hub.jpg",
            Slug = "collaboration-hub",
            Price = 29.99m
        }
    };
    
    private List<PillowButton> GetProductButtons(Product product)
    {
        return new List<PillowButton>
        {
            new PillowButton
            {
                Text = $"${product.Price:F2}",
                Url = $"/products/{product.Slug}/purchase",
                Color = ThemeColor.Primary,
                Size = ButtonSize.Normal
            },
            new PillowButton
            {
                Text = "Try Free",
                Url = $"/products/{product.Slug}/trial",
                Color = ThemeColor.Secondary,
                Size = ButtonSize.Normal
            }
        };
    }
    
    public class Product
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Slug { get; set; } = "";
        public decimal Price { get; set; }
    }
}

<style>
.product-showcase {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
    gap: 2rem;
    margin: 2rem 0;
}
</style>
```

## Layout Variations

### Different Image Positions

```razor
<div class="layout-examples">
    <!-- Top Image Layout -->
    <div class="layout-example">
        <h3>Top Image</h3>
        <OsirionFeatureCard 
            Title="Top Image Layout"
            Description="Image positioned above the content for a classic card design."
            ImageUrl="/images/sample-feature.jpg"
            ImagePosition="ImagePosition.Top"
            CardSize="CardSize.Normal" />
    </div>
    
    <!-- Left Image Layout -->
    <div class="layout-example">
        <h3>Left Image</h3>
        <OsirionFeatureCard 
            Title="Left Image Layout"
            Description="Image positioned to the left of content for horizontal layout."
            ImageUrl="/images/sample-feature.jpg"
            ImagePosition="ImagePosition.Left"
            CardSize="CardSize.Large" />
    </div>
    
    <!-- Right Image Layout -->
    <div class="layout-example">
        <h3>Right Image</h3>
        <OsirionFeatureCard 
            Title="Right Image Layout"
            Description="Image positioned to the right of content with text-first approach."
            ImageUrl="/images/sample-feature.jpg"
            ImagePosition="ImagePosition.Right"
            CardSize="CardSize.Large" />
    </div>
    
    <!-- Bottom Image Layout -->
    <div class="layout-example">
        <h3>Bottom Image</h3>
        <OsirionFeatureCard 
            Title="Bottom Image Layout"
            Description="Image positioned below content for content-first presentation."
            ImageUrl="/images/sample-feature.jpg"
            ImagePosition="ImagePosition.Bottom"
            CardSize="CardSize.Normal" />
    </div>
</div>

<style>
.layout-examples {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 2rem;
    margin: 2rem 0;
}

.layout-example h3 {
    margin-bottom: 1rem;
    color: var(--heading-color, #1f2937);
}
</style>
```

### Size Variations

```razor
<div class="size-examples">
    <!-- Small Card -->
    <OsirionFeatureCard 
        Title="Small Card"
        Description="Compact card for condensed information display."
        ImageUrl="/images/sample-small.jpg"
        CardSize="CardSize.Small"
        ContentFontSize="FontSizeVariant.Small" />
    
    <!-- Normal Card -->
    <OsirionFeatureCard 
        Title="Normal Card"
        Description="Standard card size for typical feature presentation with balanced content."
        ImageUrl="/images/sample-normal.jpg"
        CardSize="CardSize.Normal"
        ContentFontSize="FontSizeVariant.Normal" />
    
    <!-- Large Card -->
    <OsirionFeatureCard 
        Title="Large Card"
        Description="Spacious card for detailed feature descriptions and prominent display of important content."
        ImageUrl="/images/sample-large.jpg"
        CardSize="CardSize.Large"
        ContentFontSize="FontSizeVariant.Large" />
</div>

<style>
.size-examples {
    display: flex;
    flex-direction: column;
    gap: 2rem;
    margin: 2rem 0;
}
</style>
```

## Visual Effects

### Border Effects

```razor
<div class="border-examples">
    <!-- Always Visible Border -->
    <OsirionFeatureCard 
        Title="Always Border"
        Description="Card with permanently visible border for defined boundaries."
        ImageUrl="/images/sample.jpg"
        BorderTiming="EffectTiming.Always"
        BorderColor="ThemeColor.Primary" />
    
    <!-- Hover Border -->
    <OsirionFeatureCard 
        Title="Hover Border"
        Description="Border appears on hover for interactive feedback."
        ImageUrl="/images/sample.jpg"
        BorderTiming="EffectTiming.OnHover"
        BorderColor="ThemeColor.Secondary" />
    
    <!-- No Border -->
    <OsirionFeatureCard 
        Title="No Border"
        Description="Clean card without borders for minimal design."
        ImageUrl="/images/sample.jpg"
        BorderTiming="EffectTiming.Never" />
</div>
```

### Shadow Effects

```razor
<div class="shadow-examples">
    <!-- Always Shadow -->
    <OsirionFeatureCard 
        Title="Always Shadow"
        Description="Permanent shadow for elevated appearance."
        ImageUrl="/images/sample.jpg"
        ShadowTiming="EffectTiming.Always"
        ShadowColor="ThemeColor.Neutral" />
    
    <!-- Hover Shadow -->
    <OsirionFeatureCard 
        Title="Hover Shadow"
        Description="Shadow appears on hover for interactive elevation."
        ImageUrl="/images/sample.jpg"
        ShadowTiming="EffectTiming.OnHover"
        ShadowColor="ThemeColor.Primary" />
</div>
```

### Transform Effects

```razor
<div class="transform-examples">
    <!-- Hover Transform -->
    <OsirionFeatureCard 
        Title="Transform on Hover"
        Description="Card lifts and transforms when hovered for dynamic interaction."
        ImageUrl="/images/sample.jpg"
        TransformTiming="EffectTiming.OnHover"
        ImageTransformOnHover="true"
        ShadowTiming="EffectTiming.OnHover" />
    
    <!-- Always Transform -->
    <OsirionFeatureCard 
        Title="Always Transformed"
        Description="Card maintains transformed state for unique presentation."
        ImageUrl="/images/sample.jpg"
        TransformTiming="EffectTiming.Always"
        ShadowTiming="EffectTiming.Always" />
</div>
```

## Accessibility Features

### Keyboard Navigation

The OsirionFeatureCard component provides full keyboard accessibility:

- **Tab**: Navigate to the card and its interactive elements
- **Enter/Space**: Activate card links or buttons
- **Escape**: Exit focus from interactive elements

### Screen Reader Support

```razor
<OsirionFeatureCard 
    Title="Accessible Feature Card"
    Description="This card is fully accessible with proper ARIA labels and semantic structure."
    ImageUrl="/images/accessibility-demo.jpg"
    ImageAlt="Accessibility demonstration with screen reader compatible interface"
    Url="/accessibility-features"
    aria-label="Learn about accessibility features - opens in current page"
    role="article" />
```

### High Contrast Support

```css
@media (prefers-contrast: high) {
    .osirion-feature-card {
        border: 2px solid currentColor;
        background: Canvas;
        color: CanvasText;
    }
    
    .osirion-feature-card:hover {
        background: Highlight;
        color: HighlightText;
    }
}
```

## Performance Optimization

### Lazy Loading Images

```razor
<!-- Images load only when needed -->
<OsirionFeatureCard 
    Title="Performance Optimized"
    Description="Images load only when they come into view."
    ImageUrl="/images/large-feature-image.jpg"
    ImageLazyLoading="true"
    ImageAlt="Large feature demonstration image" />
```

### Efficient Rendering

```razor
<!-- Use ShowDescription to avoid rendering empty description -->
<OsirionFeatureCard 
    Title="Optimized Card"
    ShowDescription="@(!string.IsNullOrEmpty(dynamicDescription))"
    Description="@dynamicDescription"
    ImageUrl="/images/sample.jpg" />

@code {
    private string? dynamicDescription = GetDynamicDescription();
    
    private string? GetDynamicDescription()
    {
        // Only return description if needed
        return hasContent ? "This is dynamic content." : null;
    }
    
    private bool hasContent = true; // Based on your logic
}
```

## Best Practices

### Content Guidelines

1. **Title Length**: Keep titles concise (3-8 words) for better readability
2. **Description**: Limit descriptions to 1-2 sentences for optimal scan-ability
3. **Image Quality**: Use high-quality images with consistent aspect ratios
4. **Alt Text**: Always provide descriptive alt text for accessibility

### Layout Considerations

1. **Grid Systems**: Use CSS Grid or Flexbox for responsive card layouts
2. **Spacing**: Maintain consistent spacing between cards and content
3. **Responsive Design**: Test cards on various screen sizes
4. **Visual Hierarchy**: Use size and positioning to guide user attention

### Performance Tips

1. **Image Optimization**: Compress images and use appropriate formats (WebP, AVIF)
2. **Lazy Loading**: Enable for images below the fold
3. **Consistent Sizing**: Use consistent card sizes for better layout performance
4. **Minimal Effects**: Use visual effects judiciously to maintain performance

The OsirionFeatureCard component provides a powerful and flexible foundation for creating engaging, accessible, and performant feature displays in your Blazor applications.
