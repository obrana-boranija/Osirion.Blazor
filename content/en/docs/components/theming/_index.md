---
id: 'theming-components-overview'
order: 1
layout: docs
title: Theming Components Overview
permalink: /docs/components/theming
description: Complete overview of Osirion.Blazor Theming components for theme management, dark/light mode switching, CSS custom properties, and design system implementation.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Theming Components
- Design Systems
tags:
- blazor
- theming
- dark-mode
- light-mode
- css-custom-properties
- design-tokens
- design-systems
is_featured: true
published: true
slug: theming
lang: en
custom_fields: {}
seo_properties:
  title: 'Theming Components - Osirion.Blazor Design System & Theme Management'
  description: 'Explore Osirion.Blazor Theming components for comprehensive theme management, dark/light mode switching, and design system implementation.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/theming'
  lang: en
  robots: index, follow
  og_title: 'Theming Components - Osirion.Blazor'
  og_description: 'Comprehensive theme management and design system components for Blazor applications.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Theming Components - Osirion.Blazor'
  twitter_description: 'Theme management and design system components for Blazor applications.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Theming Components Overview

The Osirion.Blazor Theming module provides comprehensive theme management capabilities including dark/light mode switching, CSS custom properties management, design token systems, and framework-agnostic styling solutions. These components enable consistent design systems while maintaining flexibility for brand customization.

## Module Features

**Multi-Framework Support**: Compatible with Bootstrap, Tailwind CSS, Fluent UI, and custom frameworks
**Dark/Light Mode**: Seamless theme switching with system preference detection
**Design Tokens**: Comprehensive design token system using CSS custom properties
**Brand Customization**: Easy brand color and styling customization
**Performance Optimized**: Efficient theme loading and switching mechanisms
**SSR Compatible**: Full Server-Side Rendering support with proper styling injection

## Core Components

### Theme Management Components

Components for controlling and managing theme state and transitions.

#### ThemeToggle
Interactive component for switching between light and dark themes.

**Key Features:**
- Automatic system preference detection
- Smooth theme transitions
- Accessibility compliant
- Customizable appearance
- Persistent theme preferences
- Screen reader support

```razor
<ThemeToggle 
    OnThemeChanged="HandleThemeChange" />
```

**Basic Implementation:**
```razor
<div class="theme-toggle-container">
    <ThemeToggle OnThemeChanged="OnThemeChanged" />
</div>

@code {
    private async Task OnThemeChanged(string theme)
    {
        // Handle theme change event
        Console.WriteLine($"Theme changed to: {theme}");
        
        // Optional: Save preference to user settings
        await UserPreferencesService.SaveThemePreferenceAsync(theme);
    }
}
```

**Custom Styling:**
```razor
<ThemeToggle Class="custom-theme-toggle" />

<style>
.custom-theme-toggle {
    --toggle-size: 2.5rem;
    --toggle-background: #f3f4f6;
    --toggle-border: #e5e7eb;
    --toggle-thumb: #ffffff;
    --toggle-thumb-checked: #3b82f6;
}
</style>
```

#### OsirionStyles
Core styling component that manages CSS injection and theme variables.

**Key Features:**
- Framework detection and adaptation
- CSS custom property injection
- Theme variable management
- Style optimization
- Framework-specific styling

```razor
<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    UseDefaultStyles="true"
    CustomVariables="@customCssVariables" />
```

**Advanced Configuration:**
```razor
<OsirionStyles 
    Framework="CssFramework.Custom"
    UseDefaultStyles="false"
    CustomVariables="@GetCustomVariables()"
    GeneratedVariables="@GetGeneratedVariables()" />

@code {
    private string GetCustomVariables()
    {
        return @"
            --primary-color: #3b82f6;
            --secondary-color: #64748b;
            --success-color: #10b981;
            --warning-color: #f59e0b;
            --error-color: #ef4444;
            --background-color: #ffffff;
            --text-color: #111827;
        ";
    }
    
    private string GetGeneratedVariables()
    {
        // Generate variables based on brand colors
        return ThemeService.GenerateColorPalette(brandColor);
    }
}
```

## Installation and Setup

### Package Installation

```bash
dotnet add package Osirion.Blazor.Theming
```

### Service Registration

```csharp
// Program.cs
builder.Services.AddOsirionTheming(options =>
{
    options.Framework = CssFramework.Bootstrap;
    options.DefaultMode = ThemeMode.Light;
    options.EnableSystemPreferenceDetection = true;
    options.PersistThemePreference = true;
    options.UseDefaultStyles = true;
});
```

### Basic Setup

```razor
<!-- App.razor or MainLayout.razor -->
<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    UseDefaultStyles="true" />

<div class="app-container">
    <header class="app-header">
        <nav class="navbar">
            <div class="navbar-brand">Your App</div>
            <div class="navbar-actions">
                <ThemeToggle />
            </div>
        </nav>
    </header>
    
    <main class="app-content">
        @Body
    </main>
</div>
```

### Import Statements

```razor
@* _Imports.razor *@
@using Osirion.Blazor.Theming.Components
@using Osirion.Blazor.Theming.Services
@using Osirion.Blazor.Theming.Options
@using Osirion.Blazor.Theming.Enums
```

## Framework Integration

### Bootstrap Integration

```csharp
// Program.cs
builder.Services.AddOsirionTheming(options =>
{
    options.Framework = CssFramework.Bootstrap;
    options.BootstrapVersion = BootstrapVersion.V5;
    options.UseBootstrapDarkMode = true;
});
```

```razor
<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    UseDefaultStyles="true" />

<!-- Bootstrap-specific theme toggle -->
<ThemeToggle Class="btn btn-outline-secondary" />
```

### Tailwind CSS Integration

```csharp
builder.Services.AddOsirionTheming(options =>
{
    options.Framework = CssFramework.TailwindCSS;
    options.EnableTailwindDarkMode = true;
    options.TailwindDarkModeStrategy = TailwindDarkModeStrategy.Class;
});
```

```razor
<OsirionStyles 
    Framework="CssFramework.TailwindCSS"
    UseDefaultStyles="true" />

<!-- Tailwind-specific theme toggle -->
<ThemeToggle Class="bg-gray-200 dark:bg-gray-700 rounded-lg p-2" />
```

### Fluent UI Integration

```csharp
builder.Services.AddOsirionTheming(options =>
{
    options.Framework = CssFramework.FluentUI;
    options.EnableFluentUITokens = true;
    options.FluentUITheme = FluentUITheme.Web;
});
```

```razor
<OsirionStyles 
    Framework="CssFramework.FluentUI"
    UseDefaultStyles="true" />

<!-- Fluent UI theme toggle -->
<ThemeToggle Class="fui-Button" />
```

### Custom Framework Integration

```csharp
builder.Services.AddOsirionTheming(options =>
{
    options.Framework = CssFramework.Custom;
    options.CustomFrameworkName = "MyDesignSystem";
    options.UseDefaultStyles = false;
});
```

```razor
<OsirionStyles 
    Framework="CssFramework.Custom"
    UseDefaultStyles="false"
    CustomVariables="@customDesignTokens" />

@code {
    private string customDesignTokens = @"
        /* Color Tokens */
        --color-primary-50: #eff6ff;
        --color-primary-500: #3b82f6;
        --color-primary-900: #1e3a8a;
        
        /* Spacing Tokens */
        --spacing-xs: 0.25rem;
        --spacing-sm: 0.5rem;
        --spacing-md: 1rem;
        --spacing-lg: 1.5rem;
        --spacing-xl: 3rem;
        
        /* Typography Tokens */
        --font-size-xs: 0.75rem;
        --font-size-sm: 0.875rem;
        --font-size-base: 1rem;
        --font-size-lg: 1.125rem;
        --font-size-xl: 1.25rem;
        
        /* Border Radius Tokens */
        --border-radius-sm: 0.125rem;
        --border-radius-md: 0.375rem;
        --border-radius-lg: 0.5rem;
        --border-radius-full: 9999px;
    ";
}
```

## Design Token System

### Color System

```razor
<OsirionStyles CustomVariables="@colorTokens" />

@code {
    private string colorTokens = @"
        /* Light Theme Colors */
        --color-background: #ffffff;
        --color-foreground: #09090b;
        --color-primary: #0f172a;
        --color-primary-foreground: #f8fafc;
        --color-secondary: #f1f5f9;
        --color-secondary-foreground: #0f172a;
        --color-muted: #f1f5f9;
        --color-muted-foreground: #64748b;
        --color-accent: #f1f5f9;
        --color-accent-foreground: #0f172a;
        --color-destructive: #ef4444;
        --color-destructive-foreground: #fef2f2;
        --color-border: #e2e8f0;
        --color-input: #e2e8f0;
        --color-ring: #94a3b8;
        
        /* Dark Theme Colors */
        [data-theme='dark'] {
            --color-background: #09090b;
            --color-foreground: #fafafa;
            --color-primary: #fafafa;
            --color-primary-foreground: #09090b;
            --color-secondary: #27272a;
            --color-secondary-foreground: #fafafa;
            --color-muted: #27272a;
            --color-muted-foreground: #a1a1aa;
            --color-accent: #27272a;
            --color-accent-foreground: #fafafa;
            --color-destructive: #7f1d1d;
            --color-destructive-foreground: #fafafa;
            --color-border: #27272a;
            --color-input: #27272a;
            --color-ring: #d4d4d8;
        }
    ";
}
```

### Typography System

```razor
<OsirionStyles CustomVariables="@typographyTokens" />

@code {
    private string typographyTokens = @"
        /* Typography Scale */
        --font-size-xs: 0.75rem;    /* 12px */
        --font-size-sm: 0.875rem;   /* 14px */
        --font-size-base: 1rem;     /* 16px */
        --font-size-lg: 1.125rem;   /* 18px */
        --font-size-xl: 1.25rem;    /* 20px */
        --font-size-2xl: 1.5rem;    /* 24px */
        --font-size-3xl: 1.875rem;  /* 30px */
        --font-size-4xl: 2.25rem;   /* 36px */
        --font-size-5xl: 3rem;      /* 48px */
        
        /* Font Weights */
        --font-weight-light: 300;
        --font-weight-normal: 400;
        --font-weight-medium: 500;
        --font-weight-semibold: 600;
        --font-weight-bold: 700;
        
        /* Line Heights */
        --line-height-tight: 1.25;
        --line-height-normal: 1.5;
        --line-height-relaxed: 1.75;
        
        /* Font Families */
        --font-family-sans: 'Inter', ui-sans-serif, system-ui, sans-serif;
        --font-family-serif: ui-serif, Georgia, Cambria, serif;
        --font-family-mono: ui-monospace, 'Fira Code', monospace;
    ";
}
```

### Spacing System

```razor
<OsirionStyles CustomVariables="@spacingTokens" />

@code {
    private string spacingTokens = @"
        /* Spacing Scale */
        --spacing-0: 0;
        --spacing-px: 1px;
        --spacing-0-5: 0.125rem;  /* 2px */
        --spacing-1: 0.25rem;     /* 4px */
        --spacing-1-5: 0.375rem;  /* 6px */
        --spacing-2: 0.5rem;      /* 8px */
        --spacing-2-5: 0.625rem;  /* 10px */
        --spacing-3: 0.75rem;     /* 12px */
        --spacing-3-5: 0.875rem;  /* 14px */
        --spacing-4: 1rem;        /* 16px */
        --spacing-5: 1.25rem;     /* 20px */
        --spacing-6: 1.5rem;      /* 24px */
        --spacing-7: 1.75rem;     /* 28px */
        --spacing-8: 2rem;        /* 32px */
        --spacing-9: 2.25rem;     /* 36px */
        --spacing-10: 2.5rem;     /* 40px */
        --spacing-12: 3rem;       /* 48px */
        --spacing-16: 4rem;       /* 64px */
        --spacing-20: 5rem;       /* 80px */
        --spacing-24: 6rem;       /* 96px */
        --spacing-32: 8rem;       /* 128px */
    ";
}
```

## Advanced Usage Examples

### Brand Customization

```razor
<OsirionStyles 
    Framework="CssFramework.Bootstrap"
    CustomVariables="@GetBrandVariables()" />

@code {
    [Parameter] public string? BrandColor { get; set; } = "#3b82f6";
    [Parameter] public string? AccentColor { get; set; } = "#10b981";
    
    private string GetBrandVariables()
    {
        var brandHsl = ColorUtility.HexToHsl(BrandColor ?? "#3b82f6");
        var accentHsl = ColorUtility.HexToHsl(AccentColor ?? "#10b981");
        
        return $@"
            /* Brand Colors */
            --brand-primary: {BrandColor};
            --brand-primary-50: {ColorUtility.GenerateShade(brandHsl, 50)};
            --brand-primary-100: {ColorUtility.GenerateShade(brandHsl, 100)};
            --brand-primary-200: {ColorUtility.GenerateShade(brandHsl, 200)};
            --brand-primary-300: {ColorUtility.GenerateShade(brandHsl, 300)};
            --brand-primary-400: {ColorUtility.GenerateShade(brandHsl, 400)};
            --brand-primary-500: {BrandColor};
            --brand-primary-600: {ColorUtility.GenerateShade(brandHsl, 600)};
            --brand-primary-700: {ColorUtility.GenerateShade(brandHsl, 700)};
            --brand-primary-800: {ColorUtility.GenerateShade(brandHsl, 800)};
            --brand-primary-900: {ColorUtility.GenerateShade(brandHsl, 900)};
            
            /* Accent Colors */
            --brand-accent: {AccentColor};
            --brand-accent-50: {ColorUtility.GenerateShade(accentHsl, 50)};
            --brand-accent-500: {AccentColor};
            --brand-accent-900: {ColorUtility.GenerateShade(accentHsl, 900)};
        ";
    }
}
```

### Dynamic Theme Generation

```razor
@inject IThemeService ThemeService

<div class="theme-configurator">
    <h3>Theme Configuration</h3>
    
    <div class="color-picker-group">
        <label>Primary Color:</label>
        <input type="color" @bind="primaryColor" @onchange="UpdateTheme" />
    </div>
    
    <div class="color-picker-group">
        <label>Secondary Color:</label>
        <input type="color" @bind="secondaryColor" @onchange="UpdateTheme" />
    </div>
    
    <div class="theme-preview">
        <h4>Preview</h4>
        <button class="btn btn-primary">Primary Button</button>
        <button class="btn btn-secondary">Secondary Button</button>
    </div>
</div>

<OsirionStyles CustomVariables="@generatedTheme" />

@code {
    private string primaryColor = "#3b82f6";
    private string secondaryColor = "#64748b";
    private string generatedTheme = "";
    
    protected override void OnInitialized()
    {
        UpdateTheme();
    }
    
    private void UpdateTheme()
    {
        generatedTheme = ThemeService.GenerateThemeVariables(new ThemeConfiguration
        {
            PrimaryColor = primaryColor,
            SecondaryColor = secondaryColor,
            GenerateShades = true,
            IncludeDarkMode = true
        });
        
        StateHasChanged();
    }
}
```

### System Preference Integration

```razor
<ThemeToggle OnThemeChanged="HandleThemeChange" />

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Detect system preference
            var systemPreference = await JS.InvokeAsync<string>("detectSystemThemePreference");
            
            // Apply system preference if no user preference exists
            var userPreference = await LocalStorage.GetItemAsync<string>("theme-preference");
            
            if (string.IsNullOrEmpty(userPreference))
            {
                await ThemeService.SetThemeAsync(systemPreference == "dark" ? ThemeMode.Dark : ThemeMode.Light);
            }
        }
    }
    
    private async Task HandleThemeChange(string theme)
    {
        // Save user preference
        await LocalStorage.SetItemAsync("theme-preference", theme);
        
        // Apply theme
        await ThemeService.SetThemeAsync(theme == "dark" ? ThemeMode.Dark : ThemeMode.Light);
    }
}

<script>
    window.detectSystemThemePreference = () => {
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    };
</script>
```

### Component-Specific Theming

```razor
<!-- Theme-aware component -->
<div class="custom-card" data-theme="@currentTheme">
    <div class="card-header">
        <h3>@Title</h3>
        <ThemeToggle />
    </div>
    <div class="card-content">
        @ChildContent
    </div>
</div>

@code {
    [Parameter] public string? Title { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    [Inject] private IThemeService ThemeService { get; set; } = default!;
    
    private string currentTheme => ThemeService.CurrentMode.ToString().ToLower();
}

<style>
.custom-card {
    background: var(--color-background);
    color: var(--color-foreground);
    border: 1px solid var(--color-border);
    border-radius: var(--border-radius-lg);
    padding: var(--spacing-6);
    transition: all 0.3s ease;
}

.custom-card[data-theme="dark"] {
    background: var(--color-background);
    border-color: var(--color-border);
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.3);
}

.custom-card[data-theme="light"] {
    background: var(--color-background);
    border-color: var(--color-border);
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}
</style>
```

## Performance Optimization

### Efficient Style Loading

```csharp
// Configure optimized style loading
builder.Services.AddOsirionTheming(options =>
{
    options.OptimizeStyleLoading = true;
    options.PreloadCriticalStyles = true;
    options.DeferNonCriticalStyles = true;
    options.MinifyStyles = !builder.Environment.IsDevelopment();
});
```

### CSS Custom Property Optimization

```razor
<OsirionStyles 
    Framework="CssFramework.Custom"
    CustomVariables="@GetOptimizedVariables()" />

@code {
    private string GetOptimizedVariables()
    {
        // Only include variables that are actually used
        var usedVariables = StyleAnalyzer.GetUsedVariables(componentTree);
        
        return ThemeService.GenerateOptimizedVariables(usedVariables);
    }
}
```

## Accessibility Considerations

### High Contrast Support

```razor
<OsirionStyles CustomVariables="@GetAccessibilityVariables()" />

@code {
    private string GetAccessibilityVariables()
    {
        return @"
            /* High Contrast Mode Support */
            @media (prefers-contrast: high) {
                :root {
                    --color-background: #000000;
                    --color-foreground: #ffffff;
                    --color-primary: #ffffff;
                    --color-border: #ffffff;
                    --color-focus: #ffff00;
                }
            }
            
            /* Reduced Motion Support */
            @media (prefers-reduced-motion: reduce) {
                :root {
                    --transition-duration: 0ms;
                    --animation-duration: 0ms;
                }
            }
        ";
    }
}
```

### Color Contrast Validation

```csharp
public class ContrastValidator
{
    public ValidationResult ValidateContrast(string backgroundColor, string textColor)
    {
        var contrast = ColorUtility.CalculateContrast(backgroundColor, textColor);
        
        return new ValidationResult
        {
            IsValid = contrast >= 4.5, // WCAG AA standard
            ContrastRatio = contrast,
            Recommendation = contrast < 4.5 ? "Increase contrast for better accessibility" : "Contrast meets WCAG AA standards"
        };
    }
}
```

## Best Practices

### Design Token Organization
- Use semantic naming for design tokens
- Organize tokens by category (color, spacing, typography)
- Maintain consistent naming conventions
- Document token usage and relationships

### Theme Switching
- Provide smooth transitions between themes
- Respect user system preferences
- Persist theme preferences across sessions
- Test both light and dark modes thoroughly

### Performance
- Minimize CSS custom property usage
- Use efficient selectors for theme switching
- Implement proper style loading strategies
- Monitor bundle size impact

### Accessibility
- Maintain proper color contrast ratios
- Support high contrast and reduced motion preferences
- Provide alternative styling for accessibility needs
- Test with screen readers and assistive technologies

The Theming components provide a comprehensive solution for creating consistent, accessible, and performant design systems while maintaining flexibility for brand customization and framework integration.
