using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubRefTests
{
    [Fact]
    public void GitHubRef_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var gitHubRef = new GitHubRef();

        // Assert
        Assert.Equal(string.Empty, gitHubRef.Ref);
        Assert.Equal(string.Empty, gitHubRef.Sha);
        Assert.Equal(string.Empty, gitHubRef.Label);
    }

    [Fact]
    public void GitHubRef_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange & Act
        var gitHubRef = new GitHubRef
        {
            Ref = "refs/heads/main",
            Sha = "1234567890abcdef1234567890abcdef12345678",
            Label = "owner:main"
        };

        // Assert
        Assert.Equal("refs/heads/main", gitHubRef.Ref);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", gitHubRef.Sha);
        Assert.Equal("owner:main", gitHubRef.Label);
    }

    [Theory]
    [InlineData("refs/heads/main", "main")]
    [InlineData("refs/heads/feature/new-component", "feature/new-component")]
    [InlineData("refs/heads/release/v1.0.0", "release/v1.0.0")]
    [InlineData("refs/tags/v1.0.0", "v1.0.0")]
    public void GitHubRef_WithDifferentRefs_RefIsSetCorrectly(string refValue, string label)
    {
        // Arrange & Act
        var gitHubRef = new GitHubRef
        {
            Ref = refValue,
            Label = $"owner:{label}"
        };

        // Assert
        Assert.Equal(refValue, gitHubRef.Ref);
        Assert.Equal($"owner:{label}", gitHubRef.Label);
    }

    [Fact]
    public void GitHubRef_WithEmptyStrings_PropertiesAreSetToEmpty()
    {
        // Arrange & Act
        var gitHubRef = new GitHubRef
        {
            Ref = string.Empty,
            Sha = string.Empty,
            Label = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, gitHubRef.Ref);
        Assert.Equal(string.Empty, gitHubRef.Sha);
        Assert.Equal(string.Empty, gitHubRef.Label);
    }
}