using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Services;

public class MarkdownEditorServiceTests
{
    private readonly IMarkdownEditorService _markdownEditorService;
    private readonly ILogger<MarkdownEditorService> _logger;

    public MarkdownEditorServiceTests()
    {
        _logger = Substitute.For<ILogger<MarkdownEditorService>>();

        // Create the service with options using reflection to bypass the dependency
        var options = Microsoft.Extensions.Options.Options.Create(new Osirion.Blazor.Cms.Domain.Options.Configuration.CmsAdminOptions());
        _markdownEditorService = new MarkdownEditorService(options, _logger);
    }

    [Fact]
    public async Task RenderToHtmlAsync_WithValidMarkdown_ShouldReturnHtml()
    {
        // Arrange
        var markdown = "# Test Heading\n\nThis is a test paragraph with **bold** text.";

        // Act
        var html = await _markdownEditorService.RenderToHtmlAsync(markdown);

        // Assert
        html.ShouldNotBeNullOrEmpty();
        html.ShouldContain("<h1>Test Heading</h1>");
        html.ShouldContain("<p>This is a test paragraph with <strong>bold</strong> text.</p>");
    }

    [Fact]
    public async Task RenderToHtmlAsync_WithEmptyInput_ShouldReturnEmpty()
    {
        // Arrange
        var markdown = string.Empty;

        // Act
        var html = await _markdownEditorService.RenderToHtmlAsync(markdown);

        // Assert
        html.ShouldBe(string.Empty);
    }

    [Fact]
    public async Task ParseFrontMatterAsync_WithValidFrontMatter_ShouldReturnParsedData()
    {
        // Arrange
        var markdown = @"---
title: Test Title
description: Test Description
author: Test Author
date: 2023-01-01
tags:
  - test
  - markdown
---

# Content starts here";

        // Act
        var frontMatter = await _markdownEditorService.ParseFrontMatterAsync(markdown);

        // Assert
        frontMatter.ShouldNotBeNull();
        frontMatter.ShouldContainKey("title");
        frontMatter["title"].ShouldBe("Test Title");
        frontMatter.ShouldContainKey("description");
        frontMatter["description"].ShouldBe("Test Description");
        frontMatter.ShouldContainKey("author");
        frontMatter["author"].ShouldBe("Test Author");
        frontMatter.ShouldContainKey("date");
        frontMatter["date"].ShouldBe("2023-01-01");
    }

    [Fact]
    public async Task ParseFrontMatterAsync_WithNoFrontMatter_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var markdown = "# Just Content\n\nNo front matter here.";

        // Act
        var frontMatter = await _markdownEditorService.ParseFrontMatterAsync(markdown);

        // Assert
        frontMatter.ShouldNotBeNull();
        frontMatter.ShouldBeEmpty();
    }

    [Fact]
    public async Task ExtractContentAsync_WithFrontMatter_ShouldReturnOnlyContent()
    {
        // Arrange
        var markdown = @"---
title: Test Title
description: Test Description
---

# Just Content

This is the actual content.";

        // Act
        var content = await _markdownEditorService.ExtractContentAsync(markdown);

        // Assert
        content.ShouldNotBeNullOrEmpty();
        content.ShouldStartWith("# Just Content");
        content.ShouldContain("This is the actual content.");
        content.ShouldNotContain("title: Test Title");
    }

    [Fact]
    public async Task ExtractContentAsync_WithNoFrontMatter_ShouldReturnOriginalContent()
    {
        // Arrange
        var markdown = "# Just Content\n\nNo front matter here.";

        // Act
        var content = await _markdownEditorService.ExtractContentAsync(markdown);

        // Assert
        content.ShouldBe(markdown);
    }

    [Fact]
    public void GenerateFileNameFromTitle_ShouldReturnValidFileName()
    {
        // This method is in ContentEditorService, but I wanted to include a test for it here
        // as it's related to content editing

        // Create a ContentEditorService instance with minimal dependencies
        var repositoryAdapter = Substitute.For<Infrastructure.Adapters.IContentRepositoryAdapter>();
        var contentService = Substitute.For<IAdminContentService>();
        var eventPublisher = Substitute.For<IEventPublisher>();
        var options = Microsoft.Extensions.Options.Options.Create(new Osirion.Blazor.Cms.Domain.Options.Configuration.CmsAdminOptions());
        var logger = Substitute.For<ILogger<ContentEditorService>>();

        var contentEditorService = new ContentEditorService(
            repositoryAdapter,
            contentService,
            eventPublisher,
            options,
            logger
        );

        // Test cases
        contentEditorService.GenerateFileNameFromTitle("Test Title").ShouldBe("test-title.md");
        contentEditorService.GenerateFileNameFromTitle("Special Ch@racters & Spaces").ShouldBe("special-chracters-spaces.md");
        contentEditorService.GenerateFileNameFromTitle("Multiple---Hyphens").ShouldBe("multiple-hyphens.md");
        contentEditorService.GenerateFileNameFromTitle("").ShouldBe("new-post.md");
    }
}