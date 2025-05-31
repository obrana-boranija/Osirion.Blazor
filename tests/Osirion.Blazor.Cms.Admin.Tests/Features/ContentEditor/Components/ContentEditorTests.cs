using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components;

public class ContentEditorTests : TestContext
{
    private readonly CmsState _adminState;
    private readonly ContentEditorViewModel _viewModel;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEventSubscriber _eventSubscriber;

    public ContentEditorTests()
    {
        _adminState = new CmsState();
        _viewModel = Substitute.For<ContentEditorViewModel>(
            Substitute.For<IContentEditorService>(),
            Substitute.For<IEventPublisher>(),
            Substitute.For<IEventSubscriber>()
        );
        _eventPublisher = Substitute.For<IEventPublisher>();
        _eventSubscriber = Substitute.For<IEventSubscriber>();

        // Register services
        Services.AddSingleton(_adminState);
        Services.AddSingleton(_viewModel);
        Services.AddSingleton(_eventPublisher);
        Services.AddSingleton(_eventSubscriber);
        Services.AddSingleton(Substitute.For<NavigationManager>());

        // Register logger factory and loggers
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());
        Services.AddSingleton(loggerFactory);

        // Configure Blazor components base
        Services.AddScoped<BaseComponent>();
    }

    [Fact]
    public void ContentEditor_WhenEditingPostIsNull_ShouldShowEmptyState()
    {
        // Arrange - No editing post
        _viewModel.EditingPost.Returns((ContentItem)null!);

        // Act
        var cut = RenderComponent<Admin.Features.ContentEditor.Components.ContentEditor>();

        // Assert
        cut.Markup.ShouldContain("Select a file to edit or create a new file");
    }

    [Fact]
    public void ContentEditor_WithEditingPost_ShouldRenderEditorContent()
    {
        // Arrange
        var blogPost = new ContentItem
        {
            Path = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _viewModel.EditingPost.Returns(blogPost);
        _viewModel.When(x => x.SavePostAsync()).Do(_ => { });
        _viewModel.When(x => x.DiscardChanges()).Do(_ => { });

        // Act
        var cut = RenderComponent<Admin.Features.ContentEditor.Components.ContentEditor>();

        // Assert
        cut.Find(".content-editor").ShouldNotBeNull();
        cut.Find("button[onclick*='SaveChanges']").ShouldNotBeNull();
        cut.Find("button[onclick*='DiscardChanges']").ShouldNotBeNull();
    }

    [Fact]
    public void ContentEditor_WhenIsCreatingNew_ShouldShowFileNameInput()
    {
        // Arrange
        var blogPost = new ContentItem
        {
            Path = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _viewModel.EditingPost.Returns(blogPost);
        _viewModel.IsCreatingNew.Returns(true);
        _viewModel.FileName.Returns("new-post.md");

        // Act
        var cut = RenderComponent<Admin.Features.ContentEditor.Components.ContentEditor>();

        // Assert
        cut.Find("input[bind='ViewModel.FileName']").ShouldNotBeNull();
    }

    [Fact]
    public void ContentEditor_WhenIsSaving_ShouldDisableSaveButton()
    {
        // Arrange
        var blogPost = new ContentItem
        {
            Path = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _viewModel.EditingPost.Returns(blogPost);
        _viewModel.IsSaving.Returns(true);

        // Act
        var cut = RenderComponent<Admin.Features.ContentEditor.Components.ContentEditor>();

        // Assert
        cut.Find("button.btn-primary[disabled]").ShouldNotBeNull();
        cut.Find(".spinner-border").ShouldNotBeNull();
        cut.Markup.ShouldContain("Saving");
    }

    [Fact]
    public void ContentEditor_SaveChangesButton_ShouldCallSavePostAsync()
    {
        // Arrange
        var blogPost = new ContentItem
        {
            Path = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _viewModel.EditingPost.Returns(blogPost);

        var savePostAsyncCalled = false;
        _viewModel.When(x => x.SavePostAsync()).Do(_ => savePostAsyncCalled = true);

        // Act
        var cut = RenderComponent<Admin.Features.ContentEditor.Components.ContentEditor>();
        cut.Find("button.btn-primary").Click();

        // Assert
        savePostAsyncCalled.ShouldBeTrue();
    }

    [Fact]
    public void ContentEditor_DiscardChangesButton_ShouldCallDiscardChanges()
    {
        // Arrange
        var blogPost = new ContentItem
        {
            Path = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _viewModel.EditingPost.Returns(blogPost);

        var discardChangesCalled = false;
        _viewModel.When(x => x.DiscardChanges()).Do(_ => discardChangesCalled = true);

        // Act
        var cut = RenderComponent<Admin.Features.ContentEditor.Components.ContentEditor>();
        cut.Find("button.btn-outline-secondary").Click();

        // Assert
        discardChangesCalled.ShouldBeTrue();
    }

    [Fact]
    public void ContentEditor_ShouldRenderTabsAndAllowSwitching()
    {
        // Arrange
        var blogPost = new ContentItem
        {
            Path = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _viewModel.EditingPost.Returns(blogPost);

        // Act
        var cut = RenderComponent<Admin.Features.ContentEditor.Components.ContentEditor>();

        // Verify tabs exist
        var contentTab = cut.Find("button.nav-link[onclick*='SetActiveTab(\"content\")']");
        var metadataTab = cut.Find("button.nav-link[onclick*='SetActiveTab(\"metadata\")']");

        contentTab.ShouldNotBeNull();
        metadataTab.ShouldNotBeNull();

        // Initially content tab should be active
        contentTab.ClassList.ShouldContain("active");

        // Switch to metadata tab
        metadataTab.Click();

        // Now metadata tab should be active
        cut.Render();
        contentTab = cut.Find("button.nav-link[onclick*='SetActiveTab(\"content\")']");
        metadataTab = cut.Find("button.nav-link[onclick*='SetActiveTab(\"metadata\")']");

        contentTab.ClassList.ShouldNotContain("active");
        metadataTab.ClassList.ShouldContain("active");
    }

    [Fact]
    public void ContentEditor_ShouldRenderPreviewToggleButton()
    {
        // Arrange
        var blogPost = new ContentItem
        {
            Path = "content/blog/post.md",
            Content = "Test content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now)
        };

        _viewModel.EditingPost.Returns(blogPost);

        // Act
        var cut = RenderComponent<Admin.Features.ContentEditor.Components.ContentEditor>();

        // Verify preview toggle button exists
        var previewToggleButton = cut.Find("button[onclick*='TogglePreview']");
        previewToggleButton.ShouldNotBeNull();

        // Toggle preview
        previewToggleButton.Click();
        cut.Render();

        // Since toggling preview is managed internally in the component and affects child components,
        // we can't easily assert the visual change, but we can make sure the action doesn't throw errors
    }
}