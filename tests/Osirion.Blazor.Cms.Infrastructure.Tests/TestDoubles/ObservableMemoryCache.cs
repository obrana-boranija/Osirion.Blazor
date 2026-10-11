using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// A real <see cref="MemoryCache"/> that lets a test wait for a write to a key and make the next lookup of a key miss,
/// so background cache work can be observed without waiting on the clock.
/// </summary>
internal sealed class ObservableMemoryCache : IMemoryCache
{
    private readonly MemoryCache _inner = new(new MemoryCacheOptions());
    private readonly ConcurrentDictionary<object, TaskCompletionSource> _writeWaiters = new();
    private readonly ConcurrentDictionary<object, byte> _forcedMisses = new();

    /// <summary>Returns a task that completes when the next write of <paramref name="key"/> is committed.</summary>
    public Task WaitForWriteAsync(object key) =>
        _writeWaiters.GetOrAdd(key, _ => new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)).Task;

    /// <summary>Makes the next lookup of <paramref name="key"/> report a miss even when the key is cached.</summary>
    public void MissNextLookup(object key) => _forcedMisses[key] = 0;

    public bool TryGetValue(object key, out object? value)
    {
        if (_forcedMisses.TryRemove(key, out _))
        {
            value = null;
            return false;
        }

        return _inner.TryGetValue(key, out value);
    }

    public ICacheEntry CreateEntry(object key) => new ObservedEntry(_inner.CreateEntry(key), this);

    public void Remove(object key) => _inner.Remove(key);

    public void Dispose() => _inner.Dispose();

    private void OnCommitted(object key)
    {
        if (_writeWaiters.TryRemove(key, out var waiter))
        {
            waiter.TrySetResult();
        }
    }

    private sealed class ObservedEntry(ICacheEntry inner, ObservableMemoryCache owner) : ICacheEntry
    {
        public object Key => inner.Key;

        public object? Value
        {
            get => inner.Value;
            set => inner.Value = value;
        }

        public DateTimeOffset? AbsoluteExpiration
        {
            get => inner.AbsoluteExpiration;
            set => inner.AbsoluteExpiration = value;
        }

        public TimeSpan? AbsoluteExpirationRelativeToNow
        {
            get => inner.AbsoluteExpirationRelativeToNow;
            set => inner.AbsoluteExpirationRelativeToNow = value;
        }

        public TimeSpan? SlidingExpiration
        {
            get => inner.SlidingExpiration;
            set => inner.SlidingExpiration = value;
        }

        public IList<IChangeToken> ExpirationTokens => inner.ExpirationTokens;

        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks => inner.PostEvictionCallbacks;

        public CacheItemPriority Priority
        {
            get => inner.Priority;
            set => inner.Priority = value;
        }

        public long? Size
        {
            get => inner.Size;
            set => inner.Size = value;
        }

        public void Dispose()
        {
            inner.Dispose();
            owner.OnCommitted(inner.Key);
        }
    }
}
