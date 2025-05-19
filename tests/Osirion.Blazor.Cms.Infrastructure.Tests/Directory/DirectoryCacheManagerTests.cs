using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Directory;

public class DirectoryCacheManagerTests
{
    private readonly ILogger<DirectoryCacheManager> _logger;
    private readonly DirectoryCacheManager _cacheManager;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public DirectoryCacheManagerTests()
    {
        _logger = Substitute.For<ILogger<DirectoryCacheManager>>();
        _cacheManager = new DirectoryCacheManager(_logger, _cacheDuration);
    }

    [Fact]
    public async Task GetCachedDirectoriesAsync_WithInitialLoad_CallsLoader()
    {
        // Arrange
        var directories = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider") },
            { "dir2", DirectoryItem.Create("dir2", "path2", "Dir 2", "test-provider") }
        };

        var loaderCalled = false;

        // Act
        var result = await _cacheManager.GetCachedDirectoriesAsync(
            ct => {
                loaderCalled = true;
                return Task.FromResult(directories);
            });

        // Assert
        result.ShouldBeSameAs(directories);
        loaderCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task GetCachedDirectoriesAsync_WithCachedData_SkipsLoader()
    {
        // Arrange
        var directories = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider") },
            { "dir2", DirectoryItem.Create("dir2", "path2", "Dir 2", "test-provider") }
        };

        var loadCount = 0;
        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> loader =
            ct => {
                loadCount++;
                return Task.FromResult(directories);
            };

        // First call to populate cache
        await _cacheManager.GetCachedDirectoriesAsync(loader);
        loadCount.ShouldBe(1);

        // Act - Second call should use cache
        var result = await _cacheManager.GetCachedDirectoriesAsync(loader);

        // Assert
        result.ShouldBeSameAs(directories);
        loadCount.ShouldBe(1); // Loader should not be called again
    }

    [Fact]
    public async Task GetCachedDirectoriesAsync_WithForceRefresh_ReloadsCache()
    {
        // Arrange
        var directories1 = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider") }
        };

        var directories2 = new Dictionary<string, DirectoryItem>
        {
            { "dir2", DirectoryItem.Create("dir2", "path2", "Dir 2", "test-provider") }
        };

        var loadCount = 0;

        // First call to populate cache
        await _cacheManager.GetCachedDirectoriesAsync(
            ct => {
                loadCount++;
                return Task.FromResult(directories1);
            });

        // Act - Force refresh
        var result = await _cacheManager.GetCachedDirectoriesAsync(
            ct => {
                loadCount++;
                return Task.FromResult(directories2);
            },
            forceRefresh: true);

        // Assert
        result.ShouldBeSameAs(directories2);
        loadCount.ShouldBe(2); // Loader should be called twice
    }

    [Fact]
    public async Task InvalidateCacheAsync_ClearsCache()
    {
        // Arrange
        var directories1 = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "test-provider") }
        };

        var directories2 = new Dictionary<string, DirectoryItem>
        {
            { "dir2", DirectoryItem.Create("dir2", "path2", "Dir 2", "test-provider") }
        };

        var loadCount = 0;

        // First call to populate cache
        await _cacheManager.GetCachedDirectoriesAsync(
            ct => {
                loadCount++;
                return Task.FromResult(directories1);
            });

        // Invalidate cache
        await _cacheManager.InvalidateCacheAsync();

        // Act - Next call should load fresh data
        var result = await _cacheManager.GetCachedDirectoriesAsync(
            ct => {
                loadCount++;
                return Task.FromResult(directories2);
            });

        // Assert
        result.ShouldBeSameAs(directories2);
        loadCount.ShouldBe(2); // Loader should be called twice
    }
}