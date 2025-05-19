using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubCommitterTests
{
    [Fact]
    public void GitHubCommitter_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var committer = new GitHubCommitter();

        // Assert
        Assert.Equal(string.Empty, committer.Name);
        Assert.Equal(string.Empty, committer.Email);
    }

    [Fact]
    public void GitHubCommitter_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange & Act
        var committer = new GitHubCommitter
        {
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        // Assert
        Assert.Equal("John Doe", committer.Name);
        Assert.Equal("john.doe@example.com", committer.Email);
    }

    [Theory]
    [InlineData("John Doe", "john.doe@example.com")]
    [InlineData("Jane Smith", "jane.smith@example.com")]
    [InlineData("GitHub Actions", "actions@github.com")]
    [InlineData("Automated CI", "noreply@github.com")]
    public void GitHubCommitter_WithDifferentValues_PropertiesAreSetCorrectly(string name, string email)
    {
        // Arrange & Act
        var committer = new GitHubCommitter
        {
            Name = name,
            Email = email
        };

        // Assert
        Assert.Equal(name, committer.Name);
        Assert.Equal(email, committer.Email);
    }

    [Fact]
    public void GitHubCommitter_WithEmptyStrings_PropertiesAreSetToEmpty()
    {
        // Arrange & Act
        var committer = new GitHubCommitter
        {
            Name = string.Empty,
            Email = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, committer.Name);
        Assert.Equal(string.Empty, committer.Email);
    }
}