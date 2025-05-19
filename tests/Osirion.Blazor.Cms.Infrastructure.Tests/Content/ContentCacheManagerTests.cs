using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Infrastructure.Content;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Content;

public class ContentCacheManagerTests
{
    private readonly ILogger<ContentCacheManager> _logger;
    private readonly ContentCacheManager _cacheManager;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

    public ContentCacheManagerTests()
    {
        _logger = Substitute.For<ILogger<ContentCacheManager>>();
        _cacheManager = new ContentCacheManager(_logger, _cacheDuration);
    }

    [Fact]
    public async Task GetCachedItemsAsync_WhenCacheEmpty_CallsLoadFunction()
    {
        // Arrange
        var dictionary = new Dictionary<string, ContentItem>
        {
            { "id1", ContentItem.Create("id1", "Title 1", "Content 1", "path1.md", "test-provider") },
            { "id2", ContentItem.Create("id2", "Title 2", "Content 2", "path2.md", "test-provider") }
        };

        bool loaderCalled = false;
        Func<CancellationToken, Task<Dictionary<string, ContentItem>>> loader =
            (ct) => {
                loaderCalled = true;
                return Task.FromResult(dictionary);
            };

        // Act
        var result = await _cacheManager.GetCachedItemsAsync(loader);

        // Assert
        result.ShouldBeSameAs(dictionary);
        loaderCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task GetCachedItemsAsync_WithCachedItems_ReturnsCachedItems()
    {
        // Arrange
        var dictionary1 = new Dictionary<string, ContentItem>
        {
            { "id1", ContentItem.Create("id1", "Title 1", "Content 1", "path1.md", "test-provider") }
        };

        var dictionary2 = new Dictionary<string, ContentItem>
        {
            { "id2", ContentItem.Create("id2", "Title 2", "Content 2", "path2.md", "test-provider") }
        };

        int loadCount = 0;

        Func<CancellationToken, Task<Dictionary<string, ContentItem>>> loader =
            (ct) => {
                loadCount++;
                return Task.FromResult(loadCount == 1 ? dictionary1 : dictionary2);
            };

        // Act - First call to populate cache
        var result1 = await _cacheManager.GetCachedItemsAsync(loader);

        // Act - Second call should use cache
        var result2 = await _cacheManager.GetCachedItemsAsync(loader);

        // Assert
        result1.ShouldBeSameAs(dictionary1);
        result2.ShouldBeSameAs(dictionary1); // Should be same as first result (cached)
        loadCount.ShouldBe(1); // Load function should only be called once
    }

    [Fact]
    public async Task GetCachedItemsAsync_WithForceRefresh_ReloadsCache()
    {
        // Arrange
        var dictionary1 = new Dictionary<string, ContentItem>
        {
            { "id1", ContentItem.Create("id1", "Title 1", "Content 1", "path1.md", "test-provider") }
        };

        var dictionary2 = new Dictionary<string, ContentItem>
        {
            { "id2", ContentItem.Create("id2", "Title 2", "Content 2", "path2.md", "test-provider") }
        };

        int loadCount = 0;

        Func<CancellationToken, Task<Dictionary<string, ContentItem>>> loader =
            (ct) => {
                loadCount++;
                return Task.FromResult(loadCount == 1 ? dictionary1 : dictionary2);
            };

        // Act - First call to populate cache
        var result1 = await _cacheManager.GetCachedItemsAsync(loader);

        // Act - Second call with force refresh
        var result2 = await _cacheManager.GetCachedItemsAsync(loader, forceRefresh: true);

        // Assert
        result1.ShouldBeSameAs(dictionary1);
        result2.ShouldBeSameAs(dictionary2); // Should be the new result
        loadCount.ShouldBe(2); // Load function should be called twice
    }

    [Fact]
    public async Task InvalidateCacheAsync_ClearsCache()
    {
        // Arrange
        var dictionary1 = new Dictionary<string, ContentItem>
        {
            { "id1", ContentItem.Create("id1", "Title 1", "Content 1", "path1.md", "test-provider") }
        };

        var dictionary2 = new Dictionary<string, ContentItem>
        {
            { "id2", ContentItem.Create("id2", "Title 2", "Content 2", "path2.md", "test-provider") }
        };

        int loadCount = 0;

        Func<CancellationToken, Task<Dictionary<string, ContentItem>>> loader =
            (ct) => {
                loadCount++;
                return Task.FromResult(loadCount == 1 ? dictionary1 : dictionary2);
            };

        // Act - First call to populate cache
        await _cacheManager.GetCachedItemsAsync(loader);

        // Act - Invalidate cache
        await _cacheManager.InvalidateCacheAsync();

        // Act - Second call after invalidation
        var result = await _cacheManager.GetCachedItemsAsync(loader);

        // Assert
        result.ShouldBeSameAs(dictionary2); // Should be the new result
        loadCount.ShouldBe(2); // Load function should be called twice
    }

    [Fact]
    public async Task GetCachedItemsAsync_WhenLoaderThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        Func<CancellationToken, Task<Dictionary<string, ContentItem>>> loader =
            (_) => Task.FromException<Dictionary<string, ContentItem>>(new Exception("Test exception"));

        // Act & Assert
        await Should.ThrowAsync<Exception>(async () =>
            await _cacheManager.GetCachedItemsAsync(loader));

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Error refreshing content cache")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task GetCachedItemsAsync_WhenLoaderThrowsExceptionButCacheExists_ReturnsStaleCache()
    {
        // Arrange
        var dictionary = new Dictionary<string, ContentItem>
        {
            { "id1", ContentItem.Create("id1", "Title 1", "Content 1", "path1.md", "test-provider") }
        };

        int loadCount = 0;

        Func<CancellationToken, Task<Dictionary<string, ContentItem>>> successLoader =
            (ct) => {
                loadCount++;
                return Task.FromResult(dictionary);
            };

        Func<CancellationToken, Task<Dictionary<string, ContentItem>>> failingLoader =
            (_) => Task.FromException<Dictionary<string, ContentItem>>(new Exception("Test exception"));

        // Act - First call to populate cache with successful loader
        var result1 = await _cacheManager.GetCachedItemsAsync(successLoader);

        // Act - Second call with failing loader
        var result2 = await _cacheManager.GetCachedItemsAsync(failingLoader, forceRefresh: true);

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
            Arg.Is<object>(o => o.ToString().Contains("Returning stale content cache")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }
}