using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class InMemoryContentCacheServiceTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InMemoryContentCacheService> _logger;
    private readonly IOptions<CacheOptions> _options;
    private readonly InMemoryContentCacheService _cacheService;

    public InMemoryContentCacheServiceTests()
    {
        // Create mocks
        _memoryCache = Substitute.For<IMemoryCache>();
        _logger = Substitute.For<ILogger<InMemoryContentCacheService>>();

        // Setup options
        var cacheOptions = new CacheOptions
        {
            Enabled = true,
            MaxAgeMinutes = 30,
            StaleTimeMinutes = 5,
            CompactionPercentage = 0.25
        };
        _options = Options.Create(cacheOptions);

        // Create service with mocks
        _cacheService = new InMemoryContentCacheService(_memoryCache, _logger, _options);
    }

    [Fact]
    public async Task GetAsync_WithCacheHit_ReturnsItem()
    {
        // Arrange
        string key = "test-key";
        string value = "test-value";

        object outValue = value;
        _memoryCache.TryGetValue(key, out Arg.Any<object?>()).Returns(x => {
            x[1] = outValue;
            return true;
        });

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.ShouldBe(value);
        _memoryCache.Received(1).TryGetValue(key, out Arg.Any<object?>());
    }

    [Fact]
    public async Task GetAsync_WithCacheMiss_ReturnsNull()
    {
        // Arrange
        string key = "missing-key";
        _memoryCache.TryGetValue(key, out Arg.Any<object?>()).Returns(false);

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.ShouldBeNull();
        _memoryCache.Received(1).TryGetValue(key, out Arg.Any<object?>());
    }

    [Fact]
    public async Task GetOrCreateAsync_WithCacheHit_ReturnsCachedItem()
    {
        // Arrange
        string key = "test-key";
        string value = "test-value";
        bool factoryCalled = false;

        object outValue = value;
        _memoryCache.TryGetValue(key, out Arg.Any<object?>()).Returns(x => {
            x[1] = outValue;
            return true;
        });

        // Act
        var result = await _cacheService.GetOrCreateAsync<string>(
            key,
            async _ => {
                factoryCalled = true;
                return "factory-value";
            });

        // Assert
        result.ShouldBe(value);
        factoryCalled.ShouldBeFalse("Factory should not be called on cache hit");
        _memoryCache.Received(1).TryGetValue(key, out Arg.Any<object?>());
        _memoryCache.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<MemoryCacheEntryOptions>());
    }

    [Fact]
    public async Task GetOrCreateAsync_WithCacheMiss_CallsFactoryAndCachesResult()
    {
        // Arrange
        string key = "missing-key";
        string factoryValue = "factory-value";
        _memoryCache.TryGetValue(key, out Arg.Any<object?>()).Returns(false);

        // Act
        var result = await _cacheService.GetOrCreateAsync<string>(
            key,
            async _ => factoryValue);

        // Assert
        result.ShouldBe(factoryValue);
        _memoryCache.Received(1).TryGetValue(key, out Arg.Any<object?>());
        _memoryCache.Received(1).Set(
            Arg.Is<string>(k => k == key),
            Arg.Is<string>(v => v == factoryValue),
            Arg.Any<MemoryCacheEntryOptions>());
    }

    [Fact]
    public async Task GetOrCreateAsync_WithCacheDisabled_CallsFactoryWithoutCaching()
    {
        // Arrange
        var disabledOptions = new CacheOptions { Enabled = false };
        var options = Options.Create(disabledOptions);
        var cacheService = new InMemoryContentCacheService(_memoryCache, _logger, options);

        string key = "test-key";
        string factoryValue = "factory-value";

        // Act
        var result = await cacheService.GetOrCreateAsync<string>(
            key,
            async _ => factoryValue);

        // Assert
        result.ShouldBe(factoryValue);
        _memoryCache.DidNotReceive().TryGetValue(Arg.Any<string>(), out Arg.Any<object?>());
        _memoryCache.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<MemoryCacheEntryOptions>());
    }

    [Fact]
    public async Task RemoveAsync_RemovesItemFromCache()
    {
        // Arrange
        string key = "test-key";

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert
        _memoryCache.Received(1).Remove(key);
    }

    [Fact]
    public async Task SetAsync_WithEnabledCache_SetsItemInCache()
    {
        // Arrange
        string key = "test-key";
        string value = "test-value";

        // Act
        await _cacheService.SetAsync(key, value);

        // Assert
        _memoryCache.Received(1).Set(
            Arg.Is<string>(k => k == key),
            Arg.Is<string>(v => v == value),
            Arg.Any<MemoryCacheEntryOptions>());
    }

    [Fact]
    public async Task SetAsync_WithDisabledCache_DoesNotSetItemInCache()
    {
        // Arrange
        var disabledOptions = new CacheOptions { Enabled = false };
        var options = Options.Create(disabledOptions);
        var cacheService = new InMemoryContentCacheService(_memoryCache, _logger, options);

        string key = "test-key";
        string value = "test-value";

        // Act
        await cacheService.SetAsync(key, value);

        // Assert
        _memoryCache.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<MemoryCacheEntryOptions>());
    }

    [Fact]
    public async Task ClearAsync_WithMemoryCache_CompactsCache()
    {
        // Arrange
        var memoryCache = Substitute.For<MemoryCache>();
        var cacheService = new InMemoryContentCacheService(memoryCache, _logger, _options);

        // Act
        await cacheService.ClearAsync();

        // Assert
        memoryCache.Received(1).Compact(1.0);
    }
}