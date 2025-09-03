---
id: 'theme-toggle-component'
order: 2
layout: docs
title: ThemeToggle Component
permalink: /docs/components/theming/theme-toggle
description: Learn how to implement the ThemeToggle component for seamless dark/light mode switching with system preference detection and accessibility support.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Theming Components
- UI Controls
tags:
- blazor
- theme-toggle
- dark-mode
- light-mode
- accessibility
- user-interface
is_featured: true
published: true
slug: components/theming/theme-toggle
lang: en
custom_fields: {}
seo_properties:
  title: 'ThemeToggle Component - Dark/Light Mode Switching | Osirion.Blazor'
  description: 'Implement seamless theme switching with the ThemeToggle component. Features system preference detection, smooth transitions, and full accessibility support.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/theming/theme-toggle'
  lang: en
  robots: index, follow
  og_title: 'ThemeToggle Component - Osirion.Blazor'
  og_description: 'Seamless dark/light mode switching with system preference detection and accessibility support.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'ThemeToggle Component - Osirion.Blazor'
  twitter_description: 'Dark/light mode switching with system preference detection and accessibility support.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# ThemeToggle Component

The ThemeToggle component provides an intuitive interface for users to switch between light and dark themes. It features automatic system preference detection, smooth transitions, comprehensive accessibility support, and customizable styling options.

## Component Overview

ThemeToggle is an interactive component that allows users to seamlessly switch between different theme modes. It automatically detects the user's system preference and provides a smooth, accessible interface for theme management.

### Key Features

**System Preference Detection**: Automatically detects and respects user's system dark/light mode preference
**Smooth Transitions**: Provides smooth visual transitions when switching themes
**Accessibility Compliant**: Full keyboard navigation, screen reader support, and ARIA attributes
**Customizable Appearance**: Flexible styling options to match your design system
**Persistent Preferences**: Remembers user's theme choice across sessions
**Event Handling**: Comprehensive event system for theme change notifications

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `OnThemeChanged` | `EventCallback<string>` | `null` | Callback invoked when theme changes. Receives the new theme name. |
| `CurrentTheme` | `string?` | `null` | Current theme mode ("light", "dark", "auto"). If null, uses system preference. |
| `ShowLabel` | `bool` | `false` | Whether to display text label alongside the toggle icon. |
| `LightLabel` | `string` | `"Light"` | Text label displayed when in light mode (if ShowLabel is true). |
| `DarkLabel` | `string` | `"Dark"` | Text label displayed when in dark mode (if ShowLabel is true). |
| `AutoLabel` | `string` | `"Auto"` | Text label displayed when in auto mode (if ShowLabel is true). |
| `Size` | `ThemeToggleSize` | `Medium` | Size variant of the toggle (Small, Medium, Large). |
| `Variant` | `ThemeToggleVariant` | `Switch` | Visual variant (Switch, Button, Icon). |
| `Position` | `ThemeTogglePosition` | `Inline` | Position behavior (Inline, Fixed, Floating). |
| `Class` | `string?` | `null` | Additional CSS classes to apply to the component. |
| `Style` | `string?` | `null` | Inline styles to apply to the component. |
| `AriaLabel` | `string?` | `null` | Custom ARIA label for accessibility. If null, uses default labels. |
| `Disabled` | `bool` | `false` | Whether the toggle is disabled. |

## Basic Usage

### Simple Theme Toggle

```razor
@using Osirion.Blazor.Theming.Components

<div class="app-header">
    <div class="header-content">
        <h1>My Application</h1>
        <ThemeToggle OnThemeChanged="HandleThemeChange" />
    </div>
</div>

@code {
    private async Task HandleThemeChange(string theme)
    {
        Console.WriteLine($"Theme changed to: {theme}");
        
        // Optional: Save to user preferences
        await UserPreferences.SetThemeAsync(theme);
    }
}
```

### Theme Toggle with Label

```razor
<ThemeToggle 
    OnThemeChanged="HandleThemeChange"
    ShowLabel="true"
    LightLabel="Light Mode"
    DarkLabel="Dark Mode" />

@code {
    private async Task HandleThemeChange(string theme)
    {
        // Handle theme change
        await ThemeService.SetThemeAsync(theme);
        await InvokeAsync(StateHasChanged);
    }
}
```

### Custom Styled Toggle

```razor
<ThemeToggle 
    OnThemeChanged="HandleThemeChange"
    Size="ThemeToggleSize.Large"
    Variant="ThemeToggleVariant.Button"
    Class="custom-theme-toggle" />

<style>
.custom-theme-toggle {
    --toggle-background: #f8fafc;
    --toggle-border: #e2e8f0;
    --toggle-checked-background: #3b82f6;
    --toggle-thumb: #ffffff;
    --toggle-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    border-radius: 0.5rem;
    transition: all 0.2s ease-in-out;
}

.custom-theme-toggle:hover {
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}
</style>

@code {
    private async Task HandleThemeChange(string theme)
    {
        await ThemeService.SetThemeAsync(theme);
    }
}
```

## Advanced Usage

### Integration with User Preferences

```razor
@inject IUserPreferencesService UserPreferences
@inject IThemeService ThemeService

<ThemeToggle 
    OnThemeChanged="OnThemeChanged"
    CurrentTheme="@currentTheme"
    ShowLabel="true" />

@code {
    private string? currentTheme;
    
    protected override async Task OnInitializedAsync()
    {
        // Load user's saved theme preference
        currentTheme = await UserPreferences.GetThemeAsync();
        
        if (string.IsNullOrEmpty(currentTheme))
        {
            // Detect system preference if no saved preference
            currentTheme = await ThemeService.DetectSystemPreferenceAsync();
        }
        
        // Apply the theme
        await ThemeService.SetThemeAsync(currentTheme);
    }
    
    private async Task OnThemeChanged(string theme)
    {
        currentTheme = theme;
        
        // Save preference
        await UserPreferences.SetThemeAsync(theme);
        
        // Apply theme
        await ThemeService.SetThemeAsync(theme);
        
        // Notify other components
        await ThemeService.NotifyThemeChangedAsync(theme);
        
        StateHasChanged();
    }
}
```

### System Preference Integration

```razor
@inject IJSRuntime JS

<ThemeToggle 
    OnThemeChanged="OnThemeChanged"
    CurrentTheme="@currentTheme" />

@code {
    private string currentTheme = "auto";
    private DotNetObjectReference<ThemeToggleExample>? objRef;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            objRef = DotNetObjectReference.Create(this);
            
            // Setup system preference change listener
            await JS.InvokeVoidAsync("setupThemeListener", objRef);
            
            // Get initial system preference
            var systemPreference = await JS.InvokeAsync<string>("getSystemThemePreference");
            
            if (currentTheme == "auto")
            {
                await ApplyTheme(systemPreference);
            }
        }
    }
    
    [JSInvokable]
    public async Task OnSystemThemeChanged(string theme)
    {
        if (currentTheme == "auto")
        {
            await ApplyTheme(theme);
            StateHasChanged();
        }
    }
    
    private async Task OnThemeChanged(string theme)
    {
        currentTheme = theme;
        
        if (theme == "auto")
        {
            var systemTheme = await JS.InvokeAsync<string>("getSystemThemePreference");
            await ApplyTheme(systemTheme);
        }
        else
        {
            await ApplyTheme(theme);
        }
    }
    
    private async Task ApplyTheme(string theme)
    {
        await JS.InvokeVoidAsync("applyTheme", theme);
    }
    
    public void Dispose()
    {
        objRef?.Dispose();
    }
}

<script>
    window.getSystemThemePreference = () => {
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    };
    
    window.setupThemeListener = (dotNetRef) => {
        const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
        
        mediaQuery.addEventListener('change', (e) => {
            const theme = e.matches ? 'dark' : 'light';
            dotNetRef.invokeMethodAsync('OnSystemThemeChanged', theme);
        });
    };
    
    window.applyTheme = (theme) => {
        document.documentElement.setAttribute('data-theme', theme);
        document.documentElement.classList.toggle('dark', theme === 'dark');
    };
</script>
```

### Multiple Theme Support

```razor
@inject IThemeService ThemeService

<div class="theme-selector">
    <h3>Choose Your Theme</h3>
    
    <div class="theme-options">
        @foreach (var theme in availableThemes)
        {
            <div class="theme-option @(IsSelected(theme.Key) ? "selected" : "")">
                <ThemeToggle 
                    OnThemeChanged="() => OnThemeSelected(theme.Key)"
                    CurrentTheme="@theme.Key"
                    ShowLabel="true"
                    LightLabel="@theme.Value.LightLabel"
                    DarkLabel="@theme.Value.DarkLabel"
                    Class="theme-preview-toggle" />
                
                <div class="theme-preview" data-theme="@theme.Key">
                    <div class="preview-header">@theme.Value.Name</div>
                    <div class="preview-content">
                        <div class="preview-text">Sample text</div>
                        <div class="preview-button">Button</div>
                    </div>
                </div>
            </div>
        }
    </div>
</div>

@code {
    private string selectedTheme = "default";
    
    private readonly Dictionary<string, ThemeInfo> availableThemes = new()
    {
        ["default"] = new("Default", "Light", "Dark"),
        ["blue"] = new("Blue Theme", "Blue Light", "Blue Dark"),
        ["green"] = new("Green Theme", "Green Light", "Green Dark"),
        ["purple"] = new("Purple Theme", "Purple Light", "Purple Dark")
    };
    
    private bool IsSelected(string themeKey) => selectedTheme == themeKey;
    
    private async Task OnThemeSelected(string theme)
    {
        selectedTheme = theme;
        await ThemeService.SetThemeAsync(theme);
        StateHasChanged();
    }
    
    public record ThemeInfo(string Name, string LightLabel, string DarkLabel);
}

<style>
.theme-selector {
    padding: 2rem;
    background: var(--color-background);
    border-radius: 0.5rem;
    border: 1px solid var(--color-border);
}

.theme-options {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1rem;
    margin-top: 1rem;
}

.theme-option {
    border: 2px solid var(--color-border);
    border-radius: 0.5rem;
    padding: 1rem;
    transition: all 0.2s ease;
    cursor: pointer;
}

.theme-option.selected {
    border-color: var(--color-primary);
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.theme-option:hover {
    border-color: var(--color-primary);
}

.theme-preview {
    margin-top: 0.5rem;
    padding: 1rem;
    border-radius: 0.25rem;
    background: var(--color-background);
    border: 1px solid var(--color-border);
}

.preview-header {
    font-weight: 600;
    color: var(--color-foreground);
    margin-bottom: 0.5rem;
}

.preview-content {
    display: flex;
    gap: 0.5rem;
    align-items: center;
}

.preview-text {
    color: var(--color-muted-foreground);
    font-size: 0.875rem;
}

.preview-button {
    padding: 0.25rem 0.5rem;
    background: var(--color-primary);
    color: var(--color-primary-foreground);
    border-radius: 0.25rem;
    font-size: 0.75rem;
}
</style>
```

## Styling and Customization

### CSS Custom Properties

The ThemeToggle component supports extensive customization through CSS custom properties:

```css
.theme-toggle {
    /* Size and Dimensions */
    --toggle-width: 3rem;
    --toggle-height: 1.5rem;
    --toggle-thumb-size: 1.25rem;
    --toggle-padding: 0.125rem;
    
    /* Colors */
    --toggle-background: #e5e7eb;
    --toggle-background-checked: #3b82f6;
    --toggle-thumb: #ffffff;
    --toggle-border: #d1d5db;
    --toggle-focus: #93c5fd;
    
    /* Transitions */
    --toggle-transition: all 0.2s ease-in-out;
    --toggle-thumb-transition: transform 0.2s ease-in-out;
    
    /* Shadows */
    --toggle-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    --toggle-thumb-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
    --toggle-focus-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}
```

### Size Variants

```razor
<!-- Small -->
<ThemeToggle Size="ThemeToggleSize.Small" />

<!-- Medium (Default) -->
<ThemeToggle Size="ThemeToggleSize.Medium" />

<!-- Large -->
<ThemeToggle Size="ThemeToggleSize.Large" />
```

```css
/* Size-specific custom properties */
.theme-toggle--small {
    --toggle-width: 2rem;
    --toggle-height: 1rem;
    --toggle-thumb-size: 0.875rem;
}

.theme-toggle--large {
    --toggle-width: 4rem;
    --toggle-height: 2rem;
    --toggle-thumb-size: 1.75rem;
}
```

### Visual Variants

```razor
<!-- Switch Style (Default) -->
<ThemeToggle Variant="ThemeToggleVariant.Switch" />

<!-- Button Style -->
<ThemeToggle Variant="ThemeToggleVariant.Button" />

<!-- Icon Only -->
<ThemeToggle Variant="ThemeToggleVariant.Icon" />
```

### Custom Theme Toggle

```razor
<ThemeToggle 
    Class="custom-toggle"
    OnThemeChanged="HandleThemeChange" />

<style>
.custom-toggle {
    /* Custom gradient background */
    --toggle-background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    --toggle-background-checked: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
    
    /* Custom thumb with icon */
    --toggle-thumb: #ffffff;
    --toggle-thumb-size: 1.5rem;
    
    /* Enhanced shadows */
    --toggle-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
    --toggle-thumb-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
    
    /* Smooth spring animation */
    --toggle-transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.custom-toggle:hover {
    transform: scale(1.05);
}

.custom-toggle::before {
    content: '☀️';
    position: absolute;
    left: 0.25rem;
    top: 50%;
    transform: translateY(-50%);
    font-size: 0.75rem;
    transition: opacity 0.2s ease;
}

.custom-toggle:checked::before {
    content: '🌙';
}
</style>
```

## Accessibility Features

### Keyboard Navigation

The ThemeToggle component provides full keyboard accessibility:

- **Space/Enter**: Toggle theme
- **Tab**: Focus navigation
- **Escape**: Cancel focus (when applicable)

### Screen Reader Support

```razor
<ThemeToggle 
    AriaLabel="Toggle between light and dark theme"
    OnThemeChanged="HandleThemeChange" />
```

The component automatically provides:
- Proper ARIA labels and descriptions
- State announcements for screen readers
- Role and property attributes
- Focus management

### High Contrast Support

```css
@media (prefers-contrast: high) {
    .theme-toggle {
        --toggle-background: #000000;
        --toggle-background-checked: #ffffff;
        --toggle-thumb: #ffffff;
        --toggle-border: #ffffff;
        border: 2px solid currentColor;
    }
}
```

### Reduced Motion Support

```css
@media (prefers-reduced-motion: reduce) {
    .theme-toggle {
        --toggle-transition: none;
        --toggle-thumb-transition: none;
    }
}
```

## Integration Examples

### With Navigation Bar

```razor
<nav class="navbar">
    <div class="navbar-brand">
        <a href="/">My App</a>
    </div>
    
    <div class="navbar-nav">
        <a href="/about">About</a>
        <a href="/contact">Contact</a>
    </div>
    
    <div class="navbar-actions">
        <ThemeToggle 
            OnThemeChanged="HandleThemeChange"
            Size="ThemeToggleSize.Small"
            Class="navbar-theme-toggle" />
    </div>
</nav>

<style>
.navbar {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 1rem 2rem;
    background: var(--color-background);
    border-bottom: 1px solid var(--color-border);
}

.navbar-actions {
    display: flex;
    align-items: center;
    gap: 1rem;
}

.navbar-theme-toggle {
    margin-left: 1rem;
}
</style>
```

### With Settings Panel

```razor
<div class="settings-panel">
    <h3>Appearance Settings</h3>
    
    <div class="setting-group">
        <label class="setting-label">
            Theme Preference
        </label>
        <div class="setting-control">
            <ThemeToggle 
                OnThemeChanged="OnThemeChanged"
                ShowLabel="true"
                CurrentTheme="@currentTheme"
                Class="settings-theme-toggle" />
        </div>
        <div class="setting-description">
            Choose between light and dark theme, or use automatic based on your system preference.
        </div>
    </div>
    
    <div class="setting-group">
        <label class="setting-label">
            <input type="checkbox" @bind="enableTransitions" />
            Enable smooth transitions
        </label>
    </div>
</div>

@code {
    private string currentTheme = "auto";
    private bool enableTransitions = true;
    
    private async Task OnThemeChanged(string theme)
    {
        currentTheme = theme;
        await UserSettings.SaveThemePreferenceAsync(theme);
        
        if (enableTransitions)
        {
            await JS.InvokeVoidAsync("enableThemeTransitions");
        }
    }
}
```

## Best Practices

### Implementation Guidelines

1. **Placement**: Position theme toggles in easily accessible locations like navigation bars or settings panels
2. **Labeling**: Provide clear labels for better user understanding
3. **Persistence**: Always save user theme preferences across sessions
4. **System Integration**: Respect system preferences when no user preference exists
5. **Accessibility**: Ensure proper ARIA labels and keyboard navigation

### Performance Considerations

1. **Efficient Updates**: Use throttled event handlers for rapid theme changes
2. **CSS Optimization**: Minimize CSS custom property usage for better performance
3. **State Management**: Use efficient state management for theme persistence
4. **Bundle Size**: Consider component tree shaking for unused variants

### User Experience

1. **Smooth Transitions**: Implement smooth transitions between themes
2. **Visual Feedback**: Provide clear visual feedback for theme state
3. **Consistent Behavior**: Maintain consistent theme behavior across components
4. **Loading States**: Handle theme loading states gracefully

The ThemeToggle component provides a robust, accessible, and customizable solution for theme management in Blazor applications, supporting modern user experience expectations while maintaining excellent performance and accessibility standards.
