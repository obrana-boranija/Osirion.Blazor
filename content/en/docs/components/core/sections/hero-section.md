examples---
id: 'hero-section'
order: 1
layout: docs
title: HeroSection Component
permalink: /docs/components/core/sections/hero-section
description: Learn how to use the HeroSection component to create compelling hero areas with background images, patterns, buttons, and metadata for landing pages and articles.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Sections
tags:
- blazor
- hero
- landing page
- banner
- sections
- call to action
- background
is_featured: true
published: true
slug: components/core/sections/hero-section
lang: en
custom_fields: {}
seo_properties:
  title: 'HeroSection Component - Landing Page Heroes | Osirion.Blazor'
  description: 'Create compelling hero sections with the HeroSection component. Features background images, patterns, buttons, and flexible layouts.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/sections/hero-section'
  lang: en
  robots: index, follow
  og_title: 'HeroSection Component - Osirion.Blazor'
  og_description: 'Create compelling hero sections with background images, patterns, and call-to-action buttons.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'HeroSection Component - Osirion.Blazor'
  twitter_description: 'Create compelling hero sections with background images and call-to-action buttons.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# HeroSection Component

The HeroSection component creates compelling hero areas for landing pages, articles, and product showcases. It provides flexible layouts with support for background images, patterns, call-to-action buttons, and article metadata.

## Component Overview

HeroSection is designed to create impactful first impressions on your web pages. Whether you're building a product landing page, blog article header, or marketing showcase, this component provides the flexibility and professional styling needed to engage your audience effectively.

### Key Features

**Multiple Layout Variants**: Hero, Jumbotron, and Minimal layouts
**Background Options**: Support for images, patterns, or solid colors
**Flexible Content**: Custom render fragments for advanced layouts
**Call-to-Action Buttons**: Primary and secondary button support
**Article Metadata**: Built-in support for author, date, and read time
**Responsive Design**: Adapts to all screen sizes automatically
**SEO Optimized**: Proper semantic markup with fetchpriority hints
**Accessibility Compliant**: Full ARIA support and keyboard navigation
**Performance Focused**: Optimized image loading and rendering
**Theme Integration**: Seamless theme and styling integration

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | The hero title text. |
| `TitleContent` | `RenderFragment?` | `null` | Custom title content (overrides Title). |
| `Subtitle` | `string?` | `null` | The hero subtitle text. |
| `SubtitleContent` | `RenderFragment?` | `null` | Custom subtitle content (overrides Subtitle). |
| `Summary` | `string?` | `null` | The hero summary/description text. |
| `SummaryContent` | `RenderFragment?` | `null` | Custom summary content (overrides Summary). |
| `ImageUrl` | `string?` | `null` | URL of the hero image. |
| `ImageAlt` | `string?` | `null` | Alt text for the hero image. |
| `UseBackgroundImage` | `bool` | `false` | Whether to use image as background instead of side image. |
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | Background pattern when not using background image. |
| `Alignment` | `Alignment` | `Left` | Content alignment (Left, Center, Right, Justify). |
| `ShowPrimaryButton` | `bool` | `true` | Whether to show the primary button. |
| `ShowSecondaryButton` | `bool` | `true` | Whether to show the secondary button. |
| `PrimaryButtonText` | `string?` | `null` | Primary button text. |
| `PrimaryButtonUrl` | `string?` | `null` | Primary button URL. |
| `SecondaryButtonText` | `string?` | `null` | Secondary button text. |
| `SecondaryButtonUrl` | `string?` | `null` | Secondary button URL. |
| `BackgroundColor` | `string?` | `null` | Background color override. |
| `TextColor` | `string?` | `null` | Text color override. |
| `MinHeight` | `string` | `"60vh"` | Minimum height of the hero section. |
| `Author` | `string?` | `null` | Author name for metadata. |
| `PublishDate` | `DateTime?` | `null` | Publish date for metadata. |
| `ReadTime` | `string?` | `null` | Read time for metadata. |
| `ShowMetadata` | `bool` | `false` | Whether to show article metadata. |
| `ImagePosition` | `Alignment` | `Right` | Position of side image (Left, Right). |
| `Variant` | `HeroVariant` | `Hero` | Layout variant (Hero, Jumbotron, Minimal). |
| `HasDivider` | `bool` | `true` | Whether to show section divider. |
| `ChildContent` | `RenderFragment?` | `null` | Additional content to display. |

## Hero Variants

| Variant | Description | Best Use Case |
|---------|-------------|---------------|
| `Hero` | Standard hero layout with balanced content and image | Landing pages, product showcases |
| `Jumbotron` | Large, prominent display with minimal content | Marketing campaigns, announcements |
| `Minimal` | Clean, simple layout with focus on content | Blog articles, documentation |

## Basic Usage

### Simple Hero Section

```razor
@using Osirion.Blazor.Components

<HeroSection 
    Title="Welcome to Our Platform"
    Subtitle="Powerful tools for modern developers"
    Summary="Build faster, deploy smarter, and scale with confidence using our comprehensive development platform."
    PrimaryButtonText="Get Started Free"
    PrimaryButtonUrl="/signup"
    SecondaryButtonText="View Demo"
    SecondaryButtonUrl="/demo"
    ImageUrl="https://images.unsplash.com/photo-1551434678-e076c223a692?w=800&h=600&fit=crop"
    ImageAlt="Developer working on laptop" />
```

### Hero with Background Image

```razor
<HeroSection 
    Title="Transform Your Business"
    Subtitle="Digital solutions that drive results"
    Summary="Join thousands of companies that have accelerated their growth with our innovative platform."
    UseBackgroundImage="true"
    ImageUrl="https://images.unsplash.com/photo-1557804506-669a67965ba0?w=1920&h=1080&fit=crop"
    ImageAlt="Modern office environment"
    PrimaryButtonText="Start Your Journey"
    PrimaryButtonUrl="/get-started"
    SecondaryButtonText="Learn More"
    SecondaryButtonUrl="/about"
    TextColor="white"
    MinHeight="80vh" />
```

### Hero with Background Pattern

```razor
<HeroSection 
    Title="Innovation Meets Simplicity"
    Subtitle="Streamline your workflow"
    Summary="Discover how our platform can help you achieve more with less effort."
    BackgroundPattern="BackgroundPatternType.TechWave"
    BackgroundColor="#667eea"
    TextColor="white"
    PrimaryButtonText="Explore Features"
    PrimaryButtonUrl="/features"
    Variant="HeroVariant.Jumbotron"
    Alignment="Alignment.Center" />
```

## Advanced Usage

### Custom Content with Render Fragments

```razor
@using Osirion.Blazor.Components

<HeroSection 
    UseBackgroundImage="true"
    ImageUrl="https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=1920&h=1080&fit=crop"
    MinHeight="100vh"
    Alignment="Alignment.Center">
    
    <TitleContent>
        <h1 class="hero-custom-title">
            <span class="highlight">Revolutionary</span>
            <br />
            <span class="main-text">Development Platform</span>
            <br />
            <span class="subtitle-text">Built for the Future</span>
        </h1>
    </TitleContent>
    
    <SummaryContent>
        <div class="hero-features">
            <div class="feature-highlight">
                <i class="fas fa-rocket me-2"></i>
                <span>10x Faster Development</span>
            </div>
            <div class="feature-highlight">
                <i class="fas fa-shield-alt me-2"></i>
                <span>Enterprise Security</span>
            </div>
            <div class="feature-highlight">
                <i class="fas fa-globe me-2"></i>
                <span>Global Scale</span>
            </div>
        </div>
    </SummaryContent>
    
    <ChildContent>
        <div class="hero-extra-content">
            <div class="social-proof">
                <p class="mb-2"><strong>Trusted by 10,000+ developers</strong></p>
                <div class="rating-stars">
                    <i class="fas fa-star text-warning"></i>
                    <i class="fas fa-star text-warning"></i>
                    <i class="fas fa-star text-warning"></i>
                    <i class="fas fa-star text-warning"></i>
                    <i class="fas fa-star text-warning"></i>
                    <span class="ms-2">4.9/5 rating</span>
                </div>
            </div>
            
            <div class="hero-video mt-4">
                <button class="btn btn-light btn-lg video-play-btn" data-bs-toggle="modal" data-bs-target="#videoModal">
                    <i class="fas fa-play me-2"></i>
                    Watch 2-minute Demo
                </button>
            </div>
        </div>
    </ChildContent>
</HeroSection>

<style>
.hero-custom-title {
    font-size: clamp(2.5rem, 8vw, 6rem);
    font-weight: 900;
    line-height: 1.1;
    margin-bottom: 2rem;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}

.highlight {
    background: linear-gradient(45deg, #ff6b6b, #4ecdc4);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
}

.main-text {
    color: white;
    display: block;
}

.subtitle-text {
    font-size: 0.6em;
    color: rgba(255, 255, 255, 0.8);
    font-weight: 400;
}

.hero-features {
    display: flex;
    flex-wrap: wrap;
    gap: 2rem;
    justify-content: center;
    margin: 2rem 0;
}

.feature-highlight {
    background: rgba(255, 255, 255, 0.1);
    padding: 1rem 1.5rem;
    border-radius: 2rem;
    color: white;
    font-weight: 600;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
}

.hero-extra-content {
    text-align: center;
    margin-top: 3rem;
}

.social-proof {
    color: white;
    margin-bottom: 2rem;
}

.rating-stars {
    font-size: 1.2rem;
}

.video-play-btn {
    border-radius: 50px;
    padding: 1rem 2rem;
    font-weight: 600;
    transition: all 0.3s ease;
}

.video-play-btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

@media (max-width: 768px) {
    .hero-features {
        flex-direction: column;
        align-items: center;
    }
    
    .feature-highlight {
        width: 100%;
        text-align: center;
    }
}
</style>
```

### Article Hero with Metadata

```razor
<HeroSection 
    Title="Building Scalable Web Applications with Blazor"
    Subtitle="A comprehensive guide to modern web development"
    Summary="Learn how to create performant, maintainable web applications using Blazor Server and WebAssembly with real-world examples and best practices."
    Author="John Developer"
    PublishDate="DateTime.Parse('2025-09-03')"
    ReadTime="8 min read"
    ShowMetadata="true"
    ShowPrimaryButton="false"
    ShowSecondaryButton="false"
    ImageUrl="https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800&h=400&fit=crop"
    ImageAlt="Code on computer screen"
    Variant="HeroVariant.Minimal"
    HasDivider="true" />
```

### Product Landing Hero

```razor
@using Osirion.Blazor.Components

<HeroSection 
    UseBackgroundImage="true"
    ImageUrl="https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=1920&h=1080&fit=crop"
    MinHeight="90vh"
    BackgroundColor="#1a202c"
    TextColor="white">
    
    <TitleContent>
        <div class="product-hero-title">
            <div class="product-badge">
                <span class="badge bg-primary rounded-pill px-3 py-2">
                    <i class="fas fa-star me-1"></i>
                    New Release
                </span>
            </div>
            <h1 class="display-2 fw-bold mb-4">
                Meet <span class="text-primary">ProjectFlow</span>
            </h1>
            <p class="lead fs-3 mb-0">
                The project management tool that adapts to your team
            </p>
        </div>
    </TitleContent>
    
    <SummaryContent>
        <div class="product-highlights mt-4">
            <div class="row g-4">
                <div class="col-md-4">
                    <div class="highlight-item">
                        <div class="highlight-icon">
                            <i class="fas fa-users fa-2x text-primary"></i>
                        </div>
                        <h5>Team Collaboration</h5>
                        <p>Real-time collaboration tools that keep everyone aligned and productive.</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="highlight-item">
                        <div class="highlight-icon">
                            <i class="fas fa-chart-line fa-2x text-success"></i>
                        </div>
                        <h5>Performance Analytics</h5>
                        <p>Comprehensive insights to track progress and optimize workflows.</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="highlight-item">
                        <div class="highlight-icon">
                            <i class="fas fa-mobile-alt fa-2x text-info"></i>
                        </div>
                        <h5>Mobile Ready</h5>
                        <p>Native mobile apps that sync seamlessly across all your devices.</p>
                    </div>
                </div>
            </div>
        </div>
    </SummaryContent>
    
    <ChildContent>
        <div class="product-cta-section mt-5">
            <div class="row align-items-center">
                <div class="col-lg-8">
                    <div class="cta-buttons d-flex flex-wrap gap-3">
                        <a href="/signup" class="btn btn-primary btn-lg px-5 py-3">
                            <i class="fas fa-rocket me-2"></i>
                            Start Free Trial
                        </a>
                        <a href="/demo" class="btn btn-outline-light btn-lg px-5 py-3">
                            <i class="fas fa-play me-2"></i>
                            Watch Demo
                        </a>
                        <a href="/pricing" class="btn btn-light btn-lg px-5 py-3">
                            <i class="fas fa-dollar-sign me-2"></i>
                            View Pricing
                        </a>
                    </div>
                    
                    <div class="trial-info mt-3">
                        <small class="text-white-50">
                            <i class="fas fa-check me-1"></i>
                            No credit card required • 14-day free trial • Cancel anytime
                        </small>
                    </div>
                </div>
                <div class="col-lg-4">
                    <div class="product-stats">
                        <div class="stat-item">
                            <div class="stat-number">50K+</div>
                            <div class="stat-label">Active Users</div>
                        </div>
                        <div class="stat-item">
                            <div class="stat-number">99.9%</div>
                            <div class="stat-label">Uptime</div>
                        </div>
                        <div class="stat-item">
                            <div class="stat-number">4.8★</div>
                            <div class="stat-label">User Rating</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ChildContent>
</HeroSection>

<style>
.product-hero-title {
    text-align: center;
    max-width: 800px;
    margin: 0 auto;
}

.product-badge {
    margin-bottom: 2rem;
}

.product-highlights {
    max-width: 1000px;
    margin: 0 auto;
}

.highlight-item {
    text-align: center;
    padding: 2rem;
    background: rgba(255, 255, 255, 0.05);
    border-radius: 1rem;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.1);
    height: 100%;
    transition: transform 0.3s ease;
}

.highlight-item:hover {
    transform: translateY(-5px);
}

.highlight-icon {
    margin-bottom: 1.5rem;
}

.highlight-item h5 {
    color: white;
    margin-bottom: 1rem;
    font-weight: 600;
}

.highlight-item p {
    color: rgba(255, 255, 255, 0.8);
    margin: 0;
}

.product-cta-section {
    text-align: center;
    padding-top: 3rem;
    border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.cta-buttons {
    justify-content: center;
    margin-bottom: 1rem;
}

.btn-lg {
    border-radius: 50px;
    font-weight: 600;
    transition: all 0.3s ease;
}

.btn-lg:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.product-stats {
    display: flex;
    justify-content: space-around;
    background: rgba(255, 255, 255, 0.1);
    padding: 2rem;
    border-radius: 1rem;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.1);
}

.stat-item {
    text-align: center;
}

.stat-number {
    font-size: 2rem;
    font-weight: bold;
    color: #007bff;
    display: block;
}

.stat-label {
    font-size: 0.875rem;
    color: rgba(255, 255, 255, 0.7);
    text-transform: uppercase;
    letter-spacing: 1px;
}

@media (max-width: 768px) {
    .product-hero-title h1 {
        font-size: 2.5rem;
    }
    
    .cta-buttons {
        flex-direction: column;
        align-items: center;
    }
    
    .cta-buttons .btn {
        width: 100%;
        max-width: 300px;
    }
    
    .product-stats {
        flex-direction: column;
        gap: 1rem;
        margin-top: 2rem;
    }
}
</style>
```

### Minimal Blog Hero

```razor
<HeroSection 
    Title="Mastering Modern Web Development"
    Subtitle="Tips, tricks, and best practices for developers"
    Author="Sarah Johnson"
    PublishDate="DateTime.Parse('2025-09-03')"
    ReadTime="12 min read"
    ShowMetadata="true"
    ShowPrimaryButton="false"
    ShowSecondaryButton="false"
    Variant="HeroVariant.Minimal"
    Alignment="Alignment.Center"
    BackgroundPattern="BackgroundPatternType.Dots"
    BackgroundColor="#f8f9fa"
    MinHeight="40vh">
    
    <ChildContent>
        <div class="blog-hero-extras mt-4">
            <div class="blog-tags">
                <span class="badge bg-primary me-2">Web Development</span>
                <span class="badge bg-secondary me-2">JavaScript</span>
                <span class="badge bg-info me-2">Best Practices</span>
            </div>
            
            <div class="blog-social mt-3">
                <p class="small text-muted mb-2">Share this article:</p>
                <div class="social-buttons">
                    <a href="#" class="btn btn-outline-primary btn-sm me-2">
                        <i class="fab fa-twitter"></i>
                    </a>
                    <a href="#" class="btn btn-outline-primary btn-sm me-2">
                        <i class="fab fa-linkedin"></i>
                    </a>
                    <a href="#" class="btn btn-outline-primary btn-sm me-2">
                        <i class="fab fa-facebook"></i>
                    </a>
                    <a href="#" class="btn btn-outline-primary btn-sm">
                        <i class="fas fa-link"></i>
                    </a>
                </div>
            </div>
        </div>
    </ChildContent>
</HeroSection>

<style>
.blog-hero-extras {
    text-align: center;
    max-width: 600px;
    margin: 0 auto;
}

.blog-tags .badge {
    font-size: 0.875rem;
    padding: 0.5rem 1rem;
    border-radius: 1rem;
}

.social-buttons .btn {
    border-radius: 50%;
    width: 40px;
    height: 40px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    transition: all 0.3s ease;
}

.social-buttons .btn:hover {
    transform: translateY(-2px);
}
</style>
```

## Styling Examples

### Bootstrap Integration

```razor
<HeroSection 
    Title="Bootstrap Hero Example"
    Summary="A hero section styled with Bootstrap utilities."
    Class="bg-primary text-white"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/start"
    SecondaryButtonText="Learn More"
    SecondaryButtonUrl="/learn" />

<style>
/* Bootstrap-compatible hero styling */
.osirion-hero-section {
    position: relative;
    overflow: hidden;
}

.osirion-hero-section.bg-primary {
    background: linear-gradient(135deg, var(--bs-primary) 0%, var(--bs-info) 100%) !important;
}

.osirion-hero-container {
    position: relative;
    z-index: 2;
    padding: 4rem 0;
}

.osirion-hero-title {
    font-size: clamp(2rem, 5vw, 3.5rem);
    font-weight: 700;
    line-height: 1.2;
    margin-bottom: 1.5rem;
}

.osirion-hero-subtitle {
    font-size: 1.25rem;
    margin-bottom: 1rem;
    opacity: 0.9;
}

.osirion-hero-summary {
    font-size: 1.1rem;
    margin-bottom: 2rem;
    opacity: 0.8;
}

.osirion-hero-buttons {
    display: flex;
    gap: 1rem;
    flex-wrap: wrap;
}

@media (max-width: 768px) {
    .osirion-hero-buttons {
        flex-direction: column;
        align-items: center;
    }
    
    .osirion-hero-button {
        width: 100%;
        max-width: 300px;
    }
}
</style>
```

### Tailwind CSS Integration

```razor
<HeroSection 
    Title="Tailwind Hero Example"
    Summary="A hero section styled with Tailwind CSS utilities."
    Class="bg-gradient-to-br from-blue-600 to-purple-700 text-white"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/start" />

<style>
/* Tailwind-compatible hero styling */
.osirion-hero-section {
    @apply relative overflow-hidden;
}

.osirion-hero-container {
    @apply relative z-10 py-16 px-4;
}

.osirion-hero-title {
    @apply text-4xl md:text-6xl font-black leading-tight mb-6;
}

.osirion-hero-subtitle {
    @apply text-xl mb-4 opacity-90;
}

.osirion-hero-summary {
    @apply text-lg mb-8 opacity-80;
}

.osirion-hero-buttons {
    @apply flex gap-4 flex-wrap;
}

.osirion-hero-button {
    @apply px-8 py-4 rounded-full font-semibold transition-all duration-300 hover:-translate-y-1 hover:shadow-lg;
}

@media (max-width: 768px) {
    .osirion-hero-buttons {
        @apply flex-col items-center;
    }
    
    .osirion-hero-button {
        @apply w-full max-w-sm;
    }
}
</style>
```

## Best Practices

### Content Guidelines

1. **Clear Messaging**: Keep titles concise and impactful
2. **Value Proposition**: Clearly communicate your unique value
3. **Action-Oriented**: Use strong call-to-action buttons
4. **Visual Hierarchy**: Structure content with proper heading levels
5. **Mobile-First**: Design for mobile devices first

### Performance Optimization

1. **Image Optimization**: Use properly sized and compressed images
2. **Lazy Loading**: Implement lazy loading for non-critical images
3. **Fetchpriority**: Use fetchpriority="high" for hero images
4. **Core Web Vitals**: Optimize for LCP, FID, and CLS metrics
5. **Progressive Enhancement**: Ensure basic functionality without JavaScript

### Accessibility

1. **Semantic Markup**: Use proper heading structure and landmarks
2. **Alt Text**: Provide descriptive alt text for all images
3. **Color Contrast**: Ensure sufficient contrast for text readability
4. **Keyboard Navigation**: Make all interactive elements keyboard accessible
5. **Screen Readers**: Test with screen reader software

### User Experience

1. **Loading Performance**: Optimize hero section load times
2. **Visual Impact**: Create compelling first impressions
3. **Clear Navigation**: Provide obvious next steps for users
4. **Mobile Experience**: Ensure excellent mobile usability
5. **Content Scannability**: Make content easy to scan and digest

The HeroSection component provides a powerful foundation for creating compelling landing pages and article headers that engage users and drive conversions.
