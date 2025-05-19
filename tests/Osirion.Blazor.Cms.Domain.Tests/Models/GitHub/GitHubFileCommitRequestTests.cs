using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubFileCommitRequestTests
{
    [Fact]
    public void GitHubFileCommitRequest_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var request = new GitHubFileCommitRequest();

        // Assert
        Assert.Equal(string.Empty, request.Message);
        Assert.Equal(string.Empty, request.Content);
        Assert.Null(request.Branch);
        Assert.Null(request.Sha);
        Assert.Null(request.Committer);
    }

    [Fact]
    public void GitHubFileCommitRequest_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var committer = new GitHubCommitter
        {
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var request = new GitHubFileCommitRequest
        {
            Message = "Add new file",
            Content = "SGVsbG8gV29ybGQh", // Base64 for "Hello World!"
            Branch = "feature-branch",
            Sha = "1234567890abcdef1234567890abcdef12345678",
            Committer = committer
        };

        // Assert
        Assert.Equal("Add new file", request.Message);
        Assert.Equal("SGVsbG8gV29ybGQh", request.Content);
        Assert.Equal("feature-branch", request.Branch);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", request.Sha);
        Assert.NotNull(request.Committer);
        Assert.Equal("John Doe", request.Committer.Name);
        Assert.Equal("john.doe@example.com", request.Committer.Email);
    }

    [Fact]
    public void GitHubFileCommitRequest_ForNewFile_ShaIsNull()
    {
        // Arrange & Act
        var request = new GitHubFileCommitRequest
        {
            Message = "Add new file",
            Content = "SGVsbG8gV29ybGQh", // Base64 for "Hello World!"
            Branch = "main"
            // Sha is null for new files
        };

        // Assert
        Assert.Null(request.Sha);
    }

    [Fact]
    public void GitHubFileCommitRequest_ForUpdatingFile_ShaIsRequired()
    {
        // Arrange & Act
        var request = new GitHubFileCommitRequest
        {
            Message = "Update file",
            Content = "VXBkYXRlZCBjb250ZW50", // Base64 for "Updated content"
            Branch = "main",
            Sha = "1234567890abcdef1234567890abcdef12345678" // Sha is required for updates
        };

        // Assert
        Assert.NotNull(request.Sha);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", request.Sha);
    }

    [Fact]
    public void GitHubFileCommitRequest_WithoutCommitter_CommitterIsNull()
    {
        // Arrange & Act
        var request = new GitHubFileCommitRequest
        {
            Message = "Add file without committer",
            Content = "Q29udGVudA==" // Base64 for "Content"
        };

        // Assert
        Assert.Null(request.Committer);
    }
}