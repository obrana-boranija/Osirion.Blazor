using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Osirion.Blazor.Cms.Infrastructure.Caching;

/// <summary>
/// Shared stale-while-revalidate engine used by the content and directory cache decorators.
/// </summary>
/// <remarks>
/// Every entry written by one store is tied to a single change token, so <see cref="Clear"/> evicts exactly the
/// entries this store created and never touches other entries in the host's shared <see cref="IMemoryCache"/>.
/// Loads are single-flight per key: concurrent readers of the same key share one load, while different keys load
/// in parallel. Loads run on an independent token so one caller cancelling cannot fail the readers sharing the load.
/// </remarks>
internal sealed class StaleWhileRevalidateStore
{
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;
    private readonly TimeSpan _staleTime;
    private readonly TimeSpan _maxAge;
    private readonly ConcurrentDictionary<string, Task> _inFlight = new();
    private readonly object _generationLock = new();
    private CancellationTokenSource _generation = new();

    public StaleWhileRevalidateStore(IMemoryCache cache, ILogger logger, TimeSpan staleTime, TimeSpan maxAge)
    {
        _cache = cache;
        _logger = logger;
        _staleTime = staleTime;
        _maxAge = maxAge;
    }

    /// <summary>Evicts every entry written by this store and forgets loads that are in flight.</summary>
    public void Clear()
    {
        CancellationTokenSource previous;
        lock (_generationLock)
        {
            previous = _generation;
            _generation = new CancellationTokenSource();
        }

        _inFlight.Clear();
        previous.Cancel();
    }

    public async Task<T> GetOrLoadAsync<T>(string cacheKey, Func<CancellationToken, Task<T>> loader, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(cacheKey, out CacheEntry<T>? entry) && entry is not null)
        {
            if (DateTime.UtcNow - entry.LastUpdated > _staleTime)
            {
                StartBackgroundRefresh(cacheKey, loader);
            }

            return entry.Value;
        }

        var load = GetOrStartLoad(cacheKey, loader);
        return await load.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    private Task<T> GetOrStartLoad<T>(string cacheKey, Func<CancellationToken, Task<T>> loader)
    {
        var flightKey = FlightKey<T>(cacheKey);
        if (_inFlight.TryGetValue(flightKey, out var existing))
        {
            return (Task<T>)existing;
        }

        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_inFlight.TryAdd(flightKey, completion.Task))
        {
            return (Task<T>)_inFlight[flightKey];
        }

        _ = RunLoadAsync(cacheKey, flightKey, loader, completion);
        return completion.Task;
    }

    private async Task RunLoadAsync<T>(string cacheKey, string flightKey, Func<CancellationToken, Task<T>> loader, TaskCompletionSource<T> completion)
    {
        try
        {
            // Double check: a load that finished between the caller's miss and our registration already stored a value.
            if (_cache.TryGetValue(cacheKey, out CacheEntry<T>? current) && current is not null)
            {
                completion.TrySetResult(current.Value);
                return;
            }

            var generationToken = CurrentGenerationToken();
            var value = await loader(CancellationToken.None).ConfigureAwait(false);
            Store(cacheKey, value, generationToken);
            _logger.LogDebug("Created cache entry: {CacheKey}", cacheKey);
            completion.TrySetResult(value);
        }
        catch (Exception ex)
        {
            completion.TrySetException(ex);
        }
        finally
        {
            _inFlight.TryRemove(new KeyValuePair<string, Task>(flightKey, completion.Task));
        }
    }

    private void StartBackgroundRefresh<T>(string cacheKey, Func<CancellationToken, Task<T>> loader)
    {
        var flightKey = FlightKey<T>(cacheKey);
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_inFlight.TryAdd(flightKey, completion.Task))
        {
            return;
        }

        // Supervised: the task is observed here, a failure is logged and the stale value stays in the cache.
        _ = Task.Run(async () =>
        {
            try
            {
                var generationToken = CurrentGenerationToken();
                _logger.LogDebug("Refreshing stale cache entry: {CacheKey}", cacheKey);
                var fresh = await loader(CancellationToken.None).ConfigureAwait(false);
                Store(cacheKey, fresh, generationToken);
                completion.TrySetResult(fresh);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh cache entry: {CacheKey}", cacheKey);
                completion.TrySetException(ex);
                _ = completion.Task.Exception;
            }
            finally
            {
                _inFlight.TryRemove(new KeyValuePair<string, Task>(flightKey, completion.Task));
            }
        }, CancellationToken.None);
    }

    private CancellationToken CurrentGenerationToken()
    {
        lock (_generationLock)
        {
            return _generation.Token;
        }
    }

    private void Store<T>(string cacheKey, T value, CancellationToken generationToken)
    {
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_maxAge)
            .SetSize(1)
            .AddExpirationToken(new CancellationChangeToken(generationToken));

        _cache.Set(cacheKey, new CacheEntry<T> { Value = value, LastUpdated = DateTime.UtcNow }, options);
    }

    private static string FlightKey<T>(string cacheKey) => string.Concat(typeof(T).FullName, "|", cacheKey);
}
