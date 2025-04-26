# Osirion.Blazor.Theming

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Theming)](https://www.nuget.org/packages/Osirion.Blazor.Theming)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Theming and styling system for Blazor applications with CSS framework integration and dark mode support.

## Features

- **CSS Framework Integration**: Seamless integration with Bootstrap, FluentUI, MudBlazor, and Radzen
- **Dark Mode Support**: Built-in light/dark theme toggle with system preference detection
- **CSS Variables**: Consistent theming across components
- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Minimal JavaScript**: Progressive enhancement for interactive features

## Installation

```bash
dotnet add package Osirion.Blazor.Theming
```

## Usage

### Quick Start

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

```razor
@using Osirion.Blazor.Theming.Components

<!-- In your layout -->
<ThemeProvider>
    <ThemeToggle />
    <!-- Your application content -->
</ThemeProvider>
```

### With Custom Variables

```csharp
builder.Services.AddOsirionTheming(theming => {
    theming
        .UseFramework(CssFramework.Bootstrap)
        .WithCustomVariables("--osirion-primary-color: #0077cc;");
});
```

```razor
<ThemeProvider CustomVariables="--osirion-primary-color: #0077cc;">
    <!-- Your content -->
</ThemeProvider>
```

### Theme Toggle Component

```razor
<!-- Default with light/dark/system options -->
<ThemeToggle />

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
</ThemeToggle>
```

### CSS Variables

Core variables available for customization:

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

.dark-theme {
    --osirion-primary-color: #60a5fa;
    --osirion-color: #e5e7eb;
    --osirion-color-background: #1f2937;
    /* ...other dark theme overrides */
}
```

## Documentation

For more detailed documentation, see [Styling Documentation](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/STYLING.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.