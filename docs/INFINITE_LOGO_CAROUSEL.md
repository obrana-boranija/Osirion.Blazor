# InfiniteLogoCarousel Component

[![Component](https://img.shields.io/badge/Component-Core-blue)](https://github.com/obrana-boranija/Osirion.Blazor/tree/master/src/Osirion.Blazor.Core)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor.Core)](https://www.nuget.org/packages/Osirion.Blazor.Core)

The `InfiniteLogoCarousel` component is a self-contained, CSS-only logo carousel that displays client/partner logos in a continuous scrolling animation. It's fully SSR-compatible, requires zero JavaScript dependencies, and works seamlessly across all modern browsers.

## Features

- **Self-Contained**: All CSS animations and styles included in component file
- **Zero Dependencies**: No global CSS files or external resources needed
- **Dual Logo Support**: Automatic theme-aware logo switching for light/dark modes
- **Perfect Containment**: Hover effects never escape container bounds
- **Infinite Animation**: Seamless looping with duplicated logo sets
- **Accessibility**: Full keyboard navigation and screen reader support
- **Framework Integration**: Works with Bootstrap, Fluent UI, MudBlazor, and Radzen
- **Performance Optimized**: Hardware-accelerated CSS animations
- **Responsive Design**: Adapts to different screen sizes

## Basic Usage

```razor
@using Osirion.Blazor.Components

<!-- Simple logo carousel -->
<InfiniteLogoCarousel Title="Our Partners" />

<!-- With custom logos -->
<InfiniteLogoCarousel 
    Title="Technology Stack"
    CustomLogos="@GetPartnerLogos()" />
```

## Advanced Usage

### Full Configuration

```razor
<InfiniteLogoCarousel 
    Title="Our Technology Partners"
    SectionTitle="Trusted by Industry Leaders"
    AnimationDuration="45"
    Direction="AnimationDirection.Left"
    PauseOnHover="true"
    EnableGrayscale="true"
    LogoWidth="180"
    LogoHeight="70"
    LogoGap="20"
    MaxVisibleLogos="8"
    CustomLogos="@GetAdvancedLogos()" />

@code {
    private List<LogoItem> GetAdvancedLogos()
    {
        return new List<LogoItem>
        {
            // Dual logo with theme switching
            new("company-a-default.png", "Company A",
                LightImageUrl: "company-a-light.png",
                DarkImageUrl: "company-a-dark.png",
                Url: "https://company-a.com",
                NoFollow: false), // Trusted partner
                
            // Single logo with custom settings
            new("company-b-logo.png", "Company B",
                Url: "https://company-b.com",
                EnableGrayscale: false), // Always colorful
                
            // Logo without link
            new("partner-logo.png", "Partner Organization")
        };
    }
}
```

### E-commerce Partners Section

```razor
<InfiniteLogoCarousel 
    Title="Trusted Brands"
    AnimationDuration="60"
    PauseOnHover="true"
    LogoWidth="200"
    LogoHeight="80"
    CustomLogos="@GetBrandLogos()"
    Class="brands-carousel" />

@code {
    private List<LogoItem> GetBrandLogos()
    {
        return new List<LogoItem>
        {
            new("https://cdn.example.com/brand1.png",
                "Premium Brand",
                Url: "https://brand1.com",
                EnableGrayscale: true),
            new("https://cdn.example.com/brand2.png",
                "Luxury Brand",
                Url: "https://brand2.com",
                EnableGrayscale: true),
            // Add more brands...
        };
    }
}
```

### Technology Stack Showcase

```razor
<InfiniteLogoCarousel 
    Title="Built With Modern Technology"
    Direction="AnimationDirection.Right"
    AnimationDuration="40"
    EnableGrayscale="false"
    LogoWidth="160"
    LogoHeight="60"
    CustomLogos="@GetTechLogos()" />

@code {
    private List<LogoItem> GetTechLogos()
    {
        return new List<LogoItem>
        {
            new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/blazor/blazor-original.svg",
                "Blazor",
                Url: "https://blazor.net"),
            new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg",
                ".NET Core",
                Url: "https://dotnet.microsoft.com"),
            new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/azure/azure-original.svg",
                "Microsoft Azure",
                Url: "https://azure.microsoft.com"),
            // Add more technologies...
        };
    }
}
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | Optional title displayed above logos |
| `SectionTitle` | `string` | `"Our Clients"` | ARIA label for accessibility |
| `AnimationDuration` | `int` | `60` | Animation duration in seconds |
| `Direction` | `AnimationDirection` | `Right` | Animation direction (Left/Right) |
| `PauseOnHover` | `bool` | `true` | Pause animation on hover |
| `EnableGrayscale` | `bool` | `true` | Global grayscale effect setting |
| `LogoWidth` | `int` | `200` | Logo width in pixels |
| `LogoHeight` | `int` | `80` | Logo height in pixels |
| `LogoGap` | `int` | `24` | Gap between logos in pixels |
| `MaxVisibleLogos` | `int?` | `null` | Limit visible logos for performance |
| `CustomLogos` | `List<LogoItem>?` | `null` | Custom logo list |

## LogoItem Configuration

The `LogoItem` record provides comprehensive logo configuration:

```csharp
public record LogoItem(
    string ImageUrl,              // Primary image URL
    string AltText,               // Accessibility text
    string? LightImageUrl = null, // Light theme variant
    string? DarkImageUrl = null,  // Dark theme variant
    string? Url = null,           // Optional link URL
    string? Target = null,        // Link target (_blank, _self, etc.)
    bool? NoFollow = null,        // SEO nofollow attribute
    bool? NoOpener = null,        // Security noopener attribute
    bool? EnableGrayscale = null  // Override global grayscale setting
);
```

### LogoItem Examples

```csharp
// Basic logo
new LogoItem("logo.png", "Company Name")

// Logo with link
new LogoItem("logo.png", "Company Name", Url: "https://company.com")

// Dual logo for light/dark themes
new LogoItem("default.png", "Company",
    LightImageUrl: "light-logo.png",
    DarkImageUrl: "dark-logo.png",
    Url: "https://company.com")

// Logo with custom link behavior
new LogoItem("logo.png", "Partner",
    Url: "https://partner.com",
    Target: "_blank",
    NoFollow: false,      // Trusted partner
    NoOpener: true)       // Security

// Always colorful logo
new LogoItem("colorful-logo.png", "Vibrant Brand",
    EnableGrayscale: false)
```

## Animation System

The component uses pure CSS animations with hardware acceleration:

```css
@keyframes osirion-slide-right {
    0% { transform: translateX(0); }
    100% { transform: translateX(-50%); }
}

@keyframes osirion-slide-left {
    0% { transform: translateX(-50%); }
    100% { transform: translateX(0); }
}
```

### Animation Features

- **Hardware Accelerated**: Uses CSS transforms for smooth 60fps animation
- **Seamless Looping**: Duplicated logo sets ensure continuous flow
- **Direction Control**: Left or right scrolling directions
- **Hover Pause**: Optional pause on mouse hover
- **Reduced Motion**: Respects `prefers-reduced-motion` accessibility setting

## Dual Logo Support

Automatic theme-aware logo switching without JavaScript:

```csharp
// Light logo for light themes, dark logo for dark themes
new LogoItem("default-logo.png", "Company Name",
    LightImageUrl: "company-light.png",
    DarkImageUrl: "company-dark.png",
    Url: "https://company.com")
```

The component detects themes via multiple methods:
- `data-bs-theme` (Bootstrap)
- `data-theme` attributes
- CSS framework classes
- System preference (`prefers-color-scheme`)

## CSS Customization

Override default styling with CSS variables:

```css
:root {
    /* Carousel container */
    --osirion-carousel-background: transparent;
    --osirion-carousel-padding: 2rem 0;
    --osirion-carousel-border-radius: 0;
    
    /* Title styling */
    --osirion-carousel-title-color: #333;
    --osirion-carousel-title-size: 1.75rem;
    --osirion-carousel-title-weight: 600;
    --osirion-carousel-title-margin: 0 0 2rem 0;
    
    /* Logo styling */
    --osirion-logo-border-radius: 4px;
    --osirion-logo-shadow: 0 2px 8px rgba(0,0,0,0.1);
    --osirion-logo-grayscale: 1;
    --osirion-logo-grayscale-hover: 0;
    --osirion-logo-opacity: 0.8;
    --osirion-logo-opacity-hover: 1;
    --osirion-logo-brightness-hover: 1.1;
    
    /* Animation */
    --osirion-carousel-animation-play-state: running;
    --osirion-carousel-animation-timing: linear;
}

/* Dark theme overrides */
@media (prefers-color-scheme: dark) {
    :root {
        --osirion-carousel-title-color: #fff;
        --osirion-logo-shadow: 0 2px 8px rgba(255,255,255,0.1);
    }
}

/* Custom styling for specific implementations */
.brands-carousel {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    border-radius: 12px;
    padding: 3rem 2rem;
}

.tech-stack-carousel {
    border: 2px solid #e2e8f0;
    background: #f8fafc;
    border-radius: 8px;
}
```

## Accessibility Features

- **Semantic HTML**: Proper `<section>`, `<ul>`, and `<li>` structure
- **ARIA Labels**: Comprehensive ARIA labeling for screen readers
- **Keyboard Navigation**: Full keyboard support for logo links
- **Screen Reader**: Meaningful alt text and proper announcements
- **Focus Management**: Clear focus indicators and logical tab order
- **Reduced Motion**: Respects user motion preferences
- **High Contrast**: Enhanced visibility in high contrast mode

### Accessibility Implementation

```html
<section class="osirion-infinite-carousel" aria-label="Our Clients">
    <div class="osirion-carousel-header">
        <h2 class="osirion-carousel-title">Our Partners</h2>
    </div>
    <div class="osirion-carousel-container">
        <ul class="osirion-carousel-list" role="list">
            <li class="osirion-logo-item" role="listitem">
                <a href="https://company.com" 
                   title="Company Name"
                   rel="nofollow noopener"
                   target="_blank">
                    <img src="logo.png" 
                         alt="Company Name" 
                         width="200" 
                         height="80" 
                         loading="lazy">
                </a>
            </li>
            <!-- Duplicate items marked aria-hidden="true" -->
        </ul>
    </div>
</section>
```

## Framework Integration

### Bootstrap Integration

```razor
<InfiniteLogoCarousel 
    Title="Our Partners"
    Class="bg-light py-5"
    CustomLogos="@logos" />
```

### Fluent UI Integration

```razor
<InfiniteLogoCarousel 
    Title="Technology Stack"
    Class="ms-bgColor-neutralLighter ms-p-3"
    CustomLogos="@techLogos" />
```

### MudBlazor Integration

```razor
<InfiniteLogoCarousel 
    Title="Trusted Brands"
    Class="mud-paper mud-elevation-2 pa-6"
    CustomLogos="@brandLogos" />
```

## Performance Optimization

- **CSS-Only Animation**: No JavaScript runtime overhead
- **Hardware Acceleration**: GPU-accelerated transforms
- **Efficient Rendering**: Minimal DOM footprint
- **Lazy Loading**: Images load with `loading="lazy"`
- **Logo Limits**: `MaxVisibleLogos` parameter for performance control
- **Memory Efficient**: Reuses DOM elements for seamless looping

### Performance Tips

```csharp
// Limit logos for better performance
<InfiniteLogoCarousel 
    MaxVisibleLogos="10"
    CustomLogos="@allLogos" />

// Use optimized images
private List<LogoItem> GetOptimizedLogos()
{
    return new List<LogoItem>
    {
        // Use WebP format when possible
        new("logo.webp", "Company", Url: "https://company.com"),
        
        // Specify exact dimensions
        new("logo-200x80.png", "Brand", Url: "https://brand.com")
    };
}
```

## Browser Support

- **Modern Browsers**: Full support for Chrome, Firefox, Safari, Edge
- **CSS Custom Properties**: Uses CSS variables for theming
- **Graceful Degradation**: Fallbacks for older browsers
- **Progressive Enhancement**: Works without JavaScript

## Best Practices

1. **Image Optimization**
   - Use WebP format when possible
   - Optimize image sizes (typically 200x80px)
   - Provide 2x images for high-DPI displays

2. **Accessibility**
   - Provide meaningful alt text for all logos
   - Use descriptive titles and ARIA labels
   - Ensure sufficient color contrast

3. **Performance**
   - Limit the number of visible logos
   - Use lazy loading for images
   - Optimize animation duration for smooth performance

4. **SEO Considerations**
   - Use `NoFollow: false` for trusted partners
   - Provide descriptive alt text
   - Consider canonical URLs for partner links

5. **Design Guidelines**
   - Maintain consistent logo sizing
   - Use appropriate animation speed
   - Consider brand guidelines for logo treatment

## Real-World Examples

### SaaS Platform Partners

```razor
<InfiniteLogoCarousel 
    Title="Trusted by Leading Companies"
    AnimationDuration="50"
    PauseOnHover="true"
    LogoWidth="180"
    LogoHeight="70"
    EnableGrayscale="true"
    CustomLogos="@GetSaaSPartners()" />

@code {
    private List<LogoItem> GetSaaSPartners()
    {
        return new List<LogoItem>
        {
            new("https://cdn.company1.com/logo.svg", "Enterprise Corp",
                Url: "https://enterprise-corp.com",
                NoFollow: false),
            new("https://cdn.company2.com/logo.svg", "Tech Solutions Ltd",
                Url: "https://tech-solutions.com",
                NoFollow: false),
            // More partners...
        };
    }
}
```

### Open Source Project Sponsors

```razor
<InfiniteLogoCarousel 
    Title="Sponsored By"
    Direction="AnimationDirection.Left"
    AnimationDuration="35"
    EnableGrayscale="false"
    LogoWidth="160"
    LogoHeight="60"
    CustomLogos="@GetSponsors()" />
```

### Conference Partners

```razor
<InfiniteLogoCarousel 
    Title="Event Sponsors"
    AnimationDuration="45"
    LogoWidth="200"
    LogoHeight="100"
    EnableGrayscale="true"
    CustomLogos="@GetEventSponsors()" />
```

This component provides enterprise-grade logo carousel functionality with zero external dependencies and maximum compatibility across all modern browsers and frameworks.