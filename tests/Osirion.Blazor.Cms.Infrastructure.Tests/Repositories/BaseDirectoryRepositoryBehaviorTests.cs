using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Repositories;

public class BaseDirectoryRepositoryBehaviorTests
{
    private static DirectoryItem Directory(string id, string path, string? locale = null)
    {
        var directory = DirectoryItem.Create(id, path, "Name " + id, "scripted");
        if (locale is not null)
        {
            directory.SetLocale(locale);
        }

        return directory;
    }

    [Fact]
    public void Constructor_NullArgument_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new ScriptedDirectoryRepository(null!, new ScriptedDirectoryOptions(), NullLogger.Instance));
        Should.Throw<ArgumentNullException>(() => new ScriptedDirectoryRepository("p", null!, NullLogger.Instance));
        Should.Throw<ArgumentNullException>(() => new ScriptedDirectoryRepository("p", new ScriptedDirectoryOptions(), null!));
    }

    [Fact]
    public async Task ReadMethods_WhenTheLoadFails_WrapTheErrorInAContentProviderException()
    {
        var failure = new InvalidOperationException("offline");
        var repository = new ScriptedDirectoryRepository { LoadFailure = failure };

        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetAllAsync())).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByIdAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByPathAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByLocaleAsync("en"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetChildrenAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByUrlAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetByNameAsync("a"))).InnerException.ShouldBeSameAs(failure);
        (await Should.ThrowAsync<ContentProviderException>(() => repository.GetTreeAsync())).InnerException.ShouldBeOfType<ContentProviderException>();
    }

    [Fact]
    public async Task ReadMethods_WhenTheCacheStaysUnset_ReturnEmptyResults()
    {
        var repository = new ScriptedDirectoryRepository(Directory("a", "a")) { LeaveCacheUnset = true };

        (await repository.GetAllAsync()).ShouldBeEmpty();
        (await repository.GetByIdAsync("a")).ShouldBeNull();
        (await repository.GetByPathAsync("a")).ShouldBeNull();
        (await repository.GetByLocaleAsync()).ShouldBeEmpty();
        (await repository.GetChildrenAsync("a")).ShouldBeEmpty();
        (await repository.GetByUrlAsync("a")).ShouldBeNull();
        (await repository.GetByNameAsync("Name a")).ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MethodsTakingAKey_BlankKey_ThrowArgumentException(string key)
    {
        var repository = new ScriptedDirectoryRepository();

        await Should.ThrowAsync<ArgumentException>(() => repository.GetByIdAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.GetByPathAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.GetChildrenAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.GetByUrlAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.DeleteRecursiveAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.DeleteAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.MoveAsync(key, null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetByNameAsync_BlankName_ReturnsNullWithoutLoading(string? name)
    {
        var repository = new ScriptedDirectoryRepository { LoadFailure = new InvalidOperationException("must not load") };

        (await repository.GetByNameAsync(name)).ShouldBeNull();
    }

    [Fact]
    public async Task GetByNameAsync_WithAndWithoutLocale_MatchesTheNameAndOptionalLocale()
    {
        var english = Directory("en", "en/blog", "en");
        var german = Directory("de", "de/blog", "de");
        german.SetName(english.Name);
        var repository = new ScriptedDirectoryRepository(english, german);

        (await repository.GetByNameAsync(english.Name, "de")).ShouldBeSameAs(german);
        (await repository.GetByNameAsync(english.Name)).ShouldNotBeNull();
        (await repository.GetByNameAsync(english.Name, "fr")).ShouldBeNull();
    }

    [Fact]
    public async Task GetByUrlAsync_KnownUrl_ReturnsTheDirectory()
    {
        var blog = Directory("blog", "blog");
        blog.SetUrl("/blog");
        var repository = new ScriptedDirectoryRepository(blog, Directory("docs", "docs"));

        (await repository.GetByUrlAsync("/blog")).ShouldBeSameAs(blog);
        (await repository.GetByUrlAsync("/none")).ShouldBeNull();
    }

    [Fact]
    public async Task GetChildrenAsync_UnknownParent_ReturnsEmpty()
    {
        var repository = new ScriptedDirectoryRepository(Directory("a", "a"));

        (await repository.GetChildrenAsync("missing")).ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_UnknownId_ThrowsDirectoryNotFound()
    {
        var repository = new ScriptedDirectoryRepository();

        await Should.ThrowAsync<DirectoryNotFoundException>(() => repository.DeleteAsync("missing"));
        repository.DeleteCalls.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteRecursiveAsync_UnknownId_ThrowsDirectoryNotFound()
    {
        var repository = new ScriptedDirectoryRepository();

        await Should.ThrowAsync<DirectoryNotFoundException>(() => repository.DeleteRecursiveAsync("missing"));
        repository.DeleteCalls.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ProviderFails_WrapsTheError()
    {
        var failure = new IOException("locked");
        var repository = new ScriptedDirectoryRepository(Directory("a", "a")) { DeleteFailure = failure };

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.DeleteAsync("a"));

        exception.InnerException.ShouldBeSameAs(failure);
    }

    [Fact]
    public async Task DeleteRecursiveAsync_ProviderFails_WrapsTheError()
    {
        var failure = new IOException("locked");
        var repository = new ScriptedDirectoryRepository(Directory("a", "a")) { DeleteFailure = failure };

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.DeleteRecursiveAsync("a", "msg"));

        exception.InnerException.ShouldBeSameAs(failure);
    }

    [Fact]
    public async Task DeleteAsync_KnownId_RaisesANonRecursiveDeletedEvent()
    {
        var directory = Directory("a", "a");
        var repository = new ScriptedDirectoryRepository(directory);

        await repository.DeleteAsync("a");

        repository.DeleteCalls.ShouldHaveSingleItem().ShouldBe(("a", false, null));
        directory.DomainEvents.OfType<DirectoryDeletedEvent>().ShouldHaveSingleItem();
    }

    [Fact]
    public async Task MoveAsync_ToRoot_KeepsOnlyTheFolderNameAndClearsTheParent()
    {
        var parent = Directory("p", "docs");
        var child = Directory("c", "docs/guide");
        parent.AddChild(child);
        var repository = new ScriptedDirectoryRepository(parent, child);

        var moved = await repository.MoveAsync("c", null);

        moved.Path.ShouldBe("guide");
        moved.Parent.ShouldBeNull();
        moved.DomainEvents.OfType<DirectoryUpdatedEvent>().ShouldHaveSingleItem();
        repository.Saved.ShouldHaveSingleItem().ShouldBeSameAs(moved);
    }

    [Fact]
    public async Task MoveAsync_ToAnotherParent_CombinesTheParentPathAndTheFolderName()
    {
        var target = Directory("t", "archive\\2024");
        var source = Directory("s", "blog/post-folder");
        var repository = new ScriptedDirectoryRepository(target, source);

        var moved = await repository.MoveAsync("s", "t");

        moved.Path.ShouldBe("archive/2024/post-folder");
        moved.Parent.ShouldNotBeNull().Id.ShouldBe("t");
    }

    [Fact]
    public async Task MoveAsync_UnknownDirectory_ThrowsContentProviderExceptionWrappingNotFound()
    {
        var repository = new ScriptedDirectoryRepository(Directory("t", "t"));

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync("missing", "t"));

        exception.InnerException.ShouldBeOfType<DirectoryNotFoundException>();
        repository.Saved.ShouldBeEmpty();
    }

    [Fact]
    public async Task MoveAsync_UnknownNewParent_ThrowsContentProviderExceptionWrappingNotFound()
    {
        var repository = new ScriptedDirectoryRepository(Directory("s", "s"));

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync("s", "missing"));

        exception.InnerException.ShouldBeOfType<DirectoryNotFoundException>();
        repository.Saved.ShouldBeEmpty();
    }

    [Fact]
    public async Task MoveAsync_IntoItsOwnDescendant_IsRefused()
    {
        var parent = Directory("p", "docs");
        var child = Directory("c", "docs/guide");
        parent.AddChild(child);
        var repository = new ScriptedDirectoryRepository(parent, child);

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync("p", "c"));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        repository.Saved.ShouldBeEmpty();
    }

    [Fact]
    public async Task SaveAsync_ExistingDirectory_RaisesAnUpdatedEvent()
    {
        var directory = Directory("a", "a");
        directory.SetProviderSpecificId("a/_index.md");
        var repository = new ScriptedDirectoryRepository();

        var saved = await repository.SaveAsync(directory);

        saved.DomainEvents.OfType<DirectoryUpdatedEvent>().ShouldHaveSingleItem();
        saved.DomainEvents.OfType<DirectoryCreatedEvent>().ShouldBeEmpty();
    }

    [Fact]
    public async Task SaveAsync_ProviderFails_WrapsTheError()
    {
        var failure = new IOException("disk full");
        var repository = new ScriptedDirectoryRepository { SaveFailure = failure };

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.SaveAsync(Directory("a", "a")));

        exception.InnerException.ShouldBeSameAs(failure);
    }

    [Fact]
    public async Task SaveAsync_Null_Throws()
    {
        var repository = new ScriptedDirectoryRepository();

        await Should.ThrowAsync<ArgumentNullException>(() => repository.SaveAsync(null!));
    }
}
