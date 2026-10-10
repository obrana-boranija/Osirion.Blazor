using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;
using ContentEditorComponent = Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.ContentEditor;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components;

public class ContentEditorTests : AdminComponentTestContext
{
    private readonly IContentEditorService _editorService;
    private readonly ContentEditorViewModel _viewModel;

    public ContentEditorTests()
    {
        _editorService = Substitute.For<IContentEditorService>();
        // The view model has no virtual members, so the component gets the real one over a substituted service.
        _viewModel = new ContentEditorViewModel(
            _editorService,
            Substitute.For<IEventPublisher>(),
            Substitute.For<IEventSubscriber>());

        Services.AddSingleton(_viewModel);
        ComponentFactories.AddStub<MarkdownEditorWithPreview>();
        ComponentFactories.AddStub<MetadataEditor>();
        RenderInteractively();
    }

    private static ContentItem CreatePost() => new()
    {
        Path = "content/blog/post.md",
        Content = "Test content",
        Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
    };

    [Fact]
    public void ContentEditor_WhenEditingPostIsNull_ShouldShowEmptyState()
    {
        // Act
        var cut = Render<ContentEditorComponent>();

        // Assert
        cut.Markup.ShouldContain("Select a file to edit or create a new file");
        cut.FindAll("button.btn-primary").ShouldBeEmpty();
    }

    [Fact]
    public void ContentEditor_WithEditingPost_ShouldRenderEditorContent()
    {
        // Arrange
        _viewModel.InitializeFromState(CreatePost(), false);

        // Act
        var cut = Render<ContentEditorComponent>();

        // Assert
        cut.Find(".content-editor").ShouldNotBeNull();
        cut.Find("button.btn-primary").TextContent.Trim().ShouldBe("Save Changes");
        cut.Find("button.btn-outline-secondary").TextContent.Trim().ShouldBe("Discard Changes");
        cut.FindAll("input[type='text']").ShouldBeEmpty();
        cut.FindComponent<Stub<MarkdownEditorWithPreview>>().ShouldNotBeNull();
    }

    [Fact]
    public void ContentEditor_WhenIsCreatingNew_ShouldShowFileNameInput()
    {
        // Arrange
        _editorService.GenerateFileNameFromTitle("Test Title").Returns("new-post.md");
        _viewModel.InitializeFromState(CreatePost(), true);

        // Act
        var cut = Render<ContentEditorComponent>();

        // Assert
        cut.Find("input[type='text']").GetAttribute("value").ShouldBe("new-post.md");
    }

    [Fact]
    public void ContentEditor_WhenIsSaving_ShouldDisableSaveButton()
    {
        // Arrange
        var post = CreatePost();
        var pendingSave = new TaskCompletionSource<GitHubFileCommitResponse>();
        _editorService.SaveBlogPostAsync(post, Arg.Any<string>()).Returns(pendingSave.Task);
        _viewModel.InitializeFromState(post, false);
        var cut = Render<ContentEditorComponent>();
        cut.Find("button.btn-primary").HasAttribute("disabled").ShouldBeFalse();

        // Act
        cut.Find("button.btn-primary").Click();

        // Assert
        cut.Find("button.btn-primary").HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".spinner-border").ShouldNotBeNull();
        cut.Markup.ShouldContain("Saving");

        pendingSave.SetResult(new GitHubFileCommitResponse { Content = new GitHubFileContent { Path = post.Path, Sha = "sha" } });
    }

    [Fact]
    public void ContentEditor_SaveChangesButton_ShouldCallSavePostAsync()
    {
        // Arrange
        var post = CreatePost();
        _viewModel.InitializeFromState(post, false);
        var cut = Render<ContentEditorComponent>();

        // Act
        cut.Find("button.btn-primary").Click();

        // Assert
        _editorService.Received(1).SaveBlogPostAsync(post, _viewModel.CommitMessage);
    }

    [Fact]
    public void ContentEditor_DiscardChangesButton_ShouldCallDiscardChanges()
    {
        // Arrange
        _viewModel.InitializeFromState(CreatePost(), false);
        var cut = Render<ContentEditorComponent>();

        // Act
        cut.Find("button.btn-outline-secondary").Click();

        // Assert
        _viewModel.EditingPost.ShouldBeNull();
        cut.Markup.ShouldContain("Select a file to edit or create a new file");
    }

    [Fact]
    public void ContentEditor_ShouldRenderTabsAndAllowSwitching()
    {
        // Arrange
        _viewModel.InitializeFromState(CreatePost(), false);

        // Act
        var cut = Render<ContentEditorComponent>();

        // Assert
        var tabs = cut.FindAll("button.nav-link").ToList();
        tabs.Count.ShouldBe(2);
        tabs[0].TextContent.Trim().ShouldBe("Content");
        tabs[1].TextContent.Trim().ShouldBe("Metadata");
        tabs[0].ClassList.ShouldContain("active");
        tabs[1].ClassList.ShouldNotContain("active");

        cut.FindAll("button.nav-link").ToList()[1].Click();

        tabs = cut.FindAll("button.nav-link").ToList();
        tabs[0].ClassList.ShouldNotContain("active");
        tabs[1].ClassList.ShouldContain("active");
        cut.FindComponent<Stub<MetadataEditor>>().ShouldNotBeNull();
        cut.FindComponents<Stub<MarkdownEditorWithPreview>>().ShouldBeEmpty();
    }
}
