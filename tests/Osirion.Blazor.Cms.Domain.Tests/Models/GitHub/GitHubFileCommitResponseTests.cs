using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubFileCommitResponseTests
{
    [Fact]
    public void GitHubFileCommitResponse_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var response = new GitHubFileCommitResponse();

        // Assert
        Assert.NotNull(response.Content);
        Assert.NotNull(response.Commit);
        Assert.Equal(string.Empty, response.Content.Name);
        Assert.Equal(string.Empty, response.Commit.Sha);
        Assert.False(response.Success);
        Assert.Null(response.ErrorMessage);
    }

    [Fact]
    public void GitHubFileCommitResponse_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var content = new GitHubFileContent
        {
            Name = "README.md",
            Path = "docs/README.md",
            Sha = "1234567890abcdef1234567890abcdef12345678"
        };

        var commit = new GitHubCommitInfo
        {
            Sha = "abcdef1234567890abcdef1234567890abcdef12",
            Message = "Add README.md",
            HtmlUrl = "https://github.com/owner/repo/commit/abcdef1234567890abcdef1234567890abcdef12"
        };

        // Act
        var response = new GitHubFileCommitResponse
        {
            Content = content,
            Commit = commit,
            Success = true
        };

        // Assert
        Assert.True(response.Success);
        Assert.Null(response.ErrorMessage);
        Assert.NotNull(response.Content);
        Assert.NotNull(response.Commit);
        Assert.Equal("README.md", response.Content.Name);
        Assert.Equal("docs/README.md", response.Content.Path);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", response.Content.Sha);
        Assert.Equal("abcdef1234567890abcdef1234567890abcdef12", response.Commit.Sha);
        Assert.Equal("Add README.md", response.Commit.Message);
        Assert.Equal("https://github.com/owner/repo/commit/abcdef1234567890abcdef1234567890abcdef12", response.Commit.HtmlUrl);
    }

    [Fact]
    public void GitHubFileCommitResponse_WithErrorMessage_ErrorMessageIsSet()
    {
        // Arrange & Act
        var response = new GitHubFileCommitResponse
        {
            Success = false,
            ErrorMessage = "File already exists"
        };

        // Assert
        Assert.False(response.Success);
        Assert.Equal("File already exists", response.ErrorMessage);
    }

    [Fact]
    public void GitHubFileCommitResponse_WithSuccessTrue_SuccessIsTrue()
    {
        // Arrange & Act
        var response = new GitHubFileCommitResponse
        {
            Success = true
        };

        // Assert
        Assert.True(response.Success);
    }
}