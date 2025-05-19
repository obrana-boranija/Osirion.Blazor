using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubItemTests
{
    [Fact]
    public void GitHubItem_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var item = new GitHubItem();

        // Assert
        Assert.Equal(string.Empty, item.Name);
        Assert.Equal(string.Empty, item.Path);
        Assert.Equal(string.Empty, item.Sha);
        Assert.Equal(0, item.Size);
        Assert.Equal(string.Empty, item.Url);
        Assert.Equal(string.Empty, item.HtmlUrl);
        Assert.Null(item.DownloadUrl);
        Assert.Equal(string.Empty, item.Type);
    }

    [Fact]
    public void GitHubItem_WithFileType_IsFileReturnsTrue()
    {
        // Arrange
        var item = new GitHubItem { Type = "file" };

        // Act & Assert
        Assert.True(item.IsFile);
        Assert.False(item.IsDirectory);
    }

    [Fact]
    public void GitHubItem_WithDirType_IsDirectoryReturnsTrue()
    {
        // Arrange
        var item = new GitHubItem { Type = "dir" };

        // Act & Assert
        Assert.True(item.IsDirectory);
        Assert.False(item.IsFile);
    }

    [Fact]
    public void GitHubItem_WithOtherType_BothPropertiesReturnFalse()
    {
        // Arrange
        var item = new GitHubItem { Type = "symlink" };

        // Act & Assert
        Assert.False(item.IsDirectory);
        Assert.False(item.IsFile);
    }

    [Theory]
    [InlineData("test.md", true)]
    [InlineData("test.markdown", true)]
    [InlineData("test.txt", false)]
    [InlineData("test.md.txt", false)]
    [InlineData("markdown", false)]
    public void IsMarkdownFile_WithVariousFiles_ReturnsExpectedResult(string fileName, bool expected)
    {
        // Arrange
        var item = new GitHubItem
        {
            Name = fileName,
            Type = "file"
        };

        // Act & Assert
        Assert.Equal(expected, item.IsMarkdownFile);
    }

    [Fact]
    public void IsMarkdownFile_ForDirectory_ReturnsFalse()
    {
        // Arrange
        var item = new GitHubItem
        {
            Name = "test.md",
            Type = "dir"
        };

        // Act & Assert
        Assert.False(item.IsMarkdownFile);
    }

    [Fact]
    public void GitHubItem_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange & Act
        var item = new GitHubItem
        {
            Name = "README.md",
            Path = "docs/README.md",
            Sha = "1234567890abcdef1234567890abcdef12345678",
            Size = 1024,
            Url = "https://api.github.com/repos/owner/repo/contents/docs/README.md",
            HtmlUrl = "https://github.com/owner/repo/blob/main/docs/README.md",
            DownloadUrl = "https://raw.githubusercontent.com/owner/repo/main/docs/README.md",
            Type = "file"
        };

        // Assert
        Assert.Equal("README.md", item.Name);
        Assert.Equal("docs/README.md", item.Path);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", item.Sha);
        Assert.Equal(1024, item.Size);
        Assert.Equal("https://api.github.com/repos/owner/repo/contents/docs/README.md", item.Url);
        Assert.Equal("https://github.com/owner/repo/blob/main/docs/README.md", item.HtmlUrl);
        Assert.Equal("https://raw.githubusercontent.com/owner/repo/main/docs/README.md", item.DownloadUrl);
        Assert.Equal("file", item.Type);
        Assert.True(item.IsFile);
        Assert.False(item.IsDirectory);
        Assert.True(item.IsMarkdownFile);
    }
}