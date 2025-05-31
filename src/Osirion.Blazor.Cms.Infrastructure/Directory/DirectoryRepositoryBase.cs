using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Repositories;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.Directory;

/// <summary>
/// Base class for directory repositories with common functionality
/// </summary>
public abstract class DirectoryRepositoryBase : RepositoryBase<DirectoryItem, string>
{
    protected readonly IDirectoryCacheManager CacheManager;
    protected readonly IDirectoryMetadataProcessor MetadataProcessor;
    protected readonly IPathUtilities PathUtils;
    private readonly ContentProviderOptions _contentOptions;

    protected DirectoryRepositoryBase(
        string providerId,
        IDirectoryCacheManager cacheManager,
        IDirectoryMetadataProcessor metadataProcessor,
        IPathUtilities pathUtils,
        IOptions<ContentProviderOptions> contentOptions,
        ILogger logger)
        : base(providerId, logger)
    {
        CacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        MetadataProcessor = metadataProcessor ?? throw new ArgumentNullException(nameof(metadataProcessor));
        PathUtils = pathUtils ?? throw new ArgumentNullException(nameof(pathUtils));
        _contentOptions = contentOptions.Value;
    }

    /// <summary>
    /// Refreshes the directory cache
    /// </summary>
    public virtual async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Use the cache manager to invalidate and reload
            await CacheManager.InvalidateCacheAsync(cancellationToken);

            // Load directories again to populate the cache
            await GetDirectoryCacheAsync(cancellationToken, true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error refreshing directory cache for provider {ProviderId}", ProviderId);
        }
    }

    /// <summary>
    /// Gets the directory cache, loading it if necessary
    /// </summary>
    protected virtual async Task<Dictionary<string, DirectoryItem>> GetDirectoryCacheAsync(
        CancellationToken cancellationToken = default,
        bool forceRefresh = false)
    {
        return await CacheManager.GetCachedDirectoriesAsync(
            LoadDirectoriesAsync,
            cancellationToken,
            forceRefresh);
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<DirectoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cache = await GetDirectoryCacheAsync(cancellationToken);

            // Return only root directories (no parent)
            return cache.Values
                .Where(d => d.Parent is null)
                .ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting all directories");
            throw new ContentProviderException($"Failed to get all directories: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public override async Task<DirectoryItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        try
        {
            var cache = await GetDirectoryCacheAsync(cancellationToken);

            if (cache.TryGetValue(id, out var directory))
            {
                return directory;
            }

            return null;
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directory by ID", id);
            throw new ContentProviderException($"Failed to get directory by ID: {ex.Message}", ex, ProviderId);
        }
    }

    /// <summary>
    /// Gets directory by path
    /// </summary>
    public virtual async Task<DirectoryItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        try
        {
            var cache = await GetDirectoryCacheAsync(cancellationToken);

            var normalizedPath = PathUtils.NormalizePath(path);
            return cache.Values.FirstOrDefault(d =>
                PathUtils.NormalizePath(d.Path) == normalizedPath);
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directory by path", path);
            throw new ContentProviderException($"Failed to get directory by path: {ex.Message}", ex, ProviderId);
        }
    }

    /// <summary>
    /// Gets directory by URL
    /// </summary>
    public virtual async Task<DirectoryItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        try
        {
            var cache = await GetDirectoryCacheAsync(cancellationToken);

            return cache.Values.FirstOrDefault(d => d.Url == url);
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directory by URL", url);
            throw new ContentProviderException($"Failed to get directory by URL: {ex.Message}", ex, ProviderId);
        }
    }

    /// <summary>
    /// Gets directory by URL
    /// </summary>
    public virtual async Task<DirectoryItem?> GetByNameAsync(string? name, string? locale = default, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        try
        {
            var cache = await GetDirectoryCacheAsync(cancellationToken);

            locale = locale ?? _contentOptions.DefaultLocale;
            return cache.Values.FirstOrDefault(d => d.Name.ToLower() == name.ToLower() && d.Locale.ToLower() == locale.ToLower());
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directory by Name", name);
            throw new ContentProviderException($"Failed to get directory by Name: {ex.Message}", ex, ProviderId);
        }
    }

    /// <summary>
    /// Gets directories by locale
    /// </summary>
    public virtual async Task<IReadOnlyList<DirectoryItem>> GetByLocaleAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var cache = await GetDirectoryCacheAsync(cancellationToken);

            if (cache.Count == 0)
                return new List<DirectoryItem>();

            if (string.IsNullOrWhiteSpace(locale))
            {
                // Return all root directories
                return cache.Values
                    .Where(d => d.Parent is null)
                    .ToList();
            }
            else
            {
                // Return directories for the specified locale
                return cache.Values
                    .Where(d => d.Locale == locale && d.Parent is not null)
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directories by locale");
            throw new ContentProviderException($"Failed to get directories by locale: {ex.Message}", ex, ProviderId);
        }
    }

    /// <summary>
    /// Gets child directories
    /// </summary>
    public virtual async Task<IReadOnlyList<DirectoryItem>> GetChildrenAsync(string parentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(parentId))
            throw new ArgumentException("Parent ID cannot be empty", nameof(parentId));

        try
        {
            var cache = await GetDirectoryCacheAsync(cancellationToken);

            // Find the parent directory
            if (!cache.TryGetValue(parentId, out var parent))
                return new List<DirectoryItem>();

            // Return its children
            return parent.Children.ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting children", parentId);
            throw new ContentProviderException($"Failed to get directory children: {ex.Message}", ex, ProviderId);
        }
    }

    /// <summary>
    /// Gets directory tree for a locale
    /// </summary>
    public virtual async Task<IReadOnlyList<DirectoryItem>> GetTreeAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get directories by locale (root directories only)
            var rootDirectories = await GetByLocaleAsync(locale ?? _contentOptions.DefaultLocale, cancellationToken);

            // Children are already populated in the cache
            return rootDirectories;
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directory tree");
            throw new ContentProviderException($"Failed to get directory tree: {ex.Message}", ex, ProviderId);
        }
    }

    /// <summary>
    /// Loads all directories into the cache
    /// </summary>
    protected abstract Task<Dictionary<string, DirectoryItem>> LoadDirectoriesAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a directory recursively
    /// </summary>
    public virtual async Task DeleteRecursiveAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        LogOperation("deleting", id);

        try
        {
            // Get directory to confirm it exists
            var directory = await GetByIdAsync(id, cancellationToken);
            if (directory is null)
                throw new DirectoryNotFoundException(id);

            // Implementation-specific delete logic
            await DeleteDirectoryInternalAsync(id, true, commitMessage, cancellationToken);

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);
        }
        catch (DirectoryNotFoundException)
        {
            // Re-throw not found exception
            throw;
        }
        catch (Exception ex)
        {
            LogError(ex, "deleting", id);
            throw new ContentProviderException($"Failed to delete directory: {ex.Message}", ex, ProviderId);
        }
    }

    /// <summary>
    /// Implementation-specific directory delete logic
    /// </summary>
    protected abstract Task DeleteDirectoryInternalAsync(
        string id,
        bool recursive,
        string? commitMessage,
        CancellationToken cancellationToken);
}