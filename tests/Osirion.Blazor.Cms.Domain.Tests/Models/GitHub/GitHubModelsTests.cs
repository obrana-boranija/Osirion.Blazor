using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Domain.Tests.Models;

public class GitHubModelsTests
{
    [Fact]
    public void GitHubFileCommitResponse_Properties_ShouldBeSettable()
    {
        // Arrange
        var content = new GitHubFileContent
        {
            Name = "test.md",
            Path = "content/test.md",
            Sha = "abc123"
        };

        var commit = new GitHubCommitInfo
        {
            Sha = "def456",
            Message = "Test commit"
        };

        // Act
        var response = new GitHubFileCommitResponse
        {
            Success = true,
            Content = content,
            Commit = commit
        };

        // Assert
        Assert.True(response.Success);
        Assert.Equal("test.md", response.Content.Name);
        Assert.Equal("content/test.md", response.Content.Path);
        Assert.Equal("abc123", response.Content.Sha);
        Assert.Equal("def456", response.Commit.Sha);
        Assert.Equal("Test commit", response.Commit.Message);
    }

    [Fact]
    public void GitHubFileCommitResponse_JsonSerialization_ShouldIgnoreSuccess()
    {
        // Arrange
        var response = new GitHubFileCommitResponse
        {
            Success = true,
            ErrorMessage = "Some error",
            Content = new GitHubFileContent { Name = "test.md" },
            Commit = new GitHubCommitInfo { Sha = "def456" }
        };

        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        Assert.DoesNotContain("Success", json);
        Assert.DoesNotContain("ErrorMessage", json);
        Assert.Contains("\"content\":", json);
        Assert.Contains("\"commit\":", json);
    }

    [Fact]
    public void GitHubFileDeleteRequest_Properties_ShouldBeSettable()
    {
        // Arrange & Act
        var request = new GitHubFileDeleteRequest
        {
            Message = "Delete test file",
            Sha = "abc123",
            Branch = "main",
            Committer = new GitHubCommitter
            {
                Name = "Test User",
                Email = "test@example.com"
            }
        };

        // Assert
        Assert.Equal("Delete test file", request.Message);
        Assert.Equal("abc123", request.Sha);
        Assert.Equal("main", request.Branch);
        Assert.NotNull(request.Committer);
        Assert.Equal("Test User", request.Committer.Name);
        Assert.Equal("test@example.com", request.Committer.Email);
    }

    [Fact]
    public void GitHubFileDeleteRequest_JsonSerialization_ShouldIncludeAllProperties()
    {
        // Arrange
        var request = new GitHubFileDeleteRequest
        {
            Message = "Delete test file",
            Sha = "abc123",
            Branch = "main",
            Committer = new GitHubCommitter
            {
                Name = "Test User",
                Email = "test@example.com"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(request);

        // Assert
        Assert.Contains("\"message\":\"Delete test file\"", json);
        Assert.Contains("\"sha\":\"abc123\"", json);
        Assert.Contains("\"branch\":\"main\"", json);
        Assert.Contains("\"committer\":", json);
        Assert.Contains("\"name\":\"Test User\"", json);
        Assert.Contains("\"email\":\"test@example.com\"", json);
    }
}