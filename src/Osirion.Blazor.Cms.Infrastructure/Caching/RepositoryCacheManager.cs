using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Cms.Infrastructure.Caching
{
    /// <summary>
    /// Generic cache manager for repository entities
    /// </summary>
    /// <typeparam name="TKey">The type of entity key</typeparam>
    /// <typeparam name="TEntity">The type of entity being cached</typeparam>
    public class RepositoryCacheManager<TKey, TEntity> where TEntity : class where TKey : notnull
    {
        private readonly SemaphoreSlim _cacheLock = new(1, 1);
        private readonly ILogger _logger;
        private readonly TimeSpan _cacheDuration;
        private readonly string _providerIdentifier;

        private Dictionary<TKey, TEntity>? _cache;
        private DateTime _cacheExpiration = DateTime.MinValue;

        public RepositoryCacheManager(
            ILogger logger,
            TimeSpan cacheDuration,
            string providerIdentifier)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheDuration = cacheDuration;
            _providerIdentifier = providerIdentifier ?? throw new ArgumentNullException(nameof(providerIdentifier));
        }

        /// <summary>
        /// Gets cached entities or loads them if needed
        /// </summary>
        public async Task<Dictionary<TKey, TEntity>> GetCachedEntitiesAsync(
            Func<CancellationToken, Task<Dictionary<TKey, TEntity>>> loadFunc,
            CancellationToken cancellationToken = default,
            bool forceRefresh = false)
        {
            if (!forceRefresh && _cache != null && DateTime.UtcNow < _cacheExpiration)
            {
                return _cache;
            }

            await _cacheLock.WaitAsync(cancellationToken);
            try
            {
                // Double-check inside the lock
                if (!forceRefresh && _cache != null && DateTime.UtcNow < _cacheExpiration)
                {
                    return _cache;
                }

                // Load entities using the provided function
                var entities = await loadFunc(cancellationToken);

                // Update cache
                _cache = entities;
                _cacheExpiration = DateTime.UtcNow.Add(_cacheDuration);

                _logger.LogDebug("Cache refreshed for {Provider}, {Count} entities loaded",
                    _providerIdentifier, entities.Count);

                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing cache for {Provider}", _providerIdentifier);

                // If we have an existing cache, return it even if expired
                if (_cache != null)
                {
                    _logger.LogWarning("Returning stale cache after refresh error for {Provider}",
                        _providerIdentifier);
                    return _cache;
                }

                throw;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        /// <summary>
        /// Invalidates the cache
        /// </summary>
        public async Task InvalidateCacheAsync(CancellationToken cancellationToken = default)
        {
            await _cacheLock.WaitAsync(cancellationToken);
            try
            {
                _cache = null;
                _cacheExpiration = DateTime.MinValue;
                _logger.LogInformation("Cache invalidated for {Provider}", _providerIdentifier);
            }
            finally
            {
                _cacheLock.Release();
            }
        }
    }
}