using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubSearchResultTests
{
    [Fact]
    public void GitHubSearchResult_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var searchResult = new GitHubSearchResult();

        // Assert
        Assert.Equal(0, searchResult.TotalCount);
        Assert.False(searchResult.IncompleteResults);
        Assert.NotNull(searchResult.Items);
        Assert.Empty(searchResult.Items);
    }

    [Fact]
    public void GitHubSearchResult_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var items = new List<GitHubItem>
        {
            new GitHubItem
            {
                Name = "README.md",
                Path = "README.md",
                Type = "file"
            },
            new GitHubItem
            {
                Name = "LICENSE",
                Path = "LICENSE",
                Type = "file"
            }
        };

        // Act
        var searchResult = new GitHubSearchResult
        {
            TotalCount = 2,
            IncompleteResults = false,
            Items = items
        };

        // Assert
        Assert.Equal(2, searchResult.TotalCount);
        Assert.False(searchResult.IncompleteResults);
        Assert.NotNull(searchResult.Items);
        Assert.Equal(2, searchResult.Items.Count);
        Assert.Equal("README.md", searchResult.Items[0].Name);
        Assert.Equal("LICENSE", searchResult.Items[1].Name);
    }

    [Fact]
    public void GitHubSearchResult_WithNoItems_ItemsIsEmptyList()
    {
        // Arrange & Act
        var searchResult = new GitHubSearchResult
        {
            TotalCount = 0,
            IncompleteResults = false,
            Items = new List<GitHubItem>()
        };

        // Assert
        Assert.Equal(0, searchResult.TotalCount);
        Assert.NotNull(searchResult.Items);
        Assert.Empty(searchResult.Items);
    }

    [Fact]
    public void GitHubSearchResult_WithIncompleteResults_IncompleteResultsIsTrue()
    {
        // Arrange & Act
        var searchResult = new GitHubSearchResult
        {
            TotalCount = 100,
            IncompleteResults = true,
            Items = new List<GitHubItem> { new GitHubItem() }
        };

        // Assert
        Assert.Equal(100, searchResult.TotalCount);
        Assert.True(searchResult.IncompleteResults);
        Assert.Single(searchResult.Items);
    }

    [Fact]
    public void GitHubSearchResult_AddingToItems_ItemsAreAddedCorrectly()
    {
        // Arrange
        var searchResult = new GitHubSearchResult();

        // Act
        searchResult.Items.Add(new GitHubItem { Name = "file1.md", Type = "file" });
        searchResult.Items.Add(new GitHubItem { Name = "file2.md", Type = "file" });

        // Assert
        Assert.Equal(2, searchResult.Items.Count);
        Assert.Equal("file1.md", searchResult.Items[0].Name);
        Assert.Equal("file2.md", searchResult.Items[1].Name);
    }
}