# Osirion.Blazor.Core

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Core)](https://www.nuget.org/packages/Osirion.Blazor.Core)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Core components and utilities for Osirion.Blazor ecosystem. This package provides the foundation for building SSR-compatible Blazor components.

## Features

- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Zero-JS Dependencies**: Core functionality without JavaScript interop
- **Multi-Platform**: Supports .NET 8 and .NET 9
- **OsirionComponentBase**: Base class for all Osirion components
- **Shared Enums**: Position, ScrollBehavior and other common enumerations

## Installation

```bash
dotnet add package Osirion.Blazor.Core
```

## Usage

Add import to your `_Imports.razor`:

```razor
@using Osirion.Blazor.Components
```

Inherit from `OsirionComponentBase` when creating components:

```csharp
public class MyComponent : OsirionComponentBase
{
    // Your component code here
}
```

## Base Component Features

`OsirionComponentBase` provides:

- Common parameter handling for CSS classes
- Automatic handling of additional attributes
- Environment detection (browser vs server)
- Consistent rendering behavior

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.