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

### Mappers

Conversion between domain entities and DTOs:

```csharp
public static class ContentMappingExtensions
{
    public static ContentDto ToDto(this ContentItem entity, string processedContent)
    {
        return new ContentDto
        {
            Path = entity.Path,
            Title = entity.Title,
            RawContent = entity.Content,
            ProcessedContent = processedContent,
            Metadata = new Dictionary<string, object>(entity.Metadata),
            PublishedDate = entity.PublishedDate,
            Author = entity.Author,
            Tags = entity.Tags.ToList(),
            Categories = entity.Categories.ToList(),
            IsFeatured = entity.IsFeatured,
            FeaturedImage = entity.FeaturedImage
        };
    }

    public static ContentQuery ToDomain(this ContentQueryDto dto)
    {
        return new ContentQuery
        {
            Path = dto.Path,
            Directory = dto.Directory,
            Tag = dto.Tag,
            Category = dto.Category,
            Author = dto.Author,
            SearchQuery = dto.SearchQuery,
            Skip = dto.Skip,
            Count = dto.Count,
            SortBy = dto.SortBy,
            SortDirection = dto.SortDirection,
            IsFeatured = dto.IsFeatured,
            FromDate = dto.FromDate,
            ToDate = dto.ToDate,
            HasFeaturedImage = dto.HasFeaturedImage
        };
    }
}
```

### Markdown Processing

Handling Markdown content with Markdig:

```csharp
public class MarkdownProcessor : IMarkdownProcessor
{
    private readonly MarkdownPipeline _pipeline;

    public MarkdownProcessor(MarkdownPipelineOptions options)
    {
        var builder = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter();
            
        if (options.UseEmphasisExtras)
            builder.UseEmphasisExtras();
            
        if (options.UseTaskLists)
            builder.UseTaskLists();
            
        // Configure additional extensions
            
        _pipeline = builder.Build();
    }

    public Task<string> ProcessAsync(string markdown)
    {
        return Task.FromResult(Markdown.ToHtml(markdown, _pipeline));
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

### Content Metadata Extraction

Parsing and extracting metadata from content:

```csharp
public (string content, Dictionary<string, object> metadata) ParseFrontMatter(string rawContent)
{
    // YAML frontmatter is delimited by --- lines
    var frontMatterMatch = Regex.Match(rawContent, @"^---\r?\n(.+?)\r?\n---\r?\n(.*)$", RegexOptions.Singleline);
    
    if (!frontMatterMatch.Success)
        return (rawContent, new Dictionary<string, object>());
        
    var frontMatterYaml = frontMatterMatch.Groups[1].Value;
    var content = frontMatterMatch.Groups[2].Value;
    
    // Parse YAML to dictionary
    var deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
        
    var metadata = deserializer.Deserialize<Dictionary<string, object>>(frontMatterYaml) 
        ?? new Dictionary<string, object>();
        
    return (content, metadata);
}
```

## Usage

This library is primarily used by other Osirion.Blazor.Cms modules and is not typically referenced directly by application code. However, you can use the application services in your application if you need custom content handling:

```csharp
// In Program.cs
builder.Services.AddScoped<IContentApplicationService, ContentApplicationService>();
builder.Services.AddScoped<IMarkdownProcessor, MarkdownProcessor>();
builder.Services.Configure<MarkdownPipelineOptions>(options => {
    options.UseEmphasisExtras = true;
    options.UseTaskLists = true;
    options.UseSmartyPants = true;
});

// In a Blazor component
@inject IContentApplicationService ContentService

@code {
    private ContentDto? content;
    
    protected override async Task OnInitializedAsync()
    {
        content = await ContentService.GetContentByPathAsync("blog/welcome-post.md");
    }
}
```

## Dependencies

The application layer has the following dependencies:

- **Osirion.Blazor.Cms.Domain**: For domain entities and interfaces
- **FluentValidation**: For input validation
- **Markdig**: For Markdown processing

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.