---
id: 'infinite-logo-carousel'
order: 1
layout: docs
title: InfiniteLogoCarousel Component
permalink: /docs/components/core/carousels/infinite-logo-carousel
description: Learn how to use the InfiniteLogoCarousel component to display client logos in an infinite scrolling animation with support for light/dark themes and customizable styling.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Carousels
tags:
- blazor
- logos
- carousel
- infinite scroll
- clients
- partners
- branding
- animation
is_featured: true
published: true
slug: infinite-logo-carousel
lang: en
custom_fields: {}
seo_properties:
  title: 'InfiniteLogoCarousel Component - Client Logo Display | Osirion.Blazor'
  description: 'Display client logos in an infinite scrolling carousel. Features dual theme support, grayscale effects, and customizable animations.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/carousels/infinite-logo-carousel'
  lang: en
  robots: index, follow
  og_title: 'InfiniteLogoCarousel Component - Osirion.Blazor'
  og_description: 'Display client logos in an infinite scrolling carousel with theme support and animations.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'InfiniteLogoCarousel Component - Osirion.Blazor'
  twitter_description: 'Display client logos in an infinite scrolling carousel.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# InfiniteLogoCarousel Component

The InfiniteLogoCarousel component creates an infinite scrolling display of client logos, perfect for showcasing partnerships, clients, or technology stack. It provides smooth CSS-based animations, dual theme support, and works seamlessly in Static SSR environments.

## Component Overview

InfiniteLogoCarousel is designed to build trust and credibility by displaying your business relationships and partnerships. The component features professional grayscale effects, dual logo support for light/dark themes, and customizable animations that don't require JavaScript.

### Key Features

**Infinite Scrolling**: Seamless continuous animation without visible loops
**Dual Theme Support**: Separate logos for light and dark themes
**Grayscale Effects**: Professional grayscale with hover color effects
**CSS-Based Animation**: High performance without JavaScript dependencies
**Bidirectional Scrolling**: Support for left-to-right or right-to-left motion
**Responsive Design**: Adapts to different screen sizes automatically
**SEO Friendly**: Proper semantic markup with alt text and links
**Pause on Hover**: Optional pause functionality for better UX
**Customizable Sizing**: Flexible logo dimensions and spacing
**Performance Optimized**: Lazy loading and efficient rendering

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | Optional title to display above the logos. |
| `SectionTitle` | `string` | `"Our Clients"` | Section aria-label for accessibility. |
| `AnimationDuration` | `int` | `60` | Animation duration in seconds. |
| `CustomLogos` | `List<LogoItem>?` | `null` | Custom list of logos. Uses defaults if not provided. |
| `PauseOnHover` | `bool` | `true` | Whether to pause animation on hover. |
| `Direction` | `AnimationDirection` | `Right` | Animation direction (Left or Right). |
| `EnableGrayscale` | `bool` | `true` | Whether to enable grayscale effect for logos. |
| `LogoWidth` | `int` | `200` | Logo width in pixels. |
| `LogoHeight` | `int` | `80` | Logo height in pixels. |
| `LogoGap` | `int` | `24` | Gap between logos in pixels. |
| `MaxVisibleLogos` | `int?` | `null` | Maximum logos to display before repeating. |

## LogoItem Record

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ImageUrl` | `string` | - | Main logo image URL (required). |
| `AltText` | `string` | - | Alt text for accessibility (required). |
| `LightImageUrl` | `string?` | `null` | Logo URL for light theme. |
| `DarkImageUrl` | `string?` | `null` | Logo URL for dark theme. |
| `Url` | `string?` | `null` | Link URL when logo is clicked. |
| `Target` | `string?` | `"_blank"` | Link target (e.g., _blank, _self). |
| `NoFollow` | `bool?` | `true` | Whether to add rel="nofollow" to links. |
| `NoOpener` | `bool?` | `true` | Whether to add rel="noopener" to links. |
| `EnableGrayscale` | `bool?` | `null` | Override global grayscale setting for this logo. |

## Basic Usage

### Simple Logo Carousel

```razor
@using Osirion.Blazor.Components

<section class="clients-section py-5 bg-light">
    <div class="container">
        <div class="row">
            <div class="col-12 text-center mb-4">
                <h2 class="display-5 fw-bold">Trusted by Industry Leaders</h2>
                <p class="lead text-muted">Join thousands of companies that rely on our platform</p>
            </div>
        </div>
    </div>
    
    <InfiniteLogoCarousel />
</section>
```

### Carousel with Custom Title

```razor
<InfiniteLogoCarousel 
    Title="Our Technology Partners"
    SectionTitle="Technology partners and integrations"
    EnableGrayscale="true"
    PauseOnHover="true" />
```

### Custom Animation Settings

```razor
<InfiniteLogoCarousel 
    AnimationDuration="45"
    Direction="AnimationDirection.Left"
    LogoWidth="180"
    LogoHeight="70"
    LogoGap="32"
    EnableGrayscale="false" />
```

## Advanced Usage

### Custom Logo Data

```razor
@using Osirion.Blazor.Components

<InfiniteLogoCarousel 
    CustomLogos="clientLogos"
    Title="Featured Clients"
    EnableGrayscale="true"
    LogoWidth="220"
    LogoHeight="90" />

@code {
    private List<LogoItem> clientLogos = new()
    {
        // Standard logo
        new("https://cdn.company-a.com/logo.png",
            "Company A - Enterprise Software Solutions",
            Url: "https://company-a.com"),

        // Dual theme logo (different for light/dark)
        new("https://cdn.company-b.com/logo-default.png",
            "Company B - Digital Innovation",
            LightImageUrl: "https://cdn.company-b.com/logo-light.png",
            DarkImageUrl: "https://cdn.company-b.com/logo-dark.png",
            Url: "https://company-b.com"),

        // Logo without grayscale effect
        new("https://cdn.company-c.com/colorful-logo.png",
            "Company C - Creative Agency",
            Url: "https://company-c.com",
            EnableGrayscale: false),

        // Logo with custom link behavior
        new("https://cdn.company-d.com/logo.svg",
            "Company D - Technology Partner",
            Url: "https://company-d.com",
            Target: "_self",
            NoFollow: false,
            NoOpener: false),

        // Non-clickable logo
        new("https://cdn.company-e.com/logo.png",
            "Company E - Strategic Partner"),

        new("https://cdn.company-f.com/logo.png",
            "Company F - Global Enterprise",
            Url: "https://company-f.com"),

        new("https://cdn.company-g.com/logo.svg",
            "Company G - Innovation Labs",
            Url: "https://company-g.com"),

        new("https://cdn.company-h.com/logo.png",
            "Company H - Tech Solutions",
            Url: "https://company-h.com")
    };
}
```

### Interactive Logo Showcase

```razor
@using Osirion.Blazor.Components

<div class="logo-showcase">
    <div class="showcase-header text-center mb-5">
        <h2 class="display-4 fw-bold">Our Ecosystem</h2>
        <p class="lead">Connecting with the best in the industry</p>
        
        <div class="showcase-controls mt-4">
            <div class="btn-group" role="group" aria-label="Display options">
                <button class="btn btn-outline-primary @(showGrayscale ? "active" : "")"
                        @onclick="ToggleGrayscale">
                    <i class="fas fa-adjust me-2"></i>
                    @(showGrayscale ? "Color Mode" : "Grayscale Mode")
                </button>
                
                <button class="btn btn-outline-primary"
                        @onclick="ToggleDirection">
                    <i class="fas fa-exchange-alt me-2"></i>
                    @(currentDirection == AnimationDirection.Right ? "← Reverse" : "Forward →")
                </button>
                
                <button class="btn btn-outline-primary"
                        @onclick="ToggleSpeed">
                    <i class="fas fa-tachometer-alt me-2"></i>
                    Speed: @currentSpeed
                </button>
            </div>
        </div>
    </div>
    
    <InfiniteLogoCarousel 
        CustomLogos="partnerLogos"
        EnableGrayscale="showGrayscale"
        Direction="currentDirection"
        AnimationDuration="currentAnimationDuration"
        LogoWidth="200"
        LogoHeight="80"
        LogoGap="40"
        PauseOnHover="true" />
    
    <div class="showcase-stats mt-5">
        <div class="container">
            <div class="row text-center">
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">150+</div>
                        <div class="stat-label">Partners</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">50+</div>
                        <div class="stat-label">Countries</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">1M+</div>
                        <div class="stat-label">Users Served</div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="stat-item">
                        <div class="stat-number">99.9%</div>
                        <div class="stat-label">Uptime</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private bool showGrayscale = true;
    private AnimationDirection currentDirection = AnimationDirection.Right;
    private string currentSpeed = "Normal";
    private int currentAnimationDuration = 60;
    
    private List<LogoItem> partnerLogos = new()
    {
        // Technology Partners
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/microsoft/microsoft-original.svg",
            "Microsoft - Cloud Computing Partner",
            Url: "https://microsoft.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/google/google-original.svg",
            "Google - Cloud Platform Partner",
            Url: "https://cloud.google.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/amazonwebservices/amazonwebservices-original.svg",
            "Amazon Web Services - Infrastructure Partner",
            Url: "https://aws.amazon.com"),
            
        // Framework Partners with dual logos
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dot-net/dot-net-original.svg",
            ".NET - Development Framework",
            LightImageUrl: "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dot-net/dot-net-original.svg",
            DarkImageUrl: "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dot-net/dot-net-plain.svg",
            Url: "https://dotnet.microsoft.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/azure/azure-original.svg",
            "Microsoft Azure - Cloud Services",
            Url: "https://azure.microsoft.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/docker/docker-original.svg",
            "Docker - Containerization Platform",
            Url: "https://docker.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/kubernetes/kubernetes-plain.svg",
            "Kubernetes - Container Orchestration",
            Url: "https://kubernetes.io"),
            
        // Database Partners
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/postgresql/postgresql-original.svg",
            "PostgreSQL - Database Partner",
            Url: "https://postgresql.org"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/redis/redis-original.svg",
            "Redis - Caching Solution",
            Url: "https://redis.io"),
            
        // Monitoring and Analytics
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/grafana/grafana-original.svg",
            "Grafana - Monitoring and Analytics",
            Url: "https://grafana.com")
    };
    
    private void ToggleGrayscale()
    {
        showGrayscale = !showGrayscale;
        StateHasChanged();
    }
    
    private void ToggleDirection()
    {
        currentDirection = currentDirection == AnimationDirection.Right 
            ? AnimationDirection.Left 
            : AnimationDirection.Right;
        StateHasChanged();
    }
    
    private void ToggleSpeed()
    {
        switch (currentSpeed)
        {
            case "Slow":
                currentSpeed = "Normal";
                currentAnimationDuration = 60;
                break;
            case "Normal":
                currentSpeed = "Fast";
                currentAnimationDuration = 30;
                break;
            case "Fast":
                currentSpeed = "Slow";
                currentAnimationDuration = 90;
                break;
        }
        StateHasChanged();
    }
}

<style>
.logo-showcase {
    padding: 4rem 0;
    background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
}

.showcase-controls .btn {
    border-radius: 25px;
    padding: 0.75rem 1.5rem;
    font-weight: 500;
    transition: all 0.3s ease;
}

.showcase-controls .btn.active {
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
    margin-top: 3rem;
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
    .showcase-controls .btn-group {
        flex-direction: column;
        width: 100%;
    }
    
    .showcase-controls .btn {
        margin-bottom: 0.5rem;
        border-radius: 0.5rem;
    }
    
    .stat-number {
        font-size: 2rem;
    }
}
</style>
```

### Technology Stack Display

```razor
<section class="tech-stack-section py-5">
    <div class="container">
        <div class="row">
            <div class="col-12 text-center mb-5">
                <h2 class="display-5 fw-bold">Built with Industry-Leading Technology</h2>
                <p class="lead text-muted">Powered by the most trusted platforms and frameworks</p>
            </div>
        </div>
    </div>
    
    <!-- Frontend Technologies -->
    <div class="tech-category mb-5">
        <h4 class="text-center mb-3 text-primary">Frontend Technologies</h4>
        <InfiniteLogoCarousel 
            CustomLogos="frontendTechs"
            EnableGrayscale="false"
            LogoWidth="160"
            LogoHeight="60"
            AnimationDuration="50"
            MaxVisibleLogos="6" />
    </div>
    
    <!-- Backend Technologies -->
    <div class="tech-category mb-5">
        <h4 class="text-center mb-3 text-success">Backend & Cloud</h4>
        <InfiniteLogoCarousel 
            CustomLogos="backendTechs"
            Direction="AnimationDirection.Left"
            EnableGrayscale="true"
            LogoWidth="160"
            LogoHeight="60"
            AnimationDuration="45"
            MaxVisibleLogos="8" />
    </div>
    
    <!-- DevOps & Infrastructure -->
    <div class="tech-category">
        <h4 class="text-center mb-3 text-warning">DevOps & Infrastructure</h4>
        <InfiniteLogoCarousel 
            CustomLogos="devopsTechs"
            EnableGrayscale="false"
            LogoWidth="160"
            LogoHeight="60"
            AnimationDuration="55"
            MaxVisibleLogos="6" />
    </div>
</section>

@code {
    private List<LogoItem> frontendTechs = new()
    {
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/blazor/blazor-original.svg",
            "Blazor - Modern Web UI Framework",
            Url: "https://blazor.net"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/typescript/typescript-original.svg",
            "TypeScript - Typed JavaScript",
            Url: "https://typescriptlang.org"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/sass/sass-original.svg",
            "Sass - CSS Preprocessor",
            Url: "https://sass-lang.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/bootstrap/bootstrap-original.svg",
            "Bootstrap - CSS Framework",
            Url: "https://getbootstrap.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/tailwindcss/tailwindcss-plain.svg",
            "Tailwind CSS - Utility-First CSS",
            Url: "https://tailwindcss.com")
    };
    
    private List<LogoItem> backendTechs = new()
    {
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg",
            ".NET Core - Cross-Platform Runtime",
            Url: "https://dotnet.microsoft.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg",
            "C# - Programming Language",
            Url: "https://docs.microsoft.com/en-us/dotnet/csharp/"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/postgresql/postgresql-original.svg",
            "PostgreSQL - Advanced Database",
            Url: "https://postgresql.org"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/redis/redis-original.svg",
            "Redis - In-Memory Data Store",
            Url: "https://redis.io"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/azure/azure-original.svg",
            "Microsoft Azure - Cloud Platform",
            Url: "https://azure.microsoft.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/amazonwebservices/amazonwebservices-original.svg",
            "AWS - Cloud Services",
            Url: "https://aws.amazon.com")
    };
    
    private List<LogoItem> devopsTechs = new()
    {
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/docker/docker-original.svg",
            "Docker - Containerization",
            Url: "https://docker.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/kubernetes/kubernetes-plain.svg",
            "Kubernetes - Container Orchestration",
            Url: "https://kubernetes.io"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/github/github-original.svg",
            "GitHub - Version Control & CI/CD",
            Url: "https://github.com"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/terraform/terraform-original.svg",
            "Terraform - Infrastructure as Code",
            Url: "https://terraform.io"),
            
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/grafana/grafana-original.svg",
            "Grafana - Monitoring & Analytics",
            Url: "https://grafana.com")
    };
}

<style>
.tech-stack-section {
    background: #f8f9fa;
}

.tech-category {
    margin-bottom: 3rem;
    padding: 2rem;
    background: white;
    border-radius: 1rem;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.tech-category h4 {
    font-weight: 600;
    margin-bottom: 1.5rem;
}

/* Custom styling for different tech categories */
.tech-category:nth-child(2) {
    border-left: 4px solid #007bff;
}

.tech-category:nth-child(3) {
    border-left: 4px solid #28a745;
}

.tech-category:nth-child(4) {
    border-left: 4px solid #ffc107;
}

@media (max-width: 768px) {
    .tech-category {
        padding: 1rem;
        margin-bottom: 2rem;
    }
}
</style>
```

## Styling Examples

### Bootstrap Integration

```razor
<div class="bg-light py-5">
    <div class="container">
        <div class="row">
            <div class="col-12 text-center mb-4">
                <h3 class="fw-bold">Our Partners</h3>
            </div>
        </div>
    </div>
    
    <InfiniteLogoCarousel 
        EnableGrayscale="true"
        Class="bootstrap-logos" />
</div>

<style>
.bootstrap-logos .osirion-logo-item {
    padding: 0.5rem 1rem;
    border-radius: var(--bs-border-radius);
    transition: all 0.3s ease;
}

.bootstrap-logos .osirion-logo-item:hover {
    background: rgba(var(--bs-primary-rgb), 0.1);
    transform: translateY(-2px);
}

.bootstrap-logos .osirion-logo-grayscale img {
    filter: grayscale(100%) brightness(1.2);
}

.bootstrap-logos .osirion-logo-grayscale:hover img {
    filter: grayscale(0%) brightness(1);
}
</style>
```

### Tailwind CSS Integration

```razor
<div class="bg-gray-50 py-16">
    <div class="container mx-auto px-4">
        <div class="text-center mb-8">
            <h3 class="text-3xl font-bold text-gray-900">Trusted Partners</h3>
        </div>
    </div>
    
    <InfiniteLogoCarousel 
        EnableGrayscale="true"
        Class="tailwind-logos" />
</div>

<style>
.tailwind-logos .osirion-logo-item {
    @apply px-4 py-2 rounded-lg transition-all duration-300;
}

.tailwind-logos .osirion-logo-item:hover {
    @apply bg-blue-50 transform -translate-y-1;
}

.tailwind-logos .osirion-logo-grayscale img {
    @apply filter grayscale brightness-110;
}

.tailwind-logos .osirion-logo-grayscale:hover img {
    @apply filter-none brightness-100;
}
</style>
```

## Accessibility Features

### Screen Reader Support

```razor
<InfiniteLogoCarousel 
    CustomLogos="accessibleLogos"
    SectionTitle="Our trusted technology partners and clients"
    Title="Partner Ecosystem" />

@code {
    private List<LogoItem> accessibleLogos = new()
    {
        // Descriptive alt text for screen readers
        new("https://example.com/logo1.png",
            "Acme Corporation - Fortune 500 technology partner specializing in enterprise software solutions",
            Url: "https://acme.com"),
            
        new("https://example.com/logo2.png",
            "Global Tech Industries - Leading provider of cloud infrastructure and data analytics platforms",
            Url: "https://globaltech.com"),
            
        // Non-clickable logo with descriptive alt text
        new("https://example.com/certification.png",
            "ISO 27001 Security Certification - International standard for information security management")
    };
}
```

## Performance Optimization

### Image Optimization

```razor
@using Osirion.Blazor.Components

<!-- Preload first few logo images for better LCP -->
@foreach (var logo in GetFirstFewLogos())
{
    <link rel="preload" as="image" href="@logo.ImageUrl" />
}

<InfiniteLogoCarousel 
    CustomLogos="optimizedLogos"
    MaxVisibleLogos="12"
    EnableGrayscale="true" />

@code {
    private List<LogoItem> GetFirstFewLogos()
    {
        return optimizedLogos.Take(3).ToList();
    }
    
    private List<LogoItem> optimizedLogos = new()
    {
        // Use optimized image formats and sizes
        new("https://cdn.example.com/logo1.webp?w=200&h=80&q=85",
            "Company A - Technology Solutions",
            Url: "https://company-a.com"),
            
        new("https://cdn.example.com/logo2.webp?w=200&h=80&q=85",
            "Company B - Digital Innovation",
            Url: "https://company-b.com")
        // More optimized logos...
    };
}
```

## Best Practices

### Content Guidelines

1. **Consistent Sizing**: Use logos with consistent aspect ratios
2. **High Quality**: Provide high-resolution logos for crisp display
3. **Permission**: Ensure you have permission to display partner logos
4. **Currency**: Keep logos current and remove outdated partnerships
5. **Diversity**: Show a diverse range of partnerships and technologies

### Performance Optimization

1. **Image Formats**: Use modern formats like WebP when possible
2. **Compression**: Optimize images for web delivery
3. **Preloading**: Preload first visible logos for better LCP
4. **Lazy Loading**: Enable lazy loading for non-critical logos
5. **CDN Usage**: Serve logos from a CDN for better performance

### Accessibility

1. **Alt Text**: Provide descriptive alt text for all logos
2. **Keyboard Navigation**: Ensure logo links are keyboard accessible
3. **Screen Readers**: Use proper ARIA labels and descriptions
4. **Motion Preferences**: Consider respecting reduced motion preferences
5. **Focus Indicators**: Provide clear focus indicators for linked logos

### User Experience

1. **Animation Speed**: Choose appropriate animation speeds for readability
2. **Pause Functionality**: Allow users to pause animations
3. **Mobile Optimization**: Ensure carousels work well on mobile devices
4. **Loading States**: Provide loading indicators when needed
5. **Error Handling**: Handle cases where logos fail to load gracefully

The InfiniteLogoCarousel component provides a professional and performant way to showcase your business relationships and build credibility with visitors.
