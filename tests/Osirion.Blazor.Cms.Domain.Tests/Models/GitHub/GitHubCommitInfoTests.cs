using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubCommitInfoTests
{
    [Fact]
    public void GitHubCommitInfo_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var commitInfo = new GitHubCommitInfo();

        // Assert
        Assert.Equal(string.Empty, commitInfo.Sha);
        Assert.Equal(string.Empty, commitInfo.Url);
        Assert.Equal(string.Empty, commitInfo.HtmlUrl);
        Assert.Null(commitInfo.Author);
        Assert.Null(commitInfo.Committer);
        Assert.Equal(string.Empty, commitInfo.Message);
        Assert.NotNull(commitInfo.Tree);
        Assert.Equal(string.Empty, commitInfo.Tree.Sha);
        Assert.Equal(string.Empty, commitInfo.Tree.Url);
    }

    [Fact]
    public void GitHubCommitInfo_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var author = new GitHubAuthor
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Date = new DateTime(2025, 4, 15, 10, 30, 0, DateTimeKind.Utc)
        };

        var committer = new GitHubAuthor
        {
            Name = "Jane Smith",
            Email = "jane.smith@example.com",
            Date = new DateTime(2025, 4, 15, 11, 0, 0, DateTimeKind.Utc)
        };

        var tree = new GitHubCommitRef
        {
            Sha = "1234567890abcdef1234567890abcdef12345678",
            Url = "https://api.github.com/repos/owner/repo/git/trees/1234567890abcdef1234567890abcdef12345678"
        };

        // Act
        var commitInfo = new GitHubCommitInfo
        {
            Sha = "abcdef1234567890abcdef1234567890abcdef12",
            Url = "https://api.github.com/repos/owner/repo/commits/abcdef1234567890abcdef1234567890abcdef12",
            HtmlUrl = "https://github.com/owner/repo/commit/abcdef1234567890abcdef1234567890abcdef12",
            Author = author,
            Committer = committer,
            Message = "Add new feature",
            Tree = tree
        };

        // Assert
        Assert.Equal("abcdef1234567890abcdef1234567890abcdef12", commitInfo.Sha);
        Assert.Equal("https://api.github.com/repos/owner/repo/commits/abcdef1234567890abcdef1234567890abcdef12", commitInfo.Url);
        Assert.Equal("https://github.com/owner/repo/commit/abcdef1234567890abcdef1234567890abcdef12", commitInfo.HtmlUrl);

        Assert.NotNull(commitInfo.Author);
        Assert.Equal("John Doe", commitInfo.Author.Name);
        Assert.Equal("john.doe@example.com", commitInfo.Author.Email);
        Assert.Equal(new DateTime(2025, 4, 15, 10, 30, 0, DateTimeKind.Utc), commitInfo.Author.Date);

        Assert.NotNull(commitInfo.Committer);
        Assert.Equal("Jane Smith", commitInfo.Committer.Name);
        Assert.Equal("jane.smith@example.com", commitInfo.Committer.Email);
        Assert.Equal(new DateTime(2025, 4, 15, 11, 0, 0, DateTimeKind.Utc), commitInfo.Committer.Date);

        Assert.Equal("Add new feature", commitInfo.Message);

        Assert.NotNull(commitInfo.Tree);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", commitInfo.Tree.Sha);
        Assert.Equal("https://api.github.com/repos/owner/repo/git/trees/1234567890abcdef1234567890abcdef12345678", commitInfo.Tree.Url);
    }

    [Fact]
    public void GitHubCommitInfo_WithNullAuthorAndCommitter_AuthorAndCommitterAreNull()
    {
        // Arrange & Act
        var commitInfo = new GitHubCommitInfo
        {
            Author = null,
            Committer = null
        };

        // Assert
        Assert.Null(commitInfo.Author);
        Assert.Null(commitInfo.Committer);
    }

    [Theory]
    [InlineData("Initial commit")]
    [InlineData("Fix bug in login component")]
    [InlineData("Add new feature X\n\nDetailed description of feature X")]
    public void GitHubCommitInfo_WithDifferentMessages_MessageIsSetCorrectly(string message)
    {
        // Arrange & Act
        var commitInfo = new GitHubCommitInfo
        {
            Message = message
        };

        // Assert
        Assert.Equal(message, commitInfo.Message);
    }
}