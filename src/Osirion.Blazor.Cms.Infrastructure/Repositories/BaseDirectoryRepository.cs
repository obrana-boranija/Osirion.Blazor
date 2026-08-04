using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

/// <summary>Provides cached directory repository operations shared by content providers.</summary>
public abstract class BaseDirectoryRepository<TOptions> : IDirectoryRepository where TOptions : class
{
    /// <summary>Gets the logger used by the repository.</summary>
    protected readonly ILogger Logger;
    /// <summary>Gets the provider options.</summary>
    protected readonly TOptions Options;
    /// <summary>Gets the lock protecting the cache.</summary>
    protected readonly SemaphoreSlim CacheLock = new(1, 1);

    // Cache-related fields
    /// <summary>Gets or sets the cached directories.</summary>
    protected Dictionary<string, DirectoryItem>? DirectoryCache;
    /// <summary>Gets or sets the cache expiration time.</summary>
    protected DateTime CacheExpiration = DateTime.MinValue;
    /// <summary>Gets the provider identifier.</summary>
    protected readonly string ProviderId;

    /// <summary>Initializes a repository for a content provider.</summary>
    /// <param name="providerId">The provider identifier.</param>
    /// <param name="options">The provider options.</param>
    /// <param name="logger">The logger.</param>
    protected BaseDirectoryRepository(
        string providerId,
        TOptions options,
        ILogger logger)
    {
        ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>Ensures the directory cache is loaded.</summary>
    protected abstract Task EnsureCacheIsLoaded(CancellationToken cancellationToken, bool forceRefresh = false);
    /// <summary>Loads directories into the supplied cache.</summary>
    protected abstract Task LoadDirectoriesIntoCacheAsync(Dictionary<string, DirectoryItem> cache, CancellationToken cancellationToken);
    /// <summary>Saves a directory using the provider implementation.</summary>
    protected abstract Task<DirectoryItem> SaveDirectoryInternalAsync(DirectoryItem entity, CancellationToken cancellationToken);
    /// <summary>Deletes a directory using the provider implementation.</summary>
    protected abstract Task DeleteDirectoryInternalAsync(string id, bool recursive, string? commitMessage, CancellationToken cancellationToken);

    /// <summary>Gets root directories.</summary>
    /// <inheritdoc />
    public async Task<IReadOnlyList<DirectoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (DirectoryCache is null)
                return new List<DirectoryItem>();

            // Return only root directories (no parent)
            return DirectoryCache.Values
                .Where(d => d.Parent is null)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all directories");
            throw new ContentProviderException("Failed to get all directories", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Gets a directory by identifier.</summary>
    public async Task<DirectoryItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Directory ID cannot be empty", nameof(id));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (DirectoryCache is not null && DirectoryCache.TryGetValue(id, out var directory))
            {
                return directory;
            }

            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting directory by ID {Id}", id);
            throw new ContentProviderException($"Failed to get directory by ID: {id}", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Gets a directory by path.</summary>
    public async Task<DirectoryItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Directory path cannot be empty", nameof(path));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (DirectoryCache is null)
                return null;

            var normalizedPath = NormalizePath(path);
            return DirectoryCache.Values.FirstOrDefault(d => NormalizePath(d.Path) == normalizedPath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting directory by path {Path}", path);
            throw new ContentProviderException($"Failed to get directory by path: {path}", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Gets directories for a locale.</summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetByLocaleAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (DirectoryCache is null)
                return new List<DirectoryItem>();

            if (string.IsNullOrWhiteSpace(locale))
            {
                // Return all root directories
                return DirectoryCache.Values
                    .Where(d => d.Parent is null)
                    .ToList();
            }
            else
            {
                // Return directories for the specified locale
                return DirectoryCache.Values
                    .Where(d => d.Locale == locale && d.Parent is null)
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting directories by locale {Locale}", locale ?? "all");
            throw new ContentProviderException($"Failed to get directories by locale", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Gets the direct children of a directory.</summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetChildrenAsync(string parentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(parentId))
            throw new ArgumentException("Parent ID cannot be empty", nameof(parentId));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (DirectoryCache is null)
                return new List<DirectoryItem>();

            // Find the parent directory
            if (!DirectoryCache.TryGetValue(parentId, out var parent))
                return new List<DirectoryItem>();

            // Return its children
            return parent.Children.ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting children for directory {ParentId}", parentId);
            throw new ContentProviderException($"Failed to get directory children", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Deletes a directory and its descendants.</summary>
    public async Task DeleteRecursiveAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Directory ID cannot be empty", nameof(id));

        try
        {
            // Get directory to ensure it exists
            var directory = await GetByIdAsync(id, cancellationToken);
            if (directory is null)
                throw new DirectoryNotFoundException(id);

            // Delete directory and all its contents
            await DeleteDirectoryInternalAsync(id, true, commitMessage, cancellationToken);

            // Refresh the cache
            await RefreshCacheAsync(cancellationToken);

            // Raise domain event
            directory.RaiseDeletedEvent(true);
        }
        catch (DirectoryNotFoundException)
        {
            // Re-throw directory not found exceptions
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting directory {Id} recursively", id);
            throw new ContentProviderException($"Failed to delete directory recursively", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Gets a directory by URL.</summary>
    public async Task<DirectoryItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (DirectoryCache is null)
                return null;

            return DirectoryCache.Values.FirstOrDefault(d => d.Url == url);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting directory by URL {Url}", url);
            throw new ContentProviderException($"Failed to get directory by URL: {url}", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Gets a directory by name and optional locale.</summary>
    public async Task<DirectoryItem?> GetByNameAsync(string? name, string? locale = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (DirectoryCache is null)
                return null;

            return DirectoryCache.Values.FirstOrDefault(d => d.Name == name && (locale is not null ? d.Locale == locale : true));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "getting directory by Name {Name}", name);
            throw new ContentProviderException($"Failed to get directory by Name: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Moves a directory to a new parent.</summary>
    public async Task<DirectoryItem> MoveAsync(string id, string? newParentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Directory ID cannot be empty", nameof(id));

        try
        {
            // Get directory to move
            var directory = await GetByIdAsync(id, cancellationToken);
            if (directory is null)
                throw new DirectoryNotFoundException(id);

            // Get new parent directory if specified
            DirectoryItem? newParent = null;
            if (!string.IsNullOrWhiteSpace(newParentId))
            {
                newParent = await GetByIdAsync(newParentId, cancellationToken);
                if (newParent is null)
                    throw new DirectoryNotFoundException(newParentId);

                // Check for circular reference
                if (directory.IsAncestorOf(newParent))
                    throw new ContentValidationException("Parent", "Cannot move a directory to one of its descendants");
            }

            // Create a clone of the directory with updated parent and path
            var updatedDirectory = directory.Clone();
            updatedDirectory.SetParent(newParent);

            // Calculate new path based on parent
            string newPath;
            if (newParent is null)
            {
                // Moving to root
                newPath = Path.GetFileName(directory.Path);
            }
            else
            {
                // Moving to a new parent
                newPath = Path.Combine(newParent.Path, Path.GetFileName(directory.Path)).Replace('\\', '/');
            }
            updatedDirectory.SetPath(newPath);

            // Save the updated directory
            var result = await SaveDirectoryInternalAsync(updatedDirectory, cancellationToken);

            // Refresh the cache
            await RefreshCacheAsync(cancellationToken);

            // Raise domain event
            updatedDirectory.RaiseUpdatedEvent();

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error moving directory {Id} to parent {NewParentId}", id, newParentId ?? "root");
            throw new ContentProviderException($"Failed to move directory", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Gets the directory tree for an optional locale.</summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetTreeAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get directories by locale (root directories only)
            var rootDirectories = await GetByLocaleAsync(locale, cancellationToken);

            // Children are already populated in the cache
            return rootDirectories;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting directory tree for locale {Locale}", locale ?? "all");
            throw new ContentProviderException($"Failed to get directory tree", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Refreshes the directory cache.</summary>
    public async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await CacheLock.WaitAsync(cancellationToken);
        try
        {
            DirectoryCache = null;
            CacheExpiration = DateTime.MinValue;

            // Force reload
            await EnsureCacheIsLoaded(cancellationToken, forceRefresh: true);
        }
        finally
        {
            CacheLock.Release();
        }
    }

    /// <inheritdoc />
    /// <summary>Saves a directory.</summary>
    public async Task<DirectoryItem> SaveAsync(DirectoryItem entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            // Call the internal implementation
            var result = await SaveDirectoryInternalAsync(entity, cancellationToken);

            // Refresh the cache
            await RefreshCacheAsync(cancellationToken);

            // Raise the appropriate domain event
            if (string.IsNullOrWhiteSpace(entity.ProviderSpecificId))
            {
                // This was a new directory
                result.RaiseCreatedEvent();
            }
            else
            {
                // This was an update
                result.RaiseUpdatedEvent();
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving directory {Id}", entity.Id);
            throw new ContentProviderException($"Failed to save directory", ex, ProviderId);
        }
    }

    /// <inheritdoc />
    /// <summary>Deletes a directory.</summary>
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Directory ID cannot be empty", nameof(id));

        try
        {
            // Get directory to ensure it exists
            var directory = await GetByIdAsync(id, cancellationToken);
            if (directory is null)
                throw new DirectoryNotFoundException(id);

            // Delete directory (non-recursive)
            await DeleteDirectoryInternalAsync(id, false, null, cancellationToken);

            // Refresh the cache
            await RefreshCacheAsync(cancellationToken);

            // Raise domain event
            directory.RaiseDeletedEvent(false);
        }
        catch (DirectoryNotFoundException)
        {
            // Re-throw directory not found exceptions
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting directory {Id}", id);
            throw new ContentProviderException($"Failed to delete directory", ex, ProviderId);
        }
    }

    /// <summary>Normalizes a directory path for cache lookups.</summary>
    protected string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim('/');
    }
}
