# Osirion.Blazor

![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor)
![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)

Modern, high-performance Blazor components and utilities designed to enhance web development productivity. Features include enhanced navigation, state management, and reusable UI components for building robust Blazor applications.

## Features

- **SSR Compatible**: All components work with Server-Side Rendering
- **Zero-JavaScript Option**: Core functionality works without JavaScript
- **Cross-Platform**: Supports all Blazor hosting models (Server, WebAssembly, Static)
- **Future-Proof**: Supports .NET 8, .NET 9, and future versions

## Installation

```bash
dotnet add package Osirion.Blazor
```

## Quick Start

Add the following to your `_Imports.razor`:

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Components.Navigation
```

### Enhanced Navigation

Automatically scroll to the top on page navigation:

```razor
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

## Components

### Navigation

#### EnhancedNavigationInterceptor

Provides enhanced navigation capabilities such as automatic scroll-to-top on navigation.

```razor
<EnhancedNavigationInterceptor 
    Behavior="ScrollBehavior.Smooth"
    Enabled="true" />
```

**Parameters:**

- `Behavior` (ScrollBehavior): Defines the scrolling behavior (Auto, Instant, Smooth)
- `Enabled` (bool): Enable/disable the component

## Browser Compatibility

- Works in all modern browsers (Chrome, Firefox, Safari, Edge)
- Gracefully degrades in older browsers
- Progressive enhancement for better experiences where available

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
