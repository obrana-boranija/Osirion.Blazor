using Bunit;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NSubstitute;
using Osirion.Blazor.Cms.Core.Tests.TestFixtures;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Components.Editor;

public class MarkdownPreviewTests : BunitContext, IDisposable
{
    private readonly IMarkdownRendererService _markdownRenderer;
    private readonly CapturingLogger<MarkdownPreview> _logger = new();

    public MarkdownPreviewTests()
    {
        // Register a custom mock JSRuntime implementation
        Services.AddSingleton<IJSRuntime>(new MockJSRuntime());

        // Setup MarkdownRenderer
        _markdownRenderer = Substitute.For<IMarkdownRendererService>();
        Services.AddSingleton(_markdownRenderer);
        Services.AddSingleton<ILogger<MarkdownPreview>>(_logger);

        SetRendererInfo(new RendererInfo("Server", true));

        // Configure default behavior
        _markdownRenderer.RenderToHtml(Arg.Any<string>()).Returns(x => $"<p>{x.Arg<string>()}</p>");
    }

    [Fact]
    public void MarkdownPreview_ShouldRender_WithDefaultSettings()
    {
        // Act
        var cut = Render<MarkdownPreview>();

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
        var cut = Render<MarkdownPreview>(parameters => parameters
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
        var cut = Render<MarkdownPreview>(parameters => parameters
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
        var cut = Render<MarkdownPreview>(parameters => parameters
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
        var cut = Render<MarkdownPreview>(parameters => parameters
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
        var cut = Render<MarkdownPreview>(parameters => parameters
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
        var cut = Render<MarkdownPreview>();
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
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, markdown)
        );

        // Assert
        cut.Markup.ShouldContain("markdown-error");
        cut.Markup.ShouldContain("Error rendering markdown");
    }

    // Fails before the fix: the renderer output was written to the page unsanitized.
    [Fact]
    public void MarkdownPreview_RendererOutputWithScriptAndHandler_RendersWithoutThem()
    {
        // Arrange
        var markdown = "Preview";
        _markdownRenderer.RenderToHtml(markdown).Returns("<p>Preview</p><img src=\"/a.png\" onerror=\"alert(1)\"><script>alert(2)</script>");

        // Act
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, markdown)
        );

        // Assert
        cut.FindAll("script").ShouldBeEmpty();
        cut.Find("img").HasAttribute("onerror").ShouldBeFalse();
        cut.Find("p").TextContent.ShouldBe("Preview");
    }

    // Fails before the fix: a caller-supplied pipeline bypassed the renderer service and any sanitization.
    [Fact]
    public void MarkdownPreview_CustomPipelineWithRawHtml_RendersWithoutHandlers()
    {
        // Arrange
        var markdown = "Text <img src=\"/a.png\" onerror=\"alert(1)\">\n\n<script>alert(2)</script>";
        var customPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        // Act
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, markdown)
            .Add(p => p.Pipeline, customPipeline)
        );

        // Assert
        cut.FindAll("script").ShouldBeEmpty();
        cut.Find("img").HasAttribute("onerror").ShouldBeFalse();
    }

    // Fails before the fix: the exception message was inserted into the markup without encoding.
    [Fact]
    public void MarkdownPreview_RenderingFailsWithMarkupInMessage_RendersNoMarkupFromTheMessage()
    {
        // Arrange
        var markdown = "Test content";
        _markdownRenderer.RenderToHtml(markdown).Returns(x => { throw new Exception("<img src=x onerror=alert(1)>"); });

        // Act
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, markdown)
        );

        // Assert
        cut.FindAll(".markdown-error img").ShouldBeEmpty();
    }

    // Fails before the fix: the exception message, which can carry internal details, was shown in the page.
    [Fact]
    public void MarkdownPreview_RenderingFails_DoesNotShowTheExceptionMessage()
    {
        // Arrange
        var markdown = "Test content";
        _markdownRenderer.RenderToHtml(markdown).Returns(x => { throw new InvalidOperationException("C:\\secrets\\pipeline.config"); });

        // Act
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, markdown)
        );

        // Assert
        cut.Find(".markdown-error").TextContent.ShouldNotContain("pipeline.config");
    }

    [Fact]
    public void MarkdownPreview_RenderingFails_LogsTheException()
    {
        // Arrange
        var failure = new InvalidOperationException("boom");
        _markdownRenderer.RenderToHtml("Test content").Returns(x => { throw failure; });

        // Act
        Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, "Test content")
        );

        // Assert
        var entry = _logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Error);
        entry.Exception.ShouldBeSameAs(failure);
    }

    // Fails before the fix: every render, including the one after each scroll event, rendered and sanitized
    // the whole document again.
    [Fact]
    public async Task MarkdownPreview_ScrollEvent_DoesNotRenderTheMarkdownAgain()
    {
        // Arrange
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.SyncScroll, true)
            .Add(p => p.OnScroll, (double _) => { })
            .Add(p => p.Markdown, "Scrolled content")
        );

        // Act
        await cut.Find(".osirion-markdown-preview-content").ScrollAsync();
        await cut.Find(".osirion-markdown-preview-content").ScrollAsync();

        // Assert
        _markdownRenderer.Received(1).RenderToHtml("Scrolled content");
    }

    [Fact]
    public void MarkdownPreview_MarkdownChanges_RendersTheNewMarkdown()
    {
        // Arrange
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, "First")
        );

        // Act
        cut.Render(parameters => parameters.Add(p => p.Markdown, "Second"));

        // Assert
        cut.Find(".osirion-markdown-preview-content p").TextContent.ShouldBe("Second");
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, Exception? Exception)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Entries.Add((logLevel, exception));
    }
}