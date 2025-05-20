using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using Osirion.Blazor.Cms.Core.Tests.TestFixtures;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Components.Editor;

public class MarkdownEditorTests : TestContext, IDisposable
{
    private readonly IMarkdownRendererService _markdownRenderer;

    public MarkdownEditorTests()
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
    public void MarkdownEditor_ShouldRender_WithDefaultSettings()
    {
        // Act
        var cut = RenderComponent<MarkdownEditor>();

        // Assert
        cut.Markup.ShouldContain("osirion-markdown-editor");
        cut.Markup.ShouldContain("osirion-markdown-editor-toolbar");
        cut.Markup.ShouldContain("osirion-markdown-editor-header");
        cut.Markup.ShouldContain("osirion-markdown-editor-content");
        cut.Markup.ShouldContain("osirion-markdown-editor-textarea");

        // Default title
        cut.Markup.ShouldContain("Editor");
    }

    [Fact]
    public void MarkdownEditor_ShouldRender_WithCustomSettings()
    {
        // Act
        var cut = RenderComponent<MarkdownEditor>(parameters => parameters
            .Add(p => p.Title, "Custom Editor")
            .Add(p => p.ShowToolbar, false)
            .Add(p => p.ShowHeader, false)
            .Add(p => p.Placeholder, "Custom placeholder")
            .Add(p => p.CssClass, "custom-class")
        );

        // Assert
        cut.Markup.ShouldContain("osirion-markdown-editor");
        cut.Markup.ShouldContain("custom-class");
        cut.Markup.ShouldContain("Custom placeholder");

        // Toolbar and header should be hidden
        cut.Markup.ShouldNotContain("osirion-markdown-editor-toolbar");
        cut.Markup.ShouldNotContain("osirion-markdown-editor-header");
        cut.Markup.ShouldNotContain("Custom Editor");
    }

    [Fact]
    public void MarkdownEditor_ShouldBindContent_TwoWay()
    {
        // Arrange
        const string initialContent = "Initial content";
        const string updatedContent = "Updated content";
        string capturedContent = initialContent;

        // Act
        var cut = RenderComponent<MarkdownEditor>(parameters => parameters
            .Add(p => p.Content, initialContent)
            .Add(p => p.ContentChanged, (content) => capturedContent = content)
        );

        // Find the textarea element
        var textarea = cut.Find("textarea");

        // Set new value
        textarea.Change(updatedContent);

        // Assert
        capturedContent.ShouldBe(updatedContent);
    }

    [Fact]
    public void MarkdownEditor_ShouldShowToolbarActions_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<MarkdownEditor>(parameters => parameters
            .Add(p => p.ShowToolbar, true)
        );

        // Assert
        var toolbarButtons = cut.FindAll(".osirion-markdown-toolbar-button");
        toolbarButtons.Count.ShouldBeGreaterThan(0);

        // Check for common toolbar buttons
        cut.Markup.ShouldContain("Bold");
        cut.Markup.ShouldContain("Italic");
        cut.Markup.ShouldContain("Link");
    }

    [Fact]
    public void MarkdownEditor_ShouldUseCustomToolbarActions_WhenProvided()
    {
        // Arrange
        var customActions = new List<ToolbarAction>
        {
            new ToolbarAction("C", ToolbarActionType.Wrap, "```|```|code", "Custom Code", "📝")
        };

        // Act
        var cut = RenderComponent<MarkdownEditor>(parameters => parameters
            .Add(p => p.ShowToolbar, true)
            .Add(p => p.ToolbarActions, customActions)
        );

        // Assert
        cut.Markup.ShouldContain("Custom Code");
        cut.Markup.ShouldContain("📝");
    }

    [Fact]
    public async Task MarkdownEditor_ShouldHandleKeyboardShortcuts()
    {
        // Arrange
        var cut = RenderComponent<MarkdownEditor>();
        var textarea = cut.Find("textarea");

        // Act - Simulate tab key press
        await textarea.KeyDownAsync(new KeyboardEventArgs { Key = "Tab", ShiftKey = false });

        // Assert - Just ensure no exception was thrown
        cut.Instance.ShouldNotBeNull();
    }

    [Fact]
    public async Task MarkdownEditor_ShouldSetScrollPosition_WhenRequested()
    {
        // Arrange
        var cut = RenderComponent<MarkdownEditor>();
        var component = cut.Instance;

        // Act
        await component.SetScrollPositionAsync(0.5);

        // Assert - Just ensure no exception was thrown
        component.ShouldNotBeNull();
    }
}