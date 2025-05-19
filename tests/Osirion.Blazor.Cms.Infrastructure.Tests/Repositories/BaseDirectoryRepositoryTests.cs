using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Shouldly;
using System.Reflection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Repositories;

public class BaseDirectoryRepositoryTests
{
    private readonly TestDirectoryRepository _repository;
    private readonly ILogger<TestDirectoryRepository> _logger;
    private readonly string _providerId = "test-provider";

    public BaseDirectoryRepositoryTests()
    {
        _logger = Substitute.For<ILogger<TestDirectoryRepository>>();
        _repository = new TestDirectoryRepository(_providerId, _logger);
    }

    [Fact]
    public async Task GetAllAsync_WithNoCache_LoadsFromSource()
    {
        // Arrange
        var items = CreateTestDirectories();
        _repository.SetupRepositoryItems(items);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldContain(d => d.Id == "dir1");
        result.ShouldContain(d => d.Id == "dir2");
        _repository.CacheRefreshCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsDirectory()
    {
        // Arrange
        var items = CreateTestDirectories();
        _repository.SetupRepositoryItems(items);

        // Act
        var result = await _repository.GetByIdAsync("dir1");

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("dir1");
        result.Name.ShouldBe("Directory 1");
        _repository.CacheRefreshCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var items = CreateTestDirectories();
        _repository.SetupRepositoryItems(items);

        // Act
        var result = await _repository.GetByIdAsync("non-existent");

        // Assert
        result.ShouldBeNull();
        _repository.CacheRefreshCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetByPathAsync_WithExistingPath_ReturnsDirectory()
    {
        // Arrange
        var items = CreateTestDirectories();
        _repository.SetupRepositoryItems(items);

        // Act
        var result = await _repository.GetByPathAsync("path1");

        // Assert
        result.ShouldNotBeNull();
        result.Path.ShouldBe("path1");
        result.Name.ShouldBe("Directory 1");
        _repository.CacheRefreshCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetByLocaleAsync_WithNoLocale_ReturnsAllRootDirectories()
    {
        // Arrange
        var items = CreateTestDirectories();
        _repository.SetupRepositoryItems(items);

        // Act
        var result = await _repository.GetByLocaleAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        _repository.CacheRefreshCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetByLocaleAsync_WithSpecificLocale_ReturnsFilteredDirectories()
    {
        // Arrange
        var items = CreateTestDirectories();
        items.Values.First().SetLocale("en");
        items.Values.Last().SetLocale("fr");
        _repository.SetupRepositoryItems(items);

        // Act
        var result = await _repository.GetByLocaleAsync("en");

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Locale.ShouldBe("en");
        _repository.CacheRefreshCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetChildrenAsync_WithExistingParent_ReturnsChildren()
    {
        // Arrange
        var items = CreateTestDirectories();
        var parent = items["dir1"];
        var child = DirectoryItem.Create("child1", "path1/child1", "Child 1", _providerId);
        parent.AddChild(child);
        items.Add("child1", child);
        _repository.SetupRepositoryItems(items);

        // Act
        var result = await _repository.GetChildrenAsync("dir1");

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe("child1");
        _repository.CacheRefreshCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetTreeAsync_ReturnsDirectoryTree()
    {
        // Arrange
        var items = CreateTestDirectories();
        var parent = items["dir1"];
        var child = DirectoryItem.Create("child1", "path1/child1", "Child 1", _providerId);
        parent.AddChild(child);
        items.Add("child1", child);
        _repository.SetupRepositoryItems(items);

        // Act
        var result = await _repository.GetTreeAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        var dirWithChild = result.FirstOrDefault(d => d.Id == "dir1");
        dirWithChild.ShouldNotBeNull();
        dirWithChild.Children.Count.ShouldBe(1);
        _repository.CacheRefreshCount.ShouldBe(1);
    }

    [Fact]
    public async Task RefreshCacheAsync_InvalidatesAndReloadsCache()
    {
        // Arrange
        var items = CreateTestDirectories();
        _repository.SetupRepositoryItems(items);

        // Load once to populate cache
        await _repository.GetAllAsync();
        _repository.CacheRefreshCount.ShouldBe(1);

        // Act - Force refresh
        await _repository.RefreshCacheAsync();

        // Assert
        _repository.CacheRefreshCount.ShouldBe(2);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_CallsInternalDelete()
    {
        // Arrange
        var items = CreateTestDirectories();
        _repository.SetupRepositoryItems(items);

        // Act
        await _repository.DeleteAsync("dir1");

        // Assert
        _repository.DeletedIds.ShouldContain("dir1");
        _repository.DeletedIds.Count.ShouldBe(1);
        _repository.DeleteRecursive.ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteRecursiveAsync_WithExistingId_CallsInternalDeleteWithRecursiveFlag()
    {
        // Arrange
        var items = CreateTestDirectories();
        _repository.SetupRepositoryItems(items);

        // Act
        await _repository.DeleteRecursiveAsync("dir1");

        // Assert
        _repository.DeletedIds.ShouldContain("dir1");
        _repository.DeletedIds.Count.ShouldBe(1);
        _repository.DeleteRecursive.ShouldBeTrue();
    }

    [Fact]
    public async Task SaveAsync_WithValidDirectory_CallsInternalSave()
    {
        // Arrange
        var items = CreateTestDirectories();
        _repository.SetupRepositoryItems(items);
        var newDirectory = DirectoryItem.Create("new-dir", "new-path", "New Directory", _providerId);

        // Act
        var result = await _repository.SaveAsync(newDirectory);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("new-dir");
        _repository.SavedDirectories.ShouldContain(newDirectory);
    }

    [Fact]
    public async Task NormalizePath_ConvertsBackslashesToForwardSlashes()
    {
        // Arrange
        var path = "test\\path\\with\\backslashes";

        // Get the private method using reflection
        var normalizePathMethod = typeof(BaseDirectoryRepository<TestOptions>)
            .GetMethod("NormalizePath", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = normalizePathMethod.Invoke(_repository, new object[] { path }) as string;

        // Assert
        result.ShouldBe("test/path/with/backslashes");
    }

    private Dictionary<string, DirectoryItem> CreateTestDirectories()
    {
        var result = new Dictionary<string, DirectoryItem>();
        var dir1 = DirectoryItem.Create("dir1", "path1", "Directory 1", _providerId);
        var dir2 = DirectoryItem.Create("dir2", "path2", "Directory 2", _providerId);

        result.Add(dir1.Id, dir1);
        result.Add(dir2.Id, dir2);

        return result;
    }

    // Concrete implementation for testing the abstract class
    public class TestOptions { }

    public class TestDirectoryRepository : BaseDirectoryRepository<TestOptions>
    {
        private Dictionary<string, DirectoryItem> _items = new();
        public int CacheRefreshCount { get; private set; } = 0;
        public List<string> DeletedIds { get; private set; } = new();
        public bool DeleteRecursive { get; private set; }
        public List<DirectoryItem> SavedDirectories { get; private set; } = new();

        public TestDirectoryRepository(string providerId, ILogger logger)
            : base(providerId, new TestOptions(), logger)
        {
        }

        public void SetupRepositoryItems(Dictionary<string, DirectoryItem> items)
        {
            _items = items;
        }

        protected override Task EnsureCacheIsLoaded(CancellationToken cancellationToken, bool forceRefresh = false)
        {
            if (forceRefresh || DirectoryCache == null)
            {
                CacheRefreshCount++;
                DirectoryCache = new Dictionary<string, DirectoryItem>(_items);
                CacheExpiration = DateTime.UtcNow.AddMinutes(30);
            }
            return Task.CompletedTask;
        }

        protected override Task LoadDirectoriesIntoCacheAsync(Dictionary<string, DirectoryItem> cache, CancellationToken cancellationToken)
        {
            foreach (var item in _items)
            {
                cache[item.Key] = item.Value;
            }
            return Task.CompletedTask;
        }

        protected override Task<DirectoryItem> SaveDirectoryInternalAsync(DirectoryItem entity, CancellationToken cancellationToken)
        {
            SavedDirectories.Add(entity);
            return Task.FromResult(entity);
        }

        protected override Task DeleteDirectoryInternalAsync(string id, bool recursive, string? commitMessage, CancellationToken cancellationToken)
        {
            DeletedIds.Add(id);
            DeleteRecursive = recursive;
            return Task.CompletedTask;
        }
    }
}