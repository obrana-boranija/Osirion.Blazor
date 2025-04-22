# Osirion.Blazor.Cms

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms)](https://www.nuget.org/packages/Osirion.Blazor.Cms)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Content management components for Blazor applications with multiple provider support.

## Features

- **Multiple Content Providers**: GitHub and FileSystem implementations included
- **Provider Pattern**: Easily extend with custom providers
- **Markdown Support**: Built-in Markdown rendering with frontmatter
- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Caching**: Efficient content delivery with built-in caching
- **Content Organization**: Categories, tags, and directory-based navigation
- **Search**: Full-text content search capabilities

## Installation

```bash
dotnet add package Osirion.Blazor.Cms
```

## Usage

### Quick Start

```csharp
// In Program.cs
using Osirion.Blazor.Cms.Extensions;

builder.Services.AddOsirionContent(content => {
    content.AddGitHub(options => {
        options.Owner = "username";
        options.Repository = "content-repo";
        options.ContentPath = "content";
        options.Branch = "main";
    });
});
```

```razor
@using Osirion.Blazor.Cms.Components

<!-- Display a list of content items -->
<ContentList />

<!-- Display a specific content item -->
<ContentView Path="blog/my-post.md" />

<!-- Display categories and tags -->
<CategoriesList />
<TagCloud />

<!-- Add search functionality -->
<SearchBox />

<!-- Add directory navigation -->
<DirectoryNavigation />
```

### File System Provider

```csharp
builder.Services.AddOsirionContent(content => {
    content.AddFileSystem(options => {
        options.BasePath = "wwwroot/content";
        options.WatchForChanges = true;
    });
});
```

### Content Queries

```csharp
@inject IContentProvider ContentProvider

@code {
    private async Task LoadContentAsync()
    {
        // Get all content items
        var allItems = await ContentProvider.GetAllItemsAsync();
        
        // Get content by directory
        var blogPosts = await ContentProvider.GetItemsByQueryAsync(new ContentQuery { 
            Directory = "blog" 
        });
        
        // Get content by category
        var tutorials = await ContentProvider.GetItemsByQueryAsync(new ContentQuery { 
            Category = "tutorials" 
        });
        
        // Get content by tag
        var blazorPosts = await ContentProvider.GetItemsByQueryAsync(new ContentQuery { 
            Tag = "blazor" 
        });
        
        // Search content
        var searchResults = await ContentProvider.GetItemsByQueryAsync(new ContentQuery { 
            SearchQuery = "blazor components" 
        });
        
        // Get featured content
        var featured = await ContentProvider.GetItemsByQueryAsync(new ContentQuery { 
            IsFeatured = true 
        });
        
        // Sort content
        var recentFirst = await ContentProvider.GetItemsByQueryAsync(new ContentQuery { 
            SortBy = SortField.Date,
            SortDirection = SortDirection.Descending
        });
    }
}
```

### Markdown Frontmatter

```markdown
---
title: "My Blog Post"
author: "John Doe"
date: "2025-04-20"
description: "A brief description of my post"
tags: [blazor, webassembly, dotnet]
categories: [tutorials, web]
slug: "my-blog-post"
is_featured: true
featured_image: "https://example.com/image.jpg"
---

# My Blog Post Content

Your markdown content here...
```

## Creating Custom Providers

```csharp
public class CustomProvider : ContentProviderBase
{
    public CustomProvider(ILogger<CustomProvider> logger, IMemoryCache memoryCache) 
        : base(logger, memoryCache)
    {
    }

    public override string ProviderId => "custom";
    public override string DisplayName => "Custom Provider";
    public override bool IsReadOnly => true;

    public override Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        // Your implementation
    }

    public override Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        // Your implementation
    }

    public override Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        // Your implementation
    }
}

// Register the provider
builder.Services.AddOsirionContent(content => {
    content.AddProvider<CustomProvider>();
});
```

## Documentation

For more detailed documentation, see [GitHub CMS Documentation](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/GITHUB_CMS.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.