using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Caching;

public class RepositoryCacheManagerTests
{
    private readonly ILogger _logger;
    private readonly TimeSpan _cacheDuration;
    private readonly string _providerId;
    private readonly RepositoryCacheManager<string, DirectoryItem> _cacheManager;

    public RepositoryCacheManagerTests()
    {
        _logger = Substitute.For<ILogger>();
        _cacheDuration = TimeSpan.FromMinutes(10);
        _providerId = "test-provider";
        _cacheManager = new RepositoryCacheManager<string, DirectoryItem>(_logger, _cacheDuration, _providerId);
    }

    [Fact]
    public async Task GetCachedEntitiesAsync_WhenCacheEmpty_CallsLoadFunction()
    {
        // Arrange
        var dictionary = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider") },
            { "dir2", DirectoryItem.Create("dir2", "path2", "Dir 2", "test-provider") }
        };

        bool loaderCalled = false;
        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> loader =
            (ct) => {
                loaderCalled = true;
                return Task.FromResult(dictionary);
            };

        // Act
        var result = await _cacheManager.GetCachedEntitiesAsync(loader);

        // Assert
        result.ShouldBeSameAs(dictionary);
        loaderCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task GetCachedEntitiesAsync_WithCachedItems_ReturnsCachedItems()
    {
        // Arrange
        var dictionary1 = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider") }
        };

        var dictionary2 = new Dictionary<string, DirectoryItem>
        {
            { "dir2", DirectoryItem.Create("dir2", "path2", "Dir 2", "test-provider") }
        };

        int loadCount = 0;

        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> loader =
            (ct) => {
                loadCount++;
                return Task.FromResult(loadCount == 1 ? dictionary1 : dictionary2);
            };

        // Act - First call to populate cache
        var result1 = await _cacheManager.GetCachedEntitiesAsync(loader);

        // Act - Second call should use cache
        var result2 = await _cacheManager.GetCachedEntitiesAsync(loader);

        // Assert
        result1.ShouldBeSameAs(dictionary1);
        result2.ShouldBeSameAs(dictionary1); // Should be same as first result (cached)
        loadCount.ShouldBe(1); // Load function should only be called once
    }

    [Fact]
    public async Task GetCachedEntitiesAsync_WithForceRefresh_ReloadsCache()
    {
        // Arrange
        var dictionary1 = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider") }
        };

        var dictionary2 = new Dictionary<string, DirectoryItem>
        {
            { "dir2", DirectoryItem.Create("dir2", "path2", "Dir 2", "test-provider") }
        };

        int loadCount = 0;

        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> loader =
            (ct) => {
                loadCount++;
                return Task.FromResult(loadCount == 1 ? dictionary1 : dictionary2);
            };

        // Act - First call to populate cache
        var result1 = await _cacheManager.GetCachedEntitiesAsync(loader);

        // Act - Second call with force refresh
        var result2 = await _cacheManager.GetCachedEntitiesAsync(loader, forceRefresh: true);

        // Assert
        result1.ShouldBeSameAs(dictionary1);
        result2.ShouldBeSameAs(dictionary2); // Should be the new result
        loadCount.ShouldBe(2); // Load function should be called twice
    }

    [Fact]
    public async Task InvalidateCacheAsync_ClearsCache()
    {
        // Arrange
        var dictionary1 = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider") }
        };

        var dictionary2 = new Dictionary<string, DirectoryItem>
        {
            { "dir2", DirectoryItem.Create("dir2", "path2", "Dir 2", "test-provider") }
        };

        int loadCount = 0;

        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> loader =
            (ct) => {
                loadCount++;
                return Task.FromResult(loadCount == 1 ? dictionary1 : dictionary2);
            };

        // Act - First call to populate cache
        await _cacheManager.GetCachedEntitiesAsync(loader);

        // Act - Invalidate cache
        await _cacheManager.InvalidateCacheAsync();

        // Act - Second call after invalidation
        var result = await _cacheManager.GetCachedEntitiesAsync(loader);

        // Assert
        result.ShouldBeSameAs(dictionary2); // Should be the new result
        loadCount.ShouldBe(2); // Load function should be called twice
    }

    [Fact]
    public async Task GetCachedEntitiesAsync_WhenLoaderThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> loader =
            (_) => Task.FromException<Dictionary<string, DirectoryItem>>(new Exception("Test exception"));

        // Act & Assert
        await Should.ThrowAsync<Exception>(async () =>
            await _cacheManager.GetCachedEntitiesAsync(loader));

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Error refreshing cache for")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task GetCachedEntitiesAsync_WhenLoaderThrowsExceptionButCacheExists_ReturnsStaleCache()
    {
        // Arrange
        var dictionary = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider") }
        };

        int loadCount = 0;

        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> successLoader =
            (ct) => {
                loadCount++;
                return Task.FromResult(dictionary);
            };

        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> failingLoader =
            (_) => Task.FromException<Dictionary<string, DirectoryItem>>(new Exception("Test exception"));

        // Act - First call to populate cache with successful loader
        var result1 = await _cacheManager.GetCachedEntitiesAsync(successLoader);

        // Act - Second call with failing loader
        var result2 = await _cacheManager.GetCachedEntitiesAsync(failingLoader, forceRefresh: true);

        // Assert
        result1.ShouldBeSameAs(dictionary);
        result2.ShouldBeSameAs(dictionary); // Should return stale cache
        loadCount.ShouldBe(1); // Success loader called once, failing loader doesn't increment

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());

        _logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Returning stale cache")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }
}