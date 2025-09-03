---
id: 'osirion-styles-component'
order: 3
layout: docs
title: OsirionStyles Component
permalink: /docs/components/theming/osirion-styles
description: Learn how to use the OsirionStyles component for CSS injection, theme variable management, and framework-specific styling in Blazor applications.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Theming Components
- Styling
tags:
- blazor
- css-injection
- theme-variables
- css-custom-properties
- styling
- design-tokens
is_featured: true
published: true
slug: components/theming/osirion-styles
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionStyles Component - CSS Injection & Theme Variables | Osirion.Blazor'
  description: 'Master the OsirionStyles component for CSS injection, theme variable management, and framework-specific styling in Blazor applications.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/theming/osirion-styles'
  lang: en
  robots: index, follow
  og_title: 'OsirionStyles Component - Osirion.Blazor'
  og_description: 'CSS injection, theme variable management, and framework-specific styling for Blazor applications.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionStyles Component - Osirion.Blazor'
  twitter_description: 'CSS injection, theme variable management, and framework-specific styling for Blazor applications.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionStyles Component

The OsirionStyles component is the core styling engine for Osirion.Blazor applications. It manages CSS injection, theme variables, framework-specific styling, and design token systems. This component ensures consistent styling across different CSS frameworks while providing flexibility for custom implementations.

## Component Overview

OsirionStyles serves as the central hub for style management in Blazor applications. It automatically detects CSS frameworks, injects appropriate styles, manages theme variables, and provides a consistent styling API regardless of the underlying framework.

### Key Features

**Framework Detection**: Automatically detects and adapts to Bootstrap, Tailwind CSS, Fluent UI, and custom frameworks
**CSS Injection**: Efficient CSS injection with proper ordering and dependency management
**Theme Variables**: Comprehensive CSS custom property management for design tokens
**Performance Optimized**: Intelligent style loading with critical CSS prioritization
**SSR Compatible**: Full Server-Side Rendering support with proper hydration
**Hot Reload Support**: Development-time style updates without page refresh

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Framework` | `CssFramework` | `Auto` | CSS framework to use (Auto, Bootstrap, TailwindCSS, FluentUI, Custom). |
| `UseDefaultStyles` | `bool` | `true` | Whether to include default Osirion component styles. |
| `CustomVariables` | `string?` | `null` | Custom CSS variables to inject. Can be CSS custom properties or SCSS/LESS variables. |
| `GeneratedVariables` | `string?` | `null` | Dynamically generated CSS variables (typically from theme configuration). |
| `StylesheetUrls` | `string[]?` | `null` | Additional external stylesheet URLs to include. |
| `InlineStyles` | `string?` | `null` | Inline CSS styles to inject directly into the document head. |
| `LoadPriority` | `StyleLoadPriority` | `Normal` | Loading priority for styles (Critical, High, Normal, Low). |
| `MinifyInProduction` | `bool` | `true` | Whether to minify CSS in production builds. |
| `EnableSourceMaps` | `bool` | `false` | Whether to generate CSS source maps for debugging. |
| `CacheStrategy` | `StyleCacheStrategy` | `Aggressive` | Caching strategy for styles (None, Conservative, Aggressive). |
| `FrameworkVersion` | `string?` | `null` | Specific framework version to target (e.g., "5.3.0" for Bootstrap). |
| `CustomFrameworkName` | `string?` | `null` | Name for custom framework when Framework is set to Custom. |
| `ThemeMode` | `ThemeMode?` | `null` | Theme mode to apply (Light, Dark, Auto). If null, uses system preference. |
| `Class` | `string?` | `null` | Additional CSS classes to apply to the style container. |
| `Id` | `string?` | `null` | Unique identifier for the style container. |

## Basic Usage

### Simple Framework Integration

```razor
@using Osirion.Blazor.Theming.Components

<!-- Bootstrap Integration -->
<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    UseDefaultStyles="true" />

<!-- Tailwind CSS Integration -->
<OsirionStyles 
    Framework="CssFramework.TailwindCSS"
    UseDefaultStyles="true" />

<!-- Fluent UI Integration -->
<OsirionStyles 
    Framework="CssFramework.FluentUI"
    UseDefaultStyles="true" />
```

### Custom Variables Injection

```razor
<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    CustomVariables="@customThemeVariables"
    UseDefaultStyles="true" />

@code {
    private string customThemeVariables = @"
        /* Brand Colors */
        --brand-primary: #3b82f6;
        --brand-secondary: #64748b;
        --brand-success: #10b981;
        --brand-warning: #f59e0b;
        --brand-error: #ef4444;
        
        /* Typography */
        --font-family-base: 'Inter', sans-serif;
        --font-size-base: 1rem;
        --line-height-base: 1.5;
        
        /* Spacing */
        --spacing-unit: 0.25rem;
        --border-radius-base: 0.375rem;
        
        /* Shadows */
        --shadow-sm: 0 1px 2px rgba(0, 0, 0, 0.05);
        --shadow-md: 0 4px 6px rgba(0, 0, 0, 0.1);
        --shadow-lg: 0 10px 15px rgba(0, 0, 0, 0.1);
    ";
}
```

### External Stylesheet Integration

```razor
<OsirionStyles 
    Framework="CssFramework.Custom"
    StylesheetUrls="@externalStylesheets"
    UseDefaultStyles="true" />

@code {
    private string[] externalStylesheets = new[]
    {
        "https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap",
        "https://cdn.jsdelivr.net/npm/@tabler/icons@latest/icons-sprite.svg",
        "/css/custom-components.css",
        "/css/brand-overrides.css"
    };
}
```

## Advanced Usage

### Dynamic Theme Generation

```razor
@inject IThemeService ThemeService

<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    CustomVariables="@dynamicVariables"
    GeneratedVariables="@generatedVariables"
    UseDefaultStyles="true" />

@code {
    private string dynamicVariables = "";
    private string generatedVariables = "";
    
    [Parameter] public string? BrandColor { get; set; } = "#3b82f6";
    [Parameter] public string? AccentColor { get; set; } = "#10b981";
    
    protected override async Task OnParametersSetAsync()
    {
        await UpdateThemeVariables();
    }
    
    private async Task UpdateThemeVariables()
    {
        // Generate color palette from brand colors
        var colorPalette = await ThemeService.GenerateColorPaletteAsync(BrandColor, AccentColor);
        
        // Create custom variables
        dynamicVariables = $@"
            /* Dynamic Brand Colors */
            --brand-primary: {BrandColor};
            --brand-accent: {AccentColor};
            
            /* Computed Colors */
            --brand-primary-hover: {colorPalette.PrimaryHover};
            --brand-primary-active: {colorPalette.PrimaryActive};
            --brand-primary-focus: {colorPalette.PrimaryFocus};
            
            /* Surface Colors */
            --surface-primary: {colorPalette.SurfacePrimary};
            --surface-secondary: {colorPalette.SurfaceSecondary};
            --surface-accent: {colorPalette.SurfaceAccent};
        ";
        
        // Generate semantic color variables
        generatedVariables = await ThemeService.GenerateSemanticVariablesAsync(colorPalette);
        
        StateHasChanged();
    }
}
```

### Performance-Optimized Loading

```razor
<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    LoadPriority="StyleLoadPriority.Critical"
    CacheStrategy="StyleCacheStrategy.Aggressive"
    MinifyInProduction="true"
    UseDefaultStyles="true" />

<!-- Critical styles loaded immediately -->
<OsirionStyles 
    Framework="CssFramework.Custom"
    LoadPriority="StyleLoadPriority.Critical"
    InlineStyles="@criticalCSS" />

<!-- Non-critical styles loaded asynchronously -->
<OsirionStyles 
    Framework="CssFramework.Custom"
    LoadPriority="StyleLoadPriority.Low"
    StylesheetUrls="@nonCriticalStylesheets" />

@code {
    private string criticalCSS = @"
        /* Critical above-the-fold styles */
        .header { background: var(--brand-primary); }
        .hero { min-height: 100vh; }
        .loading-spinner { /* ... */ }
    ";
    
    private string[] nonCriticalStylesheets = new[]
    {
        "/css/components.css",
        "/css/utilities.css",
        "/css/animations.css"
    };
}
```

### Multi-Framework Support

```razor
@inject IJSRuntime JS

<OsirionStyles 
    Framework="@detectedFramework"
    FrameworkVersion="@frameworkVersion"
    UseDefaultStyles="true"
    CustomVariables="@GetFrameworkSpecificVariables()" />

@code {
    private CssFramework detectedFramework = CssFramework.Auto;
    private string? frameworkVersion;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Detect available CSS framework
            detectedFramework = await DetectCssFrameworkAsync();
            frameworkVersion = await GetFrameworkVersionAsync(detectedFramework);
            
            StateHasChanged();
        }
    }
    
    private async Task<CssFramework> DetectCssFrameworkAsync()
    {
        try
        {
            // Check for Bootstrap
            var hasBootstrap = await JS.InvokeAsync<bool>("checkFramework", "bootstrap");
            if (hasBootstrap) return CssFramework.Bootstrap;
            
            // Check for Tailwind CSS
            var hasTailwind = await JS.InvokeAsync<bool>("checkFramework", "tailwind");
            if (hasTailwind) return CssFramework.TailwindCSS;
            
            // Check for Fluent UI
            var hasFluentUI = await JS.InvokeAsync<bool>("checkFramework", "fluentui");
            if (hasFluentUI) return CssFramework.FluentUI;
            
            return CssFramework.Custom;
        }
        catch
        {
            return CssFramework.Custom;
        }
    }
    
    private async Task<string?> GetFrameworkVersionAsync(CssFramework framework)
    {
        return framework switch
        {
            CssFramework.Bootstrap => await JS.InvokeAsync<string?>("getBootstrapVersion"),
            CssFramework.TailwindCSS => await JS.InvokeAsync<string?>("getTailwindVersion"),
            CssFramework.FluentUI => await JS.InvokeAsync<string?>("getFluentUIVersion"),
            _ => null
        };
    }
    
    private string GetFrameworkSpecificVariables()
    {
        return detectedFramework switch
        {
            CssFramework.Bootstrap => GetBootstrapVariables(),
            CssFramework.TailwindCSS => GetTailwindVariables(),
            CssFramework.FluentUI => GetFluentUIVariables(),
            _ => GetCustomVariables()
        };
    }
    
    private string GetBootstrapVariables() => @"
        /* Bootstrap-specific overrides */
        --bs-primary: var(--brand-primary);
        --bs-secondary: var(--brand-secondary);
        --bs-success: var(--brand-success);
        --bs-border-radius: var(--border-radius-base);
    ";
    
    private string GetTailwindVariables() => @"
        /* Tailwind-specific configuration */
        --tw-color-primary-500: var(--brand-primary);
        --tw-color-secondary-500: var(--brand-secondary);
        --tw-border-radius-md: var(--border-radius-base);
    ";
    
    private string GetFluentUIVariables() => @"
        /* Fluent UI token overrides */
        --colorBrandBackground: var(--brand-primary);
        --colorNeutralBackground1: var(--surface-primary);
        --borderRadiusMedium: var(--border-radius-base);
    ";
    
    private string GetCustomVariables() => @"
        /* Custom framework variables */
        --primary: var(--brand-primary);
        --secondary: var(--brand-secondary);
        --surface: var(--surface-primary);
    ";
}

<script>
    window.checkFramework = (framework) => {
        switch (framework) {
            case 'bootstrap':
                return typeof window.bootstrap !== 'undefined' || 
                       document.querySelector('[href*="bootstrap"]') !== null;
            case 'tailwind':
                return document.querySelector('[href*="tailwind"]') !== null ||
                       document.documentElement.classList.contains('tailwind');
            case 'fluentui':
                return typeof window.FluentUI !== 'undefined' ||
                       document.querySelector('[href*="fluent"]') !== null;
            default:
                return false;
        }
    };
    
    window.getBootstrapVersion = () => {
        return window.bootstrap?.Tooltip?.VERSION || '5.3.0';
    };
    
    window.getTailwindVersion = () => {
        // Tailwind version detection is complex, return default
        return '3.3.0';
    };
    
    window.getFluentUIVersion = () => {
        return window.FluentUI?.version || '9.0.0';
    };
</script>
```

### Custom Design System Integration

```razor
<OsirionStyles 
    Framework="CssFramework.Custom"
    CustomFrameworkName="MyDesignSystem"
    CustomVariables="@designSystemTokens"
    UseDefaultStyles="false" />

@code {
    private string designSystemTokens = @"
        /* Design System Foundation */
        
        /* Color Tokens */
        --color-brand-primary: #2563eb;
        --color-brand-secondary: #7c3aed;
        --color-semantic-success: #059669;
        --color-semantic-warning: #d97706;
        --color-semantic-error: #dc2626;
        --color-semantic-info: #0284c7;
        
        /* Neutral Colors */
        --color-neutral-50: #f9fafb;
        --color-neutral-100: #f3f4f6;
        --color-neutral-200: #e5e7eb;
        --color-neutral-300: #d1d5db;
        --color-neutral-400: #9ca3af;
        --color-neutral-500: #6b7280;
        --color-neutral-600: #4b5563;
        --color-neutral-700: #374151;
        --color-neutral-800: #1f2937;
        --color-neutral-900: #111827;
        
        /* Typography Tokens */
        --typography-font-family-primary: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
        --typography-font-family-secondary: 'JetBrains Mono', Consolas, monospace;
        
        --typography-font-size-xs: 0.75rem;
        --typography-font-size-sm: 0.875rem;
        --typography-font-size-base: 1rem;
        --typography-font-size-lg: 1.125rem;
        --typography-font-size-xl: 1.25rem;
        --typography-font-size-2xl: 1.5rem;
        --typography-font-size-3xl: 1.875rem;
        --typography-font-size-4xl: 2.25rem;
        
        --typography-font-weight-light: 300;
        --typography-font-weight-normal: 400;
        --typography-font-weight-medium: 500;
        --typography-font-weight-semibold: 600;
        --typography-font-weight-bold: 700;
        
        --typography-line-height-tight: 1.25;
        --typography-line-height-snug: 1.375;
        --typography-line-height-normal: 1.5;
        --typography-line-height-relaxed: 1.625;
        --typography-line-height-loose: 2;
        
        /* Spacing Tokens */
        --spacing-0: 0;
        --spacing-1: 0.25rem;
        --spacing-2: 0.5rem;
        --spacing-3: 0.75rem;
        --spacing-4: 1rem;
        --spacing-5: 1.25rem;
        --spacing-6: 1.5rem;
        --spacing-8: 2rem;
        --spacing-10: 2.5rem;
        --spacing-12: 3rem;
        --spacing-16: 4rem;
        --spacing-20: 5rem;
        --spacing-24: 6rem;
        --spacing-32: 8rem;
        
        /* Border Radius Tokens */
        --border-radius-none: 0;
        --border-radius-sm: 0.125rem;
        --border-radius-base: 0.25rem;
        --border-radius-md: 0.375rem;
        --border-radius-lg: 0.5rem;
        --border-radius-xl: 0.75rem;
        --border-radius-2xl: 1rem;
        --border-radius-full: 9999px;
        
        /* Shadow Tokens */
        --shadow-xs: 0 1px 2px rgba(0, 0, 0, 0.05);
        --shadow-sm: 0 1px 3px rgba(0, 0, 0, 0.1), 0 1px 2px rgba(0, 0, 0, 0.06);
        --shadow-base: 0 4px 6px rgba(0, 0, 0, 0.1), 0 2px 4px rgba(0, 0, 0, 0.06);
        --shadow-md: 0 10px 15px rgba(0, 0, 0, 0.1), 0 4px 6px rgba(0, 0, 0, 0.05);
        --shadow-lg: 0 20px 25px rgba(0, 0, 0, 0.1), 0 10px 10px rgba(0, 0, 0, 0.04);
        --shadow-xl: 0 25px 50px rgba(0, 0, 0, 0.25);
        
        /* Z-Index Tokens */
        --z-index-dropdown: 1000;
        --z-index-sticky: 1010;
        --z-index-fixed: 1020;
        --z-index-modal-backdrop: 1030;
        --z-index-modal: 1040;
        --z-index-popover: 1050;
        --z-index-tooltip: 1060;
        
        /* Animation Tokens */
        --animation-duration-fast: 150ms;
        --animation-duration-normal: 250ms;
        --animation-duration-slow: 350ms;
        
        --animation-easing-ease-in: cubic-bezier(0.4, 0, 1, 1);
        --animation-easing-ease-out: cubic-bezier(0, 0, 0.2, 1);
        --animation-easing-ease-in-out: cubic-bezier(0.4, 0, 0.2, 1);
        --animation-easing-bounce: cubic-bezier(0.68, -0.55, 0.265, 1.55);
        
        /* Component-Specific Tokens */
        --component-button-height-sm: 2rem;
        --component-button-height-md: 2.5rem;
        --component-button-height-lg: 3rem;
        
        --component-input-height-sm: 2rem;
        --component-input-height-md: 2.5rem;
        --component-input-height-lg: 3rem;
        
        --component-card-padding: var(--spacing-6);
        --component-card-border-radius: var(--border-radius-lg);
        --component-card-shadow: var(--shadow-base);
    ";
}
```

## Framework-Specific Configurations

### Bootstrap Integration

```razor
<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    FrameworkVersion="5.3.0"
    CustomVariables="@bootstrapCustomization" />

@code {
    private string bootstrapCustomization = @"
        /* Bootstrap Variable Overrides */
        --bs-blue: #3b82f6;
        --bs-purple: #8b5cf6;
        --bs-pink: #ec4899;
        --bs-red: #ef4444;
        --bs-orange: #f97316;
        --bs-yellow: #eab308;
        --bs-green: #22c55e;
        --bs-teal: #14b8a6;
        --bs-cyan: #06b6d4;
        
        /* Bootstrap Component Customization */
        --bs-btn-border-radius: 0.5rem;
        --bs-btn-padding-y: 0.5rem;
        --bs-btn-padding-x: 1rem;
        
        --bs-card-border-radius: 0.75rem;
        --bs-card-spacer-y: 1.5rem;
        --bs-card-spacer-x: 1.5rem;
        
        --bs-modal-border-radius: 0.75rem;
        --bs-dropdown-border-radius: 0.5rem;
    ";
}
```

### Tailwind CSS Integration

```razor
<OsirionStyles 
    Framework="CssFramework.TailwindCSS"
    CustomVariables="@tailwindCustomization" />

@code {
    private string tailwindCustomization = @"
        /* Tailwind CSS Custom Properties */
        
        /* Custom Color Palette */
        --tw-color-brand-50: #eff6ff;
        --tw-color-brand-100: #dbeafe;
        --tw-color-brand-200: #bfdbfe;
        --tw-color-brand-300: #93c5fd;
        --tw-color-brand-400: #60a5fa;
        --tw-color-brand-500: #3b82f6;
        --tw-color-brand-600: #2563eb;
        --tw-color-brand-700: #1d4ed8;
        --tw-color-brand-800: #1e40af;
        --tw-color-brand-900: #1e3a8a;
        
        /* Custom Spacing */
        --tw-spacing-18: 4.5rem;
        --tw-spacing-22: 5.5rem;
        --tw-spacing-26: 6.5rem;
        
        /* Custom Font Sizes */
        --tw-text-2xs: 0.625rem;
        --tw-text-3xs: 0.5rem;
        
        /* Custom Shadows */
        --tw-shadow-outline: 0 0 0 3px rgba(59, 130, 246, 0.1);
        --tw-shadow-brand: 0 4px 14px rgba(59, 130, 246, 0.15);
    ";
}
```

### Fluent UI Integration

```razor
<OsirionStyles 
    Framework="CssFramework.FluentUI"
    CustomVariables="@fluentUICustomization" />

@code {
    private string fluentUICustomization = @"
        /* Fluent UI Design Token Overrides */
        
        /* Brand Colors */
        --colorBrandBackground: #3b82f6;
        --colorBrandBackgroundHover: #2563eb;
        --colorBrandBackgroundPressed: #1d4ed8;
        --colorBrandForeground1: #ffffff;
        --colorBrandForeground2: #f8fafc;
        
        /* Semantic Colors */
        --colorPaletteRedBackground1: #fef2f2;
        --colorPaletteRedBackground2: #fee2e2;
        --colorPaletteRedForeground1: #ef4444;
        --colorPaletteRedForeground2: #dc2626;
        
        --colorPaletteGreenBackground1: #f0fdf4;
        --colorPaletteGreenBackground2: #dcfce7;
        --colorPaletteGreenForeground1: #22c55e;
        --colorPaletteGreenForeground2: #16a34a;
        
        /* Typography */
        --fontFamilyBase: 'Segoe UI', system-ui, sans-serif;
        --fontSizeBase100: 0.75rem;
        --fontSizeBase200: 0.875rem;
        --fontSizeBase300: 1rem;
        --fontSizeBase400: 1.125rem;
        --fontSizeBase500: 1.25rem;
        --fontSizeBase600: 1.5rem;
        
        /* Spacing */
        --spacingHorizontalXXS: 0.125rem;
        --spacingHorizontalXS: 0.25rem;
        --spacingHorizontalSNudge: 0.375rem;
        --spacingHorizontalS: 0.5rem;
        --spacingHorizontalMNudge: 0.75rem;
        --spacingHorizontalM: 1rem;
        --spacingHorizontalL: 1.5rem;
        --spacingHorizontalXL: 2rem;
        --spacingHorizontalXXL: 3rem;
        
        /* Border Radius */
        --borderRadiusNone: 0;
        --borderRadiusSmall: 0.125rem;
        --borderRadiusMedium: 0.25rem;
        --borderRadiusLarge: 0.375rem;
        --borderRadiusXLarge: 0.5rem;
        --borderRadiusCircular: 50%;
        
        /* Shadows */
        --shadow2: 0 1px 2px rgba(0, 0, 0, 0.12);
        --shadow4: 0 2px 4px rgba(0, 0, 0, 0.14);
        --shadow8: 0 4px 8px rgba(0, 0, 0, 0.14);
        --shadow16: 0 8px 16px rgba(0, 0, 0, 0.14);
        --shadow28: 0 14px 28px rgba(0, 0, 0, 0.14);
        --shadow64: 0 32px 64px rgba(0, 0, 0, 0.14);
    ";
}
```

## Performance Optimization

### Critical CSS Strategy

```razor
<!-- Critical styles loaded immediately -->
<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    LoadPriority="StyleLoadPriority.Critical"
    InlineStyles="@criticalCSS" />

<!-- Above-the-fold component styles -->
<OsirionStyles 
    Framework="CssFramework.Custom"
    LoadPriority="StyleLoadPriority.High"
    CustomVariables="@aboveTheFoldStyles" />

<!-- Below-the-fold styles loaded asynchronously -->
<OsirionStyles 
    Framework="CssFramework.Custom"
    LoadPriority="StyleLoadPriority.Low"
    StylesheetUrls="@belowTheFoldStylesheets" />

@code {
    private string criticalCSS = @"
        /* Critical layout styles */
        body { margin: 0; font-family: system-ui, sans-serif; }
        .header { background: var(--brand-primary); }
        .main { min-height: 100vh; }
        .spinner { /* loading spinner styles */ }
    ";
    
    private string aboveTheFoldStyles = @"
        /* Hero section variables */
        --hero-background: linear-gradient(135deg, var(--brand-primary), var(--brand-secondary));
        --hero-text-color: #ffffff;
        --hero-padding: 4rem 2rem;
        
        /* Navigation variables */
        --nav-height: 4rem;
        --nav-background: rgba(255, 255, 255, 0.95);
        --nav-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    ";
    
    private string[] belowTheFoldStylesheets = new[]
    {
        "/css/components/cards.css",
        "/css/components/forms.css",
        "/css/components/modals.css",
        "/css/utilities.css"
    };
}
```

### Dynamic Style Loading

```razor
@inject IStyleLoadingService StyleLoader

<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    CacheStrategy="StyleCacheStrategy.Aggressive"
    MinifyInProduction="true" />

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load styles based on page requirements
            await LoadPageSpecificStyles();
        }
    }
    
    private async Task LoadPageSpecificStyles()
    {
        var currentPage = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        
        var stylesToLoad = currentPage switch
        {
            "dashboard" => new[] { "dashboard.css", "charts.css", "tables.css" },
            "profile" => new[] { "forms.css", "avatar.css" },
            "settings" => new[] { "forms.css", "toggles.css" },
            _ => Array.Empty<string>()
        };
        
        await StyleLoader.LoadStylesAsync(stylesToLoad);
    }
}
```

## Best Practices

### Organization and Structure

1. **Component Order**: Place OsirionStyles components early in your layout hierarchy
2. **Framework Detection**: Use automatic framework detection when possible
3. **Variable Organization**: Group related CSS variables logically
4. **Performance**: Use appropriate loading priorities for different style types

### CSS Custom Properties

1. **Naming Convention**: Use semantic, hierarchical naming for CSS variables
2. **Fallback Values**: Always provide fallback values for CSS custom properties
3. **Scope Management**: Properly scope variables to avoid conflicts
4. **Documentation**: Document the purpose and usage of custom variables

### Framework Integration

1. **Version Compatibility**: Specify framework versions for consistent behavior
2. **Override Strategy**: Use framework-specific variable names for overrides
3. **Progressive Enhancement**: Design for graceful degradation when frameworks fail to load
4. **Testing**: Test across different framework versions and configurations

### Performance Considerations

1. **Critical Path**: Identify and prioritize critical CSS for above-the-fold content
2. **Bundle Optimization**: Use appropriate caching and minification strategies
3. **Lazy Loading**: Load non-critical styles asynchronously
4. **CSS Containment**: Use CSS containment for component isolation

The OsirionStyles component provides a comprehensive and flexible foundation for styling Blazor applications, supporting multiple CSS frameworks while maintaining excellent performance and developer experience.
