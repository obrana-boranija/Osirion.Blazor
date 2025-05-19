using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubFileContentTests
{
    [Fact]
    public void GitHubFileContent_DefaultInitialization_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var fileContent = new GitHubFileContent();

        // Assert
        Assert.Equal(string.Empty, fileContent.Type);
        Assert.Equal(string.Empty, fileContent.Encoding);
        Assert.Equal(0, fileContent.Size);
        Assert.Equal(string.Empty, fileContent.Name);
        Assert.Equal(string.Empty, fileContent.Path);
        Assert.Equal(string.Empty, fileContent.Content);
        Assert.Equal(string.Empty, fileContent.Sha);
        Assert.Equal(string.Empty, fileContent.Url);
        Assert.Equal(string.Empty, fileContent.DownloadUrl);
        Assert.Equal(string.Empty, fileContent.GitUrl);
        Assert.Equal(string.Empty, fileContent.HtmlUrl);
    }

    [Fact]
    public void GitHubFileContent_WithAllProperties_PropertiesAreSetCorrectly()
    {
        // Arrange & Act
        var fileContent = new GitHubFileContent
        {
            Type = "file",
            Encoding = "base64",
            Size = 1024,
            Name = "README.md",
            Path = "docs/README.md",
            Content = "SGVsbG8gV29ybGQh", // Base64 encoded "Hello World!"
            Sha = "1234567890abcdef1234567890abcdef12345678",
            Url = "https://api.github.com/repos/owner/repo/contents/docs/README.md",
            DownloadUrl = "https://raw.githubusercontent.com/owner/repo/main/docs/README.md",
            GitUrl = "https://api.github.com/repos/owner/repo/git/blobs/1234567890abcdef1234567890abcdef12345678",
            HtmlUrl = "https://github.com/owner/repo/blob/main/docs/README.md"
        };

        // Assert
        Assert.Equal("file", fileContent.Type);
        Assert.Equal("base64", fileContent.Encoding);
        Assert.Equal(1024, fileContent.Size);
        Assert.Equal("README.md", fileContent.Name);
        Assert.Equal("docs/README.md", fileContent.Path);
        Assert.Equal("SGVsbG8gV29ybGQh", fileContent.Content);
        Assert.Equal("1234567890abcdef1234567890abcdef12345678", fileContent.Sha);
        Assert.Equal("https://api.github.com/repos/owner/repo/contents/docs/README.md", fileContent.Url);
        Assert.Equal("https://raw.githubusercontent.com/owner/repo/main/docs/README.md", fileContent.DownloadUrl);
        Assert.Equal("https://api.github.com/repos/owner/repo/git/blobs/1234567890abcdef1234567890abcdef12345678", fileContent.GitUrl);
        Assert.Equal("https://github.com/owner/repo/blob/main/docs/README.md", fileContent.HtmlUrl);
    }

    [Theory]
    [InlineData("README.md", true)]
    [InlineData("document.markdown", true)]
    [InlineData("script.js", false)]
    [InlineData("image.png", false)]
    [InlineData("style.css", false)]
    [InlineData("file.md.bak", false)]
    public void IsMarkdownFile_WithVariousFileTypes_ReturnsExpectedResult(string path, bool expected)
    {
        // Arrange
        var fileContent = new GitHubFileContent
        {
            Path = path
        };

        // Act
        var result = fileContent.IsMarkdownFile();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetDecodedContent_WithBase64Content_ReturnsDecodedString()
    {
        // Arrange
        var fileContent = new GitHubFileContent
        {
            Content = "SGVsbG8gV29ybGQh", // Base64 for "Hello World!"
            Encoding = "base64"
        };

        // Act
        var decodedContent = fileContent.GetDecodedContent();

        // Assert
        Assert.Equal("Hello World!", decodedContent);
    }

    [Fact]
    public void GetDecodedContent_WithEmptyContent_ReturnsEmptyString()
    {
        // Arrange
        var fileContent = new GitHubFileContent
        {
            Content = string.Empty,
            Encoding = "base64"
        };

        // Act
        var decodedContent = fileContent.GetDecodedContent();

        // Assert
        Assert.Equal(string.Empty, decodedContent);
    }

    [Fact]
    public void GetDecodedContent_WithNonBase64Encoding_ReturnsOriginalContent()
    {
        // Arrange
        var fileContent = new GitHubFileContent
        {
            Content = "Hello World!",
            Encoding = "utf-8" // Not base64
        };

        // Act
        var decodedContent = fileContent.GetDecodedContent();

        // Assert
        Assert.Equal("Hello World!", decodedContent);
    }

    [Fact]
    public void GetDecodedContent_WithBase64ContentContainingWhitespace_RemovesWhitespaceBeforeDecoding()
    {
        // Arrange
        var fileContent = new GitHubFileContent
        {
            Content = "SGVs\nbG8g\r\nV29y\rbGQh", // Base64 for "Hello World!" with newlines
            Encoding = "base64"
        };

        // Act
        var decodedContent = fileContent.GetDecodedContent();

        // Assert
        Assert.Equal("Hello World!", decodedContent);
    }
}