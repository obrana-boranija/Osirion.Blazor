using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using System.Text;

namespace Osirion.Blazor.Cms.Tests.Domain.Models;

public class BlogPostTests
{
    [Fact]
    public void Constructor_InitializesEmptyProperties()
    {
        // Act
        var blogPost = new BlogPost();

        // Assert
        Assert.Null(blogPost.Metadata);
        Assert.Empty(blogPost.Content);
        Assert.Empty(blogPost.FilePath);
        Assert.Empty(blogPost.Sha);
        Assert.Equal("", blogPost.FileName);
        Assert.Equal("", blogPost.Directory);
        Assert.Equal("", blogPost.Extension);
    }

    [Fact]
    public void Constructor_WithMetadataAndContent_InitializesProperties()
    {
        // Arrange
        var metadata = FrontMatter.Create("Test Title")
            .WithDescription("Test Description");
        string content = "# Test Content";

        // Act
        var blogPost = new BlogPost(metadata, content);

        // Assert
        Assert.Same(metadata, blogPost.Metadata);
        Assert.Equal(content, blogPost.Content);
        Assert.Empty(blogPost.FilePath);
        Assert.Empty(blogPost.Sha);
    }

    [Fact]
    public void ToMarkdown_WithMetadataAndContent_ReturnsFormattedMarkdown()
    {
        // Arrange
        var metadata = FrontMatter.Create("Test Title")
            .WithDescription("Test Description")
            .WithAuthor("Test Author")
            .WithDate(new DateTime(2025, 1, 1));

        string content = "# Test Content\n\nThis is a test.";

        var blogPost = new BlogPost(metadata, content);

        // Act
        string markdown = blogPost.ToMarkdown();

        // Assert
        Assert.Contains("---", markdown);
        Assert.Contains("title: \"Test Title\"", markdown);
        Assert.Contains("description: \"Test Description\"", markdown);
        Assert.Contains("author: \"Test Author\"", markdown);
        Assert.Contains("date: 2025-01-01", markdown);
        Assert.Contains("# Test Content", markdown);
        Assert.Contains("This is a test.", markdown);
    }

    [Fact]
    public void FromMarkdown_WithFrontMatterAndContent_ParsesCorrectly()
    {
        // Arrange
        string markdown = @"---
title: ""Markdown Title""
description: ""Markdown Description""
author: ""Markdown Author""
date: 2025-02-15
---

# Markdown Content

This is the content.";

        // Act
        var blogPost = BlogPost.FromMarkdown(markdown);

        // Assert
        Assert.NotNull(blogPost.Metadata);
        Assert.Equal("Markdown Title", blogPost.Metadata.Title);
        Assert.Equal("Markdown Description", blogPost.Metadata.Description);
        Assert.Equal("Markdown Author", blogPost.Metadata.Author);
        Assert.Equal("2025-02-15", blogPost.Metadata.Date);
        Assert.Contains("# Markdown Content", blogPost.Content);
        Assert.Contains("This is the content.", blogPost.Content);
    }

    [Fact]
    public void FromMarkdown_WithoutFrontMatter_SetsContentOnly()
    {
        // Arrange
        string markdown = "# No Front Matter\n\nJust content.";

        // Act
        var blogPost = BlogPost.FromMarkdown(markdown);

        // Assert
        Assert.NotNull(blogPost.Metadata);
        Assert.Empty(blogPost.Metadata.Title);
        Assert.Equal("# No Front Matter\n\nJust content.", blogPost.Content.Trim());
    }

    [Fact]
    public void FromMarkdown_WithEmptyString_ReturnsEmptyBlogPost()
    {
        // Arrange
        string markdown = "";

        // Act
        var blogPost = BlogPost.FromMarkdown(markdown);

        // Assert
        Assert.NotNull(blogPost.Metadata);
        Assert.Empty(blogPost.Metadata.Title);
        Assert.Empty(blogPost.Content);
    }

    [Fact]
    public void FromGitHubFile_WithMarkdownFile_ParsesContent()
    {
        // Arrange
        var fileContent = new GitHubFileContent
        {
            Name = "test-post.md",
            Path = "blog/test-post.md",
            Sha = "abc123",
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(@"---
title: ""GitHub Title""
description: ""GitHub Description""
---

# GitHub Content")),
            Encoding = "base64",
            Type = "file"
        };

        // Act
        var blogPost = BlogPost.FromGitHubFile(fileContent);

        // Assert
        Assert.Equal("blog/test-post.md", blogPost.FilePath);
        Assert.Equal("abc123", blogPost.Sha);
        Assert.Equal("test-post.md", blogPost.FileName);
        Assert.Equal("blog", blogPost.Directory);
        Assert.Equal(".md", blogPost.Extension);

        Assert.NotNull(blogPost.Metadata);
        Assert.Equal("GitHub Title", blogPost.Metadata.Title);
        Assert.Equal("GitHub Description", blogPost.Metadata.Description);
        Assert.Contains("# GitHub Content", blogPost.Content);
    }

    [Fact]
    public void FromGitHubFile_WithNonMarkdownFile_InitializesEmptyBlogPost()
    {
        // Arrange
        var fileContent = new GitHubFileContent
        {
            Name = "test-file.txt",
            Path = "files/test-file.txt",
            Sha = "def456",
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("Plain text content")),
            Encoding = "base64",
            Type = "file"
        };

        // Act
        var blogPost = BlogPost.FromGitHubFile(fileContent);

        // Assert
        Assert.Equal("files/test-file.txt", blogPost.FilePath);
        Assert.Equal("def456", blogPost.Sha);
        Assert.Equal("test-file.txt", blogPost.FileName);
        Assert.Equal("files", blogPost.Directory);
        Assert.Equal(".txt", blogPost.Extension);

        Assert.NotNull(blogPost.Metadata);
        Assert.Empty(blogPost.Metadata.Title);
        Assert.Empty(blogPost.Content);
    }

    [Fact]
    public void FromGitHubFile_WithNullFile_ReturnsEmptyBlogPost()
    {
        // Act
        var blogPost = BlogPost.FromGitHubFile(null);

        // Assert
        Assert.Empty(blogPost.FilePath);
        Assert.Empty(blogPost.Sha);
        Assert.Empty(blogPost.FileName);
        Assert.Empty(blogPost.Directory);
        Assert.Empty(blogPost.Extension);
    }

    [Fact]
    public void FileName_Directory_Extension_DerivedFromFilePath()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "blog/2025/01/test-post.md"
        };

        // Act & Assert
        Assert.Equal("test-post.md", blogPost.FileName);
        Assert.Equal("blog/2025/01", blogPost.Directory);
        Assert.Equal(".md", blogPost.Extension);
    }

    [Fact]
    public void FileName_Directory_Extension_WithEmptyFilePath_ReturnsEmpty()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = ""
        };

        // Act & Assert
        Assert.Equal("", blogPost.FileName);
        Assert.Equal("", blogPost.Directory);
        Assert.Equal("", blogPost.Extension);
    }

    [Fact]
    public void FileName_Directory_Extension_WithRootFilePath_ReturnsCorrectValues()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            FilePath = "root-file.md"
        };

        // Act & Assert
        Assert.Equal("root-file.md", blogPost.FileName);
        Assert.Equal("", blogPost.Directory);
        Assert.Equal(".md", blogPost.Extension);
    }
}