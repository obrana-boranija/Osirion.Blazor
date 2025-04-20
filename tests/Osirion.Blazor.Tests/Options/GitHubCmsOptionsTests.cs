using Osirion.Blazor.Options;
using Shouldly;
using System.Net;

namespace Osirion.Blazor.Tests.Options;

public class GitHubCmsOptionsTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var options = new GitHubCmsOptions();

        // Assert
        options.Owner.ShouldBe(string.Empty);
        options.Repository.ShouldBe(string.Empty);
        options.ContentPath.ShouldBe(string.Empty);
        options.Branch.ShouldBe("main");
        options.ApiToken.ShouldBeNull();
        options.CacheDurationMinutes.ShouldBe(30);
        options.SupportedExtensions.ShouldBe(new List<string> { ".md", ".markdown" });
    }

    [Fact]
    public void Section_ShouldBeCorrect()
    {
        // Assert
        GitHubCmsOptions.Section.ShouldBe("GitHubCms");
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var options = new GitHubCmsOptions();

        // Act
        options.Owner = "test-owner";
        options.Repository = "test-repo";
        options.ContentPath = "content";
        options.Branch = "develop";
        options.ApiToken = "test-token";
        options.CacheDurationMinutes = 60;
        options.SupportedExtensions = new List<string> { ".mdx", ".md" };

        // Assert
        options.Owner.ShouldBe("test-owner");
        options.Repository.ShouldBe("test-repo");
        options.ContentPath.ShouldBe("content");
        options.Branch.ShouldBe("develop");
        options.ApiToken.ShouldBe("test-token");
        options.CacheDurationMinutes.ShouldBe(60);
        options.SupportedExtensions.ShouldBe(new List<string> { ".mdx", ".md" });
    }
}