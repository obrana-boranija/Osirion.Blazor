# Osirion.Blazor.Cms.Domain

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Domain)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Domain)
[![License](https://img.shields.io/github/license/demonjic/Osirion.Blazor)](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt)

Domain layer for the Osirion.Blazor CMS ecosystem, containing core entity definitions, value objects, enums, and domain services.

## Overview

The Domain project implements the Domain layer in the Clean Architecture pattern, providing the core business entities and logic for the CMS system. This project has minimal dependencies and serves as the foundation for the entire CMS ecosystem.

## Key Components

### Entities

Core business objects defined by their identity. `ContentItem` (namespace `Osirion.Blazor.Cms.Domain.Entities`) derives from `DomainEntity<string>`, which derives from `Entity<TId>`:

```csharp
public class ContentItem : DomainEntity<string>
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    public IReadOnlyList<string> Tags { get; }
    public IReadOnlyList<string> Categories { get; }
    public FrontMatter? Metadata { get; set; }
    // Additional properties
}
```

`ContentItem.Create(id, title, content, path, providerId)` builds a validated item. `DirectoryItem` models a content folder.

### Value Objects

Objects that derive from `ValueObject` and are compared by value. `FrontMatter` (namespace `Osirion.Blazor.Cms.Domain.ValueObjects`) holds the parsed front matter of a content file, and `SeoMetadata` holds search engine metadata. `FrontMatter` is changed through copies:

```csharp
var frontMatter = FrontMatter.Create("My first post")
    .WithAuthor("Jane Doe")
    .WithTags(new[] { "blazor", "cms" })
    .WithFeatured(true);
```

### Enumerations

Strong typing for domain concepts (namespace `Osirion.Blazor.Cms.Domain.Enums`):

```csharp
public enum ContentStatus
{
    Draft,
    Published,
    Scheduled,
    Archived,
    InReview
}

public enum SortField
{
    Date,
    Title,
    Author,
    LastModified,
    Order,
    Created,
    PublishDate,
    Slug,
    ReadTime
}

public enum SortDirection
{
    Ascending,
    Descending
}
```

### Domain Services and Repositories

Contracts that infrastructure implements. `IContentProvider` (namespace `Osirion.Blazor.Cms.Domain.Services`) is the abstraction over a content source such as GitHub or the file system:

```csharp
public interface IContentProvider
{
    string ProviderId { get; }
    string DisplayName { get; }
    bool IsReadOnly { get; }

    Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);
    Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);
    // Additional members for ids, urls, categories, tags and directories
}
```

`IContentRepository` (namespace `Osirion.Blazor.Cms.Domain.Repositories`) adds `GetByPathAsync`, `FindByQueryAsync` and related persistence operations.

## Design Principles

This project follows these design principles:

1. **Persistence Ignorance**: Domain entities know nothing about how they are stored
2. **Rich Domain Model**: Domain entities contain both data and behavior
3. **Value semantics**: Value objects are compared by value and changed through copies
4. **Encapsulation**: Internal state is protected
5. **Domain-Driven Design**: Modeling based on the ubiquitous language of the domain

## Dependencies

The domain layer has minimal dependencies:

- **.NET 9 and .NET 10**: Target frameworks
- **Markdig**: For Markdown processing
- **YamlDotNet**: For front matter parsing
- **Microsoft.AspNetCore.App**: Shared framework reference

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt) file for details.