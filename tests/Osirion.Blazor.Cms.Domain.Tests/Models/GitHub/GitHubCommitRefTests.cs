using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubCommitRefTests
{
    [Fact]
    public void GitHubCommitRef_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var commitRef = new GitHubCommitRef();

        // Assert
        Assert.Equal(string.Empty, commitRef.Sha);
        Assert.Equal(string.Empty, commitRef.Url);
    }

    [Fact]
    public void GitHubCommitRef_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange & Act
        var commitRef = new GitHubCommitRef
        {
            Sha = "1234567890abcdef1234567890abcdef12345678",
            Url = "https://api.github.com/repos/owner/repo/git/commits/1234567890abcdef1234567890abcdef12345678"
        };

        // Assert
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", commitRef.Sha);
        Assert.Equal("https://api.github.com/repos/owner/repo/git/commits/1234567890abcdef1234567890abcdef12345678", commitRef.Url);
    }

    [Theory]
    [InlineData("1234567890abcdef1234567890abcdef12345678")]
    [InlineData("abcdef1234567890abcdef1234567890abcdef12")]
    [InlineData("0123456789abcdef0123456789abcdef01234567")]
    public void GitHubCommitRef_WithDifferentShaValues_ShaIsSetCorrectly(string sha)
    {
        // Arrange & Act
        var commitRef = new GitHubCommitRef
        {
            Sha = sha
        };

        // Assert
        Assert.Equal(sha, commitRef.Sha);
    }

    [Fact]
    public void GitHubCommitRef_WithEmptyStrings_PropertiesAreSetToEmpty()
    {
        // Arrange & Act
        var commitRef = new GitHubCommitRef
        {
            Sha = string.Empty,
            Url = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, commitRef.Sha);
        Assert.Equal(string.Empty, commitRef.Url);
    }
}