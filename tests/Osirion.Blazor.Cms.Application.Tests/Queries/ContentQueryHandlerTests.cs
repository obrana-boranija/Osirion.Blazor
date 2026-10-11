using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Application.Queries.Content;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Application.Tests.Queries;

public class ContentQueryHandlerTests
{
    private readonly IContentProviderManager _providerManager = Substitute.For<IContentProviderManager>();
    private readonly IContentProvider _defaultProvider = Substitute.For<IContentProvider>();
    private readonly IContentProvider _namedProvider = Substitute.For<IContentProvider>();
    private readonly ContentItem _item = ContentItem.Create("post-1", "Post", "<p>Body</p>", "blog/post.md", "test");

    public ContentQueryHandlerTests()
    {
        _providerManager.GetDefaultProvider().Returns(_defaultProvider);
        _providerManager.GetProvider("github").Returns(_namedProvider);
    }

    [Fact]
    public async Task GetContentByIdQueryHandler_NoProviderId_ReadsFromTheDefaultProvider()
    {
        _defaultProvider.GetItemByIdAsync("post-1", Arg.Any<CancellationToken>()).Returns(_item);

        var result = await ByIdHandler().HandleAsync(new GetContentByIdQuery { Id = "post-1" });

        result.ShouldBeSameAs(_item);
    }

    [Fact]
    public async Task GetContentByIdQueryHandler_ProviderId_ReadsFromThatProvider()
    {
        _namedProvider.GetItemByIdAsync("post-1", Arg.Any<CancellationToken>()).Returns(_item);

        var result = await ByIdHandler().HandleAsync(new GetContentByIdQuery { Id = "post-1", ProviderId = "github" });

        result.ShouldBeSameAs(_item);
        await _defaultProvider.DidNotReceive().GetItemByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetContentByIdQueryHandler_UnknownProvider_ReturnsNull()
    {
        var result = await ByIdHandler().HandleAsync(new GetContentByIdQuery { Id = "post-1", ProviderId = "missing" });

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetContentByIdQueryHandler_ProviderFails_Rethrows()
    {
        _defaultProvider.GetItemByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(new IOException("offline"));

        await Should.ThrowAsync<IOException>(() => ByIdHandler().HandleAsync(new GetContentByIdQuery { Id = "post-1" }));
    }

    [Fact]
    public async Task GetContentByPathQueryHandler_NoProviderId_ReadsFromTheDefaultProvider()
    {
        _defaultProvider.GetItemByPathAsync("blog/post.md", Arg.Any<CancellationToken>()).Returns(_item);

        var result = await ByPathHandler().HandleAsync(new GetContentByPathQuery { Path = "blog/post.md" });

        result.ShouldBeSameAs(_item);
    }

    [Fact]
    public async Task GetContentByPathQueryHandler_ProviderId_ReadsFromThatProvider()
    {
        _namedProvider.GetItemByPathAsync("blog/post.md", Arg.Any<CancellationToken>()).Returns(_item);

        var result = await ByPathHandler().HandleAsync(new GetContentByPathQuery { Path = "blog/post.md", ProviderId = "github" });

        result.ShouldBeSameAs(_item);
    }

    [Fact]
    public async Task GetContentByPathQueryHandler_NoDefaultProvider_ReturnsNull()
    {
        _providerManager.GetDefaultProvider().Returns((IContentProvider?)null);

        var result = await ByPathHandler().HandleAsync(new GetContentByPathQuery { Path = "blog/post.md" });

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetContentByPathQueryHandler_ProviderFails_Rethrows()
    {
        _defaultProvider.GetItemByPathAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(new IOException("offline"));

        await Should.ThrowAsync<IOException>(() => ByPathHandler().HandleAsync(new GetContentByPathQuery { Path = "blog/post.md" }));
    }

    [Fact]
    public async Task SearchContentQueryHandler_NoProviderId_QueriesTheDefaultProvider()
    {
        var query = new ContentQuery();
        _defaultProvider.GetItemsByQueryAsync(query, Arg.Any<CancellationToken>()).Returns([_item]);

        var result = await SearchHandler().HandleAsync(new SearchContentQuery { Query = query });

        result.ShouldBe([_item]);
    }

    [Fact]
    public async Task SearchContentQueryHandler_ProviderId_QueriesThatProvider()
    {
        var query = new ContentQuery();
        _namedProvider.GetItemsByQueryAsync(query, Arg.Any<CancellationToken>()).Returns([_item]);

        var result = await SearchHandler().HandleAsync(new SearchContentQuery { Query = query, ProviderId = "github" });

        result.ShouldBe([_item]);
    }

    [Fact]
    public async Task SearchContentQueryHandler_ProviderReturnsNull_ReturnsAnEmptyList()
    {
        _defaultProvider.GetItemsByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<ContentItem>?)null);

        var result = await SearchHandler().HandleAsync(new SearchContentQuery());

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task SearchContentQueryHandler_UnknownProvider_ReturnsAnEmptyList()
    {
        var result = await SearchHandler().HandleAsync(new SearchContentQuery { ProviderId = "missing" });

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task SearchContentQueryHandler_ProviderFails_Rethrows()
    {
        _defaultProvider.GetItemsByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).ThrowsAsync(new IOException("offline"));

        await Should.ThrowAsync<IOException>(() => SearchHandler().HandleAsync(new SearchContentQuery()));
    }

    private GetContentByIdQueryHandler ByIdHandler() =>
        new(_providerManager, NullLogger<GetContentByIdQueryHandler>.Instance);

    private GetContentByPathQueryHandler ByPathHandler() =>
        new(_providerManager, NullLogger<GetContentByPathQueryHandler>.Instance);

    private SearchContentQueryHandler SearchHandler() =>
        new(_providerManager, NullLogger<SearchContentQueryHandler>.Instance);
}
