# Osirion.Blazor.Cms.Infrastructure

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Infrastructure)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Infrastructure)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Infrastructure layer for the Osirion.Blazor CMS ecosystem, implementing repositories, external service integrations, and infrastructure concerns.

## Overview

The Infrastructure project implements the Infrastructure layer in the Clean Architecture pattern, providing concrete implementations of the interfaces defined in the Domain and Application layers. It includes repositories, API clients, storage implementations, and external service integrations.

## Features

- **GitHub Provider**: Content repository implementation using GitHub API
- **File System Provider**: Local file system content repository
- **Caching Integration**: Memory and distributed cache implementations
- **HTTP Clients**: Typed HTTP clients for external service communication
- **Local Storage**: Browser local storage adapter for client-side persistence
- **Dependency Injection**: Infrastructure service registration extensions

## GitHub Provider

Content provider implementation using the GitHub API via Octokit.NET:

```csharp
public class GitHubContentRepository : IContentRepository
{
    private readonly GitHubClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GitHubContentRepository> _logger;
    private readonly GitHubContentOptions _options;

    public GitHubContentRepository(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        IOptions<GitHubContentOptions> options,
        ILogger<GitHubContentRepository> logger)
    {
        _cache = cache;
        _logger = logger;
        _options = options.Value;
        
        _client = new GitHubClient(new ProductHeaderValue("Osirion.Blazor.Cms"));
        
        if (!string.IsNullOrWhiteSpace(_options.PersonalAccessToken))
        {
            _client.Credentials = new Credentials(_options.PersonalAccessToken);
        }
    }

    public async Task<ContentItem?> GetByPathAsync(string path)
    {
        try
        {
            var cacheKey = $"github:content:{path}";
            
            if (_cache.TryGetValue(cacheKey, out ContentItem? cachedItem))
            {
                return cachedItem;
            }
            
            var contentResponse = await _client.Repository.Content.GetAllContents(
                _options.Owner, 
                _options.Repository,
                Path.Combine(_options.ContentPath, path));
                
            var content = contentResponse.FirstOrDefault();
            
            if (content is null)
            {
                return null;
            }
            
            var decodedContent = content.Encoding == ContentEncoding.Base64
                ? Encoding.UTF8.GetString(Convert.FromBase64String(content.Content))
                : content.Content;
                
            var contentItem = await ProcessContentAsync(path, decodedContent);
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(_options.CacheExpirationMinutes));
                
            _cache.Set(cacheKey, contentItem, cacheOptions);
            
            return contentItem;
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Content not found in GitHub repository: {Path}", path);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching content from GitHub: {Path}", path);
            throw;
        }
    }
    
    // Additional repository methods
}
```

## Configuration Options

Configuration options for the infrastructure providers:

```csharp
public class GitHubContentOptions
{
    public string Owner { get; set; } = string.Empty;
    public string Repository { get; set; } = string.Empty;
    public string Branch { get; set; } = "main";
    public string ContentPath { get; set; } = "content";
    public string? PersonalAccessToken { get; set; }
    public bool UseCache { get; set; } = true;
    public int CacheExpirationMinutes { get; set; } = 60;
}

public class FileSystemContentOptions
{
    public string BasePath { get; set; } = "content";
    public bool WatchForChanges { get; set; } = false;
    public bool UseCache { get; set; } = true;
    public int CacheExpirationMinutes { get; set; } = 60;
}
```

## Dependencies

The infrastructure layer has the following dependencies:

- **Osirion.Blazor.Cms.Domain**: For domain entities and interfaces
- **Osirion.Blazor.Cms.Application**: For application services and interfaces
- **Octokit**: For GitHub API integration
- **Blazored.LocalStorage**: For browser local storage access
- **Microsoft.Extensions.Caching.Memory**: For memory caching
- **Microsoft.Extensions.Http**: For HTTP client factory

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.