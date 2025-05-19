using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Caching;

public class StaleWhileRevalidateDirectoryCacheTests
{
    private readonly IDirectoryRepository _decorated;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<StaleWhileRevalidateDirectoryCache> _logger;
    private readonly StaleWhileRevalidateDirectoryCache _decorator;

    public StaleWhileRevalidateDirectoryCacheTests()
    {
        _decorated = Substitute.For<IDirectoryRepository>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Substitute.For<ILogger<StaleWhileRevalidateDirectoryCache>>();

        _decorator = new StaleWhileRevalidateDirectoryCache(
            _decorated,
            _memoryCache,
            _logger,
            TimeSpan.FromMinutes(5),  // Stale time
            TimeSpan.FromMinutes(30), // Max age
            "test-provider");
    }

    [Fact]
    public async Task GetByIdAsync_CachesResult()
    {
        // Arrange
        var directoryId = "test-dir-id";
        var directoryItem = DirectoryItem.Create(
            directoryId,
            "test/path",
            "Test Directory",
            "test-provider");

        _decorated.GetByIdAsync(directoryId, Arg.Any<CancellationToken>())
            .Returns(directoryItem);

        // Act - First call
        var firstResult = await _decorator.GetByIdAsync(directoryId);

        // Set up to return a different result on second call to repository
        // This shouldn't be called because of caching
        _decorated.GetByIdAsync(directoryId, Arg.Any<CancellationToken>())
            .Returns(DirectoryItem.Create(
                directoryId,
                "different/path",
                "Different Directory",
                "test-provider"));

        // Act - Second call
        var secondResult = await _decorator.GetByIdAsync(directoryId);

        // Assert
        firstResult.ShouldBe(directoryItem);
        secondResult.ShouldBe(directoryItem); // Should be the same as first result due to caching

        // Repository should only be called once
        await _decorated.Received(1).GetByIdAsync(directoryId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_CachesResult()
    {
        // Arrange
        var directories = new List<DirectoryItem>
        {
            DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider"),
            DirectoryItem.Create("dir2", "path2", "Dir 2", "test-provider")
        };

        _decorated.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(directories);

        // Act - First call
        var firstResult = await _decorator.GetAllAsync();

        // Change the returned list
        var differentDirs = new List<DirectoryItem>
        {
            DirectoryItem.Create("dir3", "path3", "Dir 3", "test-provider")
        };

        _decorated.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(differentDirs);

        // Act - Second call
        var secondResult = await _decorator.GetAllAsync();

        // Assert
        firstResult.Count.ShouldBe(2);
        secondResult.Count.ShouldBe(2); // Should be the same as first result due to caching

        // Repository should only be called once
        await _decorated.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_InvalidatesCache()
    {
        // Arrange
        var directoryId = "test-dir-id";
        var directoryItem = DirectoryItem.Create(
            directoryId,
            "test/path",
            "Test Directory",
            "test-provider");

        _decorated.GetByIdAsync(directoryId, Arg.Any<CancellationToken>())
            .Returns(directoryItem);

        _decorated.SaveAsync(directoryItem, Arg.Any<CancellationToken>())
            .Returns(directoryItem);

        // First call to populate cache
        await _decorator.GetByIdAsync(directoryId);

        // Act - Save, which should invalidate cache
        await _decorator.SaveAsync(directoryItem);

        // Setup repository to return different directory on next call
        var updatedDir = DirectoryItem.Create(
            directoryId,
            "test/path",
            "Updated Directory",
            "test-provider");

        _decorated.GetByIdAsync(directoryId, Arg.Any<CancellationToken>())
            .Returns(updatedDir);

        // Get directory again
        var resultAfterSave = await _decorator.GetByIdAsync(directoryId);

        // Assert
        resultAfterSave.ShouldBe(updatedDir); // Should get the updated directory since cache was invalidated

        // Repository should be called twice (once before save, once after)
        await _decorated.Received(2).GetByIdAsync(directoryId, Arg.Any<CancellationToken>());
    }
}