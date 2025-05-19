using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubAuthorTests
{
    [Fact]
    public void GitHubAuthor_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var author = new GitHubAuthor();

        // Assert
        Assert.Equal(string.Empty, author.Name);
        Assert.Equal(string.Empty, author.Email);
        Assert.Equal(default, author.Date);
    }

    [Fact]
    public void GitHubAuthor_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var commitDate = new DateTime(2025, 4, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var author = new GitHubAuthor
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Date = commitDate
        };

        // Assert
        Assert.Equal("John Doe", author.Name);
        Assert.Equal("john.doe@example.com", author.Email);
        Assert.Equal(commitDate, author.Date);
    }

    [Fact]
    public void GitHubAuthor_WithEmptyStrings_PropertiesAreSetToEmpty()
    {
        // Arrange & Act
        var author = new GitHubAuthor
        {
            Name = string.Empty,
            Email = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, author.Name);
        Assert.Equal(string.Empty, author.Email);
    }

    [Theory]
    [InlineData("John Doe", "john.doe@example.com")]
    [InlineData("Jane Smith", "jane.smith@example.com")]
    [InlineData("GitHub Actions", "actions@github.com")]
    public void GitHubAuthor_WithDifferentNames_PropertiesAreSetCorrectly(string name, string email)
    {
        // Arrange & Act
        var author = new GitHubAuthor
        {
            Name = name,
            Email = email
        };

        // Assert
        Assert.Equal(name, author.Name);
        Assert.Equal(email, author.Email);
    }
}