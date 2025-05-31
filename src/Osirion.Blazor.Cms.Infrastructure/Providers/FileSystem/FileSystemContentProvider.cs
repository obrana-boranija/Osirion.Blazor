using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Extensions;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;


namespace Osirion.Blazor.Cms.Infrastructure.Providers;

/// <summary>
/// Reads content from the local filesystem (no caching/logging here).
/// Adheres to IReadContentProvider, IDirectoryContentProvider, IQueryContentProvider
/// (ISP) :contentReference[oaicite:3]{index=3}.
/// </summary>
public class FileSystemContentProvider : ContentProviderBase
{
    private readonly FileSystemContentRepository _contentRepository;
    private readonly FileSystemDirectoryRepository _directoryRepository;
    private readonly FileSystemOptions _options;

    public FileSystemContentProvider(
        FileSystemContentRepository contentRepository,
        FileSystemDirectoryRepository directoryRepository,
        IOptions<FileSystemOptions> options,
        IMemoryCache memoryCache,
        ILogger<FileSystemContentProvider> logger)
        : base(memoryCache, options, logger)
    {
        _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        _directoryRepository = directoryRepository ?? throw new ArgumentNullException(nameof(directoryRepository));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public override string ProviderId => _options.ProviderId ?? $"filesystem-{_options.BasePath.GetHashCode():x}";

    public override string DisplayName => $"FileSystem: {_options.BasePath}";

    public override bool IsReadOnly => false;

    public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _contentRepository.GetAllAsync(cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _contentRepository.GetByIdAsync(id, cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        return await _contentRepository.GetByPathAsync(path, cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        return await _contentRepository.GetByUrlAsync(url, cancellationToken);
    }

    public override async Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        return await _contentRepository.FindByQueryAsync(query, cancellationToken);
    }

    public override async Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        return await _directoryRepository.GetByLocaleAsync(locale, cancellationToken);
    }

    public override async Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        return await _directoryRepository.GetByPathAsync(path, cancellationToken);
    }

    public override async Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _directoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public override async Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        return await _directoryRepository.GetByUrlAsync(url, cancellationToken);
    }

    public override async Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        // Get all content items
        var allContent = await GetAllItemsAsync(cancellationToken);

        // Extract and group categories
        return allContent
            .SelectMany(item => item.Categories)
            .GroupBy(category => category.ToLowerInvariant())
            .Select(group => ContentCategory.Create(
                group.First(),
                slug: group.First().GenerateSlug(),
                count: group.Count()))
            .OrderBy(c => c.Name)
            .ToList();
    }

    public override async Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        // Get all content items
        var allContent = await GetAllItemsAsync(cancellationToken);

        // Extract and group tags
        return allContent
            .SelectMany(item => item.Tags)
            .GroupBy(tag => tag.ToLowerInvariant())
            .Select(group => ContentTag.Create(
                group.First(),
                slug: group.First().GenerateSlug(),
                count: group.Count()))
            .OrderBy(t => t.Name)
            .ToList();
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Initializing FileSystem content provider: {BasePath}", _options.BasePath);

        // Ensure base directory exists
        if (!System.IO.Directory.Exists(_options.BasePath))
        {
            if (_options.CreateDirectoriesIfNotExist)
            {
                Logger.LogInformation("Creating base directory: {BasePath}", _options.BasePath);
                System.IO.Directory.CreateDirectory(_options.BasePath);
            }
            else
            {
                Logger.LogWarning("Base directory does not exist: {BasePath}", _options.BasePath);
            }
        }

        // Pre-load some data in the background
        _ = Task.Run(async () => {
            try
            {
                await _contentRepository.RefreshCacheAsync(cancellationToken);
                await _directoryRepository.RefreshCacheAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error pre-loading content for FileSystem provider");
            }
        }, cancellationToken);

        await base.InitializeAsync(cancellationToken);
    }

    public override async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await _contentRepository.RefreshCacheAsync(cancellationToken);
        await _directoryRepository.RefreshCacheAsync(cancellationToken);
        await base.RefreshCacheAsync(cancellationToken);
    }
}
