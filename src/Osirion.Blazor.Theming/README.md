# Osirion.Blazor.Theming

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Theming)](https://www.nuget.org/packages/Osirion.Blazor.Theming)
[![License](https://img.shields.io/github/license/demonjic/Osirion.Blazor)](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt)

Theming and styling system for Blazor applications with CSS framework integration and dark mode support.

## Features

- **CSS Framework Integration**: Seamless integration with Bootstrap, FluentUI, MudBlazor, and Radzen
- **Dark Mode Support**: Built-in light/dark theme toggle with system preference detection
- **Palettes**: Osirion, Commit, Editor, Studio, Studio Blue and Contrast, each with a light and a dark variant that
  meets WCAG 2.2 AA (Contrast: AAA), applied before the first paint; code highlighting follows the palette.
  `ThemePicker` lets visitors choose one
- **Palette API**: `ThemingOptions.DefaultPalette`, `UsePalette`, `IThemeService.CurrentPalette` and `SetPalette`; the visitor's choice is kept in the `osirion-preferred-palette` cookie
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
using Osirion.Blazor.Components;
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

<!-- In the head of your layout or App.razor -->
<OsirionStyles />

<!-- Where the visitor should find the toggle -->
<ThemeToggle />
```

### With Custom Variables

```csharp
builder.Services.AddOsirionTheming(theming => {
    theming
        .UseFramework(CssFramework.Bootstrap)
        .WithCustomVariables("--osirion-action-primary: #0077cc;");
});
```

```razor
<OsirionStyles CustomVariables="--osirion-action-primary: #0077cc;" />
```

### Theme Toggle Component

A button that switches between the light and the dark theme. `Id` sets the button's id and `OnThemeChanged` receives the new theme.

```razor
<ThemeToggle />
```

### Palettes

```csharp
builder.Services.AddOsirionTheming(theming => {
    theming
        .UsePalette(ThemePalette.Studio)
        .AllowPaletteSelection();
});
```

```razor
<ThemePicker Label="Palette" FrameworkLabel="Default" />
```

`Palettes` limits the choices to the palettes you list. `ThemePalette.Framework`, the default, applies no palette.

### CSS Variables

Some of the variables available for customization:

```css
:root {
    --osirion-action-primary: #0077cc;
    --osirion-action-primary-hover: #005fa3;
    --osirion-action-primary-text: #ffffff;
    --osirion-text-primary: #1f2937;
    --osirion-background-primary: #ffffff;
    --osirion-border-color: #d1d5db;
}
```

The dark theme redefines the same variables under `[data-theme="dark"]` and `.dark-theme`. See the
[theming guide](https://github.com/demonjic/Osirion.Blazor/blob/master/THEMING.md) for the full list.

## Documentation

For more detailed documentation, see [Styling Documentation](https://github.com/demonjic/Osirion.Blazor/blob/master/docs/STYLING.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt) file for details.