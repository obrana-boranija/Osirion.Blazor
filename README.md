# Osirion.Blazor

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)
[![Build Status](https://img.shields.io/github/actions/workflow/status/obrana-boranija/Osirion.Blazor/build.yml?branch=master&label=build&logo=github)](https://github.com/obrana-boranija/Osirion.Blazor/actions/workflows/build.yml)[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=obrana-boranija_Osirion.Blazor&metric=alert_status)](https://sonarcloud.io/dashboard?id=obrana-boranija_Osirion.Blazor)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=obrana-boranija_Osirion.Blazor&metric=security_rating)](https://sonarcloud.io/dashboard?id=obrana-boranija_Osirion.Blazor)
[![codecov](https://codecov.io/gh/obrana-boranija/Osirion.Blazor/branch/master/graph/badge.svg)](https://codecov.io/gh/obrana-boranija/Osirion.Blazor)

A modular, high-performance component library for Blazor applications with SSR compatibility and zero-JS dependencies (when possible).

## ✨ Features

Osirion.Blazor is composed of specialized modules that can be used independently or together:

### 📊 Analytics

[![Docs](https://img.shields.io/badge/docs-ANALYTICS.md-blue)](docs/ANALYTICS.md)

- Multiple provider support (Microsoft Clarity, Matomo, GA4, Yandex Metrica)
- SSR compatibility with progressive enhancement
- Privacy-focused with consent management
- Easily extendable with your own providers

### 🧭 Navigation

[![Docs](https://img.shields.io/badge/docs-NAVIGATION.md-blue)](docs/NAVIGATION.md)

- Enhanced navigation with scroll restoration
- Smooth scrolling and "back to top" functionality
- Works without JavaScript through progressive enhancement
- Fully customizable appearance

### 📝 Content Management

[![Docs](https://img.shields.io/badge/docs-GITHUB_CMS.md-blue)](docs/GITHUB_CMS.md)

- GitHub and file system content providers
- Markdown rendering with frontmatter support
- Content organization with categories and tags
- Directory-based navigation and search
- SEO optimization out of the box

### 🎨 Theming

[![Docs](https://img.shields.io/badge/docs-STYLING.md-blue)](docs/STYLING.md)

- Integration with popular CSS frameworks (Bootstrap, FluentUI, MudBlazor, Radzen)
- Dark mode support with system preference detection
- CSS variable-based styling system
- Minimal JavaScript with progressive enhancement

## 📦 Installation

Install the complete package:

```bash
dotnet add package Osirion.Blazor
```

Or just the modules you need:

```bash
dotnet add package Osirion.Blazor.Core
dotnet add package Osirion.Blazor.Analytics
dotnet add package Osirion.Blazor.Navigation
dotnet add package Osirion.Blazor.Cms
dotnet add package Osirion.Blazor.Theming
```

## 🚀 Quick Start

### Service Registration

```csharp
// In Program.cs
using Osirion.Blazor.Extensions;

// Register all services at once
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .AddGitHubCms(options => {
            options.Owner = "username";
            options.Repository = "content-repo";
        })
        .AddScrollToTop()
        .AddClarityTracker(options => {
            options.SiteId = "clarity-id";
        })
        .AddOsirionStyle(CssFramework.Bootstrap);
});

// Or from configuration
builder.Services.AddOsirionBlazor(builder.Configuration);
```

### Component Usage

```razor
@using Osirion.Blazor.Components

<!-- In your layout -->
<OsirionStyles FrameworkIntegration="CssFramework.Bootstrap" />
<EnhancedNavigation Behavior="ScrollBehavior.Smooth" />
<ScrollToTop Position="Position.BottomRight" />
<ClarityTracker />

<!-- In your pages -->
<ContentList Directory="blog" />
<ContentView Path="blog/my-post.md" />
<SearchBox />
<TagCloud />
```

## 📚 Documentation

- [Quick Reference](docs/QUICK_REFERENCE.md) - Quick overview of all components
- [Analytics](docs/ANALYTICS.md) - Analytics integration documentation
- [Navigation](docs/NAVIGATION.md) - Navigation components documentation
- [GitHub CMS](docs/GITHUB_CMS.md) - Content management documentation
- [Styling](docs/STYLING.md) - Theming and styling documentation
- [Migration Guide](docs/MIGRATION.md) - Guide for upgrading between versions

## 🌟 Key Principles

- **SSR First**: All components designed for Server-Side Rendering compatibility
- **Zero-JS Dependencies**: No JavaScript dependencies where possible
- **Progressive Enhancement**: Core functionality works without JavaScript, enhanced with JS when available
- **Framework Integration**: Seamless integration with popular CSS frameworks
- **Multi-Platform**: Supports .NET 8 and .NET 9 (and future versions)
- **Provider Pattern**: Easily extend with your own providers for analytics, content, etc.

## 📋 Requirements

- .NET 8.0 or higher
- Blazor (Server, WebAssembly, or Auto)

## 🧪 Features Coming Soon

- Headless CMS support with Contentful, Sanity, and Strapi providers
- Form validation and submission components
- Authentication integration
- Enhanced SEO components with structured data
- Performance optimizations for large content repositories

## 🤝 Contributing

Contributions are welcome! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.