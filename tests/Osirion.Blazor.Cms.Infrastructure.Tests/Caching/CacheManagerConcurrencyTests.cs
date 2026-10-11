using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Osirion.Blazor.Cms.Infrastructure.Content;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Caching;

/// <summary>
/// A caller that misses the cache while another caller is loading must wait for that load and reuse its result.
/// </summary>
public class CacheManagerConcurrencyTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task RepositoryCacheManager_CallerWaitingOnALoad_ReusesItsResult()
    {
        var manager = new RepositoryCacheManager<string, DirectoryItem>(NullLogger.Instance, TimeSpan.FromHours(1), "provider");
        var release = new TaskCompletionSource<Dictionary<string, DirectoryItem>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var loads = 0;
        Task<Dictionary<string, DirectoryItem>> Load(CancellationToken _)
        {
            Interlocked.Increment(ref loads);
            return release.Task;
        }

        var first = manager.GetCachedEntitiesAsync(Load);
        var second = manager.GetCachedEntitiesAsync(Load);
        var loaded = new Dictionary<string, DirectoryItem> { ["a"] = DirectoryItem.Create("a", "a", "a", "provider") };
        release.SetResult(loaded);

        (await first.WaitAsync(Timeout)).ShouldBeSameAs(loaded);
        (await second.WaitAsync(Timeout)).ShouldBeSameAs(loaded);
        loads.ShouldBe(1);
    }

    [Fact]
    public void RepositoryCacheManager_WithNullArguments_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new RepositoryCacheManager<string, DirectoryItem>(null!, TimeSpan.Zero, "p"));
        Should.Throw<ArgumentNullException>(() => new RepositoryCacheManager<string, DirectoryItem>(NullLogger.Instance, TimeSpan.Zero, null!));
    }

    [Fact]
    public async Task RepositoryCacheManager_WithZeroDuration_ReloadsOnEveryRead()
    {
        var manager = new RepositoryCacheManager<string, DirectoryItem>(NullLogger.Instance, TimeSpan.Zero, "provider");
        var loads = 0;

        await manager.GetCachedEntitiesAsync(_ =>
        {
            loads++;
            return Task.FromResult(new Dictionary<string, DirectoryItem>());
        });
        await manager.GetCachedEntitiesAsync(_ =>
        {
            loads++;
            return Task.FromResult(new Dictionary<string, DirectoryItem>());
        });

        loads.ShouldBe(2);
    }

    [Fact]
    public async Task ContentCacheManager_CallerWaitingOnALoad_ReusesItsResult()
    {
        var manager = new ContentCacheManager(NullLogger<ContentCacheManager>.Instance);
        var release = new TaskCompletionSource<Dictionary<string, ContentItem>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var loads = 0;
        Task<Dictionary<string, ContentItem>> Load(CancellationToken _)
        {
            Interlocked.Increment(ref loads);
            return release.Task;
        }

        var first = manager.GetCachedItemsAsync(Load);
        var second = manager.GetCachedItemsAsync(Load);
        var loaded = new Dictionary<string, ContentItem> { ["a"] = ContentItem.Create("a", "A", "Body", "a.md", "provider") };
        release.SetResult(loaded);

        (await first.WaitAsync(Timeout)).ShouldBeSameAs(loaded);
        (await second.WaitAsync(Timeout)).ShouldBeSameAs(loaded);
        loads.ShouldBe(1);
    }

    [Fact]
    public void ContentCacheManager_WithNullLogger_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new ContentCacheManager(null!));
    }
}
