using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;
using Shouldly;

namespace Osirion.Blazor.Cms.Application.Tests.Commands.Content;

public class ContentCommandHandlerTests
{
    private const string Markdown = "# Heading\n\nSome *emphasis* and a [link](https://example.com).";

    private readonly IUnitOfWorkFactory _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IContentRepository _repository = Substitute.For<IContentRepository>();
    private ContentItem? _saved;
    private string? _commitMessage;

    public ContentCommandHandlerTests()
    {
        _unitOfWorkFactory.CreateForDefaultProvider().Returns(_unitOfWork);
        _unitOfWork.ContentRepository.Returns(_repository);
        _unitOfWork.ProviderId.Returns("test");
        _repository
            .SaveWithCommitMessageAsync(Arg.Any<ContentItem>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                _saved = call.Arg<ContentItem>();
                _commitMessage = call.ArgAt<string>(1);
                return _saved;
            });
    }

    [Fact]
    public async Task CreateContentCommandHandler_NewContent_IsSavedPublishedWithTheMarkdownSource()
    {
        await CreateHandler().HandleAsync(new CreateContentCommand { Title = "Post", Content = Markdown, Path = "blog/post.md" });

        _saved.ShouldNotBeNull();
        _saved.IsPublished.ShouldBeTrue();
        _saved.OriginalMarkdown.ShouldBe(Markdown);
        _commitMessage.ShouldBe("Create content: Post");
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateContentCommandHandler_OptionalFields_AreCopiedToTheSavedContent()
    {
        var command = new CreateContentCommand
        {
            Title = "Post",
            Content = Markdown,
            Path = "blog/post.md",
            Author = "Ana",
            Description = "Summary",
            Slug = "my-post",
            Locale = "sr",
            ContentId = "post-group",
            IsFeatured = true,
            Tags = ["dotnet", "blazor"],
            Categories = ["news"],
            CommitMessage = "Add post"
        };

        await CreateHandler().HandleAsync(command);

        _saved.ShouldNotBeNull();
        _saved.Author.ShouldBe("Ana");
        _saved.Description.ShouldBe("Summary");
        _saved.Slug.ShouldBe("my-post");
        _saved.Locale.ShouldBe("sr");
        _saved.ContentId.ShouldBe("post-group");
        _saved.IsFeatured.ShouldBeTrue();
        _saved.Tags.ShouldBe(["dotnet", "blazor"]);
        _saved.Categories.ShouldBe(["news"]);
        _saved.ProviderId.ShouldBe("test");
        _commitMessage.ShouldBe("Add post");
    }

    [Fact]
    public async Task CreateContentCommandHandler_ProviderId_UsesThatProvidersUnitOfWork()
    {
        _unitOfWorkFactory.Create("github").Returns(_unitOfWork);

        await CreateHandler().HandleAsync(new CreateContentCommand
        {
            Title = "Post", Content = Markdown, Path = "blog/post.md", ProviderId = "github"
        });

        _unitOfWorkFactory.Received(1).Create("github");
        _unitOfWorkFactory.DidNotReceive().CreateForDefaultProvider();
    }

    [Fact]
    public async Task CreateContentCommandHandler_SaveFails_RollsBackAndRethrows()
    {
        _repository
            .SaveWithCommitMessageAsync(Arg.Any<ContentItem>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("storage down"));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            CreateHandler().HandleAsync(new CreateContentCommand { Title = "Post", Content = Markdown, Path = "blog/post.md" }));

        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateContentCommandHandler_ExistingContent_IsSavedWithTheMarkdownSource()
    {
        GivenExisting("post-1");

        await UpdateHandler().HandleAsync(new UpdateContentCommand { Id = "post-1", Title = "Post", Content = Markdown });

        _saved.ShouldNotBeNull();
        _saved.OriginalMarkdown.ShouldBe(Markdown);
        _commitMessage.ShouldBe("Update content: Post");
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateContentCommandHandler_OptionalFields_ReplaceTheExistingValues()
    {
        var existing = GivenExisting("post-1");
        existing.AddTag("old-tag");
        existing.AddCategory("old-category");
        var command = new UpdateContentCommand
        {
            Id = "post-1",
            Title = "Renamed",
            Content = Markdown,
            Path = "blog/renamed.md",
            Author = "Ana",
            Description = "Summary",
            Slug = "renamed",
            Locale = "en",
            ContentId = "post-group",
            IsFeatured = true,
            ProviderSpecificId = "sha-2",
            Tags = ["new-tag"],
            Categories = ["new-category"],
            CommitMessage = "Rename post"
        };

        await UpdateHandler().HandleAsync(command);

        _saved.ShouldNotBeNull();
        _saved.Title.ShouldBe("Renamed");
        _saved.Path.ShouldBe("blog/renamed.md");
        _saved.Author.ShouldBe("Ana");
        _saved.Description.ShouldBe("Summary");
        _saved.Slug.ShouldBe("renamed");
        _saved.Locale.ShouldBe("en");
        _saved.ContentId.ShouldBe("post-group");
        _saved.IsFeatured.ShouldBeTrue();
        _saved.ProviderSpecificId.ShouldBe("sha-2");
        _saved.Tags.ShouldBe(["new-tag"]);
        _saved.Categories.ShouldBe(["new-category"]);
        _commitMessage.ShouldBe("Rename post");
    }

    [Fact]
    public async Task UpdateContentCommandHandler_BlankOptionalFields_KeepTheExistingValues()
    {
        var existing = GivenExisting("post-1");
        existing.SetAuthor("Ana");
        existing.SetSlug("kept");

        await UpdateHandler().HandleAsync(new UpdateContentCommand
        {
            Id = "post-1", Title = "Post", Content = Markdown, Path = " ", Author = " ", Slug = null
        });

        _saved.ShouldNotBeNull();
        _saved.Path.ShouldBe("blog/post.md");
        _saved.Author.ShouldBe("Ana");
        _saved.Slug.ShouldBe("kept");
    }

    [Fact]
    public async Task UpdateContentCommandHandler_ProviderId_UsesThatProvidersUnitOfWork()
    {
        _unitOfWorkFactory.Create("github").Returns(_unitOfWork);
        GivenExisting("post-1");

        await UpdateHandler().HandleAsync(new UpdateContentCommand
        {
            Id = "post-1", Title = "Post", Content = Markdown, ProviderId = "github"
        });

        _unitOfWorkFactory.Received(1).Create("github");
        _unitOfWorkFactory.DidNotReceive().CreateForDefaultProvider();
    }

    [Fact]
    public async Task UpdateContentCommandHandler_MissingContent_RollsBackAndThrowsNotFound()
    {
        await Should.ThrowAsync<ContentItemNotFoundException>(() =>
            UpdateHandler().HandleAsync(new UpdateContentCommand { Id = "missing", Title = "Post", Content = Markdown }));

        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _repository.DidNotReceive()
            .SaveWithCommitMessageAsync(Arg.Any<ContentItem>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateContentCommandHandler_SaveFails_RollsBackAndRethrows()
    {
        GivenExisting("post-1");
        _repository
            .SaveWithCommitMessageAsync(Arg.Any<ContentItem>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("storage down"));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            UpdateHandler().HandleAsync(new UpdateContentCommand { Id = "post-1", Title = "Post", Content = Markdown }));

        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteContentCommandHandler_ExistingContent_IsDeletedWithTheDefaultCommitMessage()
    {
        GivenExisting("post-1");

        await DeleteHandler().HandleAsync(new DeleteContentCommand { Id = "post-1" });

        await _repository.Received(1).DeleteWithCommitMessageAsync("post-1", "Delete content: post-1", Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteContentCommandHandler_CommitMessageAndProvider_AreUsed()
    {
        _unitOfWorkFactory.Create("github").Returns(_unitOfWork);
        GivenExisting("post-1");

        await DeleteHandler().HandleAsync(new DeleteContentCommand { Id = "post-1", ProviderId = "github", CommitMessage = "Remove post" });

        _unitOfWorkFactory.Received(1).Create("github");
        await _repository.Received(1).DeleteWithCommitMessageAsync("post-1", "Remove post", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteContentCommandHandler_MissingContent_RollsBackAndThrowsNotFound()
    {
        await Should.ThrowAsync<ContentItemNotFoundException>(() =>
            DeleteHandler().HandleAsync(new DeleteContentCommand { Id = "missing" }));

        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().DeleteWithCommitMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteContentCommandHandler_DeleteFails_RollsBackAndRethrows()
    {
        GivenExisting("post-1");
        _repository
            .DeleteWithCommitMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("storage down"));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            DeleteHandler().HandleAsync(new DeleteContentCommand { Id = "post-1" }));

        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    private ContentItem GivenExisting(string id)
    {
        var existing = ContentItem.Create(id, "Post", "<p>Old</p>", "blog/post.md", "test");
        existing.SetOriginalMarkdown("Old");
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        return existing;
    }

    private CreateContentCommandHandler CreateHandler() =>
        new(_unitOfWorkFactory, NullLogger<CreateContentCommandHandler>.Instance);

    private UpdateContentCommandHandler UpdateHandler() =>
        new(_unitOfWorkFactory, NullLogger<UpdateContentCommandHandler>.Instance);

    private DeleteContentCommandHandler DeleteHandler() =>
        new(_unitOfWorkFactory, NullLogger<DeleteContentCommandHandler>.Instance);
}
