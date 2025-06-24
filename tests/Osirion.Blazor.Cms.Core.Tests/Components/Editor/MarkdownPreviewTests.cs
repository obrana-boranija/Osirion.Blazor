using Bunit;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using Osirion.Blazor.Cms.Core.Tests.TestFixtures;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Components.Editor;

public class MarkdownPreviewTests : TestContext, IDisposable
{
    private readonly IMarkdownRendererService _markdownRenderer;

    public MarkdownPreviewTests()
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
    public void MarkdownPreview_ShouldRender_WithDefaultSettings()
    {
        // Act
        var cut = RenderComponent<MarkdownPreview>();

        // Assert
        cut.Markup.ShouldContain("osirion-markdown-preview");
        cut.Markup.ShouldContain("osirion-markdown-preview-header");
        cut.Markup.ShouldContain("osirion-markdown-preview-content");

        // Default title
        cut.Markup.ShouldContain("Preview");

        // Default placeholder when no content
        cut.Markup.ShouldContain("osirion-markdown-preview-placeholder");
    }

    [Fact]
    public void MarkdownPreview_ShouldRender_WithCustomSettings()
    {
        // Act
        var cut = RenderComponent<MarkdownPreview>(parameters => parameters
            .Add(p => p.Title, "Custom Preview")
            .Add(p => p.ShowHeader, false)
            .Add(p => p.Placeholder, "Custom placeholder")
            .Add(p => p.Class, "custom-class")
            .Add(p => p.ContentCssClass, "custom-content-class")
        );

        // Assert
        cut.Markup.ShouldContain("osirion-markdown-preview");
        cut.Markup.ShouldContain("custom-class");
        cut.Markup.ShouldContain("Custom placeholder");

        // Header should be hidden
        cut.Markup.ShouldNotContain("osirion-markdown-preview-header");
        cut.Markup.ShouldNotContain("Custom Preview");
    }

    [Fact]
    public void MarkdownPreview_ShouldRender_MarkdownContent()
    {
        // Arrange
        var markdown = TestData.BasicMarkdown;
        var renderedHtml = TestData.RenderedHtml;

        // Setup the markdown renderer to return our test HTML
        _markdownRenderer.RenderToHtml(markdown).Returns(renderedHtml);

        // Act
        var cut = RenderComponent<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, markdown)
        );

        // Assert
        cut.Markup.ShouldNotContain("osirion-markdown-preview-placeholder");
        cut.Markup.ShouldContain("<h1>Test Heading</h1>");
        cut.Markup.ShouldContain("<strong>bold</strong>");
        cut.Markup.ShouldContain("<em>italic</em>");
    }

    [Fact]
    public void MarkdownPreview_ShouldUseContentCssClass_WhenProvided()
    {
        // Arrange
        var markdown = "Test content";

        // Act
        var cut = RenderComponent<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, markdown)
            .Add(p => p.ContentCssClass, "custom-content-class")
        );

        // Assert
        cut.Markup.ShouldContain("custom-content-class");
    }

    [Fact]
    public void MarkdownPreview_ShouldUseCustomPipeline_WhenProvided()
    {
        // Arrange
        var markdown = TestData.BasicMarkdown;
        var customPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        // Act
        var cut = RenderComponent<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, markdown)
            .Add(p => p.Pipeline, customPipeline)
        );

        // Assert - We can only check that the component didn't crash
        // The content should be rendered using the pipeline
        cut.Markup.ShouldContain("<p>");  // Basic verification that something was rendered
    }

    [Fact]
    public async Task MarkdownPreview_ShouldInvokeScrollEvent_WhenScrolled()
    {
        // Arrange
        double capturedPosition = 0;

        // Act
        var cut = RenderComponent<MarkdownPreview>(parameters => parameters
            .Add(p => p.SyncScroll, true)
            .Add(p => p.OnScroll, (position) => capturedPosition = position)
            .Add(p => p.Markdown, TestData.BasicMarkdown)
        );

        // Find the content div
        var contentDiv = cut.Find(".osirion-markdown-preview-content");

        // Trigger scroll event
        await contentDiv.ScrollAsync();

        // Assert
        capturedPosition.ShouldBe(0.5); // Value set in the mock JSRuntime
    }

    [Fact]
    public async Task MarkdownPreview_ShouldSetScrollPosition_WhenRequested()
    {
        // Arrange
        var cut = RenderComponent<MarkdownPreview>();
        var component = cut.Instance;

        // Act
        await component.SetScrollPositionAsync(0.5);

        // Assert - Just ensure no exception was thrown
        component.ShouldNotBeNull();
    }

    [Fact]
    public void MarkdownPreview_ShouldHandleErrorsGracefully_WhenRenderingFails()
    {
        // Arrange
        var markdown = "Test content";

        // Setup the markdown renderer to throw an exception
        _markdownRenderer.RenderToHtml(markdown).Returns(x => { throw new Exception("Test rendering error"); });

        // Act
        var cut = RenderComponent<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, markdown)
        );

        // Assert
        cut.Markup.ShouldContain("markdown-error");
        cut.Markup.ShouldContain("Error rendering markdown");
    }
}