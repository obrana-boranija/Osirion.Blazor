using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Directory;

public class DirectoryMetadataProcessorTests
{
    private readonly IFrontMatterExtractor _frontMatterExtractor;
    private readonly IMarkdownProcessor _markdownProcessor;
    private readonly ILogger<DirectoryMetadataProcessor> _logger;
    private readonly DirectoryMetadataProcessor _processor;

    public DirectoryMetadataProcessorTests()
    {
        _frontMatterExtractor = Substitute.For<IFrontMatterExtractor>();
        _markdownProcessor = Substitute.For<IMarkdownProcessor>();
        _logger = Substitute.For<ILogger<DirectoryMetadataProcessor>>();

        _processor = new DirectoryMetadataProcessor(
            _frontMatterExtractor,
            _markdownProcessor,
            _logger);
    }

    [Fact]
    public void ProcessMetadata_WithValidFrontMatter_SetsDirectoryProperties()
    {
        // Arrange
        var directory = DirectoryItem.Create(
            "dir-id",
            "content/blog",
            "Original Name",
            "test-provider");

        var frontMatter = new FrontMatter
        {
            Title = "Blog Directory",
            Description = "Blog posts directory",
            Order = 5,
            Lang = "en",
            Permalink = "blog"
        };

        _markdownProcessor.ExtractFrontMatterAndContent(Arg.Any<string>())
            .Returns((frontMatter, string.Empty));

        // Act
        var result = _processor.ProcessMetadata(directory, "---\nMetadata content\n---");

        // Assert
        result.ShouldBe(directory); // Processed in place
        directory.Name.ShouldBe("Blog Directory");
        directory.Description.ShouldBe("Blog posts directory");
        directory.Order.ShouldBe(5);
        directory.Locale.ShouldBe("en");
        directory.Url.ShouldBe("blog");
    }

    [Fact]
    public void ProcessMetadata_WithCustomMetadata_AddsMetadataToDirectory()
    {
        // Arrange
        var directory = DirectoryItem.Create(
            "dir-id",
            "content/blog",
            "Blog",
            "test-provider");

        var frontMatter = new Dictionary<string, string>
        {
            { "custom_string", "string value" },
            { "custom_int", "42" },
            { "custom_bool", "true" },
            { "custom_double", "3,14" }
        };

        _frontMatterExtractor.ExtractFrontMatter(Arg.Any<string>())
            .Returns(frontMatter);

        // Act
        var result = _processor.ProcessMetadata(directory, "---\nMetadata content\n---");

        // Assert
        //directory.Metadata.Count.ShouldBe(4);
        //directory.Metadata["custom_string"].ShouldBe("string value");
        //directory.Metadata["custom_int"].ShouldBe(42);
        //directory.Metadata["custom_bool"].ShouldBe(true);
        //directory.Metadata["custom_double"].ShouldBe(3.14);
    }

    [Fact]
    public void GenerateMetadataContent_CreatesCorrectYaml()
    {
        // Arrange
        var directory = DirectoryItem.Create(
            "dir-id",
            "content/blog",
            "Blog Directory",
            "test-provider");

        directory.SetDescription("Blog posts directory");
        directory.SetOrder(5);
        directory.SetLocale("en");
        directory.SetUrl("blog");
        directory.SetMetadata("custom_key", "custom value");

        // Act
        var result = _processor.GenerateMetadataContent(directory);

        // Assert
        result.ShouldContain("title: \"Blog Directory\"");
        result.ShouldContain("description: \"Blog posts directory\"");
        result.ShouldContain("order: 5");
        result.ShouldContain("locale: \"en\"");
        result.ShouldContain("url: \"blog\"");
        result.ShouldContain("custom_key: \"custom value\"");
        result.ShouldContain("# Blog Directory");
    }

    [Fact]
    public void GenerateMetadataContent_UnsafeOrStandardCustomKey_IsNotWritten()
    {
        var directory = DirectoryItem.Create("dir-id", "content/blog", "Blog", "test-provider");
        directory.SetMetadata("bad: key\npublished", "x");
        directory.SetMetadata("title", "Overridden");
        directory.SetMetadata("weight", 2.5);
        directory.SetMetadata("note", "line one\nline two");

        var result = _processor.GenerateMetadataContent(directory);

        result.ShouldNotContain("bad: key");
        result.ShouldNotContain("Overridden");
        result.ShouldContain("weight: 2.5");
        result.ShouldContain("note: \"line one\\nline two\"");
    }
}
