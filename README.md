# Osirion.Blazor

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Modern, high-performance Blazor components and utilities with SSR compatibility, modular design, and framework integration.

## Features

- **Modular Architecture**: Use only what you need with dedicated packages
- **SSR Compatible**: Works with Server-Side Rendering, Static SSG, Interactive Server, and WebAssembly
- **Zero-JS Dependencies**: Core functionality without JavaScript interop
- **Multi-Platform**: Supports .NET 8, .NET 9, and future versions
- **Framework Integration**: Works with Bootstrap, Tailwind, FluentUI, MudBlazor, and Radzen

### Modules

- **[Core](https://www.nuget.org/packages/Osirion.Blazor.Core)**: Foundation components and utilities
- **[Analytics](https://www.nuget.org/packages/Osirion.Blazor.Analytics)**: Clarity and Matomo integration
- **[Navigation](https://www.nuget.org/packages/Osirion.Blazor.Navigation)**: Enhanced navigation and scroll components
- **[Theming](https://www.nuget.org/packages/Osirion.Blazor.Theming)**: Theme management with framework integration
- **[CMS](https://www.nuget.org/packages/Osirion.Blazor.Cms)**: Content management with GitHub and FileSystem providers

## Installation

**Main Package (includes all modules)**

```bash
dotnet add package Osirion.Blazor
```

**Individual Modules**

```bash
dotnet add package Osirion.Blazor.Core
dotnet add package Osirion.Blazor.Analytics
dotnet add package Osirion.Blazor.Navigation
dotnet add package Osirion.Blazor.Theming
dotnet add package Osirion.Blazor.Cms
```

## Quick Start

### Using the Fluent API

```csharp
// In Program.cs
using Osirion.Blazor.Extensions;

builder.Services.AddOsirion(osirion => {
    osirion
        // Configure content providers
        .UseContent(content => {
            content.AddGitHub(options => {
                options.Owner = "username";
                options.Repository = "content-repo";
            });
        })
        
        // Configure analytics
        .UseAnalytics(analytics => {
            analytics.AddClarity(options => {
                options.SiteId = "your-clarity-id";
            });
        })
        
        // Configure navigation
        .UseNavigation(navigation => {
            navigation
                .UseEnhancedNavigation()
                .AddScrollToTop();
        })
        
        // Configure theming
        .UseTheming(theming => {
            theming
                .UseFramework(CssFramework.Bootstrap)
                .EnableDarkMode();
        });
});
```

### Layout Setup

```razor
@using Osirion.Blazor.Navigation.Components
@using Osirion.Blazor.Analytics.Components
@using Osirion.Blazor.Theming.Components

<head>
    <!-- Other head elements -->
    <ThemeProvider>
        <!-- Your styles -->
    </ThemeProvider>
</head>

<body>
    <ClarityTracker />
    <MatomoTracker />
    <EnhancedNavigation />
    <ScrollToTop />
    
    <ThemeToggle />
    
    <main>
        @Body
    </main>
</body>
```

### Content Pages

```razor
@page "/blog"
@using Osirion.Blazor.Cms.Components

<h1>Blog Posts</h1>

<div class="sidebar">
    <CategoriesList />
    <TagCloud />
    <SearchBox />
</div>

<div class="content">
    <ContentList Directory="blog" />
</div>
```

## Configuration from appsettings.json

```json
{
  "Osirion": {
    "Content": {
      "GitHub": {
        "Owner": "username",
        "Repository": "content-repo",
        "ContentPath": "content",
        "Branch": "main"
      }
    },
    "Analytics": {
      "Clarity": {
        "SiteId": "your-clarity-id"
      },
      "Matomo": {
        "SiteId": "1",
        "TrackerUrl": "//analytics.example.com/"
      }
    },
    "Navigation": {
      "ScrollToTop": {
        "Position": "BottomRight",
        "Behavior": "Smooth"
      }
    },
    "Theming": {
      "Framework": "Bootstrap",
      "EnableDarkMode": true,
      "FollowSystemPreference": true
    }
  }
}
```

```csharp
// In Program.cs
builder.Services.AddOsirion(builder.Configuration);
```

### Directory Structure with _index.md

The CMS module supports directory metadata using `_index.md` files:

```
content/
├── en/
│   ├── _index.md
│   ├── blog/
│   │   ├── _index.md
│   │   ├── post1.md
│   │   └── post2.md
│   └── docs/
│       ├── _index.md
│       └── guide.md
└── es/
    ├── _index.md
    └── blog/
        ├── _index.md
        └── post1.md
```

The `_index.md` file can include metadata for the directory:

```markdown
---
id: "blog"               # Unique ID for the directory (same across locales)
title: "Blog Posts"      # Display name (localized)
description: "Our latest articles about Blazor development"
order: 1                 # Controls ordering in navigation
---
```

### Markdown Frontmatter with SEO and Localization

```markdown
---
# Basic metadata
title: "My Blog Post"
author: "John Doe"
date: "2025-04-20"
description: "A brief description of my post"
tags: [blazor, webassembly, dotnet]
categories: [tutorials, web]
slug: "my-blog-post"
is_featured: true
featured_image: "https://example.com/image.jpg"

# Localization
locale: "en"                       # Content locale (e.g., en, es, fr)
localization_id: "my-blog-post"    # Shared ID across all translations

# SEO metadata
meta_title: "Optimized Title for SEO | My Site"    # <title> tag (if different from title)
meta_description: "Longer description optimized for search engines with key phrases..."
canonical_url: "https://mysite.com/blog/my-blog-post"
robots: "index, follow"

# Open Graph metadata
og_title: "My Blog Post - Perfect for Sharing"
og_description: "Description optimized for social sharing..."
og_image: "https://example.com/image-for-social.jpg"
og_type: "article"

# Twitter Card metadata
twitter_card: "summary_large_image"
twitter_title: "My Blog Post - Twitter Title"
twitter_description: "Description optimized for Twitter..."
twitter_image: "https://example.com/twitter-image.jpg"

# Structured Data / Rich Snippets
schema_type: "Article"       # or BlogPosting, TechArticle, etc.
json_ld: |                  # Optional custom JSON-LD
  {
    "@context": "https://schema.org",
    "@type": "Article",
    "headline": "My Blog Post",
    "author": {
      "@type": "Person",
      "name": "John Doe"
    },
    "datePublished": "2025-04-20",
    "image": "https://example.com/image.jpg",
    "publisher": {
      "@type": "Organization",
      "name": "My Company",
      "logo": {
        "@type": "ImageObject",
        "url": "https://example.com/logo.png"
      }
    }
  }
---

# My Blog Post Content

Your markdown content here...

```

## Documentation

- [Navigation Components](./docs/NAVIGATION.md)
- [Analytics Components](./docs/ANALYTICS.md)
- [GitHub CMS Components](./docs/GITHUB_CMS.md)
- [Styling Guide](./docs/STYLING.md)
- [Quick Reference](./docs/QUICK_REFERENCE.md)
- [Migration Guide](./docs/MIGRATION.md)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.

## Contributing

We welcome contributions! Please see our [contributing guidelines](CONTRIBUTING.md) for details.