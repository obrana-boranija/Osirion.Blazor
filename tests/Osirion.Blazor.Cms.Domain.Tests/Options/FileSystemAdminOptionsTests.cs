using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Domain.Tests.Options;

public class FileSystemAdminOptionsTests
{
    [Fact]
    public void DefaultProperties_HaveExpectedValues()
    {
        // Arrange
        var options = new FileSystemAdminOptions();

        // Assert
        options.RootPath.ShouldBe(string.Empty);
        options.ContentDirectory.ShouldBe("content");
        options.CreateDirectoriesIfNotExist.ShouldBeTrue();
    }

    [Fact]
    public void Properties_CanBeSet_AndRetrieved()
    {
        // Arrange
        var options = new FileSystemAdminOptions
        {
            RootPath = "/var/www/data",
            ContentDirectory = "custom-content",
            CreateDirectoriesIfNotExist = false
        };

        // Assert
        options.RootPath.ShouldBe("/var/www/data");
        options.ContentDirectory.ShouldBe("custom-content");
        options.CreateDirectoriesIfNotExist.ShouldBeFalse();
    }

    [Fact]
    public void ContentDirectory_DefaultValueIsCorrect()
    {
        // Arrange
        var options = new FileSystemAdminOptions();

        // Assert
        options.ContentDirectory.ShouldBe("content");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("/path/to/content")]
    [InlineData("C:\\path\\to\\content")]
    public void RootPath_CanBeSetToAnyValue(string path)
    {
        // Arrange
        var options = new FileSystemAdminOptions
        {
            RootPath = path
        };

        // Assert
        options.RootPath.ShouldBe(path ?? string.Empty);
    }

    [Fact]
    public void CreateDirectoriesIfNotExist_DefaultValueIsTrue()
    {
        // Arrange
        var options = new FileSystemAdminOptions();

        // Assert
        options.CreateDirectoriesIfNotExist.ShouldBeTrue();
    }
}