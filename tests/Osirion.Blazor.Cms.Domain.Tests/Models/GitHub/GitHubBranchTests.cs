using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubBranchTests
{
    [Fact]
    public void GitHubBranch_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var branch = new GitHubBranch();

        // Assert
        Assert.Equal(string.Empty, branch.Name);
        Assert.NotNull(branch.Commit);
        Assert.Equal(string.Empty, branch.Commit.Sha);
        Assert.Equal(string.Empty, branch.Commit.Url);
        Assert.False(branch.Protected);
    }

    [Fact]
    public void GitHubBranch_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var commitRef = new GitHubCommitRef
        {
            Sha = "1234567890abcdef1234567890abcdef12345678",
            Url = "https://api.github.com/repos/owner/repo/git/commits/1234567890abcdef1234567890abcdef12345678"
        };

        // Act
        var branch = new GitHubBranch
        {
            Name = "main",
            Commit = commitRef,
            Protected = true
        };

        // Assert
        Assert.Equal("main", branch.Name);
        Assert.NotNull(branch.Commit);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", branch.Commit.Sha);
        Assert.Equal("https://api.github.com/repos/owner/repo/git/commits/1234567890abcdef1234567890abcdef12345678", branch.Commit.Url);
        Assert.True(branch.Protected);
    }

    [Theory]
    [InlineData("main")]
    [InlineData("develop")]
    [InlineData("feature/new-component")]
    [InlineData("release/v1.0.0")]
    [InlineData("hotfix/critical-issue")]
    public void GitHubBranch_WithDifferentNames_BranchNameIsSetCorrectly(string name)
    {
        // Arrange & Act
        var branch = new GitHubBranch
        {
            Name = name
        };

        // Assert
        Assert.Equal(name, branch.Name);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GitHubBranch_WithProtectionSetting_ProtectedIsSetCorrectly(bool isProtected)
    {
        // Arrange & Act
        var branch = new GitHubBranch
        {
            Protected = isProtected
        };

        // Assert
        Assert.Equal(isProtected, branch.Protected);
    }
}