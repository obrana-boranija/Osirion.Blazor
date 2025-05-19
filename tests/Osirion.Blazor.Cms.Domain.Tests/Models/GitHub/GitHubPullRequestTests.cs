using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubPullRequestTests
{
    [Fact]
    public void GitHubPullRequest_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var pullRequest = new GitHubPullRequest();

        // Assert
        Assert.Equal(0, pullRequest.Id);
        Assert.Equal(0, pullRequest.Number);
        Assert.Equal(string.Empty, pullRequest.Url);
        Assert.Equal(string.Empty, pullRequest.Title);
        Assert.Equal(string.Empty, pullRequest.Body);
        Assert.Equal(string.Empty, pullRequest.State);
        Assert.NotNull(pullRequest.Head);
        Assert.NotNull(pullRequest.Base);
    }

    [Fact]
    public void GitHubPullRequest_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var head = new GitHubRef
        {
            Ref = "feature-branch",
            Sha = "1234567890abcdef1234567890abcdef12345678",
            Label = "owner:feature-branch"
        };

        var baseBranch = new GitHubRef
        {
            Ref = "main",
            Sha = "abcdef1234567890abcdef1234567890abcdef12",
            Label = "owner:main"
        };

        // Act
        var pullRequest = new GitHubPullRequest
        {
            Id = 12345,
            Number = 42,
            Url = "https://github.com/owner/repo/pull/42",
            Title = "Add new feature",
            Body = "This pull request adds a new feature",
            State = "open",
            Head = head,
            Base = baseBranch
        };

        // Assert
        Assert.Equal(12345, pullRequest.Id);
        Assert.Equal(42, pullRequest.Number);
        Assert.Equal("https://github.com/owner/repo/pull/42", pullRequest.Url);
        Assert.Equal("Add new feature", pullRequest.Title);
        Assert.Equal("This pull request adds a new feature", pullRequest.Body);
        Assert.Equal("open", pullRequest.State);

        // Verify Head properties
        Assert.Equal("feature-branch", pullRequest.Head.Ref);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", pullRequest.Head.Sha);
        Assert.Equal("owner:feature-branch", pullRequest.Head.Label);

        // Verify Base properties
        Assert.Equal("main", pullRequest.Base.Ref);
        Assert.Equal("abcdef1234567890abcdef1234567890abcdef12", pullRequest.Base.Sha);
        Assert.Equal("owner:main", pullRequest.Base.Label);
    }

    [Theory]
    [InlineData("open")]
    [InlineData("closed")]
    [InlineData("merged")]
    public void GitHubPullRequest_WithDifferentStates_StatesAreSetCorrectly(string state)
    {
        // Arrange & Act
        var pullRequest = new GitHubPullRequest
        {
            State = state
        };

        // Assert
        Assert.Equal(state, pullRequest.State);
    }
}