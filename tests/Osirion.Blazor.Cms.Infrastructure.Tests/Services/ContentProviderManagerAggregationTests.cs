using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Shouldly;
using SortDirection = Osirion.Blazor.Cms.Domain.Enums.SortDirection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class ContentProviderManagerAggregationTests
{
    private readonly IContentProviderRegistry _registry = Substitute.For<IContentProviderRegistry>();
    private readonly ContentProviderManager _manager;

    public ContentProviderManagerAggregationTests()
    {
        _manager = new ContentProviderManager(_registry, NullLogger<ContentProviderManager>.Instance);
    }

    private static ContentItem Item(string id, string title = "Title", string author = "Author", int day = 1, string? locale = null)
    {
        var item = ContentItem.Create(id, title, "Body", id + ".md", "provider");
        item.SetAuthor(author);
        item.SetCreatedDate(new DateTime(2024, 1, day, 0, 0, 0, DateTimeKind.Utc));
        if (locale is not null)
        {
            item.SetLocale(locale);
        }

        return item;
    }

    private static IContentProvider Provider(params ContentItem[] items)
    {
        var provider = Substitute.For<IContentProvider>();
        provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns(items.ToList());
        return provider;
    }

    private static IContentProvider ProviderThatCancels(CancellationTokenSource cts)
    {
        var provider = Substitute.For<IContentProvider>();
        provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<ContentItem>?>(_ =>
            {
                cts.Cancel();
                throw new OperationCanceledException(cts.Token);
            });
        provider.GetDirectoriesAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<DirectoryItem>>(_ =>
            {
                cts.Cancel();
                throw new OperationCanceledException(cts.Token);
            });
        return provider;
    }

    private void RegisterAll(params IContentProvider[] providers) =>
        _registry.GetAllProviders().Returns(providers);

    [Fact]
    public void Constructor_WithNullArguments_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new ContentProviderManager(null!, NullLogger<ContentProviderManager>.Instance));
        Should.Throw<ArgumentNullException>(() => new ContentProviderManager(_registry, null!));
    }

    [Fact]
    public async Task GetContentByQueryAsync_WithNoProviders_ReturnsEmpty()
    {
        RegisterAll();

        var result = await _manager.GetContentByQueryAsync(new ContentQuery());

        result.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(SortField.Title, SortDirection.Ascending, "a,b,c")]
    [InlineData(SortField.Title, SortDirection.Descending, "c,b,a")]
    [InlineData(SortField.Author, SortDirection.Ascending, "b,c,a")]
    [InlineData(SortField.Author, SortDirection.Descending, "a,c,b")]
    [InlineData(SortField.Date, SortDirection.Ascending, "c,a,b")]
    [InlineData(SortField.Date, SortDirection.Descending, "b,a,c")]
    [InlineData(SortField.Slug, SortDirection.Ascending, "b,a,c")]
    public async Task GetContentByQueryAsync_WithSeveralProviders_SortsTheCombinedItems(
        SortField sortBy, SortDirection direction, string expectedIds)
    {
        RegisterAll(
            Provider(Item("b", title: "B", author: "Ann", day: 3), Item("a", title: "A", author: "Zed", day: 2)),
            Provider(Item("c", title: "C", author: "Max", day: 1)));

        var result = await _manager.GetContentByQueryAsync(new ContentQuery { SortBy = sortBy, SortDirection = direction });

        string.Join(",", result.Select(item => item.Id)).ShouldBe(expectedIds);
    }

    [Fact]
    public async Task GetContentByQueryAsync_WithSeveralProviders_AppliesSkipAndTakeAfterSorting()
    {
        RegisterAll(
            Provider(Item("b", title: "B"), Item("a", title: "A")),
            Provider(Item("d", title: "D"), Item("c", title: "C")));

        var result = await _manager.GetContentByQueryAsync(new ContentQuery
        {
            SortBy = SortField.Title,
            SortDirection = SortDirection.Ascending,
            Skip = 1,
            Take = 2
        });

        result.Select(item => item.Id).ShouldBe(["b", "c"]);
    }

    [Fact]
    public async Task GetContentByQueryAsync_WithSeveralProvidersAndOneItem_ReturnsIt()
    {
        RegisterAll(Provider(Item("a")), Provider());

        var result = await _manager.GetContentByQueryAsync(new ContentQuery { SortBy = SortField.Title });

        result.Select(item => item.Id).ShouldBe(["a"]);
    }

    [Fact]
    public async Task GetContentByQueryAsync_WithOneProvider_LeavesSortingAndPagingToTheProvider()
    {
        RegisterAll(Provider(Item("b", title: "B"), Item("a", title: "A"), Item("c", title: "C")));

        var result = await _manager.GetContentByQueryAsync(new ContentQuery
        {
            SortBy = SortField.Title,
            Skip = 1,
            Take = 1
        });

        result.Select(item => item.Id).ShouldBe(["b", "a", "c"]);
    }

    [Fact]
    public async Task GetContentByQueryAsync_WhenAProviderFails_ReturnsTheOtherProvidersItems()
    {
        var failing = Substitute.For<IContentProvider>();
        failing.GetItemsByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("boom"));
        var empty = Substitute.For<IContentProvider>();
        empty.GetItemsByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<ContentItem>?)null);
        RegisterAll(failing, empty, Provider(Item("a")));

        var result = await _manager.GetContentByQueryAsync(new ContentQuery());

        result.Select(item => item.Id).ShouldBe(["a"]);
    }

    [Fact]
    public async Task GetContentByQueryAsync_WhenCancelledBeforeStart_Throws()
    {
        RegisterAll(Provider(Item("a")));
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(
            () => _manager.GetContentByQueryAsync(new ContentQuery(), cts.Token));
    }

    [Fact]
    public async Task GetContentByQueryAsync_WhenProviderObservesCancellation_RethrowsIt()
    {
        using var cts = new CancellationTokenSource();
        RegisterAll(ProviderThatCancels(cts));

        await Should.ThrowAsync<OperationCanceledException>(
            () => _manager.GetContentByQueryAsync(new ContentQuery(), cts.Token));
    }

    [Fact]
    public async Task GetContentByQueryAsync_WhenProviderThrowsCancellationWithoutRequest_SkipsTheProvider()
    {
        var provider = Substitute.For<IContentProvider>();
        provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());
        RegisterAll(provider, Provider(Item("a")));

        var result = await _manager.GetContentByQueryAsync(new ContentQuery());

        result.Select(item => item.Id).ShouldBe(["a"]);
    }

    [Fact]
    public async Task GetContentByQueryFromDefaultAsync_WithoutDefaultProvider_ReturnsEmpty()
    {
        _registry.GetDefaultProvider().Returns((IContentProvider?)null);

        var result = await _manager.GetContentByQueryFromDefaultAsync(new ContentQuery());

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetContentByQueryFromProviderAsync_UsesOnlyTheNamedProvider()
    {
        var provider = Provider(Item("n"));
        _registry.GetProvider("named").Returns(provider);

        var result = await _manager.GetContentByQueryFromProviderAsync("named", new ContentQuery());

        result.Select(item => item.Id).ShouldBe(["n"]);
    }

    [Fact]
    public async Task GetContentByQueryFromProviderAsync_WithUnknownProvider_ReturnsEmpty()
    {
        _registry.GetProvider("missing").Returns((IContentProvider?)null);

        var result = await _manager.GetContentByQueryFromProviderAsync("missing", new ContentQuery());

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetDirectoryTreeAsync_WithNoProviders_ReturnsEmpty()
    {
        RegisterAll();

        var result = await _manager.GetDirectoryTreeAsync();

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetDirectoryTreeAsync_CombinesProvidersAndSkipsFailures()
    {
        var first = Substitute.For<IContentProvider>();
        first.GetDirectoriesAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns([DirectoryItem.Create("d1", "p1", "Dir 1", "first")]);
        var failing = Substitute.For<IContentProvider>();
        failing.GetDirectoriesAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("boom"));
        var empty = Substitute.For<IContentProvider>();
        empty.GetDirectoriesAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new List<DirectoryItem>());
        RegisterAll(first, failing, empty);

        var result = await _manager.GetDirectoryTreeAsync("en");

        result.Select(directory => directory.Id).ShouldBe(["d1"]);
    }

    [Fact]
    public async Task GetDirectoryTreeAsync_WhenProviderObservesCancellation_RethrowsIt()
    {
        using var cts = new CancellationTokenSource();
        RegisterAll(ProviderThatCancels(cts));

        await Should.ThrowAsync<OperationCanceledException>(() => _manager.GetDirectoryTreeAsync(null, cts.Token));
    }

    [Fact]
    public async Task GetDirectoryTreeFromDefaultAsync_UsesTheDefaultProvider()
    {
        var provider = Substitute.For<IContentProvider>();
        provider.GetDirectoriesAsync("en", Arg.Any<CancellationToken>())
            .Returns([DirectoryItem.Create("d1", "p1", "Dir 1", "default")]);
        _registry.GetDefaultProvider().Returns(provider);

        var result = await _manager.GetDirectoryTreeFromDefaultAsync("en");

        result.Select(directory => directory.Id).ShouldBe(["d1"]);
    }

    [Fact]
    public async Task GetDirectoryTreeFromDefaultAsync_WithoutDefaultProvider_ReturnsEmpty()
    {
        _registry.GetDefaultProvider().Returns((IContentProvider?)null);

        var result = await _manager.GetDirectoryTreeFromDefaultAsync();

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetDirectoryTreeFromProviderAsync_UsesTheNamedProvider()
    {
        var provider = Substitute.For<IContentProvider>();
        provider.GetDirectoriesAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns([DirectoryItem.Create("d2", "p2", "Dir 2", "named")]);
        _registry.GetProvider("named").Returns(provider);

        var result = await _manager.GetDirectoryTreeFromProviderAsync("named");

        result.Select(directory => directory.Id).ShouldBe(["d2"]);
    }

    [Fact]
    public async Task GetDirectoryTreeFromProviderAsync_WithUnknownProvider_ReturnsEmpty()
    {
        _registry.GetProvider("missing").Returns((IContentProvider?)null);

        var result = await _manager.GetDirectoryTreeFromProviderAsync("missing");

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetLocalizedContentAsync_WithoutDefaultProvider_ReturnsNull()
    {
        _registry.GetDefaultProvider().Returns((IContentProvider?)null);

        var result = await _manager.GetLocalizedContentAsync("loc", "de");

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetLocalizedContentAsync_ReturnsTheRequestedLocaleIgnoringCase()
    {
        var provider = Provider(Item("en", locale: "en"), Item("de", locale: "de"));
        _registry.GetDefaultProvider().Returns(provider);

        var result = await _manager.GetLocalizedContentAsync("loc", "DE");

        result.ShouldNotBeNull().Id.ShouldBe("de");
    }

    [Fact]
    public async Task GetLocalizedContentAsync_WithoutTranslations_ReturnsNull()
    {
        var provider = Provider();
        _registry.GetDefaultProvider().Returns(provider);

        var result = await _manager.GetLocalizedContentAsync("loc", "de");

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetLocalizedContentAsync_WhenLocaleMissing_FallsBackToEnglish()
    {
        var provider = Provider(Item("fr", locale: "fr"), Item("en", locale: "en"));
        _registry.GetDefaultProvider().Returns(provider);

        var result = await _manager.GetLocalizedContentAsync("loc", "de");

        result.ShouldNotBeNull().Id.ShouldBe("en");
    }

    [Fact]
    public async Task GetLocalizedContentAsync_WhenLocaleMissing_FallsBackToTheProviderDefaultLocale()
    {
        var provider = Substitute.For<IContentProvider, IContentLocalization>();
        provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns([Item("en", locale: "en"), Item("fr", locale: "fr")]);
        ((IContentLocalization)provider).DefaultLocale.Returns("fr");
        _registry.GetDefaultProvider().Returns(provider);

        var result = await _manager.GetLocalizedContentAsync("loc", "de");

        result.ShouldNotBeNull().Id.ShouldBe("fr");
    }

    [Fact]
    public async Task GetLocalizedContentAsync_QueriesPublishedTranslationsOfTheLocalizationId()
    {
        var provider = Provider(Item("en", locale: "en"));
        _registry.GetDefaultProvider().Returns(provider);

        await _manager.GetLocalizedContentAsync("loc-1", "en");

        await provider.Received(1).GetItemsByQueryAsync(
            Arg.Is<ContentQuery>(q => q.LocalizationId == "loc-1" && !q.IncludeUnpublished),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetLocalizedContentAsync_WhenProviderFails_ReturnsNull()
    {
        var provider = Substitute.For<IContentProvider>();
        provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("boom"));
        _registry.GetDefaultProvider().Returns(provider);

        var result = await _manager.GetLocalizedContentAsync("loc", "de");

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetLocalizedContentAsync_WhenProviderObservesCancellation_RethrowsIt()
    {
        using var cts = new CancellationTokenSource();
        var provider = ProviderThatCancels(cts);
        _registry.GetDefaultProvider().Returns(provider);

        await Should.ThrowAsync<OperationCanceledException>(() => _manager.GetLocalizedContentAsync("loc", "de", cts.Token));
    }

    [Fact]
    public async Task GetLocalizedContentFromProviderAsync_UsesTheNamedProvider()
    {
        var provider = Provider(Item("de", locale: "de"));
        _registry.GetProvider("named").Returns(provider);

        var result = await _manager.GetLocalizedContentFromProviderAsync("named", "loc", "de");

        result.ShouldNotBeNull().Id.ShouldBe("de");
    }

    [Fact]
    public async Task GetLocalizedContentFromProviderAsync_WithUnknownProvider_ReturnsNull()
    {
        _registry.GetProvider("missing").Returns((IContentProvider?)null);

        var result = await _manager.GetLocalizedContentFromProviderAsync("missing", "loc", "de");

        result.ShouldBeNull();
    }
}
