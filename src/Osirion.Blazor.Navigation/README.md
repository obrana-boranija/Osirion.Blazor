# Osirion.Blazor.Navigation

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Navigation)](https://www.nuget.org/packages/Osirion.Blazor.Navigation)
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

### Scroll to Top Button

```razor
<!-- Basic usage -->
<ScrollToTop />

<!-- Customized -->
<ScrollToTop 
    Position="Position.BottomRight" 
    Behavior="ScrollBehavior.Smooth"
    VisibilityThreshold="300" 
    Text="Top" />
```

### Enhanced Navigation

```razor
<!-- Customize scroll behavior -->
<EnhancedNavigation 
    Behavior="ScrollBehavior.Smooth"
    ResetScrollOnNavigation="true"
    PreserveScrollForSamePageNavigation="true" />
```

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

## Documentation

For more detailed documentation, see [Navigation Documentation](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/NAVIGATION.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.