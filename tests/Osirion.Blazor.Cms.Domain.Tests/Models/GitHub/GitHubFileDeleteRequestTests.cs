using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubFileDeleteRequestTests
{
    [Fact]
    public void GitHubFileDeleteRequest_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var request = new GitHubFileDeleteRequest();

        // Assert
        Assert.Equal(string.Empty, request.Message);
        Assert.Equal(string.Empty, request.Sha);
        Assert.Null(request.Branch);
        Assert.Null(request.Committer);
    }

    [Fact]
    public void GitHubFileDeleteRequest_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var committer = new GitHubCommitter
        {
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var request = new GitHubFileDeleteRequest
        {
            Message = "Delete file",
            Sha = "1234567890abcdef1234567890abcdef12345678",
            Branch = "main",
            Committer = committer
        };

        // Assert
        Assert.Equal("Delete file", request.Message);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", request.Sha);
        Assert.Equal("main", request.Branch);
        Assert.NotNull(request.Committer);
        Assert.Equal("John Doe", request.Committer.Name);
        Assert.Equal("john.doe@example.com", request.Committer.Email);
    }

    [Theory]
    [InlineData("Delete README.md")]
    [InlineData("Remove obsolete file")]
    [InlineData("Cleanup repository structure\n\nRemoves unused files")]
    public void GitHubFileDeleteRequest_WithDifferentMessages_MessageIsSetCorrectly(string message)
    {
        // Arrange & Act
        var request = new GitHubFileDeleteRequest
        {
            Message = message
        };

        // Assert
        Assert.Equal(message, request.Message);
    }

    [Theory]
    [InlineData("main")]
    [InlineData("develop")]
    [InlineData("feature/new-component")]
    [InlineData("release/v1.0.0")]
    public void GitHubFileDeleteRequest_WithDifferentBranches_BranchIsSetCorrectly(string branch)
    {
        // Arrange & Act
        var request = new GitHubFileDeleteRequest
        {
            Branch = branch
        };

        // Assert
        Assert.Equal(branch, request.Branch);
    }

    [Fact]
    public void GitHubFileDeleteRequest_WithNullCommitter_CommitterIsNull()
    {
        // Arrange & Act
        var request = new GitHubFileDeleteRequest
        {
            Committer = null
        };

        // Assert
        Assert.Null(request.Committer);
    }
}