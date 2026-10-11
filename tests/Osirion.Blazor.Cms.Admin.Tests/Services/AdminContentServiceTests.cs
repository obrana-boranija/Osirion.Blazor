using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Application.Queries.Content;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Services;

public class AdminContentServiceTests
{
    private readonly ICommandDispatcher _commands = Substitute.For<ICommandDispatcher>();
    private readonly IQueryDispatcher _queries = Substitute.For<IQueryDispatcher>();
    private readonly CmsEventMediator _mediator = new(NullLogger<CmsEventMediator>.Instance);
    private readonly RecordingLogger<AdminContentService> _logger = new();
    private readonly List<ErrorOccurredEvent> _errors;
    private readonly List<ContentSavedEvent> _saved;
    private readonly List<ContentDeletedEvent> _deleted;
    private readonly List<StatusNotificationEvent> _notifications;
    private readonly AdminContentService _service;

    public AdminContentServiceTests()
    {
        _errors = EventRecorder.Record<ErrorOccurredEvent>(_mediator);
        _saved = EventRecorder.Record<ContentSavedEvent>(_mediator);
        _deleted = EventRecorder.Record<ContentDeletedEvent>(_mediator);
        _notifications = EventRecorder.Record<StatusNotificationEvent>(_mediator);
        _service = new AdminContentService(_commands, _queries, _mediator, _logger);
    }

    private void ReturnContentById(string id, ContentItem? item) =>
        _queries.DispatchAsync<GetContentByIdQuery, ContentItem?>(Arg.Is<GetContentByIdQuery>(query => query.Id == id), Arg.Any<CancellationToken>())
            .Returns(item);

    [Fact]
    public void Constructor_NullDependencies_Throw()
    {
        Should.Throw<ArgumentNullException>(() => new AdminContentService(null!, _queries, _mediator, _logger)).ParamName.ShouldBe("commandDispatcher");
        Should.Throw<ArgumentNullException>(() => new AdminContentService(_commands, null!, _mediator, _logger)).ParamName.ShouldBe("queryDispatcher");
        Should.Throw<ArgumentNullException>(() => new AdminContentService(_commands, _queries, null!, _logger)).ParamName.ShouldBe("eventMediator");
        Should.Throw<ArgumentNullException>(() => new AdminContentService(_commands, _queries, _mediator, null!)).ParamName.ShouldBe("logger");
    }

    [Fact]
    public async Task GetContentByIdAsync_Found_ReturnsTheItemAndPassesTheProvider()
    {
        var item = new ContentItem { Path = "blog/post.md" };
        _queries.DispatchAsync<GetContentByIdQuery, ContentItem?>(
                Arg.Is<GetContentByIdQuery>(query => query.Id == "1" && query.ProviderId == "github"), Arg.Any<CancellationToken>())
            .Returns(item);

        var result = await _service.GetContentByIdAsync("1", "github");

        result.ShouldBeSameAs(item);
        _logger.HasEntry(LogLevel.Information, "Content found with ID: 1").ShouldBeTrue();
    }

    [Fact]
    public async Task GetContentByIdAsync_NotFound_ReturnsNullAndWarns()
    {
        ReturnContentById("1", null);

        var result = await _service.GetContentByIdAsync("1");

        result.ShouldBeNull();
        _logger.HasEntry(LogLevel.Warning, "Content not found with ID: 1").ShouldBeTrue();
    }

    [Fact]
    public async Task GetContentByIdAsync_QueryFails_PublishesAnErrorAndRethrows()
    {
        var failure = new InvalidOperationException("offline");
        _queries.DispatchAsync<GetContentByIdQuery, ContentItem?>(Arg.Any<GetContentByIdQuery>(), Arg.Any<CancellationToken>()).ThrowsAsync(failure);

        await Should.ThrowAsync<InvalidOperationException>(() => _service.GetContentByIdAsync("1"));

        var error = _errors.ShouldHaveSingleItem();
        error.Message.ShouldBe("Failed to load content: offline");
        error.Exception.ShouldBeSameAs(failure);
    }

    [Fact]
    public async Task SearchContentAsync_ReturnsTheResults()
    {
        var searchQuery = new ContentQuery();
        IReadOnlyList<ContentItem> items = [new ContentItem()];
        _queries.DispatchAsync<SearchContentQuery, IReadOnlyList<ContentItem>>(
                Arg.Is<SearchContentQuery>(query => query.Query == searchQuery && query.ProviderId == "github"), Arg.Any<CancellationToken>())
            .Returns(items);

        var result = await _service.SearchContentAsync(searchQuery, "github");

        result.ShouldBeSameAs(items);
        _logger.HasEntry(LogLevel.Information, "Found 1 content items").ShouldBeTrue();
    }

    [Fact]
    public async Task SearchContentAsync_NullResult_ReturnsAnEmptyList()
    {
        _queries.DispatchAsync<SearchContentQuery, IReadOnlyList<ContentItem>>(Arg.Any<SearchContentQuery>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<ContentItem>)null!);

        var result = await _service.SearchContentAsync(new ContentQuery());

        result.ShouldBeEmpty();
        _logger.HasEntry(LogLevel.Information, "Found 0 content items").ShouldBeTrue();
    }

    [Fact]
    public async Task SearchContentAsync_QueryFails_PublishesAnErrorAndRethrows()
    {
        _queries.DispatchAsync<SearchContentQuery, IReadOnlyList<ContentItem>>(Arg.Any<SearchContentQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("offline"));

        await Should.ThrowAsync<InvalidOperationException>(() => _service.SearchContentAsync(new ContentQuery()));

        _errors.ShouldHaveSingleItem().Message.ShouldBe("Failed to search content: offline");
    }

    [Fact]
    public async Task CreateContentAsync_Defaults_DispatchesTheCommandAndReturnsTheCreatedItem()
    {
        CreateContentCommand? dispatched = null;
        _commands.DispatchAsync(Arg.Do<CreateContentCommand>(command =>
        {
            dispatched = command;
            command.ContentId = "new-id";
        }), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var created = new ContentItem { Path = "blog/post.md" };
        ReturnContentById("new-id", created);

        var result = await _service.CreateContentAsync("Title", "Body", "blog/post.md");

        result.ShouldBeSameAs(created);
        dispatched.ShouldNotBeNull();
        dispatched.Title.ShouldBe("Title");
        dispatched.Content.ShouldBe("Body");
        dispatched.Tags.ShouldBeEmpty();
        dispatched.Categories.ShouldBeEmpty();
        dispatched.CommitMessage.ShouldBe("Create post.md");
        _saved.ShouldHaveSingleItem().Path.ShouldBe("blog/post.md");
        _notifications.ShouldHaveSingleItem().Type.ShouldBe(StatusType.Success);
    }

    [Fact]
    public async Task CreateContentAsync_AllArguments_AreCopiedToTheCommand()
    {
        CreateContentCommand? dispatched = null;
        _commands.DispatchAsync(Arg.Do<CreateContentCommand>(command => dispatched = command), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        await _service.CreateContentAsync("Title", "Body", "post.md", "Ann", "Desc", "slug", ["t"], ["c"], true, "github", "Custom message");

        dispatched.ShouldNotBeNull();
        dispatched.Author.ShouldBe("Ann");
        dispatched.Description.ShouldBe("Desc");
        dispatched.Slug.ShouldBe("slug");
        dispatched.Tags.ShouldBe(["t"]);
        dispatched.Categories.ShouldBe(["c"]);
        dispatched.IsFeatured.ShouldBeTrue();
        dispatched.ProviderId.ShouldBe("github");
        dispatched.CommitMessage.ShouldBe("Custom message");
    }

    [Fact]
    public async Task CreateContentAsync_CommandFails_PublishesAnErrorAndRethrows()
    {
        _commands.DispatchAsync(Arg.Any<CreateContentCommand>(), Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("conflict"));

        await Should.ThrowAsync<InvalidOperationException>(() => _service.CreateContentAsync("Title", "Body", "post.md"));

        _errors.ShouldHaveSingleItem().Message.ShouldBe("Failed to create content: conflict");
        _saved.ShouldBeEmpty();
    }

    [Fact]
    public async Task UpdateContentAsync_Defaults_DispatchesTheCommandAndReturnsTheUpdatedItem()
    {
        UpdateContentCommand? dispatched = null;
        _commands.DispatchAsync(Arg.Do<UpdateContentCommand>(command => dispatched = command), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var updated = new ContentItem { Path = "blog/post.md" };
        ReturnContentById("1", updated);

        var result = await _service.UpdateContentAsync("1", "Title", "Body", "blog/post.md");

        result.ShouldBeSameAs(updated);
        dispatched.ShouldNotBeNull();
        dispatched.Id.ShouldBe("1");
        dispatched.Tags.ShouldBeEmpty();
        dispatched.CommitMessage.ShouldBe("Update post.md");
        _saved.ShouldHaveSingleItem().Path.ShouldBe("blog/post.md");
        _notifications.ShouldHaveSingleItem().Message.ShouldBe("Content updated successfully");
    }

    [Fact]
    public async Task UpdateContentAsync_AllArguments_AreCopiedToTheCommand()
    {
        UpdateContentCommand? dispatched = null;
        _commands.DispatchAsync(Arg.Do<UpdateContentCommand>(command => dispatched = command), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        await _service.UpdateContentAsync("1", "Title", "Body", "post.md", "Ann", "Desc", "slug", ["t"], ["c"], true, "github", "Custom message");

        dispatched.ShouldNotBeNull();
        dispatched.Author.ShouldBe("Ann");
        dispatched.Description.ShouldBe("Desc");
        dispatched.Slug.ShouldBe("slug");
        dispatched.Categories.ShouldBe(["c"]);
        dispatched.IsFeatured.ShouldBeTrue();
        dispatched.ProviderId.ShouldBe("github");
        dispatched.CommitMessage.ShouldBe("Custom message");
    }

    [Fact]
    public async Task UpdateContentAsync_CommandFails_PublishesAnErrorAndRethrows()
    {
        _commands.DispatchAsync(Arg.Any<UpdateContentCommand>(), Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("conflict"));

        await Should.ThrowAsync<InvalidOperationException>(() => _service.UpdateContentAsync("1", "Title", "Body", "post.md"));

        _errors.ShouldHaveSingleItem().Message.ShouldBe("Failed to update content: conflict");
    }

    [Fact]
    public async Task DeleteContentAsync_ExistingContent_DispatchesAndPublishesTheDeletedPath()
    {
        DeleteContentCommand? dispatched = null;
        _commands.DispatchAsync(Arg.Do<DeleteContentCommand>(command => dispatched = command), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        ReturnContentById("1", new ContentItem { Path = "blog/post.md" });

        await _service.DeleteContentAsync("1", "github");

        dispatched.ShouldNotBeNull();
        dispatched.Id.ShouldBe("1");
        dispatched.ProviderId.ShouldBe("github");
        dispatched.CommitMessage.ShouldBe("Delete content 1");
        _deleted.ShouldHaveSingleItem().Path.ShouldBe("blog/post.md");
        _notifications.ShouldHaveSingleItem().Message.ShouldBe("Content deleted successfully");
    }

    [Fact]
    public async Task DeleteContentAsync_UnknownContent_DispatchesWithoutADeletedEvent()
    {
        DeleteContentCommand? dispatched = null;
        _commands.DispatchAsync(Arg.Do<DeleteContentCommand>(command => dispatched = command), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        ReturnContentById("1", null);

        await _service.DeleteContentAsync("1", commitMessage: "Remove it");

        dispatched!.CommitMessage.ShouldBe("Remove it");
        _deleted.ShouldBeEmpty();
        _notifications.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task DeleteContentAsync_CommandFails_PublishesAnErrorAndRethrows()
    {
        ReturnContentById("1", null);
        _commands.DispatchAsync(Arg.Any<DeleteContentCommand>(), Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("denied"));

        await Should.ThrowAsync<InvalidOperationException>(() => _service.DeleteContentAsync("1"));

        _errors.ShouldHaveSingleItem().Message.ShouldBe("Failed to delete content: denied");
    }
}
