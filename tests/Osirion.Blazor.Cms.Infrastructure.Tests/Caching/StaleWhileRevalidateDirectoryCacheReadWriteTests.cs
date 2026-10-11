using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Caching;

public sealed class StaleWhileRevalidateDirectoryCacheReadWriteTests : IDisposable
{
    private readonly IDirectoryRepository _decorated = Substitute.For<IDirectoryRepository>();
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
    private readonly StaleWhileRevalidateDirectoryCache _cache;

    public StaleWhileRevalidateDirectoryCacheReadWriteTests()
    {
        _cache = new StaleWhileRevalidateDirectoryCache(
            _decorated,
            _memoryCache,
            NullLogger<StaleWhileRevalidateDirectoryCache>.Instance,
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(30),
            "provider");
    }

    public void Dispose() => _memoryCache.Dispose();

    private static DirectoryItem Directory(string id) => DirectoryItem.Create(id, id, "Name " + id, "provider");

    [Fact]
    public void Constructor_WithNullArguments_Throws()
    {
        var logger = NullLogger<StaleWhileRevalidateDirectoryCache>.Instance;
        var stale = TimeSpan.FromMinutes(1);
        var max = TimeSpan.FromMinutes(2);

        Should.Throw<ArgumentNullException>(() => new StaleWhileRevalidateDirectoryCache(null!, _memoryCache, logger, stale, max, "p"));
        Should.Throw<ArgumentNullException>(() => new StaleWhileRevalidateDirectoryCache(_decorated, null!, logger, stale, max, "p"));
        Should.Throw<ArgumentNullException>(() => new StaleWhileRevalidateDirectoryCache(_decorated, _memoryCache, null!, stale, max, "p"));
        Should.Throw<ArgumentNullException>(() => new StaleWhileRevalidateDirectoryCache(_decorated, _memoryCache, logger, stale, max, null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task SingleItemReads_WithBlankKey_Throw(string blank)
    {
        await Should.ThrowAsync<ArgumentException>(() => _cache.GetByIdAsync(blank));
        await Should.ThrowAsync<ArgumentException>(() => _cache.GetByPathAsync(blank));
        await Should.ThrowAsync<ArgumentException>(() => _cache.GetByUrlAsync(blank));
    }

    [Fact]
    public async Task GetByPathAsync_CachesResult()
    {
        _decorated.GetByPathAsync("blog", Arg.Any<CancellationToken>()).Returns(Directory("blog"));

        await _cache.GetByPathAsync("blog");
        var result = await _cache.GetByPathAsync("blog");

        result!.Id.ShouldBe("blog");
        await _decorated.Received(1).GetByPathAsync("blog", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByUrlAsync_CachesResult()
    {
        _decorated.GetByUrlAsync("/blog", Arg.Any<CancellationToken>()).Returns(Directory("blog"));

        await _cache.GetByUrlAsync("/blog");
        var result = await _cache.GetByUrlAsync("/blog");

        result!.Id.ShouldBe("blog");
        await _decorated.Received(1).GetByUrlAsync("/blog", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByLocaleAsync_CachesEachLocaleSeparately()
    {
        _decorated.GetByLocaleAsync(null, Arg.Any<CancellationToken>()).Returns([Directory("all")]);
        _decorated.GetByLocaleAsync("de", Arg.Any<CancellationToken>()).Returns([Directory("de")]);

        await _cache.GetByLocaleAsync();
        var all = await _cache.GetByLocaleAsync();
        await _cache.GetByLocaleAsync("de");
        var german = await _cache.GetByLocaleAsync("de");

        all.Single().Id.ShouldBe("all");
        german.Single().Id.ShouldBe("de");
        await _decorated.Received(1).GetByLocaleAsync(null, Arg.Any<CancellationToken>());
        await _decorated.Received(1).GetByLocaleAsync("de", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetChildrenAsync_CachesEachParentSeparately()
    {
        _decorated.GetChildrenAsync("p1", Arg.Any<CancellationToken>()).Returns([Directory("c1")]);
        _decorated.GetChildrenAsync("p2", Arg.Any<CancellationToken>()).Returns([Directory("c2")]);

        await _cache.GetChildrenAsync("p1");
        var first = await _cache.GetChildrenAsync("p1");
        var second = await _cache.GetChildrenAsync("p2");

        first.Single().Id.ShouldBe("c1");
        second.Single().Id.ShouldBe("c2");
        await _decorated.Received(1).GetChildrenAsync("p1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTreeAsync_CachesEachLocaleSeparately()
    {
        _decorated.GetTreeAsync(null, Arg.Any<CancellationToken>()).Returns([Directory("all")]);
        _decorated.GetTreeAsync("fr", Arg.Any<CancellationToken>()).Returns([Directory("fr")]);

        await _cache.GetTreeAsync();
        var all = await _cache.GetTreeAsync();
        var french = await _cache.GetTreeAsync("fr");

        all.Single().Id.ShouldBe("all");
        french.Single().Id.ShouldBe("fr");
        await _decorated.Received(1).GetTreeAsync(null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByNameAsync_CachesResult()
    {
        _decorated.GetByNameAsync("blog", "en", Arg.Any<CancellationToken>()).Returns(Directory("blog"));

        await _cache.GetByNameAsync("blog", "en");
        var result = await _cache.GetByNameAsync("blog", "en");

        result!.Id.ShouldBe("blog");
        await _decorated.Received(1).GetByNameAsync("blog", "en", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_SavesRefreshesTheRepositoryAndEvictsCachedReads()
    {
        var saved = Directory("saved");
        _decorated.SaveAsync(saved, Arg.Any<CancellationToken>()).Returns(saved);
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).Returns([Directory("a")]);
        await _cache.GetAllAsync();

        var result = await _cache.SaveAsync(saved);
        await _cache.GetAllAsync();

        result.ShouldBeSameAs(saved);
        await _decorated.Received(1).RefreshCacheAsync(Arg.Any<CancellationToken>());
        await _decorated.Received(2).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_DeletesAndEvictsCachedReads()
    {
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).Returns([Directory("a")]);
        await _cache.GetAllAsync();

        await _cache.DeleteAsync("a");
        await _cache.GetAllAsync();

        await _decorated.Received(1).DeleteAsync("a", Arg.Any<CancellationToken>());
        await _decorated.Received(1).RefreshCacheAsync(Arg.Any<CancellationToken>());
        await _decorated.Received(2).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteRecursiveAsync_DeletesWithTheCommitMessageAndEvictsCachedReads()
    {
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).Returns([Directory("a")]);
        await _cache.GetAllAsync();

        await _cache.DeleteRecursiveAsync("a", "remove a");
        await _cache.GetAllAsync();

        await _decorated.Received(1).DeleteRecursiveAsync("a", "remove a", Arg.Any<CancellationToken>());
        await _decorated.Received(1).RefreshCacheAsync(Arg.Any<CancellationToken>());
        await _decorated.Received(2).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MoveAsync_MovesAndEvictsCachedReads()
    {
        var moved = Directory("a");
        _decorated.MoveAsync("a", "parent", Arg.Any<CancellationToken>()).Returns(moved);
        _decorated.GetChildrenAsync("parent", Arg.Any<CancellationToken>()).Returns([], [moved]);
        await _cache.GetChildrenAsync("parent");

        var result = await _cache.MoveAsync("a", "parent");
        var children = await _cache.GetChildrenAsync("parent");

        result.ShouldBeSameAs(moved);
        children.ShouldBe([moved]);
        await _decorated.Received(1).RefreshCacheAsync(Arg.Any<CancellationToken>());
    }
}
