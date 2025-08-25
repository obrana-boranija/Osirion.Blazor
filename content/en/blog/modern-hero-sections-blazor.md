---
title: "Building Modern Hero Sections with Osirion.Blazor"
author: "Dejan Demonji?"
date: "2025-01-22"
description: "Learn how to create compelling hero sections using the new HeroSection component in Osirion.Blazor v1.5, with support for multiple variants, background patterns, and responsive design."
tags: [Components, UI/UX, Design, Hero Section, Landing Pages]
categories: [Components, Design]
slug: "modern-hero-sections-blazor"
is_featured: true
featured_image: "https://images.unsplash.com/photo-1460925895917-afdab827c52f?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80"
seo_properties:
  title: "Building Modern Hero Sections with Osirion.Blazor v1.5"
  description: "Create stunning hero sections with the new HeroSection component featuring multiple variants, background patterns, and call-to-action buttons."
  og_image_url: "https://images.unsplash.com/photo-1460925895917-afdab827c52f?ixlib=rb-4.0.3&auto=format&fit=crop&w=1200&q=80"
  type: "Article"
---

# Building Modern Hero Sections with Osirion.Blazor

Hero sections are often the first thing users see when they visit your website. They need to capture attention, communicate value, and guide users toward action—all while looking great across devices. With Osirion.Blazor v1.5, we've introduced the `HeroSection` component that makes creating compelling hero sections effortless and flexible.

## The New HeroSection Component

The `HeroSection` component is designed to handle the most common hero section patterns while providing extensive customization options:

```razor
<HeroSection 
    Title="Transform Your Development Workflow"
    Subtitle="Build better applications faster"
    Summary="Osirion.Blazor provides the components and tools you need to create modern, accessible web applications with minimal effort."
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/docs/getting-started"
    SecondaryButtonText="View Examples"
    SecondaryButtonUrl="/examples"
    ImageUrl="/images/hero-illustration.svg"
    Variant="HeroVariant.Hero" />
```

## Hero Variants for Different Use Cases

### 1. Hero Variant (Default)

Perfect for landing pages and product showcases:

```razor
<HeroSection 
    Title="Revolutionize Your Blazor Development"
    Subtitle="Components that just work"
    Summary="From navigation to content management, Osirion.Blazor provides everything you need to build professional web applications."
    ImageUrl="/images/blazor-development.png"
    ImageAlt="Blazor development illustration"
    UseBackgroundImage="false"
    ImagePosition="Alignment.Right"
    PrimaryButtonText="Start Building"
    PrimaryButtonUrl="/get-started"
    SecondaryButtonText="See Demo"
    SecondaryButtonUrl="/demo" />
```

### 2. Jumbotron Variant

Ideal for major announcements and prominent messaging:

```razor
<HeroSection 
    Title="Osirion.Blazor v1.5 is Here!"
    Subtitle="More components, better performance, enhanced developer experience"
    Summary="Discover new components like HeroSection, Breadcrumbs, and Cookie Consent, plus improved CSS framework integration and comprehensive documentation."
    Variant="HeroVariant.Jumbotron"
    Alignment="Alignment.Center"
    MinHeight="100vh"
    BackgroundColor="linear-gradient(135deg, #667eea 0%, #764ba2 100%)"
    TextColor="#ffffff"
    PrimaryButtonText="Explore v1.5"
    PrimaryButtonUrl="/docs/migration"
    SecondaryButtonText="Download Now"
    SecondaryButtonUrl="/download" />
```

### 3. Minimal Variant

Perfect for blog posts and article headers:

```razor
<HeroSection 
    Title="@article.Title"
    Summary="@article.Description"
    Author="@article.Author"
    PublishDate="@article.PublishDate"
    ReadTime="@article.ReadTime"
    ShowMetadata="true"
    Variant="HeroVariant.Minimal"
    ShowPrimaryButton="false"
    ShowSecondaryButton="false"
    ImageUrl="@article.FeaturedImage"
    UseBackgroundImage="true"
    HasDivider="true" />
```

## Background Images and Patterns

### Background Images

Create immersive experiences with background images:

```razor
<HeroSection 
    Title="Join the Developer Community"
    Subtitle="Connect, Learn, and Build Together"
    Summary="Be part of a growing community of developers building amazing applications with Osirion.Blazor."
    ImageUrl="/images/community-hero.jpg"
    UseBackgroundImage="true"
    TextColor="#ffffff"
    MinHeight="80vh"
    Alignment="Alignment.Center"
    PrimaryButtonText="Join Community"
    PrimaryButtonUrl="/community"
    SecondaryButtonText="Learn More"
    SecondaryButtonUrl="/about-community" />
```

### Decorative Patterns

Add visual interest with built-in background patterns:

```razor
<HeroSection 
    Title="Enterprise-Grade Components"
    Subtitle="Built for scale and performance"
    Summary="Every component is designed with enterprise requirements in mind: accessibility, performance, and maintainability."
    BackgroundPattern="BackgroundPatternType.Dots"
    BackgroundColor="#f8f9fa"
    ImageUrl="/images/enterprise-architecture.svg"
    ImagePosition="Alignment.Right" />
```

Available patterns:
- `Dots`: Subtle dot pattern
- `Grid`: Grid line pattern
- `Lines`: Diagonal line pattern
- `Waves`: Wave pattern
- `Geometric`: Geometric shapes

## Custom Content with RenderFragments

For maximum flexibility, use custom content:

```razor
<HeroSection Variant="HeroVariant.Jumbotron" Alignment="Alignment.Center">
    <TitleContent>
        <h1 class="display-1 fw-bold text-gradient">
            Build <span class="text-primary">Faster</span><br />
            Ship <span class="text-success">Smarter</span><br />
            Scale <span class="text-warning">Better</span>
        </h1>
    </TitleContent>
    
    <SummaryContent>
        <div class="lead mb-4">
            <p class="fs-4">The complete Blazor component ecosystem</p>
            <div class="feature-grid mt-4">
                <div class="feature-item">
                    <i class="bi bi-lightning-charge text-primary fs-2"></i>
                    <span>Lightning Fast</span>
                </div>
                <div class="feature-item">
                    <i class="bi bi-shield-check text-success fs-2"></i>
                    <span>Enterprise Ready</span>
                </div>
                <div class="feature-item">
                    <i class="bi bi-universal-access text-info fs-2"></i>
                    <span>Fully Accessible</span>
                </div>
            </div>
        </div>
    </SummaryContent>
    
    <ChildContent>
        <div class="mt-5">
            <div class="d-flex justify-content-center gap-4 mb-4">
                <div class="stat-item text-center">
                    <div class="stat-number">15+</div>
                    <div class="stat-label">Components</div>
                </div>
                <div class="stat-item text-center">
                    <div class="stat-number">4</div>
                    <div class="stat-label">CSS Frameworks</div>
                </div>
                <div class="stat-item text-center">
                    <div class="stat-number">85%+</div>
                    <div class="stat-label">Test Coverage</div>
                </div>
            </div>
            
            <div class="text-center">
                <span class="badge bg-success fs-6 px-3 py-2">
                    <i class="bi bi-check-circle me-2"></i>
                    Now with .NET 9 Support
                </span>
            </div>
        </div>
    </ChildContent>
</HeroSection>
```

## Responsive Design and Framework Integration

### Bootstrap Integration

The HeroSection works seamlessly with Bootstrap:

```razor
<HeroSection 
    Title="Bootstrap-Powered Components"
    Summary="Fully integrated with Bootstrap 5+ for consistent styling and responsive behavior."
    Class="bg-gradient-primary text-white"
    ImageUrl="/images/bootstrap-integration.svg"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/docs/bootstrap" />
```

### Custom CSS Framework Integration

Adapt to any CSS framework:

```razor
<HeroSection 
    Title="Framework Agnostic Design"
    Summary="Use with Bootstrap, Tailwind, Bulma, or your custom CSS framework."
    Class="custom-hero-theme"
    CssVariables="--hero-bg: linear-gradient(45deg, #ff6b6b, #4ecdc4);"
    ImageUrl="/images/css-frameworks.svg" />
```

## Real-World Examples

### SaaS Landing Page

```razor
<HeroSection 
    Title="Streamline Your Development Process"
    Subtitle="Build faster, deploy smarter, scale effortlessly"
    Summary="Our platform provides everything you need to go from idea to production in record time, with enterprise-grade security and performance."
    ImageUrl="/images/saas-dashboard.png"
    ImageAlt="SaaS dashboard preview"
    UseBackgroundImage="false"
    ImagePosition="Alignment.Right"
    BackgroundPattern="BackgroundPatternType.Dots"
    BackgroundColor="#f8fafc"
    MinHeight="90vh"
    PrimaryButtonText="Start Free Trial"
    PrimaryButtonUrl="/trial?utm_source=hero"
    SecondaryButtonText="Book Demo"
    SecondaryButtonUrl="/demo?utm_source=hero">
    
    <ChildContent>
        <div class="mt-4">
            <div class="d-flex align-items-center justify-content-center gap-4 text-muted">
                <span class="small">Trusted by 10,000+ developers</span>
                <div class="customer-logos">
                    <InfiniteLogoCarousel 
                        CustomLogos="@customerLogos"
                        LogoHeight="30"
                        EnableGrayscale="true"
                        AnimationDuration="20" />
                </div>
            </div>
        </div>
    </ChildContent>
</HeroSection>
```

### Open Source Project

```razor
<HeroSection 
    Title="Open Source Blazor Components"
    Subtitle="Built by developers, for developers"
    Summary="Join thousands of developers building better Blazor applications with our open-source component library."
    ImageUrl="/images/open-source-community.svg"
    UseBackgroundImage="false"
    ImagePosition="Alignment.Left"
    Alignment="Alignment.Left"
    PrimaryButtonText="View on GitHub"
    PrimaryButtonUrl="https://github.com/obrana-boranija/Osirion.Blazor"
    SecondaryButtonText="Read Documentation"
    SecondaryButtonUrl="/docs">
    
    <ChildContent>
        <div class="mt-4">
            <div class="github-stats d-flex gap-4">
                <div class="stat-badge">
                    <i class="bi bi-star"></i>
                    <span>1.2k Stars</span>
                </div>
                <div class="stat-badge">
                    <i class="bi bi-download"></i>
                    <span>50k+ Downloads</span>
                </div>
                <div class="stat-badge">
                    <i class="bi bi-people"></i>
                    <span>100+ Contributors</span>
                </div>
            </div>
        </div>
    </ChildContent>
</HeroSection>
```

### E-commerce Product

```razor
<HeroSection 
    Title="Premium Blazor UI Kit"
    Subtitle="Professional components for modern applications"
    Summary="Save months of development time with our carefully crafted, production-ready components designed for enterprise applications."
    ImageUrl="/images/ui-kit-preview.png"
    UseBackgroundImage="false"
    ImagePosition="Alignment.Right"
    BackgroundColor="linear-gradient(135deg, #667eea 0%, #764ba2 100%)"
    TextColor="#ffffff"
    PrimaryButtonText="Buy Now - $99"
    PrimaryButtonUrl="/purchase"
    SecondaryButtonText="View Components"
    SecondaryButtonUrl="/components">
    
    <ChildContent>
        <div class="mt-4">
            <div class="features-list">
                <div class="feature-item">
                    <i class="bi bi-check-circle text-success"></i>
                    <span>50+ Premium Components</span>
                </div>
                <div class="feature-item">
                    <i class="bi bi-check-circle text-success"></i>
                    <span>Complete Documentation</span>
                </div>
                <div class="feature-item">
                    <i class="bi bi-check-circle text-success"></i>
                    <span>Lifetime Updates</span>
                </div>
                <div class="feature-item">
                    <i class="bi bi-check-circle text-success"></i>
                    <span>Priority Support</span>
                </div>
            </div>
        </div>
    </ChildContent>
</HeroSection>
```

## Performance and Accessibility

### Performance Optimizations

The HeroSection component is optimized for performance:

```csharp
// Lazy image loading
<img src="@GetLogoUrl(logo)" 
     width="@LogoWidth" 
     height="@LogoHeight" 
     alt="@logo.AltText" 
     loading="lazy">

// Efficient CSS custom properties
private string GetBackgroundStyle()
{
    var styles = new List<string>();
    
    if (UseBackgroundImage && !string.IsNullOrWhiteSpace(ImageUrl))
    {
        styles.Add($"background-image: url('{ImageUrl}')");
    }
    
    return string.Join("; ", styles);
}
```

### Accessibility Features

Full accessibility support is built-in:

```html
<!-- Semantic HTML structure -->
<section class="osirion-hero-section" aria-label="@SectionTitle">
    <header class="hero-header">
        <h1 class="hero-title">@Title</h1>
        <h2 class="hero-subtitle">@Subtitle</h2>
    </header>
    
    <div class="hero-content">
        <p class="hero-summary">@Summary</p>
        
        <!-- Accessible buttons -->
        <div class="hero-actions">
            <a href="@PrimaryButtonUrl" 
               class="btn btn-primary"
               role="button">
                @PrimaryButtonText
            </a>
        </div>
    </div>
    
    <!-- Properly labeled images -->
    <img src="@ImageUrl" 
         alt="@ImageAlt" 
         class="hero-image"
         role="img">
</section>
```

## Customization and Theming

### CSS Variables

Customize appearance with CSS variables:

```css
:root {
    --osirion-hero-background: #ffffff;
    --osirion-hero-text: #333333;
    --osirion-hero-accent: #007bff;
    --osirion-hero-padding: 4rem 0;
    --osirion-hero-gap: 2rem;
    --osirion-hero-button-primary: #007bff;
    --osirion-hero-button-secondary: transparent;
    --osirion-hero-image-border-radius: 0.5rem;
    --osirion-hero-image-shadow: 0 10px 25px rgba(0,0,0,0.1);
}

/* Dark theme */
.dark-theme {
    --osirion-hero-background: #1a1a1a;
    --osirion-hero-text: #ffffff;
    --osirion-hero-accent: #4dabf7;
}
```

### Custom Styling

Apply custom styling for specific use cases:

```css
/* Landing page hero */
.landing-hero {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    min-height: 100vh;
    display: flex;
    align-items: center;
}

/* Product showcase hero */
.product-hero {
    background: #f8f9fa;
    border-bottom: 1px solid #dee2e6;
}

.product-hero .hero-image {
    border-radius: 12px;
    box-shadow: 0 20px 40px rgba(0,0,0,0.1);
}

/* Blog post hero */
.blog-hero {
    padding: 2rem 0;
    border-bottom: 1px solid #e9ecef;
}

.blog-hero .hero-metadata {
    margin-top: 1rem;
    font-size: 0.875rem;
    color: #6c757d;
}
```

## Best Practices

### 1. Content Strategy

```razor
<!-- Keep titles concise and impactful -->
<HeroSection 
    Title="Ship Faster"  <!-- Clear, action-oriented -->
    Subtitle="Build modern web apps with confidence"  <!-- Benefit-focused -->
    Summary="Everything you need to go from idea to production in days, not months."  <!-- Under 200 characters -->
    />
```

### 2. Visual Hierarchy

```razor
<!-- Use variant appropriate for content importance -->
<HeroSection 
    Variant="HeroVariant.Jumbotron"  <!-- For major announcements -->
    Title="Revolutionary New Features"
    Alignment="Alignment.Center"
    MinHeight="100vh" />

<HeroSection 
    Variant="HeroVariant.Minimal"  <!-- For articles/blogs -->
    Title="How to Build Better Components"
    ShowMetadata="true" />
```

### 3. Mobile Optimization

```css
/* Responsive hero sections */
.osirion-hero-section {
    padding: 2rem 1rem;
}

@media (min-width: 768px) {
    .osirion-hero-section {
        padding: 4rem 2rem;
    }
}

@media (min-width: 1200px) {
    .osirion-hero-section {
        padding: 6rem 2rem;
    }
}

/* Mobile-first image handling */
.hero-image {
    width: 100%;
    height: auto;
    margin-top: 2rem;
}

@media (min-width: 768px) {
    .hero-image {
        margin-top: 0;
        margin-left: 2rem;
    }
}
```

## Conclusion

The HeroSection component in Osirion.Blazor v1.5 provides everything you need to create compelling, accessible, and performant hero sections. With three distinct variants, extensive customization options, and seamless framework integration, you can build hero sections that convert visitors into users.

Whether you're building a SaaS landing page, an open-source project site, or a corporate website, the HeroSection component adapts to your needs while maintaining excellent performance and accessibility standards.

Start creating amazing hero sections today with Osirion.Blazor v1.5!