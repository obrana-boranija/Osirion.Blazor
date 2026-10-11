using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Directory;

public class DirectoryMetadataProcessorEdgeCaseTests
{
    private readonly IMarkdownProcessor _markdownProcessor = Substitute.For<IMarkdownProcessor>();
    private readonly DirectoryMetadataProcessor _processor;

    public DirectoryMetadataProcessorEdgeCaseTests()
    {
        _processor = new DirectoryMetadataProcessor(
            Substitute.For<IFrontMatterExtractor>(),
            _markdownProcessor,
            NullLogger<DirectoryMetadataProcessor>.Instance);
    }

    [Fact]
    public void ProcessMetadata_NullDirectory_Throws()
    {
        Should.Throw<ArgumentNullException>(() => _processor.ProcessMetadata(null!, "---"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void ProcessMetadata_BlankContent_ReturnsTheDirectoryUnchanged(string content)
    {
        var directory = DirectoryItem.Create("a", "a", "Original", "test");

        var result = _processor.ProcessMetadata(directory, content);

        result.ShouldBeSameAs(directory);
        result.Name.ShouldBe("Original");
        _markdownProcessor.DidNotReceiveWithAnyArgs().ExtractFrontMatterAndContent(default!);
    }

    [Fact]
    public void ProcessMetadata_ExtractionFails_ReturnsTheDirectoryUnchanged()
    {
        _markdownProcessor.ExtractFrontMatterAndContent(Arg.Any<string>()).Throws(new FormatException("bad yaml"));
        var directory = DirectoryItem.Create("a", "a", "Original", "test");

        var result = _processor.ProcessMetadata(directory, "---\ntitle: [\n---");

        result.ShouldBeSameAs(directory);
        result.Name.ShouldBe("Original");
    }

    [Fact]
    public void GenerateMetadataContent_NullDirectory_Throws()
    {
        Should.Throw<ArgumentNullException>(() => _processor.GenerateMetadataContent(null!));
    }

    [Fact]
    public void GenerateMetadataContent_CustomValuesOfEachKind_AreFormattedForYaml()
    {
        var directory = DirectoryItem.Create("a", "a", "Docs", "test");
        directory.SetMetadata("draft", true);
        directory.SetMetadata("hidden", false);
        directory.SetMetadata("weight", 2.5);
        directory.SetMetadata("note", "say \"hi\"");

        var content = _processor.GenerateMetadataContent(directory);

        var lines = content.Split(Environment.NewLine);
        lines.ShouldContain("draft: true");
        lines.ShouldContain("hidden: false");
        lines.ShouldContain("weight: 2.5");
        lines.ShouldContain("note: \"say \\\"hi\\\"\"");
    }
}
