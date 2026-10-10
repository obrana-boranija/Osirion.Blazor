using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Domain.Entities;
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
                return _saved;
            });
    }

    [Fact]
    public async Task CreateContentCommandHandler_NewContent_IsSavedPublishedWithTheMarkdownSource()
    {
        var handler = new CreateContentCommandHandler(_unitOfWorkFactory, NullLogger<CreateContentCommandHandler>.Instance);

        await handler.HandleAsync(new CreateContentCommand { Title = "Post", Content = Markdown, Path = "blog/post.md" });

        _saved.ShouldNotBeNull();
        _saved.IsPublished.ShouldBeTrue();
        _saved.OriginalMarkdown.ShouldBe(Markdown);
    }

    [Fact]
    public async Task UpdateContentCommandHandler_ExistingContent_IsSavedWithTheMarkdownSource()
    {
        var existing = ContentItem.Create("post-1", "Post", "<p>Old</p>", "blog/post.md", "test");
        existing.SetOriginalMarkdown("Old");
        _repository.GetByIdAsync("post-1", Arg.Any<CancellationToken>()).Returns(existing);
        var handler = new UpdateContentCommandHandler(_unitOfWorkFactory, NullLogger<UpdateContentCommandHandler>.Instance);

        await handler.HandleAsync(new UpdateContentCommand { Id = "post-1", Title = "Post", Content = Markdown });

        _saved.ShouldNotBeNull();
        _saved.OriginalMarkdown.ShouldBe(Markdown);
    }
}
