using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Tests.Options;

public class GitHubAdminOptionsTests
{
    [Fact]
    public void DefaultProperties_HaveExpectedValues()
    {
        // Arrange
        var options = new GitHubAdminOptions();

        // Assert
        options.Owner.ShouldBe(string.Empty);
        options.Repository.ShouldBe(string.Empty);
        options.DefaultBranch.ShouldBe("main");
        options.ApiUrl.ShouldBe("https://api.github.com");
        options.ContentPath.ShouldBe(string.Empty);
        options.CommitterName.ShouldBeNull();
        options.CommitterEmail.ShouldBeNull();
        options.AllowBranchCreation.ShouldBeTrue();
        options.AllowFileDelete.ShouldBeTrue();
    }

    [Fact]
    public void Properties_CanBeSet_AndRetrieved()
    {
        // Arrange
        var options = new GitHubAdminOptions
        {
            Owner = "test-owner",
            Repository = "test-repo",
            DefaultBranch = "develop",
            ApiUrl = "https://github.acme.com/api/v3",
            ContentPath = "docs",
            CommitterName = "Test User",
            CommitterEmail = "test@example.com",
            AllowBranchCreation = false,
            AllowFileDelete = false
        };

        // Assert
        options.Owner.ShouldBe("test-owner");
        options.Repository.ShouldBe("test-repo");
        options.DefaultBranch.ShouldBe("develop");
        options.ApiUrl.ShouldBe("https://github.acme.com/api/v3");
        options.ContentPath.ShouldBe("docs");
        options.CommitterName.ShouldBe("Test User");
        options.CommitterEmail.ShouldBe("test@example.com");
        options.AllowBranchCreation.ShouldBeFalse();
        options.AllowFileDelete.ShouldBeFalse();
    }

    [Fact]
    public void DefaultBranch_DefaultValueIsMain()
    {
        // Arrange
        var options = new GitHubAdminOptions();

        // Assert
        options.DefaultBranch.ShouldBe("main");
    }

    [Fact]
    public void ApiUrl_DefaultValueIsGitHubCom()
    {
        // Arrange
        var options = new GitHubAdminOptions();

        // Assert
        options.ApiUrl.ShouldBe("https://api.github.com");
    }

    [Fact]
    public void AllowBranchCreation_DefaultValueIsTrue()
    {
        // Arrange
        var options = new GitHubAdminOptions();

        // Assert
        options.AllowBranchCreation.ShouldBeTrue();
    }

    [Fact]
    public void AllowFileDelete_DefaultValueIsTrue()
    {
        // Arrange
        var options = new GitHubAdminOptions();

        // Assert
        options.AllowFileDelete.ShouldBeTrue();
    }

    [Theory]
    [InlineData("owner1", "repo1")]
    [InlineData("", "")]
    public void OwnerAndRepository_CanBeSetToAnyValue(string owner, string repo)
    {
        // Arrange
        var options = new GitHubAdminOptions
        {
            Owner = owner,
            Repository = repo
        };

        // Assert
        options.Owner.ShouldBe(owner);
        options.Repository.ShouldBe(repo);
    }
}