# Osirion.Blazor

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=obrana-boranija_Osirion.Blazor&metric=security_rating)](https://sonarcloud.io/dashboard?id=obrana-boranija_Osirion.Blazor)
[![codecov](https://codecov.io/gh/obrana-boranija/Osirion.Blazor/branch/master/graph/badge.svg)](https://codecov.io/gh/obrana-boranija/Osirion.Blazor)

A modular, high-performance component library for Blazor applications with SSR compatibility and zero-JS dependencies (when possible).

## ✨ Features

Osirion.Blazor is composed of specialized modules that can be used independently or together:

### 🎯 Core Components

[![Docs](https://img.shields.io/badge/docs-Core_README.md-blue)](src/Osirion.Blazor.Core/README.md)

- **Layout Components**: HeroSection, PageLayout, Footer, Sticky Sidebar
- **Navigation Components**: Breadcrumbs, Article Metadata
- **Content Components**: HTML Renderer, Background Patterns
- **State Components**: Loading indicators, 404 pages
- **Interactive Components**: Cookie Consent, Logo Carousel
- **SSR Compatible**: All components work without JavaScript
- **Framework Integration**: Bootstrap, FluentUI, MudBlazor, Radzen support

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
- Menu components with hierarchical support
- Works without JavaScript through progressive enhancement
- Fully customizable appearance

### 📝 Content Management

[![Docs](https://img.shields.io/badge/docs-GITHUB_CMS.md-blue)](docs/GITHUB_CMS.md)

- GitHub and file system content providers
- Markdown rendering with frontmatter support
- Content organization with categories and tags
- Directory-based navigation and search
- SEO optimization out of the box
- Localization support with multi-language content
- Admin interface for content editing

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

// Register all services at once with fluent API
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

<!-- Hero section for landing pages -->
<HeroSection 
    Title="Welcome to Our Platform"
    Subtitle="Build amazing applications with ease"
    Summary="Experience the power of modern web development."
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/getting-started"
    ImageUrl="/images/hero.jpg"
    UseBackgroundImage="true" />

<!-- Breadcrumb navigation -->
<OsirionBreadcrumbs Path="@Navigation.Uri" />

<!-- Content management -->
<ContentList Directory="blog" />
<ContentView Path="blog/my-post.md" />
<SearchBox />
<TagCloud />

<!-- Cookie consent (GDPR compliant) -->
<OsirionCookieConsent 
    PolicyLink="/privacy-policy"
    ShowCustomizeButton="true" />

<!-- Logo carousel -->
<InfiniteLogoCarousel 
    Title="Our Partners"
    CustomLogos="@partnerLogos" />
```

## 📚 Documentation

- [Quick Reference](docs/QUICK_REFERENCE.md) - Quick overview of all components
- [Core Components](src/Osirion.Blazor.Core/README.md) - Layout, navigation, and content components
- [Analytics](docs/ANALYTICS.md) - Analytics integration documentation
- [Navigation](docs/NAVIGATION.md) - Navigation components documentation
- [GitHub CMS](docs/GITHUB_CMS.md) - Content management documentation
- [Styling](docs/STYLING.md) - Theming and styling documentation
- [Migration Guide](docs/MIGRATION.md) - Guide for upgrading between versions

### Component-Specific Documentation

- [HeroSection](src/Osirion.Blazor.Core/Components/Sections/HeroSection.md) - Comprehensive hero section component
- [OsirionBreadcrumbs](src/Osirion.Blazor.Core/Components/Navigation/OsirionBreadcrumbs.md) - Automatic breadcrumb navigation
- [OsirionCookieConsent](src/Osirion.Blazor.Core/Components/Popups/OsirionCookieConsent.md) - GDPR-compliant cookie consent
- [InfiniteLogoCarousel](src/Osirion.Blazor.Core/Components/InfiniteLogoCarousel.md) - Self-contained logo carousel
- [Markdown Editor](src/Osirion.Blazor.Cms.Core/Components/Editor/MARKDOWN_EDITOR.md) - Admin markdown editor components

## 🌟 Key Principles

- **SSR First**: All components designed for Server-Side Rendering compatibility
- **Zero-JS Dependencies**: No JavaScript dependencies where possible
- **Progressive Enhancement**: Core functionality works without JavaScript, enhanced with JS when available
- **Framework Integration**: Seamless integration with popular CSS frameworks
- **Multi-Platform**: Supports .NET 8 and .NET 9 (and future versions)
- **Provider Pattern**: Easily extend with your own providers for analytics, content, etc.
- **Accessibility**: WCAG 2.1 compliant components with full keyboard and screen reader support
- **GDPR Compliance**: Built-in privacy and consent management features

## 📋 Requirements

- .NET 8.0 or higher
- Blazor (Server, WebAssembly, or Auto)

## 🆕 What's New in v1.5.0

### Major Features

- **Fluent API**: New `AddOsirionBlazor()` method for streamlined service registration
- **CSS Framework Integration**: Automatic integration with Bootstrap, FluentUI, MudBlazor, and Radzen
- **Comprehensive Core Components**: HeroSection, Breadcrumbs, Cookie Consent, and more
- **Enhanced Documentation**: Component-specific documentation with examples
- **Improved Styling**: Renamed `osirion-cms.css` to `osirion.css` for broader scope

### Breaking Changes

- `osirion-cms.css` renamed to `osirion.css`
- Styling options moved from `GitHubCmsOptions` to dedicated `OsirionStyleOptions`
- Service registration methods updated (legacy methods still supported)

See [Migration Guide](docs/MIGRATION.md) for upgrade instructions.

## 🧪 Features Coming Soon

- Headless CMS support with Contentful, Sanity, and Strapi providers
- Form validation and submission components
- Authentication integration
- Enhanced SEO components with structured data
- Performance optimizations for large content repositories
- Visual component editor for admin interface
- Multi-tenant content management

## 🤝 Contributing

Contributions are welcome! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Current Focus Areas

- Additional CSS framework integrations
- Performance optimizations
- Accessibility improvements
- Documentation enhancements
- Test coverage expansion

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.

## 🙏 Acknowledgments

- Built with [Blazor](https://blazor.net/) and [ASP.NET Core](https://asp.net/)
- Markdown processing by [Markdig](https://github.com/xoofx/markdig)
- Testing with [bUnit](https://bunit.dev/) and [Shouldly](https://shouldly.io/)
- Icons and examples use resources from various open-source projects

## 📊 Project Stats

- **Components**: 15+ production-ready components
- **Packages**: 10+ NuGet packages
- **Frameworks**: .NET 8 & .NET 9 support
- **CSS Frameworks**: 4 major frameworks supported
- **Test Coverage**: 85%+ code coverage
- **Documentation**: Comprehensive guides and examples