using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Caching;

/// <summary>
/// Drives the shared stale-while-revalidate store through the directory decorator. Entries are seeded with fixed
/// timestamps, so staleness never depends on the clock, and background work is awaited through cache writes.
/// </summary>
public sealed class StaleWhileRevalidateStoreRefreshTests : IDisposable
{
    private const string AllKey = "directory:all:provider";
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private readonly IDirectoryRepository _decorated = Substitute.For<IDirectoryRepository>();
    private readonly ILogger<StaleWhileRevalidateDirectoryCache> _logger = Substitute.For<ILogger<StaleWhileRevalidateDirectoryCache>>();
    private readonly ObservableMemoryCache _memoryCache = new();
    private readonly StaleWhileRevalidateDirectoryCache _cache;

    public StaleWhileRevalidateStoreRefreshTests()
    {
        _cache = new StaleWhileRevalidateDirectoryCache(
            _decorated, _memoryCache, _logger, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), "provider");
    }

    public void Dispose() => _memoryCache.Dispose();

    private static IReadOnlyList<DirectoryItem> Directories(string id) => [DirectoryItem.Create(id, id, id, "provider")];

    private void Seed(IReadOnlyList<DirectoryItem> value, DateTime lastUpdated) =>
        _memoryCache.CreateEntry(AllKey).SetValue(new CacheEntry<IReadOnlyList<DirectoryItem>>
        {
            Value = value,
            LastUpdated = lastUpdated
        }).Dispose();

    [Fact]
    public async Task GetAllAsync_WithStaleEntry_ReturnsItAndStoresTheRefreshedValue()
    {
        var stale = Directories("stale");
        var fresh = Directories("fresh");
        Seed(stale, DateTime.MinValue);
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).Returns(fresh);
        var written = _memoryCache.WaitForWriteAsync(AllKey);

        var first = await _cache.GetAllAsync();
        await written.WaitAsync(Timeout);
        var second = await _cache.GetAllAsync();

        first.ShouldBeSameAs(stale);
        second.ShouldBeSameAs(fresh);
        await _decorated.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_WhenTheRefreshFails_LogsAndKeepsTheStaleValue()
    {
        var stale = Directories("stale");
        Seed(stale, DateTime.MinValue);
        var failure = new InvalidOperationException("boom");
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        var logged = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _logger
            .When(logger => logger.Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<Arg.AnyType>(), failure, Arg.Any<Func<Arg.AnyType, Exception?, string>>()))
            .Do(_ => logged.TrySetResult());

        var first = await _cache.GetAllAsync();
        await logged.Task.WaitAsync(Timeout);

        first.ShouldBeSameAs(stale);
        CacheExtensions.TryGetValue(_memoryCache, AllKey, out CacheEntry<IReadOnlyList<DirectoryItem>>? entry).ShouldBeTrue();
        entry!.Value.ShouldBeSameAs(stale);
    }

    [Fact]
    public async Task GetAllAsync_WhileARefreshIsRunning_DoesNotStartAnother()
    {
        Seed(Directories("stale"), DateTime.MinValue);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<IReadOnlyList<DirectoryItem>>(TaskCreationOptions.RunContinuationsAsynchronously);
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            started.TrySetResult();
            return release.Task;
        });
        var written = _memoryCache.WaitForWriteAsync(AllKey);

        await _cache.GetAllAsync();
        await started.Task.WaitAsync(Timeout);
        await _cache.GetAllAsync();
        release.SetResult(Directories("fresh"));
        await written.WaitAsync(Timeout);

        await _decorated.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_WithFreshEntry_DoesNotRefresh()
    {
        var cached = Directories("cached");
        Seed(cached, DateTime.MaxValue);

        var result = await _cache.GetAllAsync();

        result.ShouldBeSameAs(cached);
        await _decorated.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_WhenAnotherLoadStoredTheValueAfterTheMiss_ReturnsItWithoutLoading()
    {
        var stored = Directories("stored");
        Seed(stored, DateTime.MaxValue);
        _memoryCache.MissNextLookup(AllKey);

        var result = await _cache.GetAllAsync();

        result.ShouldBeSameAs(stored);
        await _decorated.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_WhenTheLoadFails_ThrowsAndCachesNothing()
    {
        _decorated.GetAllAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _cache.GetAllAsync());

        _memoryCache.TryGetValue(AllKey, out _).ShouldBeFalse();
    }
}
