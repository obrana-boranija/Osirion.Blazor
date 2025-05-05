# Osirion.Blazor.Navigation

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Navigation)](https://www.nuget.org/packages/Osirion.Blazor.Navigation)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor.Navigation)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Enhanced navigation components for Blazor applications that work seamlessly with SSR.

## Features

- **EnhancedNavigation**: Improves Blazor's navigation experience with scroll restoration
- **ScrollToTop**: Adds a customizable "back to top" button
- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Minimal JavaScript**: Uses progressive enhancement for interactive features
- **Framework Integration**: Works with any CSS framework

## Installation

```bash
dotnet add package Osirion.Blazor.Navigation
```

## Usage

### Quick Start

```csharp
// In Program.cs
using Osirion.Blazor.Navigation.Extensions;

builder.Services.AddOsirionNavigation(navigation => {
    navigation
        .UseEnhancedNavigation()
        .AddScrollToTop();
});
```

```razor
@using Osirion.Blazor.Navigation.Components

<!-- In your layout -->
<EnhancedNavigation Behavior="ScrollBehavior.Smooth" />
<ScrollToTop Position="Position.BottomRight" />
```

### With Configuration (appsettings.json)

```json
{
  "Osirion": {
    "Navigation": {
      "Enhanced": {
        "Behavior": "Smooth",
        "ResetScrollOnNavigation": true,
        "PreserveScrollForSamePageNavigation": true
      },
      "ScrollToTop": {
        "Position": "BottomRight",
        "Behavior": "Smooth",
        "VisibilityThreshold": 300,
        "Text": "Top"
      }
    }
  }
}
```

```csharp
// In Program.cs
builder.Services.AddOsirionNavigation(builder.Configuration);

// Or with the full Osirion.Blazor package:
builder.Services.AddOsirion(builder.Configuration);
```

## Component Documentation

### EnhancedNavigation

The `EnhancedNavigation` component improves Blazor's navigation experience by:

1. Restoring scroll position when navigating between pages
2. Controlling scroll behavior (smooth, instant, auto)
3. Preserving scroll position for same-page navigation

```razor
<EnhancedNavigation 
    Behavior="ScrollBehavior.Smooth"
    ResetScrollOnNavigation="true"
    PreserveScrollForSamePageNavigation="true" />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Behavior` | ScrollBehavior | Auto | Controls the scroll animation behavior |
| `ResetScrollOnNavigation` | bool | true | Whether to reset scroll position when navigating |
| `PreserveScrollForSamePageNavigation` | bool | true | Whether to maintain scroll position for same-page navigation |

### ScrollToTop

The `ScrollToTop` component adds a button that appears when the user scrolls down the page, allowing them to quickly return to the top.

```razor
<ScrollToTop 
    Position="Position.BottomRight" 
    Behavior="ScrollBehavior.Smooth"
    VisibilityThreshold="300" 
    Text="Top" />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Position` | Position | BottomRight | Position of the button on the screen |
| `Behavior` | ScrollBehavior | Smooth | Animation behavior when scrolling |
| `VisibilityThreshold` | int | 300 | Scroll position in pixels when button appears |
| `Text` | string | null | Optional text to display on the button |
| `Title` | string | "Scroll to top" | Tooltip/accessibility title |
| `CssClass` | string | null | Additional CSS classes |
| `CustomIcon` | string | null | Custom SVG icon markup |

### ScrollToTopProvider

The `ScrollToTopProvider` component uses global configuration to render a ScrollToTop button:

```razor
<ScrollToTopProvider />
```

This component takes its configuration from the registered `ScrollToTopManager`, which can be configured through dependency injection.

## Customizing with CSS Variables

Customize the appearance with CSS variables:

```css
:root {
    --osirion-scroll-background: rgba(0, 0, 0, 0.3);
    --osirion-scroll-color: #ffffff;
    --osirion-scroll-size: 40px;
    --osirion-scroll-margin: 20px;
    --osirion-scroll-border-radius: 4px;
}
```

## Server-Side Rendering (SSR) Compatibility

All components are designed to be SSR-compatible:

1. **Progressive Enhancement**: Components work without JavaScript, enhancing functionality when available
2. **Minimal Dependencies**: No external dependencies required
3. **Static Site Generation**: Compatible with .NET 8+ Static Site Generation (SSG)

## Advanced Usage

### Programmatically Scroll Using the Service

You can inject the `INavigationService` to programmatically control scrolling:

```csharp
@inject INavigationService NavigationService

<button @onclick="ScrollToTop">Scroll to Top</button>
<button @onclick="ScrollToSection">Scroll to Section</button>

@code {
    private async Task ScrollToTop()
    {
        await NavigationService.ScrollToTopAsync(ScrollBehavior.Smooth);
    }
    
    private async Task ScrollToSection()
    {
        await NavigationService.ScrollToElementAsync("section-id", ScrollBehavior.Smooth);
    }
}
```

### Controlling ScrollToTop Globally

You can use the `ScrollToTopManager` to control the button globally:

```csharp
@inject ScrollToTopManager Manager

<button @onclick="ToggleButton">Toggle ScrollToTop Button</button>

@code {
    private void ToggleButton()
    {
        Manager.IsEnabled = !Manager.IsEnabled;
    }
}
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.