using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using Osirion.Blazor.Cms.Core.Tests.TestFixtures;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Components.Editor;

public class MarkdownEditorPreviewTests : TestContext, IDisposable
{
    private readonly IMarkdownRendererService _markdownRenderer;

    public MarkdownEditorPreviewTests()
    {
        // Register a custom mock JSRuntime implementation
        Services.AddSingleton<IJSRuntime>(new MockJSRuntime());

        // Setup MarkdownRenderer
        _markdownRenderer = Substitute.For<IMarkdownRendererService>();
        Services.AddSingleton(_markdownRenderer); 
        
        SetRendererInfo(new RendererInfo("Server", true));

        // Configure default behavior
        _markdownRenderer.RenderToHtml(Arg.Any<string>()).Returns(x => $"<p>{x.Arg<string>()}</p>");
    }

    [Fact]
    public void MarkdownEditorPreview_ShouldRender_WithDefaultSettings()
    {
        // Act
        var cut = RenderComponent<MarkdownEditorPreview>();

        // Assert
        cut.Markup.ShouldContain("osirion-markdown-editor-preview");
        cut.Markup.ShouldContain("osirion-markdown-editor-preview-actions");
        cut.Markup.ShouldContain("osirion-markdown-editor-preview-container");
        cut.Markup.ShouldContain("osirion-markdown-editor-section");
        cut.Markup.ShouldContain("osirion-markdown-preview-section");
    }

    [Fact]
    public void MarkdownEditorPreview_ShouldRender_WithCustomSettings()
    {
        // Act
        var cut = RenderComponent<MarkdownEditorPreview>(parameters => parameters
            .Add(p => p.EditorTitle, "Custom Editor")
            .Add(p => p.PreviewTitle, "Custom Preview")
            .Add(p => p.ShowActionsBar, false)
            .Add(p => p.ShowToolbar, false)
            .Add(p => p.ShowEditorHeader, false)
            .Add(p => p.ShowPreviewHeader, false)
            .Add(p => p.Class, "custom-class")
            .Add(p => p.EditorCssClass, "custom-editor-class")
            .Add(p => p.PreviewCssClass, "custom-preview-class")
        );

        // Assert
        cut.Markup.ShouldContain("osirion-markdown-editor-preview");
        cut.Markup.ShouldContain("custom-class");
        cut.Markup.ShouldContain("custom-editor-class");
        cut.Markup.ShouldContain("custom-preview-class");

        // Actions bar should be hidden
        cut.Markup.ShouldNotContain("osirion-markdown-editor-preview-actions");

        // Headers should be hidden since we set them to false
        cut.Markup.ShouldNotContain("Custom Editor");
        cut.Markup.ShouldNotContain("Custom Preview");
    }

    [Fact]
    public void MarkdownEditorPreview_ShouldBindContent_TwoWay()
    {
        // Arrange
        const string initialContent = "Initial content";
        const string updatedContent = "Updated content";
        string capturedContent = initialContent;

        // Act
        var cut = RenderComponent<MarkdownEditorPreview>(parameters => parameters
            .Add(p => p.Content, initialContent)
            .Add(p => p.ContentChanged, (content) => capturedContent = content)
        );

        // Find the editor's textarea element within the component
        var textarea = cut.Find("textarea");

        // Set new value
        textarea.Change(updatedContent);

        // Assert
        capturedContent.ShouldBe(updatedContent);
    }

    [Fact]
    public void MarkdownEditorPreview_ShouldTogglePreview_WhenButtonClicked()
    {
        // Arrange
        var cut = RenderComponent<MarkdownEditorPreview>(parameters => parameters
            .Add(p => p.ShowPreview, true)
        );

        // Verify initial state - preview should be visible
        cut.Markup.ShouldContain("osirion-markdown-preview-section");

        // Find and click the toggle button
        var toggleButton = cut.Find("button.osirion-markdown-action-button");

        // Act
        toggleButton.Click();

        // Assert - preview should be hidden
        cut.Markup.ShouldNotContain("osirion-markdown-preview-section");
        cut.Markup.ShouldContain("preview-hidden");

        // Click again to show preview
        toggleButton.Click();

        // Preview should be visible again
        cut.Markup.ShouldContain("osirion-markdown-preview-section");
        cut.Markup.ShouldContain("preview-visible");
    }

    [Fact]
    public async Task MarkdownEditorPreview_ShouldSyncScroll_BetweenEditorAndPreview()
    {
        // Arrange
        var cut = RenderComponent<MarkdownEditorPreview>(parameters => parameters
            .Add(p => p.SyncScroll, true)
            .Add(p => p.Content, TestData.BasicMarkdown)
        );

        // Find editor content div
        var editorContent = cut.Find(".osirion-markdown-editor-content");

        // Act - trigger scroll in editor
        await editorContent.ScrollAsync();

        // Assert - Just ensure no exception was thrown
        cut.Instance.ShouldNotBeNull();

        // Find preview content div
        var previewContent = cut.Find(".osirion-markdown-preview-content");

        // Act - trigger scroll in preview
        await previewContent.ScrollAsync();

        // Assert - Just ensure no exception was thrown
        cut.Instance.ShouldNotBeNull();
    }

    [Fact]
    public async Task MarkdownEditorPreview_ShouldProvideMethodsForExternalControl()
    {
        // Arrange
        var cut = RenderComponent<MarkdownEditorPreview>();
        var component = cut.Instance;

        // Act & Assert

        // Test FocusEditorAsync
        await component.FocusEditorAsync();

        // Test InsertTextAsync
        await component.InsertTextAsync("Test text");

        // Test WrapTextAsync
        await component.WrapTextAsync("**", "**", "bold text");

        // Just assert the component is still valid
        component.ShouldNotBeNull();
    }
}