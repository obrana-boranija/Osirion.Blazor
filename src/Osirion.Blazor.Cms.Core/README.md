# Osirion.Blazor.Cms.Core

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Core)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Core)
[![License](https://img.shields.io/github/license/demonjic/Osirion.Blazor)](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt)

Core content management abstractions and base implementations for the Osirion.Blazor ecosystem. This package provides the foundation for building content providers and content management functionality.

## Features

- **Provider Abstractions**: Base interfaces and classes for content providers
- **Content Model**: Rich content model with metadata support
- **SSR Compatible**: Designed for Server-Side Rendering and Static SSG
- **Multi-Platform**: Supports .NET 9 and .NET 10
- **Content Queries**: Powerful filtering and search capabilities
- **Markdown Processing**: Rendering, sanitizing and front matter extraction through `IMarkdownProcessor`
- **Caching Support**: Efficient content delivery with built-in caching abstractions
- **Components**: Markdown editor and preview, page layouts and a document tree

## Installation

```bash
dotnet add package Osirion.Blazor.Cms.Core
```

## Usage

The Core package is primarily for building content providers. End users should typically use a concrete implementation like `Osirion.Blazor.Cms` instead.

### Creating Custom Providers

Inherit from `ContentProviderBase` to create your own content provider:

```csharp
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Providers;

public class CustomProvider : ContentProviderBase
{
    public CustomProvider(
        IMemoryCache memoryCache,
        IOptions<ContentProviderOptions> options,
        ILogger<CustomProvider> logger)
        : base(memoryCache, options, logger)
    {
    }

    public override string ProviderId => "custom";
    public override string DisplayName => "Custom Provider";
    public override bool IsReadOnly => true;

    public override Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        // Implement your logic to retrieve all content items
        return Task.FromResult<IReadOnlyList<ContentItem>>(new List<ContentItem>());
    }

    public override Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        // Implement your logic to retrieve a specific content item
        return Task.FromResult<ContentItem?>(null);
    }

    public override Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        // Implement your querying logic
        throw new NotImplementedException();
    }

    // Implement the remaining abstract members of ContentProviderBase as well
    // (GetItemByIdAsync, GetItemByUrlAsync, GetCategoriesAsync, GetTagsAsync, the directory methods)
}
```

### Registering Custom Providers

Register your custom provider in `Program.cs`. `AddOsirionContent` comes from the `Osirion.Blazor.Cms.Web` package
(namespace `Osirion.Blazor.Cms.Web.DependencyInjection`):

```csharp
builder.Services.AddOsirionContent(content => {
    content.AddProvider<CustomProvider>();
});
```

### Content Model

`ContentItem` (namespace `Osirion.Blazor.Cms.Domain.Entities`) carries, among others:

```csharp
public class ContentItem
{
    // Content identification
    public string Path { get; set; }
    public string Slug { get; set; }
    public string Url { get; set; }
    public string ContentId { get; set; }
    public string Locale { get; set; }
    public DirectoryItem? Directory { get; set; }

    // Content data
    public string Content { get; set; }
    public string? OriginalMarkdown { get; set; }

    // Common metadata properties
    public string Title { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? LastModified { get; set; }
    public IReadOnlyList<string> Categories { get; }
    public IReadOnlyList<string> Tags { get; }
    public bool IsFeatured { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public ContentStatus Status { get; set; }
    public FrontMatter? Metadata { get; set; }
}
```

### Content Queries

The `ContentQuery` class (namespace `Osirion.Blazor.Cms.Domain.Repositories`) filters and searches content:

```csharp
var query = new ContentQuery
{
    Directory = "blog",
    Category = "tutorials",
    Tag = "blazor",
    Author = "Jane Developer",
    SearchQuery = "blazor components",
    DateFrom = new DateTime(2025, 1, 1),
    DateTo = DateTime.UtcNow,
    Take = 10,
    Skip = 0,
    SortBy = SortField.Date,
    SortDirection = SortDirection.Descending,
    IsFeatured = true
};
```

`SortField` and `SortDirection` are in `Osirion.Blazor.Cms.Domain.Enums`.

## Core Interfaces

### IContentProvider

`IContentProvider` is in `Osirion.Blazor.Cms.Domain.Services`:

```csharp
public interface IContentProvider
{
    string ProviderId { get; }
    string DisplayName { get; }
    bool IsReadOnly { get; }

    Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);
    Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default);
    Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default);
    Task<Dictionary<string, ContentItem>> GetContentTranslationsAsync(string localizationId);
    Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default);
    Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default);
    Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default);
    Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default);
}
```

## Working with Markdown

`IMarkdownProcessor` (namespace `Osirion.Blazor.Cms.Domain.Interfaces`) is registered with the CMS services. It renders
Markdown to HTML (sanitized by default) and splits front matter from the content:

```razor
@using Osirion.Blazor.Cms.Domain.Interfaces
@inject IMarkdownProcessor MarkdownProcessor

@code {
    private void Render(string markdown)
    {
        var html = MarkdownProcessor.RenderToHtml(markdown);
        var (frontMatter, content) = MarkdownProcessor.ExtractFrontMatterAndContent(markdown);
    }
}
```

## Components

The package includes Razor components:

- `MarkdownEditor`, `MarkdownPreview` and `MarkdownEditorPreview` (namespace `Osirion.Blazor.Cms`), a Markdown editor
  with a toolbar and a live preview. `Content` and `ContentChanged` support `@bind-Content`.
- `PageLayout`, `BlogLayout`, `DocumentationLayout` and `TwoColumnLayout`, and `DocumentTree` (namespace
  `Osirion.Blazor.Cms.Core.Components`).

```razor
@using Osirion.Blazor.Cms

<MarkdownEditorPreview @bind-Content="markdown" />
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt) file for details.