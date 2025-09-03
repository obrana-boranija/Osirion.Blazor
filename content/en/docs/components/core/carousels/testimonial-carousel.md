---
id: 'osirion-testimonial-carousel'
order: 2
layout: docs
title: OsirionTestimonialCarousel Component
permalink: /docs/components/core/carousels/testimonial-carousel
description: Learn how to use the OsirionTestimonialCarousel component to display customer testimonials in an infinite scrolling carousel with customizable animations and styling.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Carousels
tags:
- blazor
- testimonials
- carousel
- infinite scroll
- animation
- social proof
- reviews
- customers
is_featured: true
published: true
slug: components/core/carousels/testimonial-carousel
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionTestimonialCarousel Component - Customer Testimonials | Osirion.Blazor'
  description: 'Display customer testimonials in an infinite scrolling carousel. Features customizable animations, responsive design, and accessibility support.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/carousels/testimonial-carousel'
  lang: en
  robots: index, follow
  og_title: 'OsirionTestimonialCarousel Component - Osirion.Blazor'
  og_description: 'Display customer testimonials in an infinite scrolling carousel with customizable animations.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionTestimonialCarousel Component - Osirion.Blazor'
  twitter_description: 'Display customer testimonials in an infinite scrolling carousel.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionTestimonialCarousel Component

The OsirionTestimonialCarousel component creates an infinite scrolling carousel that displays customer testimonials continuously. It's designed to showcase social proof and customer satisfaction with smooth animations and responsive design.

## Component Overview

OsirionTestimonialCarousel provides an engaging way to display customer testimonials without requiring user interaction. The component uses CSS animations for smooth, performant scrolling and works perfectly in Static SSR environments without JavaScript requirements.

### Key Features

**Infinite Scrolling**: Seamless continuous animation without visible restarts
**CSS-Based Animation**: High performance with no JavaScript dependencies
**Customizable Speed**: Control animation timing with slow, normal, or fast speeds
**Bidirectional Animation**: Support for left-to-right or right-to-left scrolling
**Responsive Design**: Adapts to different screen sizes automatically
**Pause on Hover**: Optional pause functionality for better user experience
**Accessibility Compliant**: Proper ARIA labels and semantic markup
**Performance Optimized**: Priority loading for first visible testimonials
**Theme Integration**: Inherits or overrides theme settings
**Flexible Content**: Support for custom testimonial data or built-in samples

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | Optional title to display above the testimonials. |
| `SectionTitle` | `string` | `"Customer Testimonials"` | Section aria-label for accessibility. |
| `AnimationDuration` | `int` | `60` | Animation duration in seconds. |
| `CustomTestimonials` | `List<TestimonialItem>?` | `null` | Custom list of testimonials. Uses samples if not provided. |
| `PauseOnHover` | `bool` | `true` | Whether to pause animation on hover. |
| `Direction` | `AnimationDirection` | `Right` | Animation direction (Left or Right). |
| `CardWidth` | `int` | `400` | Card width in pixels. |
| `CardGap` | `int` | `24` | Gap between cards in pixels. |
| `MaxVisibleTestimonials` | `int?` | `null` | Maximum testimonials to display before repeating. |
| `CardVariant` | `TestimonialVariant` | `Default` | Testimonial card variant. |
| `CardSize` | `TestimonialSize` | `Normal` | Testimonial card size. |
| `CardElevated` | `bool` | `true` | Whether cards should have elevated appearance. |
| `CardBorderless` | `bool` | `false` | Whether cards should be borderless. |
| `Speed` | `CarouselSpeed` | `Normal` | Animation speed (Slow, Normal, Fast). |
| `ShowRating` | `bool` | `false` | Whether to show rating stars on cards. |
| `InheritTheme` | `bool` | `false` | Whether cards inherit carousel theme instead of System. |

## TestimonialItem Record

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Name` | `string` | - | Customer name (required). |
| `Position` | `string?` | `null` | Customer's job position. |
| `Company` | `string?` | `null` | Customer's company name. |
| `TestimonialText` | `string?` | `null` | The testimonial content. |
| `ProfileImageUrl` | `string?` | `null` | URL to customer's profile image. |
| `LinkedInUrl` | `string?` | `null` | URL to customer's LinkedIn profile. |
| `ShowRating` | `bool` | `false` | Whether to show rating for this testimonial. |
| `Rating` | `int` | `5` | Rating value (1-5 stars). |
| `ReadMoreHref` | `string?` | `null` | URL for "read more" link. |
| `ReadMoreText` | `string?` | `"Read more"` | Text for "read more" link. |
| `ReadMoreVariant` | `ReadMoreVariant` | `Default` | Variant for "read more" link. |
| `ReadMoreTarget` | `string?` | `null` | Target for "read more" link. |
| `AdditionalCssClass` | `string?` | `null` | Additional CSS classes for the card. |

## Basic Usage

### Simple Testimonial Carousel

```razor
@using Osirion.Blazor.Components

<section class="testimonials-section py-5">
    <div class="container">
        <div class="row">
            <div class="col-12 text-center mb-5">
                <h2 class="display-5 fw-bold">What Our Customers Say</h2>
                <p class="lead text-muted">Hear from the companies that trust our platform</p>
            </div>
        </div>
        
        <OsirionTestimonialCarousel 
            ShowRating="true"
            Speed="CarouselSpeed.Normal"
            PauseOnHover="true" />
    </div>
</section>
```

### Carousel with Custom Title

```razor
<OsirionTestimonialCarousel 
    Title="Customer Success Stories"
    SectionTitle="Customer testimonials and success stories"
    ShowRating="true"
    CardElevated="true"
    AnimationDuration="45"
    Speed="CarouselSpeed.Slow" />
```

### Custom Styling Options

```razor
<OsirionTestimonialCarousel 
    CardVariant="TestimonialVariant.Minimal"
    CardSize="TestimonialSize.Large"
    CardBorderless="true"
    CardWidth="450"
    CardGap="32"
    Direction="AnimationDirection.Left"
    InheritTheme="true" />
```

## Advanced Usage

### Custom Testimonials Data

```razor
@using Osirion.Blazor.Components

<OsirionTestimonialCarousel 
    CustomTestimonials="customTestimonials"
    Title="Featured Customer Stories"
    ShowRating="true"
    Speed="CarouselSpeed.Normal"
    CardElevated="true" />

@code {
    private List<TestimonialItem> customTestimonials = new()
    {
        new("Alex Rodriguez", 
            "VP of Engineering", 
            "TechCorp Solutions",
            "This platform revolutionized our development process. The team's expertise and support throughout the implementation was exceptional. We've seen a 50% increase in productivity since adoption.",
            "/images/testimonials/alex-rodriguez.jpg",
            "https://linkedin.com/in/alexrodriguez",
            ShowRating: true,
            Rating: 5,
            ReadMoreHref: "/case-studies/techcorp-solutions",
            ReadMoreText: "Read full case study",
            ReadMoreVariant: ReadMoreVariant.Primary),

        new("Jennifer Kim", 
            "Product Manager", 
            "Innovation Labs",
            "The integration was seamless and the results exceeded our expectations. Our customer satisfaction scores have improved significantly, and the platform continues to deliver value every day.",
            "/images/testimonials/jennifer-kim.jpg",
            "https://linkedin.com/in/jenniferkim",
            ShowRating: true,
            Rating: 5,
            ReadMoreHref: "/testimonials/jennifer-kim",
            ReadMoreText: "Learn more"),

        new("Marcus Thompson", 
            "CTO", 
            "Digital Dynamics",
            "Outstanding platform with incredible performance. The scalability and reliability have been perfect for our growing business needs. Highly recommend to any serious organization.",
            "/images/testimonials/marcus-thompson.jpg",
            "https://linkedin.com/in/marcusthompson",
            ShowRating: true,
            Rating: 5),

        new("Sarah Chen", 
            "Lead Designer", 
            "Creative Agency Pro",
            "Beautiful, intuitive, and powerful. Our design workflow has become much more efficient, and our clients love the improved collaboration capabilities.",
            "/images/testimonials/sarah-chen.jpg",
            "https://linkedin.com/in/sarahchen",
            ShowRating: true,
            Rating: 4,
            ReadMoreHref: "/success-stories/creative-agency-pro",
            ReadMoreText: "View success story",
            ReadMoreVariant: ReadMoreVariant.Secondary),

        new("David Park", 
            "Founder & CEO", 
            "StartupCo",
            "This solution has been a game-changer for our startup. The ROI was evident within the first month, and the ongoing support has been fantastic.",
            "/images/testimonials/david-park.jpg",
            "https://linkedin.com/in/davidpark",
            ShowRating: true,
            Rating: 5,
            AdditionalCssClass: "featured-testimonial")
    };
}
```

### Interactive Carousel with Controls

```razor
@using Osirion.Blazor.Components

<div class="testimonial-showcase">
    <div class="showcase-header text-center mb-4">
        <h2 class="display-4 fw-bold">Customer Testimonials</h2>
        <p class="lead">See what our customers have to say about us</p>
        
        <div class="carousel-controls mt-4">
            <div class="btn-group" role="group" aria-label="Carousel controls">
                <button class="btn btn-outline-primary @(currentSpeed == CarouselSpeed.Slow ? "active" : "")"
                        @onclick="() => SetSpeed(CarouselSpeed.Slow)">
                    Slow
                </button>
                <button class="btn btn-outline-primary @(currentSpeed == CarouselSpeed.Normal ? "active" : "")"
                        @onclick="() => SetSpeed(CarouselSpeed.Normal)">
                    Normal
                </button>
                <button class="btn btn-outline-primary @(currentSpeed == CarouselSpeed.Fast ? "active" : "")"
                        @onclick="() => SetSpeed(CarouselSpeed.Fast)">
                    Fast
                </button>
            </div>
            
            <button class="btn btn-secondary ms-3" @onclick="ToggleDirection">
                <i class="fas fa-exchange-alt me-2"></i>
                @(currentDirection == AnimationDirection.Right ? "← Left" : "Right →")
            </button>
        </div>
    </div>
    
    <OsirionTestimonialCarousel 
        CustomTestimonials="featuredTestimonials"
        Speed="currentSpeed"
        Direction="currentDirection"
        ShowRating="true"
        PauseOnHover="true"
        CardElevated="true"
        CardSize="TestimonialSize.Large"
        MaxVisibleTestimonials="6" />
    
    <div class="showcase-stats mt-5">
        <div class="container">
            <div class="row text-center">
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">500+</div>
                        <div class="stat-label">Happy Customers</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">4.9/5</div>
                        <div class="stat-label">Average Rating</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">99.9%</div>
                        <div class="stat-label">Satisfaction Rate</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">24/7</div>
                        <div class="stat-label">Support</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private CarouselSpeed currentSpeed = CarouselSpeed.Normal;
    private AnimationDirection currentDirection = AnimationDirection.Right;
    
    private List<TestimonialItem> featuredTestimonials = new()
    {
        new("Elena Rodriguez", 
            "VP of Product", 
            "GlobalTech Inc.",
            "This platform has transformed how we approach product development. The insights and analytics have been invaluable for making data-driven decisions.",
            "https://images.unsplash.com/photo-1494790108755-2616b612b278?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/elenarodriguez",
            ShowRating: true,
            Rating: 5,
            ReadMoreHref: "/case-studies/globaltech",
            ReadMoreText: "Read case study"),

        new("James Wilson", 
            "Technical Director", 
            "Enterprise Solutions",
            "Exceptional platform with outstanding support. Our development team has become more productive, and our deployment confidence has increased dramatically.",
            "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/jameswilson",
            ShowRating: true,
            Rating: 5),

        new("Priya Patel", 
            "Head of Operations", 
            "ScaleTech",
            "The scalability and reliability of this solution have been perfect for our rapidly growing business. Implementation was smooth and results were immediate.",
            "https://images.unsplash.com/photo-1580489944761-15a19d654956?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/priyapatel",
            ShowRating: true,
            Rating: 5,
            ReadMoreHref: "/testimonials/priya-patel",
            ReadMoreText: "Watch video testimonial"),

        new("Robert Chen", 
            "Lead Architect", 
            "Cloud Innovations",
            "Best-in-class architecture and design. The platform's flexibility has allowed us to customize solutions that perfectly fit our unique requirements.",
            "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/robertchen",
            ShowRating: true,
            Rating: 5),

        new("Maria Santos", 
            "Digital Transformation Lead", 
            "Fortune500 Corp",
            "This solution was key to our digital transformation success. The ROI exceeded expectations, and our operational efficiency has improved by 60%.",
            "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/mariasantos",
            ShowRating: true,
            Rating: 5,
            ReadMoreHref: "/case-studies/fortune500",
            ReadMoreText: "View transformation story"),

        new("Andrew Kim", 
            "Startup Founder", 
            "InnovateLab",
            "As a startup, we needed something reliable and scalable. This platform delivered on both fronts and has grown with us from day one.",
            "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/andrewkim",
            ShowRating: true,
            Rating: 4)
    };
    
    private void SetSpeed(CarouselSpeed speed)
    {
        currentSpeed = speed;
        StateHasChanged();
    }
    
    private void ToggleDirection()
    {
        currentDirection = currentDirection == AnimationDirection.Right 
            ? AnimationDirection.Left 
            : AnimationDirection.Right;
        StateHasChanged();
    }
}

<style>
.testimonial-showcase {
    padding: 4rem 0;
    background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
}

.showcase-header {
    margin-bottom: 3rem;
}

.carousel-controls .btn {
    border-radius: 25px;
    padding: 0.5rem 1.5rem;
    font-weight: 500;
    transition: all 0.3s ease;
}

.carousel-controls .btn.active {
    background-color: #007bff;
    border-color: #007bff;
    color: white;
}

.showcase-stats {
    background: rgba(255, 255, 255, 0.9);
    border-radius: 1rem;
    padding: 3rem 0;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
}

.stat-item {
    padding: 1rem;
}

.stat-number {
    font-size: 2.5rem;
    font-weight: bold;
    color: #007bff;
    margin-bottom: 0.5rem;
}

.stat-label {
    font-size: 1rem;
    color: #6c757d;
    text-transform: uppercase;
    letter-spacing: 1px;
}

@media (max-width: 768px) {
    .carousel-controls {
        flex-direction: column;
        gap: 1rem;
    }
    
    .stat-number {
        font-size: 2rem;
    }
}
</style>
```

### Full-Width Testimonial Section

```razor
<section class="testimonials-hero position-relative overflow-hidden">
    <div class="hero-background">
        <div class="gradient-overlay"></div>
    </div>
    
    <div class="container-fluid px-0">
        <div class="row">
            <div class="col-12 text-center mb-5 pt-5">
                <div class="hero-content">
                    <h1 class="display-3 fw-bold text-white mb-4">
                        Trusted by Industry Leaders
                    </h1>
                    <p class="lead text-white-50 mb-5">
                        Join thousands of companies that have transformed their business with our platform
                    </p>
                </div>
            </div>
        </div>
        
        <OsirionTestimonialCarousel 
            CustomTestimonials="industryLeaderTestimonials"
            ShowRating="true"
            Speed="CarouselSpeed.Slow"
            CardVariant="TestimonialVariant.Glass"
            CardSize="TestimonialSize.Large"
            CardWidth="500"
            CardGap="40"
            PauseOnHover="true"
            InheritTheme="true" />
    </div>
    
    <div class="container mt-5 pb-5">
        <div class="row justify-content-center">
            <div class="col-lg-8 text-center">
                <div class="cta-section">
                    <h3 class="text-white mb-4">Ready to Join Them?</h3>
                    <div class="d-flex gap-3 justify-content-center">
                        <button class="btn btn-light btn-lg">Start Free Trial</button>
                        <button class="btn btn-outline-light btn-lg">Schedule Demo</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>

@code {
    private List<TestimonialItem> industryLeaderTestimonials = new()
    {
        new("Sarah Mitchell", 
            "Chief Technology Officer", 
            "Fortune 100 Enterprise",
            "This platform has been instrumental in our digital transformation journey. The enterprise-grade security and scalability features give us confidence to innovate at scale.",
            "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=200&h=200&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/sarahmitchell",
            ShowRating: true,
            Rating: 5,
            ReadMoreHref: "/enterprise-success",
            ReadMoreText: "Read enterprise case study"),

        new("Michael Rodriguez", 
            "VP of Engineering", 
            "Global Software Corp",
            "The development velocity improvements we've achieved are remarkable. Our time-to-market has decreased by 40% while maintaining the highest quality standards.",
            "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=200&h=200&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/michaelrodriguez",
            ShowRating: true,
            Rating: 5),

        new("Lisa Zhang", 
            "Head of Digital Innovation", 
            "TechGiant Industries",
            "Exceptional platform that has enabled us to streamline operations across multiple business units. The ROI has been substantial and continues to grow.",
            "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=200&h=200&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/lisazhang",
            ShowRating: true,
            Rating: 5,
            ReadMoreHref: "/digital-transformation",
            ReadMoreText: "Explore transformation story"),

        new("David Thompson", 
            "Lead Solutions Architect", 
            "Cloud Infrastructure Pro",
            "The architectural flexibility and cloud-native design have been perfect for our multi-cloud strategy. Performance and reliability are outstanding.",
            "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=200&h=200&fit=crop&crop=face&auto=format&q=80",
            "https://linkedin.com/in/davidthompson",
            ShowRating: true,
            Rating: 5)
    };
}

<style>
.testimonials-hero {
    min-height: 100vh;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    display: flex;
    flex-direction: column;
    justify-content: center;
}

.hero-background {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    z-index: 0;
}

.gradient-overlay {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear-gradient(45deg, 
        rgba(102, 126, 234, 0.8) 0%, 
        rgba(118, 75, 162, 0.8) 100%);
}

.hero-content {
    position: relative;
    z-index: 2;
    padding: 2rem;
}

.cta-section {
    position: relative;
    z-index: 2;
    background: rgba(255, 255, 255, 0.1);
    padding: 3rem;
    border-radius: 1rem;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
}

.btn-lg {
    padding: 0.75rem 2rem;
    font-size: 1.125rem;
    border-radius: 25px;
    font-weight: 500;
    transition: all 0.3s ease;
}

.btn-light:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.btn-outline-light:hover {
    transform: translateY(-2px);
    background: rgba(255, 255, 255, 0.1);
}

/* Ensure testimonial carousel has proper z-index */
.osirion-testimonial-carousel {
    position: relative;
    z-index: 1;
}

@media (max-width: 768px) {
    .testimonials-hero {
        min-height: auto;
        padding: 3rem 0;
    }
    
    .hero-content h1 {
        font-size: 2.5rem;
    }
    
    .cta-section {
        padding: 2rem;
    }
    
    .d-flex.gap-3 {
        flex-direction: column;
        gap: 1rem !important;
    }
}
</style>
```

## Performance Optimization

### Loading Strategies

```razor
<!-- Preload first testimonial image for LCP optimization -->
@{
    var firstImageUrl = GetFirstTestimonialImage();
}

@if (!string.IsNullOrEmpty(firstImageUrl))
{
    <link rel="preload" as="image" href="@firstImageUrl" />
}

<OsirionTestimonialCarousel 
    CustomTestimonials="optimizedTestimonials"
    MaxVisibleTestimonials="8"
    ShowRating="true"
    Speed="CarouselSpeed.Normal" />

@code {
    private string? GetFirstTestimonialImage()
    {
        return optimizedTestimonials?.FirstOrDefault()?.ProfileImageUrl;
    }
    
    private List<TestimonialItem> optimizedTestimonials = new()
    {
        // Use optimized image URLs with proper sizing and format
        new("John Doe", 
            "CTO", 
            "TechCorp",
            "Amazing platform!",
            "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face&auto=format&q=80&fm=webp",
            ShowRating: true,
            Rating: 5)
        // More testimonials...
    };
}
```

## Styling Examples

### Bootstrap Integration

```razor
<div class="bg-light py-5">
    <div class="container">
        <div class="row">
            <div class="col-12 text-center mb-5">
                <h2 class="display-5 fw-bold">Customer Reviews</h2>
            </div>
        </div>
    </div>
    
    <OsirionTestimonialCarousel 
        ShowRating="true"
        CardElevated="true"
        Speed="CarouselSpeed.Normal"
        Class="bootstrap-testimonials" />
</div>

<style>
.bootstrap-testimonials .osirion-testimonial-card {
    background: white;
    border: 1px solid var(--bs-border-color);
    border-radius: var(--bs-border-radius);
    box-shadow: var(--bs-box-shadow);
}

.bootstrap-testimonials .osirion-testimonial-card:hover {
    border-color: var(--bs-primary);
    box-shadow: var(--bs-box-shadow-lg);
}
</style>
```

### Tailwind CSS Integration

```razor
<div class="bg-gray-50 py-16">
    <div class="container mx-auto px-4">
        <div class="text-center mb-12">
            <h2 class="text-4xl font-bold text-gray-900">What Our Customers Say</h2>
        </div>
    </div>
    
    <OsirionTestimonialCarousel 
        ShowRating="true"
        CardVariant="TestimonialVariant.Minimal"
        Speed="CarouselSpeed.Slow"
        Class="tailwind-testimonials" />
</div>

<style>
.tailwind-testimonials .osirion-testimonial-card {
    @apply bg-white rounded-lg shadow-md border border-gray-200 transition-all duration-300;
}

.tailwind-testimonials .osirion-testimonial-card:hover {
    @apply shadow-lg border-blue-300 transform -translate-y-1;
}
</style>
```

## Best Practices

### Content Guidelines

1. **Authentic Testimonials**: Use real customer testimonials with permission
2. **Diverse Representation**: Include testimonials from various customer types
3. **Specific Details**: Include specific results and metrics when possible
4. **Regular Updates**: Keep testimonials current and relevant
5. **Quality Images**: Use high-quality, professional profile photos

### Performance Optimization

1. **Image Optimization**: Use properly sized and compressed images
2. **Limit Testimonials**: Use MaxVisibleTestimonials for better performance
3. **Lazy Loading**: Implement lazy loading for non-critical testimonials
4. **CSS Animations**: Leverage CSS animations over JavaScript for better performance
5. **Preload Critical Images**: Preload first visible testimonial images

### Accessibility

1. **Semantic Markup**: Use proper ARIA labels and roles
2. **Keyboard Navigation**: Ensure carousel is accessible via keyboard
3. **Screen Reader Support**: Provide appropriate alt text and descriptions
4. **Pause Controls**: Allow users to pause animations if needed
5. **Reduced Motion**: Respect user preferences for reduced motion

### User Experience

1. **Appropriate Speed**: Choose animation speed that's comfortable to read
2. **Pause on Hover**: Enable pause on hover for better readability
3. **Mobile Optimization**: Ensure testimonials work well on mobile devices
4. **Loading States**: Provide loading indicators when needed
5. **Error Handling**: Handle cases where testimonials fail to load

The OsirionTestimonialCarousel component provides a powerful and flexible way to showcase customer testimonials that builds trust and social proof for your business.
