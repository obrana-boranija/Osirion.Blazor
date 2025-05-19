# Osirion.Blazor.Theming

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Theming)](https://www.nuget.org/packages/Osirion.Blazor.Theming)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Comprehensive theming and styling system for Blazor applications with CSS framework integration and dark mode support.

## Features

- **CSS Framework Integration**: Seamless integration with Bootstrap, FluentUI, MudBlazor, and Radzen
- **Dark Mode Support**: Built-in light/dark theme toggle with system preference detection
- **CSS Variables**: Consistent theming across components using CSS custom properties
- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Minimal JavaScript**: Progressive enhancement for interactive features
- **Theme Switcher**: User-controlled theme selection with persistent preferences
- **Responsive Design**: Mobile-friendly theming across device sizes

## Installation

```bash
dotnet add package Osirion.Blazor.Theming
```

## Basic Usage

### 1. Register Theming Services

Register the theming services in your `Program.cs` file:

```csharp
// In Program.cs
using Osirion.Blazor.Theming.Extensions;

builder.Services.AddOsirionTheming(theming => {
    theming
        .UseFramework(CssFramework.Bootstrap)
        .EnableDarkMode()
        .UseSystemPreference();
});
```

### 2. Add Components to Your Layout

Add the theming components to your layout:

```razor
@using Osirion.Blazor.Theming.Components

<!-- In your layout -->
<ThemeProvider>
    <ThemeToggle />
    <!-- Your application content -->
</ThemeProvider>
```

## CSS Framework Integration

Osirion.Blazor.Theming can integrate with popular CSS frameworks:

### Bootstrap Integration

```csharp
builder.Services.AddOsirionTheming(theming => {
    theming.UseFramework(CssFramework.Bootstrap);
});
```

```razor
<ThemeProvider Framework="CssFramework.Bootstrap">
    <!-- Your content -->
</ThemeProvider>
```

### FluentUI Integration

```csharp
builder.Services.AddOsirionTheming(theming => {
    theming.UseFramework(CssFramework.FluentUI);
});
```

```razor
<ThemeProvider Framework="CssFramework.FluentUI">
    <!-- Your content -->
</ThemeProvider>
```

### MudBlazor Integration

```csharp
builder.Services.AddOsirionTheming(theming => {
    theming.UseFramework(CssFramework.MudBlazor);
});
```

```razor
<ThemeProvider Framework="CssFramework.MudBlazor">
    <!-- Your content -->
</ThemeProvider>
```

### Radzen Integration

```csharp
builder.Services.AddOsirionTheming(theming => {
    theming.UseFramework(CssFramework.Radzen);
});
```

```razor
<ThemeProvider Framework="CssFramework.Radzen">
    <!-- Your content -->
</ThemeProvider>
```

## Dark Mode Support

### Basic Dark Mode Toggle

```razor
<ThemeToggle />
```

### Customizing the Theme Toggle

```razor
<!-- Simplified with just light/dark options -->
<ThemeToggle ShowSystemOption="false" />

<!-- Custom labels -->
<ThemeToggle 
    LightLabel="Day" 
    DarkLabel="Night"
    SystemLabel="Auto" />

<!-- Custom icons -->
<ThemeToggle>
    <LightIconTemplate>
        <i class="fas fa-sun"></i>
    </LightIconTemplate>
    <DarkIconTemplate>
        <i class="fas fa-moon"></i>
    </DarkIconTemplate>
    <SystemIconTemplate>
        <i class="fas fa-desktop"></i>
    </SystemIconTemplate>
</ThemeToggle>
```

### Programmatic Theme Control

```razor
@inject IThemeService ThemeService

<button @onclick="ToggleDarkMode">Toggle Dark Mode</button>

@code {
    private async Task ToggleDarkMode()
    {
        if (ThemeService.CurrentTheme == Theme.Dark)
            await ThemeService.SetThemeAsync(Theme.Light);
        else
            await ThemeService.SetThemeAsync(Theme.Dark);
    }
}
```

## Custom Variables

You can customize the theme by providing custom CSS variables:

### Via ThemeProvider Component

```razor
<ThemeProvider CustomVariables="--osirion-primary-color: #0077cc; --osirion-border-radius: 0.25rem;">
    <!-- Your content -->
</ThemeProvider>
```

### Via Service Registration

```csharp
builder.Services.AddOsirionTheming(theming => {
    theming
        .UseFramework(CssFramework.Bootstrap)
        .WithCustomVariables(@"
            --osirion-primary-color: #0077cc;
            --osirion-border-radius: 0.25rem;
        ");
});
```

### Via Configuration

```json
{
  "Osirion": {
    "Theming": {
      "Framework": "Bootstrap",
      "EnableDarkMode": true,
      "UseSystemPreference": true,
      "CustomVariables": "--osirion-primary-color: #0077cc; --osirion-border-radius: 0.25rem;"
    }
  }
}
```

```csharp
builder.Services.AddOsirionTheming(builder.Configuration);
```

## CSS Variables Reference

Osirion.Blazor.Theming uses CSS variables for consistent styling across components:

### Core Variables

```css
:root {
    /* Colors */
    --osirion-primary-color: #2563eb;
    --osirion-primary-color-hover: #1d4ed8;
    --osirion-color: #374151;
    --osirion-color-light: #6b7280;
    --osirion-color-border: #e5e7eb;
    --osirion-color-background: #ffffff;
    --osirion-color-background-light: #f3f4f6;
    
    /* Sizes */
    --osirion-border-radius: 0.5rem;
    --osirion-padding: 1.5rem;
    --osirion-gap: 1.5rem;
    
    /* Effects */
    --osirion-transition-speed: 0.2s;
    --osirion-box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}
```

### Dark Theme Variables

```css
.dark-theme {
    --osirion-primary-color: #60a5fa;
    --osirion-primary-color-hover: #93c5fd;
    --osirion-color: #e5e7eb;
    --osirion-color-light: #9ca3af;
    --osirion-color-border: #4b5563;
    --osirion-color-background: #1f2937;
    --osirion-color-background-light: #374151;
}
```

## ThemeProvider Component

The `ThemeProvider` component manages theme application and persistence:

```razor
<ThemeProvider 
    Framework="CssFramework.Bootstrap"
    DefaultTheme="Theme.System"
    PersistPreference="true"
    CustomVariables="--osirion-primary-color: #0077cc;"
    StorageKey="app-theme-preference">
    <!-- Your application content -->
</ThemeProvider>
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Framework` | CssFramework | None | CSS framework to integrate with |
| `DefaultTheme` | Theme | System | Default theme (Light/Dark/System) |
| `PersistPreference` | bool | true | Whether to save user preference |
| `CustomVariables` | string | null | Custom CSS variables |
| `StorageKey` | string | "theme-preference" | Local storage key for preference |

## ThemeToggle Component

The `ThemeToggle` component allows users to switch between themes:

```razor
<ThemeToggle 
    ShowSystemOption="true"
    LightLabel="Light"
    DarkLabel="Dark"
    SystemLabel="System"
    Position="Position.TopRight" />
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowSystemOption` | bool | true | Whether to show system preference option |
| `LightLabel` | string | "Light" | Label for light theme option |
| `DarkLabel` | string | "Dark" | Label for dark theme option |
| `SystemLabel` | string | "System" | Label for system theme option |
| `Position` | Position | None | Position of the toggle (TopRight, TopLeft, etc.) |
| `CssClass` | string | null | Additional CSS classes |

### Custom Icon Templates

```razor
<ThemeToggle>
    <LightIconTemplate>
        <!-- Custom light theme icon -->
    </LightIconTemplate>
    <DarkIconTemplate>
        <!-- Custom dark theme icon -->
    </DarkIconTemplate>
    <SystemIconTemplate>
        <!-- Custom system theme icon -->
    </SystemIconTemplate>
</ThemeToggle>
```

## IThemeService Interface

The `IThemeService` interface allows programmatic theme control:

```csharp
public interface IThemeService
{
    Theme CurrentTheme { get; }
    Theme EffectiveTheme { get; }
    CssFramework Framework { get; }
    event EventHandler<ThemeChangedEventArgs> ThemeChanged;
    
    Task SetThemeAsync(Theme theme);
    Task InitializeAsync();
    Task DetectSystemPreferenceAsync();
    string GetThemeClass();
}
```

### Usage Example

```razor
@inject IThemeService ThemeService
@implements IDisposable

<div class="theme-info">
    Current Theme: @ThemeService.CurrentTheme
    Effective Theme: @ThemeService.EffectiveTheme
</div>

<button @onclick="SetLightTheme">Light</button>
<button @onclick="SetDarkTheme">Dark</button>
<button @onclick="SetSystemTheme">System</button>

@code {
    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task SetLightTheme() => await ThemeService.SetThemeAsync(Theme.Light);
    private async Task SetDarkTheme() => await ThemeService.SetThemeAsync(Theme.Dark);
    private async Task SetSystemTheme() => await ThemeService.SetThemeAsync(Theme.System);

    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
    }
}
```

## Framework Integration Details

Each CSS framework integration adapts Osirion's CSS variables to the framework's native variables:

### Bootstrap Integration

```css
.osirion-bootstrap-integration {
    --bs-primary: var(--osirion-primary-color);
    --bs-border-radius: var(--osirion-border-radius);
    --bs-body-color: var(--osirion-color);
    --bs-body-bg: var(--osirion-color-background);
    /* Additional mappings... */
}
```

### MudBlazor Integration

```css
.osirion-mudblazor-integration {
    --mud-palette-primary: var(--osirion-primary-color);
    --mud-palette-background: var(--osirion-color-background);
    --mud-palette-text-primary: var(--osirion-color);
    /* Additional mappings... */
}
```

## Custom Themes Example

Creating a custom theme:

```razor
@inject IThemeService ThemeService

<div class="theme-selector">
    <button @onclick="() => ApplyTheme('default')">Default</button>
    <button @onclick="() => ApplyTheme('forest')">Forest</button>
    <button @onclick="() => ApplyTheme('ocean')">Ocean</button>
    <button @onclick="() => ApplyTheme('sunset')">Sunset</button>
</div>

@code {
    private Dictionary<string, string> themes = new()
    {
        ["default"] = "",
        ["forest"] = @"
            --osirion-primary-color: #2d6a4f;
            --osirion-primary-color-hover: #1b4332;
        ",
        ["ocean"] = @"
            --osirion-primary-color: #0077b6;
            --osirion-primary-color-hover: #023e8a;
        ",
        ["sunset"] = @"
            --osirion-primary-color: #e76f51;
            --osirion-primary-color-hover: #e63946;
        "
    };

    private async Task ApplyTheme(string themeName)
    {
        if (themes.TryGetValue(themeName, out var variables))
        {
            await JSRuntime.InvokeVoidAsync("eval", $@"
                document.documentElement.style.cssText = `{variables}`;
            ");
        }
    }
}
```

## Server-Side Rendering (SSR) Compatibility

All theming components are designed to be SSR-compatible:

1. The theming system works without JavaScript by using CSS variables
2. Theme toggling degrades gracefully in environments without JavaScript
3. System preference detection works with progressive enhancement

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.