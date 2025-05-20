# Osirion.Blazor.Cms.Application

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Application)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Application)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Application layer for the Osirion.Blazor CMS ecosystem, implementing core business logic, use cases, and application services.

## Overview

The Application project represents the application layer in the Clean Architecture pattern, bridging the domain models with the infrastructure and presentation layers. It contains application services, DTOs, validators, mappers, and command/query handlers.

## Components

### Application Services

Services that orchestrate the application logic:

```csharp
public class ContentApplicationService : IContentApplicationService
{
    private readonly IContentRepository _repository;
    private readonly IMarkdownProcessor _markdownProcessor;
    private readonly ILogger<ContentApplicationService> _logger;

    public ContentApplicationService(
        IContentRepository repository, 
        IMarkdownProcessor markdownProcessor,
        ILogger<ContentApplicationService> logger)
    {
        _repository = repository;
        _markdownProcessor = markdownProcessor;
        _logger = logger;
    }

    public async Task<ContentDto> GetContentByPathAsync(string path)
    {
        var content = await _repository.GetByPathAsync(path);
        if (content == null)
        {
            _logger.LogWarning("Content not found for path: {Path}", path);
            throw new ContentNotFoundException(path);
        }

        var processedContent = await _markdownProcessor.ProcessAsync(content.Content);
        return MapToDto(content, processedContent);
    }

    // Additional methods
}
```

### DTOs (Data Transfer Objects)

Objects used to transfer data between the application and presentation layers:

```csharp
public class ContentDto
{
    public string Path { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ProcessedContent { get; set; } = string.Empty;
    public string RawContent { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime? PublishedDate { get; set; }
    public string? Author { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public bool IsFeatured { get; set; }
    public string? FeaturedImage { get; set; }
}
```

### Validators

Input validation using FluentValidation:

```csharp
public class ContentQueryValidator : AbstractValidator<ContentQueryDto>
{
    public ContentQueryValidator()
    {
        RuleFor(x => x.Path)
            .NotEmpty()
            .When(x => x.Directory == null && x.Tag == null && x.Category == null);
        
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0);
        
        RuleFor(x => x.Count)
            .GreaterThanOrEqualTo(1)
            .When(x => x.Count.HasValue);
    }
}
```

## Key Features

### Markdown Processing

Advanced markdown processing with support for:

- Front matter extraction and parsing
- Syntax highlighting
- Table rendering
- Task lists
- Automatic linking
- Diagrams and charts
- Custom extensions

### Content Caching

Efficient content handling with caching:

```csharp
public async Task<ContentDto> GetContentByPathAsync(string path)
{
    var cacheKey = $"content:{path}";
    
    if (_cache.TryGetValue(cacheKey, out ContentDto? cachedContent))
        return cachedContent!;
        
    var content = await _repository.GetByPathAsync(path);
    // Process and map content
    
    var options = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromMinutes(30))
        .SetAbsoluteExpiration(TimeSpan.FromHours(2));
        
    _cache.Set(cacheKey, result, options);
    
    return result;
}
```

## Dependencies

The application layer has the following dependencies:

- **Osirion.Blazor.Cms.Domain**: For domain entities and interfaces
- **FluentValidation**: For input validation
- **Markdig**: For Markdown processing

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.