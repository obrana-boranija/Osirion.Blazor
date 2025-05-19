using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Domain.Tests.Options;

public class ContentRulesOptionsTests
{
    [Fact]
    public void DefaultProperties_HaveExpectedValues()
    {
        // Arrange
        var options = new ContentRulesOptions();

        // Assert
        options.RequireApproval.ShouldBeFalse();
        options.MaximumDraftAge.ShouldBe(30);
        options.EnforceFrontMatterValidation.ShouldBeTrue();
        options.RequiredFrontMatterFields.ShouldNotBeNull();
        options.RequiredFrontMatterFields.ShouldContain("title");
        options.AutoGenerateSlugs.ShouldBeTrue();
        options.AllowedFileExtensions.ShouldNotBeNull();
        options.AllowedFileExtensions.ShouldContain(".md");
        options.AllowedFileExtensions.ShouldContain(".markdown");
        options.AllowFileDeletion.ShouldBeTrue();
        options.MaximumFileSize.ShouldBe(5 * 1024 * 1024); // 5MB
    }

    [Fact]
    public void Properties_CanBeSet_AndRetrieved()
    {
        // Arrange
        var options = new ContentRulesOptions
        {
            RequireApproval = true,
            MaximumDraftAge = 15,
            EnforceFrontMatterValidation = false,
            RequiredFrontMatterFields = new List<string> { "title", "description", "author" },
            AutoGenerateSlugs = false,
            AllowedFileExtensions = new List<string> { ".md", ".txt" },
            AllowFileDeletion = false,
            MaximumFileSize = 10 * 1024 * 1024 // 10MB
        };

        // Assert
        options.RequireApproval.ShouldBeTrue();
        options.MaximumDraftAge.ShouldBe(15);
        options.EnforceFrontMatterValidation.ShouldBeFalse();
        options.RequiredFrontMatterFields.Count.ShouldBe(3);
        options.RequiredFrontMatterFields.ShouldContain("title");
        options.RequiredFrontMatterFields.ShouldContain("description");
        options.RequiredFrontMatterFields.ShouldContain("author");
        options.AutoGenerateSlugs.ShouldBeFalse();
        options.AllowedFileExtensions.Count.ShouldBe(2);
        options.AllowedFileExtensions.ShouldContain(".md");
        options.AllowedFileExtensions.ShouldContain(".txt");
        options.AllowFileDeletion.ShouldBeFalse();
        options.MaximumFileSize.ShouldBe(10 * 1024 * 1024);
    }

    [Fact]
    public void RequiredFrontMatterFields_DefaultValueIsCorrect()
    {
        // Arrange
        var options = new ContentRulesOptions();

        // Assert
        options.RequiredFrontMatterFields.ShouldNotBeNull();
        options.RequiredFrontMatterFields.Count.ShouldBe(1);
        options.RequiredFrontMatterFields.ShouldContain("title");
    }

    [Fact]
    public void AllowedFileExtensions_DefaultValueIsCorrect()
    {
        // Arrange
        var options = new ContentRulesOptions();

        // Assert
        options.AllowedFileExtensions.ShouldNotBeNull();
        options.AllowedFileExtensions.Count.ShouldBe(2);
        options.AllowedFileExtensions.ShouldContain(".md");
        options.AllowedFileExtensions.ShouldContain(".markdown");
    }
}