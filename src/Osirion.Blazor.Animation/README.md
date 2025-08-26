# Osirion.Blazor - Animation Module

[![NuGet Version](https://img.shields.io/nuget/v/Osirion.Blazor.Animation?style=flat-square)](https://www.nuget.org/packages/Osirion.Blazor.Animation)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Osirion.Blazor.Animation?style=flat-square)](https://www.nuget.org/packages/Osirion.Blazor.Animation)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/github/actions/workflow/status/obrana-boranija/Osirion.Blazor/ci.yml?branch=master&style=flat-square)](https://github.com/obrana-boranija/Osirion.Blazor/actions)

A powerful, **fully self-contained** animation module for Blazor applications powered by [AOS.js (Animate On Scroll)](https://michalsnik.github.io/aos/). Built with performance, accessibility, and developer experience in mind.

## Key Features

- **🚀 Zero Configuration** - Completely self-contained with automatic AOS.js loading
- **📦 No Manual Setup** - CSS and JavaScript automatically loaded when needed
- **🔄 Static SSR Compatible** - Works perfectly with Static SSG, Server Interactive, and WebAssembly
- **📚 AOS.js Powered** - Leverages the battle-tested AOS library for smooth animations
- **♿ Accessibility First** - Respects `prefers-reduced-motion` and follows WCAG guidelines
- **📱 Mobile Optimized** - Responsive animations with mobile-first design
- **🎨 11 Animation Effects** - From subtle fades to attention-grabbing zooms
- **⚡ High Performance** - Hardware-accelerated animations for 60fps performance
- **🔧 Developer Friendly** - Type-safe API with comprehensive IntelliSense
- **📖 Well Documented** - Extensive documentation with real-world examples

## Quick Start

### Installation

```bash
dotnet add package Osirion.Blazor.Animation
```

### Basic Setup

Add the namespace to your `_Imports.razor`:

```razor
@using Osirion.Blazor.Components
```

### Your First Animation - That's It!

```razor
<OsirionAnimation Effect="AnimationEffect.FadeUp">
    <h1>Hello, Animated World! 👋</h1>
</OsirionAnimation>
```

**No additional setup required!** The component automatically:
- ✅ Loads AOS.js CSS and JavaScript from CDN
- ✅ Initializes AOS with proper settings
- ✅ Handles reduced motion preferences
- ✅ Manages Blazor navigation updates

### Hero Section Example

```razor
<section class="hero">
    <OsirionAnimation Effect="AnimationEffect.Fade" Speed="AnimationSpeed.Slow">
        <h1>Build Amazing Apps</h1>
    </OsirionAnimation>
    
    <OsirionAnimation Effect="AnimationEffect.FadeUp" Delay="200">
        <p>With beautiful, performant animations</p>
    </OsirionAnimation>
    
    <OsirionAnimation Effect="AnimationEffect.FadeLeft" Delay="400">
        <button class="btn-primary">Get Started</button>
    </OsirionAnimation>
</section>
```

## Animation Effects

| Effect | AOS Animation | Description | Best Use Case |
|--------|---------------|-------------|---------------|
| `Fade` | `fade` | Simple opacity fade | Subtle content reveals |
| `FadeUp` | `fade-up` | Fade with upward movement | Default choice for most content |
| `FadeDown` | `fade-down` | Fade with downward movement | Dropdown content, headers |
| `FadeLeft` | `fade-left` | Fade from right to left | Left-to-right reading flow |
| `FadeRight` | `fade-right` | Fade from left to right | Call-to-action buttons |
| `SlideUp` | `slide-up` | Slide without opacity change | Pure movement effects |
| `SlideDown` | `slide-down` | Slide downward | Modal content |
| `SlideLeft` | `slide-left` | Slide from right (no fade) | Navigation elements |
| `SlideRight` | `slide-right` | Slide from left (no fade) | Sidebar content |
| `ZoomIn` | `zoom-in` | Scale up with fade | Attention-grabbing elements |
| `ZoomOut` | `zoom-out` | Scale down with fade | Modal dialogs |

## Configuration Options

### Core Parameters

```razor
<OsirionAnimation 
    Effect="AnimationEffect.FadeUp"          // Animation type
    Speed="AnimationSpeed.Fast"              // Duration preset
    Easing="AnimationEasing.EaseOut"         // Timing function
    Delay="200"                              // Delay in milliseconds (0-3000)
    Class="custom-class"                     // Additional CSS
    RespectReducedMotion="true">             // Honor accessibility preferences
    <div>Your content here</div>
</OsirionAnimation>
```

### Advanced Parameters

```razor
<OsirionAnimation 
    Effect="AnimationEffect.ZoomIn"
    Duration="800"                           // Custom duration (50-3000ms)
    Distance="50"                            // Custom distance in pixels
    Once="true"                              // Animate only once
    Mirror="false"                           // Don't animate on scroll out
    Anchor="#custom-anchor"                  // Custom anchor selector
    Offset="100">                           // Offset from trigger point
    <div>Advanced animated content</div>
</OsirionAnimation>
```

## Real-World Examples

### Staggered Card Grid

```razor
<div class="card-grid">
    @foreach (var (product, index) in products.Select((p, i) => (p, i)))
    {
        <OsirionAnimation 
            Effect="@(index % 2 == 0 ? AnimationEffect.FadeLeft : AnimationEffect.FadeRight)"
            Delay="@(index * 100)"
            Speed="AnimationSpeed.Fast">
            <div class="product-card">
                <h3>@product.Name</h3>
                <p>@product.Description</p>
                <span class="price">@product.Price.ToString("C")</span>
            </div>
        </OsirionAnimation>
    }
</div>
```

### Dashboard Statistics

```razor
<div class="stats-grid">
    @foreach (var (stat, index) in statistics.Select((s, i) => (s, i)))
    {
        <OsirionAnimation 
            Effect="AnimationEffect.ZoomIn" 
            Delay="@(index * 150)"
            Easing="AnimationEasing.Bounce"
            Speed="AnimationSpeed.Fast"
            Once="true">
            <div class="stat-card">
                <div class="stat-value">@stat.Value</div>
                <div class="stat-label">@stat.Label</div>
            </div>
        </OsirionAnimation>
    }
</div>
```

### Loading States

```razor
@if (isLoading)
{
    <OsirionAnimation 
        Effect="AnimationEffect.Fade" 
        Speed="AnimationSpeed.Fast"
        Once="true">
        <div class="loading-spinner">
            <div class="spinner"></div>
            <p>Loading awesome content...</p>
        </div>
    </OsirionAnimation>
}
else
{
    <OsirionAnimation Effect="AnimationEffect.FadeUp">
        <div class="content">@LoadedContent</div>
    </OsirionAnimation>
}
```

## Static SSR Compatibility

This component is designed to work seamlessly with Blazor's Static SSR mode with **zero configuration**. When `IsInteractive` is `false`, the component automatically:

1. **Loads AOS.js CSS** from CDN for styling
2. **Loads AOS.js JavaScript** from CDN for animations
3. **Initializes AOS** with proper configuration
4. **Respects accessibility preferences** by checking for `prefers-reduced-motion`
5. **Handles Blazor enhanced navigation** by refreshing AOS on page changes
6. **Prevents duplicate loading** with intelligent initialization guards

### How It Works

```razor
@* Component automatically generates this in Static SSR mode: *@
<script>
  // Intelligent AOS loading and initialization
  // - Prevents duplicate loads
  // - Handles reduced motion
  // - Manages Blazor navigation
  // - Provides error handling
</script>

@* Your animated content *@
<div data-aos="fade-up" data-aos-duration="600">
    @ChildContent
</div>
```

## Framework Integration

### Bootstrap 5

```razor
<div class="container">
    <div class="row">
        <div class="col-md-6">
            <OsirionAnimation Effect="AnimationEffect.FadeLeft">
                <div class="card h-100">
                    <div class="card-body">
                        <h5 class="card-title">Animated Bootstrap Card</h5>
                        <p class="card-text">Seamlessly integrated with Bootstrap components.</p>
                    </div>
                </div>
            </OsirionAnimation>
        </div>
    </div>
</div>
```

### MudBlazor

```razor
<MudContainer MaxWidth="MaxWidth.Large">
    <MudGrid>
        @for (int i = 0; i < 6; i++)
        {
            <MudItem xs="12" sm="6" md="4">
                <OsirionAnimation 
                    Effect="AnimationEffect.FadeUp" 
                    Delay="@(i * 100)">
                    <MudPaper Class="pa-4" Elevation="2">
                        <MudStack>
                            <MudIcon Icon="@Icons.Material.Filled.Star" Color="Color.Primary" />
                            <MudText Typo="Typo.h6">Animated MudBlazor</MudText>
                            <MudText Typo="Typo.body2">
                                Perfect integration with Material Design components.
                            </MudText>
                        </MudStack>
                    </MudPaper>
                </OsirionAnimation>
            </MudItem>
        }
    </MudGrid>
</MudContainer>
```

## Browser Support

| Browser | Version | AOS Support | Fallback |
|---------|---------|-------------|----------|
| Chrome | 60+ | ✅ Full Support | N/A |
| Firefox | 55+ | ✅ Full Support | N/A |
| Safari | 12+ | ✅ Full Support | N/A |
| Edge | 79+ | ✅ Full Support | N/A |
| IE11 | All | ❌ | ✅ No animation (graceful degradation) |

## Performance Benefits

- **🚀 Automatic Loading** - AOS.js only loads when component is actually used
- **📦 Smart Caching** - Prevents duplicate library loads across components
- **⚡ CDN Delivery** - AOS.js served from fast, global CDN
- **🎯 Minimal Bundle** - Only ~12KB gzipped for AOS.js + CSS
- **🔄 Efficient Updates** - AOS efficiently manages DOM observations
- **📱 Mobile Optimized** - Reduced animations on smaller screens

## Accessibility Features

- ✅ **Respects `prefers-reduced-motion`** automatically via AOS configuration
- ✅ **Screen reader friendly** - doesn't interfere with content reading
- ✅ **Keyboard navigation** compatible
- ✅ **High contrast mode** support with custom styling
- ✅ **Focus management** preserved during animations
- ✅ **Semantic HTML** structure maintained

## API Reference

### OsirionAnimation Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` | `null` | The content to be animated |
| `Effect` | `AnimationEffect` | `FadeUp` | The animation effect to apply |
| `Speed` | `AnimationSpeed` | `Normal` | Animation duration preset |
| `Easing` | `AnimationEasing` | `EaseOut` | Animation easing function |
| `Delay` | `int` | `0` | Delay before animation starts (0-3000ms) |
| `Once` | `bool` | `false` | Whether animation should happen only once |
| `Mirror` | `bool` | `false` | Whether to mirror animation on scroll out |
| `Anchor` | `string?` | `null` | Anchor element ID or selector |
| `Distance` | `int?` | `null` | Custom animation distance in pixels |
| `Duration` | `int?` | `null` | Custom animation duration (50-3000ms) |
| `RespectReducedMotion` | `bool` | `true` | Whether to respect accessibility preferences |
| `Offset` | `string?` | `null` | Offset from trigger point (px or %) |

### AnimationSpeed Values

| Speed | Duration |
|-------|----------|
| `Slow` | 1000ms |
| `Normal` | 600ms |
| `Fast` | 400ms |
| `ExtraFast` | 250ms |

### AnimationEasing Values

| Easing | CSS Function |
|--------|-------------|
| `Linear` | `linear` |
| `EaseIn` | `ease-in` |
| `EaseOut` | `ease-out` |
| `EaseInOut` | `ease-in-out` |
| `Bounce` | `ease-in-back` |
| `Elastic` | `ease-out-back` |

## Advanced Usage

### Global AOS Refresh

When dynamically adding content, you can refresh AOS:

```javascript
// Call this after adding dynamic content
window.osirionAosRefresh();
```

### Disable/Enable Animations

```javascript
// Disable all animations
window.osirionAosToggle(true);

// Re-enable animations
window.osirionAosToggle(false);
```

## Installation & Setup

### Package Manager

```bash
dotnet add package Osirion.Blazor.Animation
```

### PackageReference

```xml
<PackageReference Include="Osirion.Blazor.Animation" Version="2.1.7" />
```

**That's it!** The component is completely self-contained and requires no additional setup. AOS.js is automatically loaded when needed.

## Migration from Custom CSS

If you're migrating from a custom CSS animation solution:

1. **Replace your animation component** with `OsirionAnimation`
2. **Map your effects** to the corresponding `AnimationEffect` enum values
3. **Update delays** from multipliers to milliseconds (old: `Delay="3"` → new: `Delay="300"`)
4. **Remove custom CSS** - AOS handles all animation styling
5. **Remove manual script/CSS references** - Component loads everything automatically
6. **Test accessibility** - AOS provides better reduced motion support

## Why This Approach?

### Before (Manual Setup Required):
- ❌ Manual CSS reference in layout
- ❌ Manual script loading
- ❌ Complex initialization code
- ❌ Risk of duplicate loads
- ❌ Manual reduced motion handling

### After (Fully Automatic):
- ✅ Zero configuration required
- ✅ Automatic resource loading
- ✅ Intelligent initialization
- ✅ Duplicate prevention
- ✅ Built-in accessibility

## Contributing

We welcome contributions! Areas where help is appreciated:

- 🐛 **Bug Reports** - Help us identify and fix issues
- 💡 **Feature Requests** - Suggest new animation effects or parameters
- 📖 **Documentation** - Improve examples and guides
- 🧪 **Testing** - Help test across different browsers and scenarios

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## Credits & Acknowledgments

- **[AOS.js](https://michalsnik.github.io/aos/)** by Michał Sajnóg - The amazing animation library that powers this component
- **Blazor Team** at Microsoft for the excellent web framework
- **Community Contributors** who provided feedback and testing

---

<div align="center">

**Made with ❤️ by the Osirion.Blazor Team**

[Star on GitHub](https://github.com/obrana-boranija/Osirion.Blazor) • 
[View on NuGet](https://www.nuget.org/packages/Osirion.Blazor.Animation) • 
[Report Issues](https://github.com/obrana-boranija/Osirion.Blazor/issues)

</div>