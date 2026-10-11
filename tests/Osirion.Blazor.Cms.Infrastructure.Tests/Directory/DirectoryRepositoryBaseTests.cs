using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Directory;

public class DirectoryRepositoryBaseTests
{
    private static DirectoryItem Item(string id, string path, string locale = "en")
    {
        var directory = DirectoryItem.Create(id, path, "Name " + id, "scripted");
        directory.SetLocale(locale);
        return directory;
    }

    private static ScriptedDirectoryRepositoryBase Repository(params DirectoryItem[] directories)
    {
        var repository = ScriptedDirectoryRepositoryBase.Create();
        repository.Directories.AddRange(directories);
        return repository;
    }

    [Fact]
    public async Task ReadMethods_WhenTheLoadFails_WrapTheErrorInAContentProviderException()
    {
        var failure = new InvalidOperationException("offline");
        var repository = Repository();
        repository.LoadFailure = failure;

        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetAllAsync())).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByIdAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByPathAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByUrlAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByNameAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByLocaleAsync("en"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetChildrenAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetTreeAsync())).InnerException.ShouldBeOfType<ContentProviderException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MethodsTakingAKey_BlankKey_ThrowArgumentException(string key)
    {
        var repository = Repository();

        await Should.ThrowAsync<ArgumentException>(() => repository.GetByIdAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.GetByPathAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.GetByUrlAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.GetChildrenAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.DeleteRecursiveAsync(key));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public async Task GetByNameAsync_BlankName_ReturnsNull(string? name)
    {
        var repository = Repository(Item("a", "a"));

        (await repository.GetByNameAsync(name)).ShouldBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_KnownAndUnknownId_ReturnsTheDirectoryOrNull()
    {
        var blog = Item("blog", "blog");
        var repository = Repository(blog);

        (await repository.GetByIdAsync("blog")).ShouldBeSameAs(blog);
        (await repository.GetByIdAsync("none")).ShouldBeNull();
    }

    [Fact]
    public async Task GetByPathAsync_PathWithBackslashesAndSlashes_MatchesTheNormalizedPath()
    {
        var guide = Item("guide", "docs/guide");
        var repository = Repository(Item("docs", "docs"), guide);

        (await repository.GetByPathAsync("\\docs\\guide/")).ShouldBeSameAs(guide);
    }

    [Fact]
    public async Task GetByUrlAsync_KnownUrl_ReturnsTheDirectory()
    {
        var blog = Item("blog", "blog");
        blog.SetUrl("/blog");
        var repository = Repository(blog, Item("docs", "docs"));

        (await repository.GetByUrlAsync("/blog")).ShouldBeSameAs(blog);
        (await repository.GetByUrlAsync("/none")).ShouldBeNull();
    }

    [Fact]
    public async Task GetByNameAsync_NoLocale_MatchesIgnoringCaseInTheDefaultLocale()
    {
        var english = Item("en", "en/blog", "en");
        var german = Item("de", "de/blog", "de");
        var repository = Repository(german, english);

        (await repository.GetByNameAsync("NAME EN")).ShouldBeSameAs(english);
        (await repository.GetByNameAsync("name de")).ShouldBeNull();
        (await repository.GetByNameAsync("name de", "DE")).ShouldBeSameAs(german);
    }

    [Fact]
    public async Task GetByLocaleAsync_EmptyCache_ReturnsEmpty()
    {
        var repository = Repository();

        (await repository.GetByLocaleAsync("en")).ShouldBeEmpty();
    }

    [Fact]
    public async Task GetByLocaleAsync_NoLocale_ReturnsTheRootDirectories()
    {
        var docs = Item("docs", "docs");
        var guide = Item("guide", "docs/guide");
        docs.AddChild(guide);
        var repository = Repository(docs, guide);

        (await repository.GetByLocaleAsync()).ShouldHaveSingleItem().ShouldBeSameAs(docs);
    }

    // Pins current behavior: with a locale, only nested directories are returned (Parent is not null), unlike
    // BaseDirectoryRepository, which returns the root directories of that locale.
    [Fact]
    public async Task GetByLocaleAsync_WithLocale_ReturnsTheNestedDirectoriesOfThatLocale()
    {
        var docs = Item("docs", "docs");
        var guide = Item("guide", "docs/guide");
        docs.AddChild(guide);
        var germanChild = Item("anleitung", "docs/anleitung", "de");
        docs.AddChild(germanChild);
        var repository = Repository(docs, guide, germanChild);

        (await repository.GetByLocaleAsync("en")).ShouldHaveSingleItem().ShouldBeSameAs(guide);
    }

    [Fact]
    public async Task GetTreeAsync_NoLocale_UsesTheDefaultLocale()
    {
        var docs = Item("docs", "docs");
        var guide = Item("guide", "docs/guide");
        docs.AddChild(guide);
        var repository = Repository(docs, guide);

        var tree = await repository.GetTreeAsync();

        tree.ShouldBe(await repository.GetByLocaleAsync("en"));
    }

    [Fact]
    public async Task GetChildrenAsync_KnownAndUnknownParent_ReturnsItsChildrenOrEmpty()
    {
        var docs = Item("docs", "docs");
        var guide = Item("guide", "docs/guide");
        docs.AddChild(guide);
        var repository = Repository(docs, guide);

        (await repository.GetChildrenAsync("docs")).ShouldHaveSingleItem().ShouldBeSameAs(guide);
        (await repository.GetChildrenAsync("none")).ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteRecursiveAsync_KnownId_DeletesRecursivelyAndRefreshesTheCache()
    {
        var repository = Repository(Item("a", "a"));

        await repository.DeleteRecursiveAsync("a", "remove a");

        repository.DeleteCalls.ShouldHaveSingleItem().ShouldBe(("a", true, "remove a"));
        await repository.CacheManagerSubstitute.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteRecursiveAsync_UnknownId_ThrowsDirectoryNotFound()
    {
        var repository = Repository();

        await Should.ThrowAsync<DirectoryNotFoundException>(() => repository.DeleteRecursiveAsync("missing"));
        repository.DeleteCalls.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteRecursiveAsync_ProviderFails_WrapsTheError()
    {
        var failure = new IOException("locked");
        var repository = Repository(Item("a", "a"));
        repository.DeleteFailure = failure;

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.DeleteRecursiveAsync("a"));

        exception.InnerException.ShouldBeSameAs(failure);
    }

    [Fact]
    public async Task RefreshCacheAsync_InvalidationFails_SwallowsTheError()
    {
        var repository = Repository();
        repository.CacheManagerSubstitute.InvalidateCacheAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("busy"));

        await Should.NotThrowAsync(() => repository.RefreshCacheAsync());
    }

    [Fact]
    public async Task RefreshCacheAsync_ReloadsWithForceRefresh()
    {
        var repository = Repository();

        await repository.RefreshCacheAsync();

        await repository.CacheManagerSubstitute.Received(1).GetCachedDirectoriesAsync(
            Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
            Arg.Any<CancellationToken>(),
            true);
    }
}
