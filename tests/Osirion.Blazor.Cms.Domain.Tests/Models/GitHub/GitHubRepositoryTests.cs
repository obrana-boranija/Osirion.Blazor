using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubRepositoryTests
{
    [Fact]
    public void GitHubRepository_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var repository = new GitHubRepository();

        // Assert
        Assert.Equal(0, repository.Id);
        Assert.Equal(string.Empty, repository.Name);
        Assert.Equal(string.Empty, repository.FullName);
        Assert.Equal(string.Empty, repository.HtmlUrl);
        Assert.Null(repository.Description);
        Assert.False(repository.Private);
        Assert.Equal("main", repository.DefaultBranch);
    }

    [Fact]
    public void GitHubRepository_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange & Act
        var repository = new GitHubRepository
        {
            Id = 123456789,
            Name = "Osirion.Blazor",
            FullName = "obrana-boranija/Osirion.Blazor",
            HtmlUrl = "https://github.com/obrana-boranija/Osirion.Blazor",
            Description = "Content management components for Blazor applications",
            Private = true,
            DefaultBranch = "master"
        };

        // Assert
        Assert.Equal(123456789, repository.Id);
        Assert.Equal("Osirion.Blazor", repository.Name);
        Assert.Equal("obrana-boranija/Osirion.Blazor", repository.FullName);
        Assert.Equal("https://github.com/obrana-boranija/Osirion.Blazor", repository.HtmlUrl);
        Assert.Equal("Content management components for Blazor applications", repository.Description);
        Assert.True(repository.Private);
        Assert.Equal("master", repository.DefaultBranch);
    }

    [Theory]
    [InlineData("main")]
    [InlineData("master")]
    [InlineData("development")]
    [InlineData("feature/new-design")]
    public void GitHubRepository_WithDifferentDefaultBranches_DefaultBranchIsSetCorrectly(string branch)
    {
        // Arrange & Act
        var repository = new GitHubRepository
        {
            DefaultBranch = branch
        };

        // Assert
        Assert.Equal(branch, repository.DefaultBranch);
    }

    [Fact]
    public void GitHubRepository_WithNullDescription_DescriptionIsNull()
    {
        // Arrange & Act
        var repository = new GitHubRepository
        {
            Description = null
        };

        // Assert
        Assert.Null(repository.Description);
    }

    [Fact]
    public void GitHubRepository_WithEmptyDescription_DescriptionIsEmpty()
    {
        // Arrange & Act
        var repository = new GitHubRepository
        {
            Description = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, repository.Description);
    }
}