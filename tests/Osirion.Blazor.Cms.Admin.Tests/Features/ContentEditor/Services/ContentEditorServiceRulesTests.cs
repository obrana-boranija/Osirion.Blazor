using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Services;

public class ContentEditorServiceRulesTests
{
    private readonly IContentRepositoryAdapter _repositoryAdapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly RecordingLogger<ContentEditorService> _logger = new();
    private readonly ContentRulesOptions _rules = new();

    private ContentEditorService CreateService() => new(
        _repositoryAdapter,
        Substitute.For<IAdminContentService>(),
        _eventPublisher,
        Microsoft.Extensions.Options.Options.Create(new CmsAdminOptions { ContentRules = _rules }),
        _logger);

    private static ContentItem CreatePost(string title, string content = "Body", string path = "blog/post.md") => new()
    {
        Path = path,
        Content = content,
        Metadata = FrontMatter.Create(title)
    };

    private void SaveSucceeds() =>
        _repositoryAdapter.SaveContentAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>())
            .Returns(new GitHubFileCommitResponse { Content = new GitHubFileContent { Path = "p", Sha = "s" } });

    [Fact]
    public void Constructor_NullDependency_Throws()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new CmsAdminOptions());
        var adminContent = Substitute.For<IAdminContentService>();

        Should.Throw<ArgumentNullException>(() => new ContentEditorService(null!, adminContent, _eventPublisher, options, _logger));
        Should.Throw<ArgumentNullException>(() => new ContentEditorService(_repositoryAdapter, null!, _eventPublisher, options, _logger));
        Should.Throw<ArgumentNullException>(() => new ContentEditorService(_repositoryAdapter, adminContent, null!, options, _logger));
        Should.Throw<ArgumentNullException>(() => new ContentEditorService(_repositoryAdapter, adminContent, _eventPublisher, null!, _logger));
        Should.Throw<ArgumentNullException>(() => new ContentEditorService(_repositoryAdapter, adminContent, _eventPublisher, options, null!));
    }

    [Fact]
    public async Task SaveBlogPostAsync_UnknownRequiredField_IsIgnored()
    {
        _rules.RequiredFrontMatterFields = ["doesNotExist"];
        SaveSucceeds();

        await CreateService().SaveBlogPostAsync(CreatePost("Title"), "msg");

        await _repositoryAdapter.Received(1).SaveContentAsync("blog/post.md", Arg.Any<string>(), "msg", Arg.Any<string?>());
    }

    [Fact]
    public async Task SaveBlogPostAsync_EmptyContent_ThrowsValidationException()
    {
        var exception = await Should.ThrowAsync<ValidationException>(() =>
            CreateService().SaveBlogPostAsync(CreatePost("Title", content: " "), "msg"));

        exception.Message.ShouldBe("Content cannot be empty");
        _eventPublisher.Received(1).Publish(Arg.Is<ErrorOccurredEvent>(e => e.Message == "Failed to save file: blog/post.md"));
    }

    [Fact]
    public async Task SaveBlogPostAsync_AutoSlug_BuildsSlugFromTitle()
    {
        _rules.RequiredFrontMatterFields = [];
        var post = CreatePost("Hello,   World -- Again!");
        SaveSucceeds();

        await CreateService().SaveBlogPostAsync(post, "msg");

        post.Metadata!.Slug.ShouldBe("hello-world-again");
    }

    [Fact]
    public async Task SaveBlogPostAsync_AutoSlugWithBlankTitle_UsesPost()
    {
        _rules.RequiredFrontMatterFields = [];
        var post = CreatePost(" ");
        SaveSucceeds();

        await CreateService().SaveBlogPostAsync(post, "msg");

        post.Metadata!.Slug.ShouldBe("post");
    }

    [Fact]
    public async Task SaveBlogPostAsync_ExistingSlug_IsKept()
    {
        var post = CreatePost("Title");
        post.Metadata!.Slug = "kept";
        SaveSucceeds();

        await CreateService().SaveBlogPostAsync(post, "msg");

        post.Metadata.Slug.ShouldBe("kept");
    }

    [Fact]
    public async Task SaveBlogPostAsync_ValidationDisabledAndBlankMessage_SavesWithUpdateMessage()
    {
        _rules.EnforceFrontMatterValidation = false;
        SaveSucceeds();

        await CreateService().SaveBlogPostAsync(CreatePost("Title", content: ""), " ");

        await _repositoryAdapter.Received(1).SaveContentAsync("blog/post.md", Arg.Any<string>(), "Update post.md", Arg.Any<string?>());
    }

    [Fact]
    public async Task DeleteBlogPostAsync_AdapterFails_LogsPublishesAndRethrows()
    {
        _repositoryAdapter.DeleteFileAsync("blog/a.md", Arg.Any<string>(), "sha")
            .ThrowsAsync(new InvalidOperationException("gone"));

        await Should.ThrowAsync<InvalidOperationException>(() => CreateService().DeleteBlogPostAsync("blog/a.md", "sha"));

        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception != null);
        _eventPublisher.Received(1).Publish(Arg.Is<ErrorOccurredEvent>(e => e.Message == "Failed to delete file: blog/a.md"));
    }

    [Theory]
    [InlineData("!!!", "new-post.md")]
    [InlineData("", "new-post.md")]
    [InlineData("Already.MD", "already.md")]
    [InlineData("  Two   Words  ", "two-words.md")]
    public void GenerateFileNameFromTitle_EdgeCases_ProduceSafeNames(string title, string expected)
    {
        CreateService().GenerateFileNameFromTitle(title).ShouldBe(expected);
    }

    [Theory]
    [InlineData("", "new-post.md")]
    [InlineData("blog/", "blog/new-post.md")]
    [InlineData("blog", "blog/new-post.md")]
    public void CreateNewBlogPost_Directory_IsJoinedWithFileName(string directory, string expected)
    {
        var post = CreateService().CreateNewBlogPost(directory);

        post.Path.ShouldBe(expected);
        post.Metadata!.Title.ShouldBe("New Post");
        post.Content.ShouldStartWith("## New Post");
    }
}
