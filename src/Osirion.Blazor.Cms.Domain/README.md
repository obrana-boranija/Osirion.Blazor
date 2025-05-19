# Osirion.Blazor.Cms.Domain

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Domain)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Domain)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Domain layer for the Osirion.Blazor CMS ecosystem, containing core entity definitions, value objects, enums, and domain services.

## Overview

The Domain project implements the Domain layer in the Clean Architecture pattern, providing the core business entities and logic for the CMS system. This project has minimal dependencies and serves as the foundation for the entire CMS ecosystem.

## Key Components

### Entities

Core business objects defined by their identity:

```csharp
public class ContentItem : Entity<Guid>
{
    public string Path { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, object> Metadata { get; set; } 
        = new Dictionary<string, object>();
    // Additional properties
}
```

### Value Objects

Immutable objects defined by their properties:

```csharp
public class ContentMetadata : ValueObject
{
    public string? Title { get; }
    public string? Description { get; }
    public DateTime? PublishedDate { get; }
    public string? Author { get; }
    public IReadOnlyList<string> Tags { get; } 
    public IReadOnlyList<string> Categories { get; }
    public bool IsFeatured { get; }
    
    // Constructor, Equals, GetHashCode implementations
}
```

### Enumerations

Strong typing for domain concepts:

```csharp
public enum ContentStatus
{
    Draft,
    Published,
    Archived
}

public enum SortField
{
    Title,
    Date,
    Author,
    Path
}

public enum SortDirection
{
    Ascending,
    Descending
}
```

### Domain Services

Core business logic not naturally fitting into entities:

```csharp
public interface IContentService
{
    Task<IReadOnlyList<ContentItem>> GetContentItemsAsync(ContentQuery query);
    Task<ContentItem?> GetContentItemByPathAsync(string path);
    Task<IReadOnlyList<string>> GetAvailableTagsAsync();
    Task<IReadOnlyList<string>> GetAvailableCategoriesAsync();
}
```

### Exceptions

Domain-specific exceptions:

```csharp
public class ContentNotFoundException : Exception
{
    public string ContentPath { get; }
    
    public ContentNotFoundException(string contentPath) 
        : base($"Content not found at path: {contentPath}")
    {
        ContentPath = contentPath;
    }
}
```

## Usage

This library is primarily used by other Osirion.Blazor.Cms modules and is not typically referenced directly by application code. However, you can use the domain types in your application when implementing custom content providers or services:

```csharp
// Example of working with domain types
public class CustomContentService
{
    private readonly IContentRepository _repository;
    
    public CustomContentService(IContentRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IReadOnlyList<ContentItem>> GetFeaturedContentAsync()
    {
        var query = new ContentQuery
        {
            IsFeatured = true,
            SortBy = SortField.Date,
            SortDirection = SortDirection.Descending
        };
        
        return await _repository.QueryContentAsync(query);
    }
}
```

## Design Principles

This project follows these design principles:

1. **Persistence Ignorance**: Domain entities know nothing about how they are stored
2. **Rich Domain Model**: Domain entities contain both data and behavior
3. **Immutability**: Value objects are immutable
4. **Encapsulation**: Internal state is protected
5. **Domain-Driven Design**: Modeling based on the ubiquitous language of the domain

## Dependencies

The domain layer has minimal dependencies:

- **.NET 8.0/.NET 9.0**: Target frameworks
- **Microsoft.Extensions.DependencyInjection**: For dependency injection
- **Microsoft.Extensions.Logging.Abstractions**: For logging abstractions
- **Markdig**: For Markdown processing

## Extension Points

The domain layer provides several extension points for custom implementations:

- **IContentRepository**: Abstract repository for content storage and retrieval
- **IContentQueryHandler**: Interface for implementing custom query handlers
- **IContentEventHandler**: Interface for domain event handlers

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.