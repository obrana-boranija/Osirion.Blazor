// README.md (update)
# Osirion.Blazor

![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor)
![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)

Modern, high-performance Blazor components and utilities that work with SSR, Server, and WebAssembly hosting models.

## Features

- 🚀 SSR Compatible (works with Server-Side Rendering)
- 🔒 Zero-JS Dependencies for core functionality
- 🎯 Multi-Platform (.NET 8, .NET 9+)
- 📊 Analytics Integration (Microsoft Clarity, Matomo)
- 🧭 Enhanced Navigation Support
- 📝 GitHub CMS for markdown-based content management

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

// Basic setup
builder.Services.AddOsirionBlazor(); // not needed in this moment

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

3. Add components to your `App.razor` just under `_framework/blazor.web.js`:
```razor
<script src="_framework/blazor.web.js"></script>

<!-- Enhanced navigation -->
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

4. Add components to your layout (`MainLayout.razor` or `App.razor` or `YourPageName.razor`):
```razor

<!-- GitHub CMS components -->
<ContentList Directory="blog" />
<CategoriesList />
<TagCloud />
<SearchBox />
```

## Documentation

- [Navigation Components](./docs/NAVIGATION.md)
- [Analytics Components](./docs/ANALYTICS.md)
- [GitHub CMS Components](./docs/GITHUB_CMS.md)
- [Examples](./examples/)

## License

MIT License - see [LICENSE](LICENSE.txt)

// QUICK_REFERENCE.md (update)
# Quick Reference

## Navigation

### Basic Usage
```razor
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

## Analytics

### Microsoft Clarity
```razor
<ClarityTracker Options="@clarityOptions" />
```

### Matomo
```razor
<MatomoTracker Options="@matomoOptions" />
```

## GitHub CMS

### Configuration
```csharp
builder.Services.AddGitHubCms(options =>
{
    options.Owner = "username";
    options.Repository = "repo";
    options.ContentPath = "content";
});
```

### Content List
```razor
<!-- All content -->
<ContentList />

<!-- Filtered content -->
<ContentList Directory="blog" />
<ContentList Category="tutorials" />
<ContentList Tag="blazor" />
<ContentList FeaturedCount="3" />
```

### Content View
```razor
<ContentView Path="blog/my-post.md" />
```

### Categories and Tags
```razor
<CategoriesList />
<TagCloud MaxTags="20" />
```

### Search
```razor
<SearchBox Placeholder="Search..." />
```

### Directory Navigation
```razor
<DirectoryNavigation CurrentDirectory="@currentDir" />
```