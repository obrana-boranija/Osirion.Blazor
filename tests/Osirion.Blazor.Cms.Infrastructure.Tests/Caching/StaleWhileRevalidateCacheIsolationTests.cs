using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Caching;

public class StaleWhileRevalidateCacheIsolationTests
{
    private readonly IContentRepository _decorated = Substitute.For<IContentRepository>();
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

    private StaleWhileRevalidateCacheDecorator CreateDecorator(TimeSpan? staleTime = null, MemoryCache? cache = null) =>
        new(
            _decorated,
            cache ?? _memoryCache,
            NullLogger<StaleWhileRevalidateCacheDecorator>.Instance,
            staleTime ?? TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(30),
            "provider");

    private static ContentItem Item(string id) => ContentItem.Create(id, "Title " + id, "Body", id + ".md", "provider");

    [Fact]
    public async Task RefreshCacheAsync_DoesNotEvictHostCacheEntries()
    {
        _memoryCache.Set("host:key", "host value");
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).Returns([Item("a")]);
        var decorator = CreateDecorator();
        await decorator.GetAllAsync();

        await decorator.RefreshCacheAsync();

        _memoryCache.TryGetValue("host:key", out string? hostValue).ShouldBeTrue();
        hostValue.ShouldBe("host value");
    }

    [Fact]
    public async Task RefreshCacheAsync_EvictsOwnEntries()
    {
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).Returns([Item("a")]);
        var decorator = CreateDecorator();
        await decorator.GetAllAsync();
        await decorator.GetAllAsync();
        await _decorated.Received(1).GetAllAsync(Arg.Any<CancellationToken>());

        await decorator.RefreshCacheAsync();
        await decorator.GetAllAsync();

        await _decorated.Received(2).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshCacheAsync_OfOneDecorator_DoesNotEvictAnotherDecoratorSharingTheCache()
    {
        var other = Substitute.For<IContentRepository>();
        other.GetAllAsync(Arg.Any<CancellationToken>()).Returns([Item("o")]);
        var otherDecorator = new StaleWhileRevalidateCacheDecorator(
            other, _memoryCache, NullLogger<StaleWhileRevalidateCacheDecorator>.Instance,
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), "other");
        await otherDecorator.GetAllAsync();
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).Returns([Item("a")]);
        var decorator = CreateDecorator();

        await decorator.RefreshCacheAsync();
        await otherDecorator.GetAllAsync();

        await other.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FindByQueryAsync_IncludeUnpublishedIsPartOfTheKey()
    {
        var published = new List<ContentItem> { Item("pub") };
        var withDrafts = new List<ContentItem> { Item("pub"), Item("draft") };
        _decorated.FindByQueryAsync(Arg.Is<ContentQuery>(q => q.IncludeUnpublished), Arg.Any<CancellationToken>()).Returns(withDrafts);
        _decorated.FindByQueryAsync(Arg.Is<ContentQuery>(q => !q.IncludeUnpublished), Arg.Any<CancellationToken>()).Returns(published);
        var decorator = CreateDecorator();

        var admin = await decorator.FindByQueryAsync(new ContentQuery { IncludeUnpublished = true });
        var publicResult = await decorator.FindByQueryAsync(new ContentQuery());

        admin!.Count.ShouldBe(2);
        publicResult!.Count.ShouldBe(1);
    }

    [Fact]
    public async Task FindByQueryAsync_ExcludeIdsDifferingQueriesDoNotCollide()
    {
        _decorated.FindByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns(call => (IReadOnlyList<ContentItem>?)new List<ContentItem> { Item(string.Join(",", call.Arg<ContentQuery>().ExcludeIds ?? [])) });
        var decorator = CreateDecorator();

        var first = await decorator.FindByQueryAsync(new ContentQuery { ExcludeIds = ["a"] });
        var second = await decorator.FindByQueryAsync(new ContentQuery { ExcludeIds = ["b"] });

        first![0].Id.ShouldNotBe(second![0].Id);
    }

    [Fact]
    public async Task FindByQueryAsync_SetsAreOrderIndependent()
    {
        _decorated.FindByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).Returns([Item("x")]);
        var decorator = CreateDecorator();

        await decorator.FindByQueryAsync(new ContentQuery { Tags = ["a", "b"], ExcludeIds = ["1", "2"] });
        await decorator.FindByQueryAsync(new ContentQuery { Tags = ["b", "a"], ExcludeIds = ["2", "1"] });

        await _decorated.Received(1).FindByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("Path")]
    [InlineData("Url")]
    [InlineData("LocalizationId")]
    [InlineData("ProviderId")]
    public async Task FindByQueryAsync_EveryFilterFieldIsPartOfTheKey(string field)
    {
        _decorated.FindByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).Returns([Item("x")]);
        var decorator = CreateDecorator();
        var changed = new ContentQuery();
        typeof(ContentQuery).GetProperty(field)!.SetValue(changed, "value");

        await decorator.FindByQueryAsync(new ContentQuery());
        await decorator.FindByQueryAsync(changed);

        await _decorated.Received(2).FindByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FindByQueryAsync_WithSizeLimitedCache_StoresEntriesWithSize()
    {
        using var sizedCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 2 });
        _decorated.FindByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).Returns([Item("x")]);
        var decorator = CreateDecorator(cache: sizedCache);

        for (var i = 0; i < 10; i++)
        {
            await decorator.FindByQueryAsync(new ContentQuery { SearchQuery = new string('q', 500) + i });
        }

        sizedCache.Count.ShouldBeLessThanOrEqualTo(2);
    }

    [Fact]
    public async Task FindByQueryAsync_VeryLongSearchTextProducesBoundedKey()
    {
        _decorated.FindByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).Returns([Item("x")]);
        var decorator = CreateDecorator();

        await decorator.FindByQueryAsync(new ContentQuery { SearchQuery = new string('q', 100_000) });

        _memoryCache.Keys.OfType<string>().ShouldAllBe(key => key.Length < 300);
    }

    [Fact]
    public async Task GetByIdAsync_DifferentKeysLoadConcurrently()
    {
        var firstStarted = new TaskCompletionSource();
        var releaseFirst = new TaskCompletionSource<ContentItem?>();
        _decorated.GetByIdAsync("slow", Arg.Any<CancellationToken>()).Returns(_ =>
        {
            firstStarted.SetResult();
            return releaseFirst.Task;
        });
        _decorated.GetByIdAsync("fast", Arg.Any<CancellationToken>()).Returns(Item("fast"));
        var decorator = CreateDecorator();

        var slow = decorator.GetByIdAsync("slow");
        await firstStarted.Task;
        var fast = await decorator.GetByIdAsync("fast").WaitAsync(TimeSpan.FromSeconds(5));

        fast!.Id.ShouldBe("fast");
        slow.IsCompleted.ShouldBeFalse();
        releaseFirst.SetResult(Item("slow"));
        (await slow)!.Id.ShouldBe("slow");
    }

    [Fact]
    public async Task GetByIdAsync_ConcurrentMissesOnSameKeyShareOneLoad()
    {
        var release = new TaskCompletionSource<ContentItem?>();
        _decorated.GetByIdAsync("a", Arg.Any<CancellationToken>()).Returns(release.Task);
        var decorator = CreateDecorator();

        var first = decorator.GetByIdAsync("a");
        var second = decorator.GetByIdAsync("a");
        release.SetResult(Item("a"));
        await Task.WhenAll(first, second);

        await _decorated.Received(1).GetByIdAsync("a", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_CancellingOneCallerDoesNotFailTheSharedLoad()
    {
        var release = new TaskCompletionSource<ContentItem?>();
        _decorated.GetByIdAsync("a", Arg.Any<CancellationToken>()).Returns(release.Task);
        var decorator = CreateDecorator();
        using var cts = new CancellationTokenSource();

        var cancelled = decorator.GetByIdAsync("a", cts.Token);
        var patient = decorator.GetByIdAsync("a");
        await cts.CancelAsync();
        release.SetResult(Item("a"));

        await Should.ThrowAsync<OperationCanceledException>(cancelled);
        (await patient)!.Id.ShouldBe("a");
    }

    [Fact]
    public async Task StaleEntry_BackgroundRefreshIgnoresTheRequestsCancellationToken()
    {
        var tokens = new List<CancellationToken>();
        var refreshed = new TaskCompletionSource();
        var calls = 0;
        _decorated.GetByIdAsync("a", Arg.Any<CancellationToken>()).Returns(call =>
        {
            tokens.Add(call.Arg<CancellationToken>());
            if (Interlocked.Increment(ref calls) == 2)
            {
                refreshed.SetResult();
            }

            return Task.FromResult<ContentItem?>(Item("a"));
        });
        var decorator = CreateDecorator(staleTime: TimeSpan.Zero);
        using var cts = new CancellationTokenSource();

        await decorator.GetByIdAsync("a", cts.Token);
        await Task.Delay(10);
        await decorator.GetByIdAsync("a", cts.Token);
        await cts.CancelAsync();
        await refreshed.Task.WaitAsync(TimeSpan.FromSeconds(5));

        tokens.ShouldAllBe(token => !token.CanBeCanceled);
    }
}
