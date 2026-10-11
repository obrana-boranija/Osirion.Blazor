using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.Caching;

/// <summary>
/// Implements the stale-while-revalidate caching pattern for directory repositories
/// </summary>
public class StaleWhileRevalidateDirectoryCache : IDirectoryRepository
{
    private readonly IDirectoryRepository _decorated;
    private readonly ILogger _logger;
    private readonly TimeSpan _staleTime;
    private readonly TimeSpan _maxAge;
    private readonly StaleWhileRevalidateStore _store;
    private readonly string _providerIdentifier;

    /// <summary>Initializes a directory cache decorator.</summary>
    public StaleWhileRevalidateDirectoryCache(
        IDirectoryRepository decorated,
        IMemoryCache cache,
        ILogger<StaleWhileRevalidateDirectoryCache> logger,
        TimeSpan staleTime,
        TimeSpan maxAge,
        string providerIdentifier)
    {
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        ArgumentNullException.ThrowIfNull(cache);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _providerIdentifier = providerIdentifier ?? throw new ArgumentNullException(nameof(providerIdentifier));
        _staleTime = staleTime;
        _maxAge = maxAge;
        _store = new StaleWhileRevalidateStore(cache, logger, staleTime, maxAge);
    }

    /// <summary>Gets all directories, using the cache when available.</summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"directory:all:{_providerIdentifier}";

        return await _store.GetOrLoadAsync(
            cacheKey,
            token => _decorated.GetAllAsync(token),
            cancellationToken);
    }

    /// <summary>Gets a directory by identifier.</summary>
    public async Task<DirectoryItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        var cacheKey = $"directory:{id}:{_providerIdentifier}";

        return await _store.GetOrLoadAsync(
            cacheKey,
            token => _decorated.GetByIdAsync(id, token),
            cancellationToken);
    }

    /// <summary>Gets a directory by path.</summary>
    public async Task<DirectoryItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        var cacheKey = $"directory:path:{path}:{_providerIdentifier}";

        return await _store.GetOrLoadAsync(
            cacheKey,
            token => _decorated.GetByPathAsync(path, token),
            cancellationToken);
    }

    /// <summary>Gets a directory by URL.</summary>
    public async Task<DirectoryItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        var cacheKey = $"directory:url:{url}:{_providerIdentifier}";

        return await _store.GetOrLoadAsync(
            cacheKey,
            token => _decorated.GetByUrlAsync(url, token),
            cancellationToken);
    }

    /// <summary>Gets directories for a locale.</summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetByLocaleAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var localeKey = locale ?? "all";
        var cacheKey = $"directory:locale:{localeKey}:{_providerIdentifier}";

        return await _store.GetOrLoadAsync(
            cacheKey,
            token => _decorated.GetByLocaleAsync(locale, token),
            cancellationToken);
    }

    /// <summary>Gets the children of a directory.</summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetChildrenAsync(string parentId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"directory:children:{parentId}:{_providerIdentifier}";

        return await _store.GetOrLoadAsync(
            cacheKey,
            token => _decorated.GetChildrenAsync(parentId, token),
            cancellationToken);
    }

    /// <summary>Gets the directory tree for a locale.</summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetTreeAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var localeKey = locale ?? "all";
        var cacheKey = $"directory:tree:{localeKey}:{_providerIdentifier}";

        return await _store.GetOrLoadAsync(
            cacheKey,
            token => _decorated.GetTreeAsync(locale, token),
            cancellationToken);
    }

    /// <summary>Gets a directory by name and locale.</summary>
    public async Task<DirectoryItem?> GetByNameAsync(string? name, string? locale = null, CancellationToken cancellationToken = default)
    {
        var localeKey = locale ?? "all";
        var cacheKey = $"directory:name:{name}:{localeKey}:{_providerIdentifier}";

        return await _store.GetOrLoadAsync(
            cacheKey,
            token => _decorated.GetByNameAsync(name, locale, token),
            cancellationToken);
    }

    // Write operations should invalidate the cache
    /// <summary>Saves a directory and refreshes the cache.</summary>
    public async Task<DirectoryItem> SaveAsync(DirectoryItem entity, CancellationToken cancellationToken = default)
    {
        var result = await _decorated.SaveAsync(entity, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
        return result;
    }

    /// <summary>Deletes a directory and refreshes the cache.</summary>
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _decorated.DeleteAsync(id, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
    }

    /// <summary>Recursively deletes a directory and refreshes the cache.</summary>
    public async Task DeleteRecursiveAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default)
    {
        await _decorated.DeleteRecursiveAsync(id, commitMessage, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
    }

    /// <summary>Moves a directory and refreshes the cache.</summary>
    public async Task<DirectoryItem> MoveAsync(string id, string? newParentId, CancellationToken cancellationToken = default)
    {
        var result = await _decorated.MoveAsync(id, newParentId, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
        return result;
    }

    /// <summary>
    /// Evicts only the entries this decorator created (never the host's shared cache) and refreshes the
    /// underlying repository.
    /// </summary>
    public async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        _store.Clear();
        _logger.LogInformation("Cache cleared for provider: {ProviderId}", _providerIdentifier);

        await _decorated.RefreshCacheAsync(cancellationToken);

        // Readers that loaded from the repository while it was refreshing may have cached the old state.
        _store.Clear();
    }
}
