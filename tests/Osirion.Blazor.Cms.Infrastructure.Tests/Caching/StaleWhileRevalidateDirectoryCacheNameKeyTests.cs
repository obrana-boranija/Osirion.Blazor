using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Caching;

public sealed class StaleWhileRevalidateDirectoryCacheNameKeyTests : IDisposable
{
    private readonly IDirectoryRepository _decorated = Substitute.For<IDirectoryRepository>();
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

    public void Dispose() => _memoryCache.Dispose();

    [Fact]
    public async Task GetByNameAsync_WithDifferentNamesInOneLocale_ReturnsEachDirectory()
    {
        _decorated.GetByNameAsync("blog", "en", Arg.Any<CancellationToken>()).Returns(DirectoryItem.Create("blog", "blog", "blog", "provider"));
        _decorated.GetByNameAsync("docs", "en", Arg.Any<CancellationToken>()).Returns(DirectoryItem.Create("docs", "docs", "docs", "provider"));
        var cache = new StaleWhileRevalidateDirectoryCache(
            _decorated, _memoryCache, NullLogger<StaleWhileRevalidateDirectoryCache>.Instance,
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), "provider");

        var blog = await cache.GetByNameAsync("blog", "en");
        var docs = await cache.GetByNameAsync("docs", "en");

        blog!.Id.ShouldBe("blog");
        docs!.Id.ShouldBe("docs");
    }
}
