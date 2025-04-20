# Osirion.Blazor

![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor)
![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)

Modern, high-performance Blazor components and utilities that work with SSR, Server, and WebAssembly hosting models.

## Features

- 🚀 **SSR Compatible**: Works with Server-Side Rendering
- 🔒 **Zero-JS Dependencies**: Core functionality without JavaScript interop
- 🎯 **Multi-Platform**: Supports .NET 8, .NET 9, and future versions
- 📊 **Analytics Integration**: Microsoft Clarity, Matomo
- 🧭 **Enhanced Navigation**: Improved scrolling behavior
- 📝 **GitHub CMS**: Markdown-based content management
- 🎨 **Customizable Styling**: CSS variables for easy theming

## Installation

```bash
dotnet add package Osirion.Blazor
```

## Getting Started

1. Add to your `_Imports.razor`:
```razor
@using Osirion.Blazor.Components.Navigation
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options
@using Osirion.Blazor.Components.GitHubCms
@using Osirion.Blazor.Services.GitHub
@using Osirion.Blazor.Models.Cms
```

2. Configure services in `Program.cs`:
```csharp
using Osirion.Blazor.Extensions;

// GitHub CMS
builder.Services.AddGitHubCms(options =>
{
    options.Owner = "your-github-username";
    options.Repository = "your-content-repo";
    options.ContentPath = "content";
    options.Branch = "main";
});

// Analytics (optional)
builder.Services.AddClarityTracker(builder.Configuration);
builder.Services.AddMatomoTracker(builder.Configuration);
```

3. Add styles and components to your application:
```html
<!-- In App.razor or _Host.cshtml head section -->
<link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion-cms.css" />
```

```razor
<!-- In your layout -->
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />

<!-- Add GitHub CMS components to your pages -->
<ContentList Directory="blog" />
<CategoriesList />
<TagCloud />
<SearchBox />
```

## Styling Components

Osirion.Blazor components use CSS variables for easy styling customization:

```html
<!-- In App.razor or _Host.cshtml head section -->
<link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion-cms.css" />

<!-- Override variables to match your design -->
<style>
    :root {
        --osirion-primary-color: #0077cc;
        --osirion-border-radius: 0.25rem;
        --osirion-font-size: 1.1rem;
    }
</style>
```

Alternatively, use the OsirionStyles component:

```razor
@using Osirion.Blazor.Components.GitHubCms
<OsirionStyles CustomVariables="--osirion-primary-color: #0077cc;" />
```

## Documentation

- [Navigation Components](./docs/NAVIGATION.md)
- [Analytics Components](./docs/ANALYTICS.md)
- [GitHub CMS Components](./docs/GITHUB_CMS.md)
- [Styling Guide](./docs/STYLING.md)
- [Quick Reference](./docs/QUICK_REFERENCE.md)
- [Migration Guide](./docs/MIGRATION.md)

## License

MIT License - see [LICENSE](LICENSE.txt)

## Contributing

We welcome contributions! Please see our [contributing guidelines](CONTRIBUTING.md) for details.