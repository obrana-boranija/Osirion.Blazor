using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public sealed class InMemoryContentCacheServiceFactoryFailureTests : IDisposable
{
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

    public void Dispose() => _memoryCache.Dispose();

    [Fact]
    public async Task GetOrCreateAsync_WhenTheFactoryFails_RethrowsAndCachesNothing()
    {
        var service = new InMemoryContentCacheService(
            _memoryCache, NullLogger<InMemoryContentCacheService>.Instance, Options.Create(new CacheOptions()));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            service.GetOrCreateAsync<string>("key", _ => throw new InvalidOperationException("boom")));

        _memoryCache.TryGetValue("key", out _).ShouldBeFalse();
    }

    [Fact]
    public void Constructor_WithNullDependencies_Throws()
    {
        var logger = NullLogger<InMemoryContentCacheService>.Instance;
        var options = Options.Create(new CacheOptions());

        Should.Throw<ArgumentNullException>(() => new InMemoryContentCacheService(null!, logger, options));
        Should.Throw<ArgumentNullException>(() => new InMemoryContentCacheService(_memoryCache, null!, options));
    }
}
