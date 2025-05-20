using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.ViewModels;

public class ContentEditorViewModelTests
{
    private readonly IContentEditorService _editorService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEventSubscriber _eventSubscriber;
    private readonly ContentEditorViewModel _viewModel;

    public ContentEditorViewModelTests()
    {
        _editorService = Substitute.For<IContentEditorService>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _eventSubscriber = Substitute.For<IEventSubscriber>();

        _viewModel = new ContentEditorViewModel(_editorService, _eventPublisher, _eventSubscriber);
    }

    [Fact]
    public void InitializeFromState_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now),
            Sha = "test-sha"
        };
        bool isCreatingNew = true;

        // Act
        _viewModel.InitializeFromState(blogPost, isCreatingNew);

        // Assert
        _viewModel.EditingPost.ShouldBe(blogPost);
        _viewModel.IsCreatingNew.ShouldBe(isCreatingNew);
        _viewModel.FileName.ShouldNotBeNullOrEmpty();
        _viewModel.CommitMessage.ShouldContain("Create");
    }

    [Fact]
    public void InitializeFromState_WhenNotCreatingNew_ShouldSetUpdateCommitMessage()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now),
            Sha = "test-sha"
        };
        bool isCreatingNew = false;

        // Act
        _viewModel.InitializeFromState(blogPost, isCreatingNew);

        // Assert
        _viewModel.EditingPost.ShouldBe(blogPost);
        _viewModel.IsCreatingNew.ShouldBe(isCreatingNew);
        _viewModel.CommitMessage.ShouldContain("Update");
    }

    [Fact]
    public async Task LoadPostAsync_ShouldLoadPostAndUpdateState()
    {
        // Arrange
        var path = "content/blog/post.md";
        var blogPost = new BlogPost
        {
            FilePath = path,
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now),
            Sha = "test-sha"
        };

        _editorService.GetBlogPostAsync(path).Returns(blogPost);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.LoadPostAsync(path);

        // Assert
        _viewModel.EditingPost.ShouldBe(blogPost);
        _viewModel.IsCreatingNew.ShouldBeFalse();
        _viewModel.CommitMessage.ShouldContain("Update");
        _viewModel.CommitMessage.ShouldContain(System.IO.Path.GetFileName(path));
        stateChangedCalled.ShouldBeTrue();

        await _editorService.Received(1).GetBlogPostAsync(path);
    }

    [Fact]
    public async Task LoadPostAsync_WhenExceptionThrown_ShouldSetErrorAndPublishEvent()
    {
        // Arrange
        var path = "content/blog/post.md";
        var exception = new InvalidOperationException("Test error");

        _editorService.GetBlogPostAsync(path).Throws(exception);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.LoadPostAsync(path);

        // Assert
        _viewModel.ErrorMessage.ShouldNotBeNullOrEmpty();
        _viewModel.ErrorMessage.ShouldContain("Failed to load post");
        _viewModel.ErrorMessage.ShouldContain(exception.Message);
        stateChangedCalled.ShouldBeTrue();

        _eventPublisher.Received(1).Publish(
            Arg.Is<ErrorOccurredEvent>(e =>
                e.Message.Contains("Failed to load post") &&
                e.Message.Contains(exception.Message) &&
                e.Exception == exception)
        );
    }

    [Fact]
    public async Task SavePostAsync_ShouldSavePostAndPublishEvents()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        var commitResponse = new GitHubFileCommitResponse
        {
            Content = new GitHubFileContent
            {
                Path = blogPost.FilePath,
                Sha = "new-sha-123"
            }
        };

        _viewModel.InitializeFromState(blogPost, false);
        _editorService.SaveBlogPostAsync(blogPost, _viewModel.CommitMessage).Returns(commitResponse);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.SavePostAsync();

        // Assert
        _viewModel.EditingPost.Sha.ShouldBe("new-sha-123");
        _viewModel.IsSaving.ShouldBeFalse();
        stateChangedCalled.ShouldBeTrue();

        await _editorService.Received(1).SaveBlogPostAsync(blogPost, _viewModel.CommitMessage);

        _eventPublisher.Received(1).Publish(
            Arg.Is<ContentSavedEvent>(e => e.Path == blogPost.FilePath)
        );

        _eventPublisher.Received(1).Publish(
            Arg.Is<StatusNotificationEvent>(e =>
                e.Message.Contains("saved") &&
                e.Message.Contains(System.IO.Path.GetFileName(blogPost.FilePath)) &&
                e.Type == StatusType.Success)
        );
    }

    [Fact]
    public async Task SavePostAsync_WhenCreatingNew_ShouldUpdateFilePathAndResetCreatingState()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "content/blog",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        var commitResponse = new GitHubFileCommitResponse
        {
            Content = new GitHubFileContent
            {
                Path = "content/blog/test-post.md",
                Sha = "new-sha-123"
            }
        };

        _viewModel.InitializeFromState(blogPost, true);
        _viewModel.FileName = "test-post.md";

        _editorService
            .SaveBlogPostAsync(Arg.Any<BlogPost>(), _viewModel.CommitMessage)
            .Returns(commitResponse);

        // Act
        await _viewModel.SavePostAsync();

        // Assert
        _viewModel.EditingPost.FilePath.ShouldBe("content/blog/test-post.md");
        _viewModel.IsCreatingNew.ShouldBeFalse();
        _viewModel.FileName.ShouldBeEmpty();

        await _editorService.Received(1).SaveBlogPostAsync(
            Arg.Is<BlogPost>(p => p.FilePath == "content/blog/test-post.md"),
            _viewModel.CommitMessage
        );
    }

    [Fact]
    public async Task SavePostAsync_WhenExceptionThrown_ShouldSetErrorAndPublishEvent()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        var exception = new InvalidOperationException("Test error");

        _viewModel.InitializeFromState(blogPost, false);
        _editorService.SaveBlogPostAsync(blogPost, _viewModel.CommitMessage).Throws(exception);

        // Act
        await _viewModel.SavePostAsync();

        // Assert
        _viewModel.ErrorMessage.ShouldNotBeNullOrEmpty();
        _viewModel.ErrorMessage.ShouldContain("Failed to save post");
        _viewModel.ErrorMessage.ShouldContain(exception.Message);
        _viewModel.IsSaving.ShouldBeFalse();

        _eventPublisher.Received(1).Publish(
            Arg.Is<ErrorOccurredEvent>(e =>
                e.Message.Contains("Failed to save post") &&
                e.Message.Contains(exception.Message) &&
                e.Exception == exception)
        );
    }

    [Fact]
    public void UpdateContent_ShouldUpdateEditingPostContent()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "content/blog/post.md",
            Content = "Original content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _viewModel.InitializeFromState(blogPost, false);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        _viewModel.UpdateContent("Updated content");

        // Assert
        _viewModel.EditingPost.Content.ShouldBe("Updated content");
        stateChangedCalled.ShouldBeTrue();
    }

    [Fact]
    public void UpdateMetadata_ShouldUpdateEditingPostMetadata()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Original Title", "Original Description", System.DateTime.Now)
        };

        _viewModel.InitializeFromState(blogPost, false);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        var updatedMetadata = FrontMatter.Create("Updated Title", "Updated Description", System.DateTime.Now);

        // Act
        _viewModel.UpdateMetadata(updatedMetadata);

        // Assert
        _viewModel.EditingPost.Metadata.ShouldBe(updatedMetadata);
        stateChangedCalled.ShouldBeTrue();
    }

    [Fact]
    public void DiscardChanges_ShouldResetState()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _viewModel.InitializeFromState(blogPost, true);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        _viewModel.DiscardChanges();

        // Assert
        _viewModel.EditingPost.ShouldBeNull();
        _viewModel.IsCreatingNew.ShouldBeFalse();
        _viewModel.FileName.ShouldBeEmpty();
        _viewModel.CommitMessage.ShouldBeEmpty();
        _viewModel.ErrorMessage.ShouldBeNull();
        stateChangedCalled.ShouldBeTrue();
    }

    [Fact]
    public void OnContentSelected_ShouldLoadPost()
    {
        // Arrange - Access private method through reflection
        var method = typeof(ContentEditorViewModel).GetMethod(
            "OnContentSelected",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var path = "content/blog/post.md";
        var blogPost = new BlogPost
        {
            FilePath = path,
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _editorService.GetBlogPostAsync(path).Returns(blogPost);

        // Act
        method?.Invoke(_viewModel, new object[] { new ContentSelectedEvent(path) });

        // Assert - Since this calls LoadPostAsync asynchronously, we can just verify that it was called
        _editorService.Received(1).GetBlogPostAsync(path);
    }

    [Fact]
    public void OnCreateNewContent_ShouldCreateNewPost()
    {
        // Arrange - Access private method through reflection
        var method = typeof(ContentEditorViewModel).GetMethod(
            "OnCreateNewContent",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var directory = "content/blog";
        var newPost = new BlogPost
        {
            FilePath = "content/blog/new-post.md",
            Content = "Start writing your content here...",
            Metadata = FrontMatter.Create("New Post", "Description", System.DateTime.Now)
        };

        _editorService.CreateNewBlogPost(directory).Returns(newPost);
        _editorService.GenerateFileNameFromTitle("New Post").Returns("new-post.md");

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        method?.Invoke(_viewModel, new object[] { new CreateNewContentEvent(directory) });

        // Assert
        _viewModel.EditingPost.ShouldNotBeNull();
        _viewModel.IsCreatingNew.ShouldBeTrue();
        _viewModel.FileName.ShouldNotBeNullOrEmpty();
        stateChangedCalled.ShouldBeTrue();
    }
}