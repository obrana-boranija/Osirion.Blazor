using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Application.Queries.Directory;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Application.Tests.Queries;

public class DirectoryQueryHandlerTests
{
    private readonly IContentProviderManager _providerManager = Substitute.For<IContentProviderManager>();
    private readonly IContentProvider _defaultProvider = Substitute.For<IContentProvider>();
    private readonly IContentProvider _namedProvider = Substitute.For<IContentProvider>();
    private readonly DirectoryItem _directory = DirectoryItem.Create("blog", "blog", "Blog", "test");

    public DirectoryQueryHandlerTests()
    {
        _providerManager.GetDefaultProvider().Returns(_defaultProvider);
        _providerManager.GetProvider("github").Returns(_namedProvider);
    }

    [Fact]
    public void Constructors_NullDependencies_Throw()
    {
        Should.Throw<ArgumentNullException>(() => new GetDirectoryTreeQueryHandler(null!, NullLogger<GetDirectoryTreeQueryHandler>.Instance));
        Should.Throw<ArgumentNullException>(() => new GetDirectoryTreeQueryHandler(_providerManager, null!));
        Should.Throw<ArgumentNullException>(() => new GetDirectoryByPathQueryHandler(null!, NullLogger<GetDirectoryByPathQueryHandler>.Instance));
        Should.Throw<ArgumentNullException>(() => new GetDirectoryByPathQueryHandler(_providerManager, null!));
    }

    [Fact]
    public async Task GetDirectoryTreeQueryHandler_NoProviderId_PassesTheLocaleToTheDefaultProvider()
    {
        _defaultProvider.GetDirectoriesAsync("sr", Arg.Any<CancellationToken>()).Returns([_directory]);

        var result = await TreeHandler().HandleAsync(new GetDirectoryTreeQuery { Locale = "sr" });

        result.ShouldBe([_directory]);
    }

    [Fact]
    public async Task GetDirectoryTreeQueryHandler_ProviderId_ReadsFromThatProvider()
    {
        _namedProvider.GetDirectoriesAsync(null, Arg.Any<CancellationToken>()).Returns([_directory]);

        var result = await TreeHandler().HandleAsync(new GetDirectoryTreeQuery { ProviderId = "github" });

        result.ShouldBe([_directory]);
    }

    [Fact]
    public async Task GetDirectoryTreeQueryHandler_UnknownProvider_ReturnsAnEmptyList()
    {
        var result = await TreeHandler().HandleAsync(new GetDirectoryTreeQuery { ProviderId = "missing" });

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetDirectoryTreeQueryHandler_ProviderFails_Rethrows()
    {
        _defaultProvider.GetDirectoriesAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>()).ThrowsAsync(new IOException("offline"));

        await Should.ThrowAsync<IOException>(() => TreeHandler().HandleAsync(new GetDirectoryTreeQuery()));
    }

    [Fact]
    public async Task GetDirectoryByPathQueryHandler_NoProviderId_ReadsFromTheDefaultProvider()
    {
        _defaultProvider.GetDirectoryByPathAsync("blog", Arg.Any<CancellationToken>()).Returns(_directory);

        var result = await ByPathHandler().HandleAsync(new GetDirectoryByPathQuery { Path = "blog" });

        result.ShouldBeSameAs(_directory);
    }

    [Fact]
    public async Task GetDirectoryByPathQueryHandler_ProviderId_ReadsFromThatProvider()
    {
        _namedProvider.GetDirectoryByPathAsync("blog", Arg.Any<CancellationToken>()).Returns(_directory);

        var result = await ByPathHandler().HandleAsync(new GetDirectoryByPathQuery { Path = "blog", ProviderId = "github" });

        result.ShouldBeSameAs(_directory);
    }

    [Fact]
    public async Task GetDirectoryByPathQueryHandler_UnknownProvider_ReturnsNull()
    {
        var result = await ByPathHandler().HandleAsync(new GetDirectoryByPathQuery { Path = "blog", ProviderId = "missing" });

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetDirectoryByPathQueryHandler_ProviderFails_Rethrows()
    {
        _defaultProvider.GetDirectoryByPathAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(new IOException("offline"));

        await Should.ThrowAsync<IOException>(() => ByPathHandler().HandleAsync(new GetDirectoryByPathQuery { Path = "blog" }));
    }

    private GetDirectoryTreeQueryHandler TreeHandler() =>
        new(_providerManager, NullLogger<GetDirectoryTreeQueryHandler>.Instance);

    private GetDirectoryByPathQueryHandler ByPathHandler() =>
        new(_providerManager, NullLogger<GetDirectoryByPathQueryHandler>.Instance);
}
