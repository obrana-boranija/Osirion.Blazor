# InfiniteLogoCarousel - CSS-Only Self-Contained Component

The `InfiniteLogoCarousel` component is fully SSR-compatible and works without JavaScript. It's completely self-contained with all CSS included in the component file - no external dependencies required.

## Key Features

- **Self-Contained**: All CSS animations and styles included in component file
- **Zero Dependencies**: No global CSS files or external resources needed
- **Dual Logo Support**: Automatic theme-aware logo switching
- **Perfect Containment**: Hover effects never escape container bounds
- **Infinite Animation**: Seamless looping with duplicated logo sets
- **Accessibility**: Full keyboard navigation and screen reader support

## Animation System

The component uses a simple, reliable animation system with keyframes defined directly in the component's CSS file:

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

Animation is applied via inline styles for maximum compatibility:
```csharp
$"animation: {animationName} {GetAnimationDuration()} linear infinite"
```

## Dual Logo Support

Dual logos automatically switch between light and dark themes without JavaScript:

```csharp
// Light logo for light themes, dark logo for dark themes
new LogoItem("primary-logo.png", "Company Name",
    LightImageUrl: "light-theme-logo.png",
    DarkImageUrl: "dark-theme-logo.png",
    Url: "https://company.com")
```

The component detects themes via multiple methods:
- `data-bs-theme` (Bootstrap)
- `data-theme` attributes
- CSS framework classes
- System preference (`prefers-color-scheme`)

## Usage Examples

### Basic Usage

```razor
<InfiniteLogoCarousel Title="Our Partners"
                      CustomLogos="GetLogos()" />
```

### Advanced Configuration

```razor
<InfiniteLogoCarousel Title="Technology Stack"
                      AnimationDuration="45"
                      Direction="AnimationDirection.Left"
                      PauseOnHover="true"
                      LogoWidth="180"
                      LogoHeight="70"
                      LogoGap="20"
                      MaxVisibleLogos="8"
                      CustomLogos="GetAdvancedLogos()" />
```

### Logo Data Examples

```csharp
private List<LogoItem> GetAdvancedLogos()
{
    return new List<LogoItem>
    {
        // Dual logo with theme switching
        new("default-logo.png", "Company A",
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
```

## Hover Effects

The component uses filter-based hover effects that never escape container bounds:

- **Dual Logos**: `filter: brightness(1.2) contrast(1.1)`
- **Grayscale Logos**: Removes grayscale + adds brightness
- **Regular Logos**: `filter: opacity(1) brightness(1.1)`
- **No Scaling**: Prevents logos from escaping their containers

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | Optional title above logos |
| `SectionTitle` | `string` | `"Our Clients"` | ARIA label for accessibility |
| `AnimationDuration` | `int` | `60` | Animation duration in seconds |
| `Direction` | `AnimationDirection` | `Right` | Animation direction |
| `PauseOnHover` | `bool` | `true` | Pause animation on hover |
| `EnableGrayscale` | `bool` | `true` | Global grayscale setting |
| `LogoWidth` | `int` | `200` | Logo width in pixels |
| `LogoHeight` | `int` | `80` | Logo height in pixels |
| `LogoGap` | `int` | `24` | Gap between logos in pixels |
| `MaxVisibleLogos` | `int?` | `null` | Limit visible logos for performance |
| `CustomLogos` | `List<LogoItem>?` | `null` | Custom logo list |

## Accessibility Features

- **Reduced Motion**: Respects `prefers-reduced-motion` preference
- **High Contrast**: Enhanced contrast in high contrast mode
- **Keyboard Navigation**: Full keyboard support for logo links
- **Screen Readers**: Proper ARIA labels and semantic markup
- **Focus Management**: Clear focus indicators

## Browser Support

- Modern browsers with CSS custom properties support
- Progressive enhancement for older browsers
- Graceful degradation without JavaScript
- Hardware-accelerated animations where supported

## Performance

- **CSS-Only**: No JavaScript runtime overhead
- **Hardware Acceleration**: Uses CSS transforms
- **Efficient Rendering**: Optimized for smooth 60fps animation
- **Memory Efficient**: Minimal DOM footprint with logo limit options

## Real-World Example

```razor
@* Complete example with dual logos and custom settings *@
<InfiniteLogoCarousel Title="Our Technology Partners"
                      AnimationDuration="50"
                      PauseOnHover="true"
                      LogoWidth="180"
                      LogoHeight="70"
                      CustomLogos="GetPartnerLogos()" />

@code {
    private List<LogoItem> GetPartnerLogos()
    {
        return new List<LogoItem>
        {
            // Perfect for logos with transparent backgrounds
            new("https://example.com/logo-transparent.png",
                "Technology Partner",
                LightImageUrl: "https://example.com/logo-light.png",
                DarkImageUrl: "https://example.com/logo-dark.png",
                Url: "https://partner.com",
                NoFollow: false),
                
            // Regular logo with grayscale
            new("https://example.com/partner2.png",
                "Strategic Partner",
                Url: "https://partner2.com"),
                
            // Always colorful logo
            new("https://example.com/sponsor.png",
                "Event Sponsor",
                Url: "https://sponsor.com",
                EnableGrayscale: false)
        };
    }
}
```

This component provides enterprise-grade logo carousel functionality with zero external dependencies and maximum compatibility across all modern browsers and frameworks.