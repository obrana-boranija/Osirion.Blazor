# Osirion.Blazor.Cms.Core

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Core)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Core)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Core content management abstractions and base implementations for the Osirion.Blazor ecosystem. This package provides the foundation for building content providers and content management functionality.

## Features

- **Provider Abstractions**: Base interfaces and classes for content providers
- **Content Model**: Rich content model with metadata support
- **SSR Compatible**: Designed for Server-Side Rendering and Static SSG
- **Multi-Platform**: Supports .NET 8 and .NET 9
- **Content Queries**: Powerful filtering and search capabilities
- **Markdown Processing**: Base Markdown parsing and rendering utilities
- **Caching Support**: Efficient content delivery with built-in caching abstractions

## Installation

```bash
dotnet add package Osirion.Blazor.Cms.Core
```

## Usage

The Core package is primarily for building content providers. End users should typically use a concrete implementation like `Osirion.Blazor.Cms` instead.

### Creating Custom Providers

Inherit from `ContentProviderBase` to create your own content provider:

```csharp
using Osirion.Blazor.Cms.Core;
using Osirion.Blazor.Cms.Core.Models;
using Osirion.Blazor.Cms.Core.Providers;

public class CustomProvider : ContentProviderBase
{
    public CustomProvider(ILogger<CustomProvider> logger, IMemoryCache memoryCache) 
        : base(logger, memoryCache)
    {
    }

    public override string ProviderId => "custom";
    public override string DisplayName => "Custom Provider";
    public override bool IsReadOnly => true;

    public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        // Implement your logic to retrieve all content items
        var items = new List<ContentItem>();
        
        // Add caching if desired
        return await CacheItemsAsync(items, "all_items", cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        // Implement your logic to retrieve a specific content item
        
        // Use caching for better performance
        return await CacheItemAsync(
            async () => await FetchItemByPathAsync(path), 
            $"item_{path}", 
            cancellationToken);
    }

    public override async Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        // Implement querying logic
        var items = await GetAllItemsAsync(cancellationToken);
        
        // Apply query filters
        return ApplyQuery(items, query);
    }
    
    private async Task<ContentItem?> FetchItemByPathAsync(string path)
    {
        // Your custom implementation for fetching content
        return null;
    }
}
```

### Registering Custom Providers

Register your custom provider in `Program.cs`:

```csharp
builder.Services.AddOsirionContent(content => {
    content.AddProvider<CustomProvider>();
});
```

### Content Model

The core content model includes:

```csharp
public class ContentItem
{
    // Content identification
    public string Path { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    
    // Content data
    public string RawContent { get; set; } = string.Empty;
    public string ProcessedContent { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    // Common metadata properties
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string Author { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string Slug { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public string FeaturedImage { get; set; } = string.Empty;
    
    // Provider-specific data
    public string ProviderId { get; set; } = string.Empty;
    public Dictionary<string, object> ProviderMetadata { get; set; } = new();
}
```

### Content Queries

The `ContentQuery` class provides a powerful way to filter and search content:

```csharp
var query = new ContentQuery
{
    Directory = "blog",
    Category = "tutorials",
    Tag = "blazor",
    Author = "Jane Developer",
    SearchQuery = "blazor components",
    FromDate = new DateTime(2025, 1, 1),
    ToDate = DateTime.Now,
    Count = 10,
    Skip = 0,
    SortBy = SortField.Date,
    SortDirection = SortDirection.Descending,
    IsFeatured = true,
    HasFeaturedImage = true
};
```

### Caching Helper Methods

The base class provides helper methods for caching:

```csharp
// Cache a single item
var item = await CacheItemAsync(
    fetchFunction: async () => await GetItemInternalAsync(id),
    cacheKey: $"item_{id}",
    cancellationToken: cancellationToken,
    absoluteExpirationMinutes: 60);

// Cache a collection of items
var items = await CacheItemsAsync(
    items: await FetchAllItemsInternalAsync(),
    cacheKey: "all_items",
    cancellationToken: cancellationToken,
    absoluteExpirationMinutes: 60);
```

## Core Interfaces

### IContentProvider

```csharp
public interface IContentProvider
{
    string ProviderId { get; }
    string DisplayName { get; }
    bool IsReadOnly { get; }
    
    Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);
    Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDirectoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetTagsAsync(CancellationToken cancellationToken = default);
}
```

### IContentTransformer

```csharp
public interface IContentTransformer
{
    Task<string> TransformAsync(string content, ContentItem item);
}
```

### IFrontmatterParser

```csharp
public interface IFrontmatterParser
{
    (string content, Dictionary<string, object> metadata) Parse(string rawContent);
}
```

## Extension Points

### Content Transformers

Create custom content transformers to modify content before rendering:

```csharp
public class ExternalLinkTransformer : IContentTransformer
{
    public Task<string> TransformAsync(string content, ContentItem item)
    {
        // Add target="_blank" to external links
        var transformed = Regex.Replace(
            content,
            @"<a\s+href=""(https?://[^""]*)""",
            @"<a href=""$1"" target=""_blank"" rel=""noopener noreferrer"""
        );
        
        return Task.FromResult(transformed);
    }
}
```

### Frontmatter Parsers

Implement custom frontmatter parsers:

```csharp
public class CustomFrontmatterParser : IFrontmatterParser
{
    public (string content, Dictionary<string, object> metadata) Parse(string rawContent)
    {
        // Your custom frontmatter parsing logic
        return (content, metadata);
    }
}
```

## Working with Markdown

The Core package includes base Markdown processing capabilities:

```csharp
// Parse Markdown with default settings
var processor = new MarkdownProcessor();
var processed = await processor.ProcessAsync(markdownContent);

// Configure Markdown with specific options
services.Configure<MarkdownOptions>(options => {
    options.UseEmphasisExtras = true;
    options.UseSmartyPants = true;
    options.UseTaskLists = true;
});
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.