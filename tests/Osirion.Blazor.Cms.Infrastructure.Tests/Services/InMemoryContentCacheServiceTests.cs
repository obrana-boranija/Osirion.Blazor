using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

#pragma warning disable CS0618 // The options under test are obsolete and keep working until they are removed.

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

/// <summary>
/// The service stores entries through the <c>Set</c> extension method, which cannot be intercepted on a substitute,
/// so the tests run against a real <see cref="MemoryCache"/> and assert on its content.
/// </summary>
public sealed class InMemoryContentCacheServiceTests : IDisposable
{
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
    private readonly InMemoryContentCacheService _cacheService;

    public InMemoryContentCacheServiceTests()
    {
        _cacheService = CreateService(new CacheOptions
        {
            Enabled = true,
            MaxAgeMinutes = 30,
            StaleTimeMinutes = 5,
            CompactionPercentage = 0.25
        });
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
    }

    [Fact]
    public async Task GetAsync_WithCacheHit_ReturnsItem()
    {
        // Arrange
        _memoryCache.Set("test-key", "test-value");

        // Act
        var result = await _cacheService.GetAsync<string>("test-key");

        // Assert
        result.ShouldBe("test-value");
    }

    [Fact]
    public async Task GetAsync_WithCacheMiss_ReturnsNull()
    {
        // Act
        var result = await _cacheService.GetAsync<string>("missing-key");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetOrCreateAsync_WithCacheHit_ReturnsCachedItem()
    {
        // Arrange
        _memoryCache.Set("test-key", "test-value");
        var factoryCalled = false;

        // Act
        var result = await _cacheService.GetOrCreateAsync<string>(
            "test-key",
            _ =>
            {
                factoryCalled = true;
                return Task.FromResult("factory-value");
            });

        // Assert
        result.ShouldBe("test-value");
        factoryCalled.ShouldBeFalse("Factory should not be called on cache hit");
        _memoryCache.Get<string>("test-key").ShouldBe("test-value");
    }

    [Fact]
    public async Task GetOrCreateAsync_WithCacheMiss_CallsFactoryAndCachesResult()
    {
        // Act
        var result = await _cacheService.GetOrCreateAsync<string>("missing-key", _ => Task.FromResult("factory-value"));

        // Assert
        result.ShouldBe("factory-value");
        _memoryCache.TryGetValue("missing-key", out string? cached).ShouldBeTrue();
        cached.ShouldBe("factory-value");
    }

    [Fact]
    public async Task GetOrCreateAsync_WithCacheMiss_StoresEntryWithConfiguredExpirationAndSize()
    {
        // Arrange
        var expiration = TimeSpan.FromMinutes(7);
        var cache = new EntryCapturingMemoryCache(_memoryCache);
        var service = new InMemoryContentCacheService(cache, NullLogger<InMemoryContentCacheService>.Instance, Options.Create(new CacheOptions()));

        // Act
        await service.GetOrCreateAsync<string>("missing-key", _ => Task.FromResult("factory-value"), expiration);

        // Assert
        var entry = cache.CreatedEntries.ShouldHaveSingleItem();
        entry.AbsoluteExpirationRelativeToNow.ShouldBe(expiration);
        entry.Size.ShouldBe(1);
    }

    [Fact]
    public async Task GetOrCreateAsync_WithCacheDisabled_CallsFactoryWithoutCaching()
    {
        // Arrange
        var cacheService = CreateService(new CacheOptions { Enabled = false });

        // Act
        var result = await cacheService.GetOrCreateAsync<string>("test-key", _ => Task.FromResult("factory-value"));

        // Assert
        result.ShouldBe("factory-value");
        _memoryCache.Count.ShouldBe(0);
    }

    [Fact]
    public async Task RemoveAsync_RemovesItemFromCache()
    {
        // Arrange
        _memoryCache.Set("test-key", "test-value");

        // Act
        await _cacheService.RemoveAsync("test-key");

        // Assert
        _memoryCache.TryGetValue("test-key", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task SetAsync_WithEnabledCache_SetsItemInCache()
    {
        // Act
        await _cacheService.SetAsync("test-key", "test-value");

        // Assert
        _memoryCache.TryGetValue("test-key", out string? cached).ShouldBeTrue();
        cached.ShouldBe("test-value");
    }

    [Fact]
    public async Task SetAsync_WithDisabledCache_DoesNotSetItemInCache()
    {
        // Arrange
        var cacheService = CreateService(new CacheOptions { Enabled = false });

        // Act
        await cacheService.SetAsync("test-key", "test-value");

        // Assert
        _memoryCache.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ClearAsync_RemovesOnlyEntriesWrittenByTheService()
    {
        // Arrange
        await _cacheService.SetAsync("first-key", "first-value");
        await _cacheService.SetAsync("second-key", "second-value");
        _memoryCache.Set("host-key", "host-value");

        // Act
        await _cacheService.ClearAsync();

        // Assert
        _memoryCache.TryGetValue("first-key", out _).ShouldBeFalse();
        _memoryCache.TryGetValue("second-key", out _).ShouldBeFalse();
        _memoryCache.TryGetValue("host-key", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task RemoveByPrefixAsync_RemovesOnlyMatchingEntries()
    {
        await _cacheService.SetAsync("content:a", "a");
        await _cacheService.SetAsync("content:b", "b");
        await _cacheService.SetAsync("other:c", "c");
        _memoryCache.Set("content:host", "host");

        await _cacheService.RemoveByPrefixAsync("content:");

        _memoryCache.TryGetValue("content:a", out _).ShouldBeFalse();
        _memoryCache.TryGetValue("content:b", out _).ShouldBeFalse();
        _memoryCache.TryGetValue("other:c", out _).ShouldBeTrue();
        _memoryCache.TryGetValue("content:host", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task RemoveByPrefixAsync_AfterOverwritingAKey_StillRemovesIt()
    {
        await _cacheService.SetAsync("content:a", "one");
        await _cacheService.SetAsync("content:a", "two");
        await Task.Delay(50);

        await _cacheService.RemoveByPrefixAsync("content:");

        _memoryCache.TryGetValue("content:a", out _).ShouldBeFalse();
    }

    private InMemoryContentCacheService CreateService(CacheOptions options) =>
        new(_memoryCache, NullLogger<InMemoryContentCacheService>.Instance, Options.Create(options));

    /// <summary>
    /// Forwards to a real cache and keeps the entries the service created, so their options can be asserted.
    /// </summary>
    private sealed class EntryCapturingMemoryCache(IMemoryCache inner) : IMemoryCache
    {
        public List<ICacheEntry> CreatedEntries { get; } = [];

        public bool TryGetValue(object key, out object? value) => inner.TryGetValue(key, out value);

        public ICacheEntry CreateEntry(object key)
        {
            var entry = inner.CreateEntry(key);
            CreatedEntries.Add(entry);
            return entry;
        }

        public void Remove(object key) => inner.Remove(key);

        public void Dispose()
        {
        }
    }
}
