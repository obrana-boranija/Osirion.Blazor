using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.ViewModels;

public class ContentEditorViewModelEdgeCaseTests
{
    private readonly IContentEditorService _editorService = Substitute.For<IContentEditorService>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly IEventSubscriber _eventSubscriber = Substitute.For<IEventSubscriber>();
    private readonly ContentEditorViewModel _viewModel;

    public ContentEditorViewModelEdgeCaseTests()
    {
        _viewModel = new ContentEditorViewModel(_editorService, _eventPublisher, _eventSubscriber);
    }

    private static ContentItem CreatePost(string path) => new()
    {
        Path = path,
        Content = "Body",
        Metadata = FrontMatter.Create("Title")
    };

    private static GitHubFileCommitResponse Committed(string sha) =>
        new() { Content = new GitHubFileContent { Path = "p", Sha = sha } };

    [Fact]
    public async Task SavePostAsync_WithoutEditingPost_DoesNotSave()
    {
        var changes = 0;
        _viewModel.StateChanged += () => changes++;

        await _viewModel.SavePostAsync();

        await _editorService.DidNotReceiveWithAnyArgs().SaveBlogPostAsync(default!, default!);
        changes.ShouldBe(0);
    }

    [Fact]
    public async Task SavePostAsync_NewPostAtRootWithoutExtension_AddsExtensionAndCreateMessage()
    {
        _editorService.GenerateFileNameFromTitle(Arg.Any<string>()).Returns("draft");
        _viewModel.InitializeFromState(CreatePost("draft.md"), isCreatingNew: true);
        _viewModel.FileName = "  my-post  ";
        _viewModel.CommitMessage = " ";
        _editorService.SaveBlogPostAsync(Arg.Any<ContentItem>(), Arg.Any<string>()).Returns(Committed("sha-1"));

        await _viewModel.SavePostAsync();

        await _editorService.Received(1).SaveBlogPostAsync(
            Arg.Is<ContentItem>(post => post.Path == "my-post.md"),
            "Create my-post.md");
        _viewModel.EditingPost!.Sha.ShouldBe("sha-1");
        _eventPublisher.Received(1).Publish(Arg.Is<ContentSavedEvent>(e => e.Path == "my-post.md"));
    }

    [Fact]
    public async Task SavePostAsync_ExistingPostWithEmptyMessage_UsesUpdateMessage()
    {
        _viewModel.InitializeFromState(CreatePost("blog/a.md"), isCreatingNew: false);
        _viewModel.CommitMessage = string.Empty;
        _editorService.SaveBlogPostAsync(Arg.Any<ContentItem>(), Arg.Any<string>()).Returns(Committed("sha-2"));

        await _viewModel.SavePostAsync();

        await _editorService.Received(1).SaveBlogPostAsync(Arg.Any<ContentItem>(), "Update a.md");
        _viewModel.IsSaving.ShouldBeFalse();
    }

    [Fact]
    public async Task SavePostAsync_ServiceReturnsNoResult_KeepsShaAndStillReportsSaved()
    {
        var post = CreatePost("blog/a.md");
        post.Sha = "old";
        _viewModel.InitializeFromState(post, isCreatingNew: false);
        _editorService.SaveBlogPostAsync(Arg.Any<ContentItem>(), Arg.Any<string>()).Returns((GitHubFileCommitResponse)null!);

        await _viewModel.SavePostAsync();

        _viewModel.EditingPost!.Sha.ShouldBe("old");
        _eventPublisher.Received(1).Publish(Arg.Any<StatusNotificationEvent>());
    }

    [Fact]
    public void UpdateSeoMetadata_WithEditingPost_StoresItAndNotifies()
    {
        var post = CreatePost("blog/a.md");
        _viewModel.InitializeFromState(post, isCreatingNew: false);
        var changes = 0;
        _viewModel.StateChanged += () => changes++;
        var seo = SeoMetadata.Create("Seo", "Desc");

        _viewModel.UpdateSeoMetadata(seo);

        post.Metadata!.SeoProperties.Title.ShouldBe("Seo");
        changes.ShouldBe(1);
    }

    [Fact]
    public void UpdateSeoMetadata_WithoutEditingPost_DoesNotNotify()
    {
        var changes = 0;
        _viewModel.StateChanged += () => changes++;

        _viewModel.UpdateSeoMetadata(SeoMetadata.Create("Seo", "Desc"));

        changes.ShouldBe(0);
    }

    [Fact]
    public void UpdateContentAndMetadata_WithoutEditingPost_DoNotNotify()
    {
        var changes = 0;
        _viewModel.StateChanged += () => changes++;

        _viewModel.UpdateContent("x");
        _viewModel.UpdateMetadata(FrontMatter.Create("x"));

        changes.ShouldBe(0);
        _viewModel.EditingPost.ShouldBeNull();
    }

    [Fact]
    public async Task ReloadPostAsync_WithEditingPost_LoadsItAgain()
    {
        _viewModel.InitializeFromState(CreatePost("blog/a.md"), isCreatingNew: false);
        var reloaded = CreatePost("blog/a.md");
        _editorService.GetBlogPostAsync("blog/a.md").Returns(reloaded);

        await _viewModel.ReloadPostAsync();

        _viewModel.EditingPost.ShouldBeSameAs(reloaded);
    }

    [Fact]
    public async Task ReloadPostAsync_WithoutPathOrPost_DoesNotLoad()
    {
        await _viewModel.ReloadPostAsync();
        _viewModel.InitializeFromState(CreatePost(" "), isCreatingNew: false);

        await _viewModel.ReloadPostAsync();

        await _editorService.DidNotReceiveWithAnyArgs().GetBlogPostAsync(default!);
    }

    [Fact]
    public void InitializeFromState_NewPostWithoutMetadata_UsesFallbackTitle()
    {
        _editorService.GenerateFileNameFromTitle("new-document-name").Returns("new-document-name.md");

        _viewModel.InitializeFromState(new ContentItem { Path = "x.md", Metadata = null }, isCreatingNew: true);

        _viewModel.FileName.ShouldBe("new-document-name.md");
        _viewModel.CommitMessage.ShouldBe("Create new-document-name.md");
    }

    [Fact]
    public void Dispose_UnsubscribesFromEvents()
    {
        _viewModel.Dispose();

        _eventSubscriber.Received(1).Unsubscribe(Arg.Any<Action<ContentSelectedEvent>>());
        _eventSubscriber.Received(1).Unsubscribe(Arg.Any<Action<CreateNewContentEvent>>());
    }
}
