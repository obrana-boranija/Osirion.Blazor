using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Directory;

public class DirectoryCacheManagerConcurrencyTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private readonly DirectoryCacheManager _cacheManager = new(NullLogger<DirectoryCacheManager>.Instance);

    private static Dictionary<string, DirectoryItem> Directories(params string[] ids) =>
        ids.ToDictionary(id => id, id => DirectoryItem.Create(id, id, id, "test"));

    [Fact]
    public void Constructor_NullLogger_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new DirectoryCacheManager(null!));
    }

    [Fact]
    public async Task GetCachedDirectoriesAsync_WhileALoadIsRunning_ReturnsTheExistingCacheWithoutWaiting()
    {
        var release = new TaskCompletionSource<Dictionary<string, DirectoryItem>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var running = _cacheManager.GetCachedDirectoriesAsync(_ => release.Task);

        var concurrent = await _cacheManager.GetCachedDirectoriesAsync(_ => throw new InvalidOperationException("must not load"));

        concurrent.ShouldBeEmpty();
        release.SetResult(Directories("a"));
        (await running.WaitAsync(Timeout)).Keys.ShouldBe(["a"]);
    }

    [Fact]
    public async Task InvalidateCacheAsync_WhileALoadIsRunning_IsSkipped()
    {
        var release = new TaskCompletionSource<Dictionary<string, DirectoryItem>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var running = _cacheManager.GetCachedDirectoriesAsync(_ => release.Task);

        await _cacheManager.InvalidateCacheAsync();
        release.SetResult(Directories("a"));
        await running.WaitAsync(Timeout);

        var afterwards = await _cacheManager.GetCachedDirectoriesAsync(_ => throw new InvalidOperationException("must not reload"));
        afterwards.Keys.ShouldBe(["a"]);
    }

    [Fact]
    public async Task GetCachedDirectoriesAsync_LoaderReturnsNull_CachesAnEmptyDictionary()
    {
        var result = await _cacheManager.GetCachedDirectoriesAsync(_ => Task.FromResult<Dictionary<string, DirectoryItem>>(null!));

        result.ShouldNotBeNull().ShouldBeEmpty();
    }

    [Fact]
    public async Task GetCachedDirectoriesAsync_LoaderThrows_RethrowsAndLetsTheNextCallLoad()
    {
        await Should.ThrowAsync<InvalidOperationException>(() =>
            _cacheManager.GetCachedDirectoriesAsync(_ => throw new InvalidOperationException("offline")));

        var result = await _cacheManager.GetCachedDirectoriesAsync(_ => Task.FromResult(Directories("a")));

        result.Keys.ShouldBe(["a"]);
    }

    [Fact]
    public async Task GetCachedDirectoriesAsync_AfterInvalidation_ReloadsEvenThoughEntriesExist()
    {
        await _cacheManager.GetCachedDirectoriesAsync(_ => Task.FromResult(Directories("a")));
        await _cacheManager.InvalidateCacheAsync();

        var result = await _cacheManager.GetCachedDirectoriesAsync(_ => Task.FromResult(Directories("b")));

        result.Keys.ShouldBe(["b"]);
    }
}
