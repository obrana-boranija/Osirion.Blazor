using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Tests.Domain.Models.GitHub;

public class GitHubModelsTests
{
    [Fact]
    public void GitHubApiResponse_BaseProperties_InitializeCorrectly()
    {
        // Arrange
        var response = new TestGitHubApiResponse();

        // Assert
        Assert.False(response.Success);
        Assert.Null(response.ErrorMessage);
    }

    [Fact]
    public void GitHubAuthor_Serialization_DeserializesCorrectly()
    {
        // Arrange
        string json = @"{
                ""name"": ""Test User"",
                ""email"": ""test@example.com"",
                ""date"": ""2025-01-15T10:30:00Z""
            }";

        // Act
        var author = JsonSerializer.Deserialize<GitHubAuthor>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(author);
        Assert.Equal("Test User", author.Name);
        Assert.Equal("test@example.com", author.Email);
        Assert.Equal(new DateTime(2025, 01, 15, 10, 30, 00, DateTimeKind.Utc), author.Date);
    }

    [Fact]
    public void GitHubBranch_Serialization_DeserializesCorrectly()
    {
        // Arrange
        string json = @"{
                ""name"": ""main"",
                ""commit"": {
                    ""sha"": ""abc123def456"",
                    ""url"": ""https://api.github.com/repos/owner/repo/commits/abc123def456""
                },
                ""protected"": true
            }";

        // Act
        var branch = JsonSerializer.Deserialize<GitHubBranch>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(branch);
        Assert.Equal("main", branch.Name);
        Assert.Equal("abc123def456", branch.Commit.Sha);
        Assert.Equal("https://api.github.com/repos/owner/repo/commits/abc123def456", branch.Commit.Url);
        Assert.True(branch.Protected);
    }

    [Fact]
    public void GitHubCommitInfo_Serialization_DeserializesCorrectly()
    {
        // Arrange
        string json = @"{
                ""sha"": ""abc123def456"",
                ""url"": ""https://api.github.com/repos/owner/repo/commits/abc123def456"",
                ""html_url"": ""https://github.com/owner/repo/commit/abc123def456"",
                ""author"": {
                    ""name"": ""Author Name"",
                    ""email"": ""author@example.com"",
                    ""date"": ""2025-01-15T10:30:00Z""
                },
                ""committer"": {
                    ""name"": ""Committer Name"",
                    ""email"": ""committer@example.com"",
                    ""date"": ""2025-01-15T10:35:00Z""
                },
                ""message"": ""Commit message"",
                ""tree"": {
                    ""sha"": ""tree123"",
                    ""url"": ""https://api.github.com/repos/owner/repo/git/trees/tree123""
                }
            }";

        // Act
        var commitInfo = JsonSerializer.Deserialize<GitHubCommitInfo>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(commitInfo);
        Assert.Equal("abc123def456", commitInfo.Sha);
        Assert.Equal("https://api.github.com/repos/owner/repo/commits/abc123def456", commitInfo.Url);
        Assert.Equal("https://github.com/owner/repo/commit/abc123def456", commitInfo.HtmlUrl);
        Assert.Equal("Commit message", commitInfo.Message);

        Assert.NotNull(commitInfo.Author);
        Assert.Equal("Author Name", commitInfo.Author.Name);
        Assert.Equal("author@example.com", commitInfo.Author.Email);

        Assert.NotNull(commitInfo.Committer);
        Assert.Equal("Committer Name", commitInfo.Committer.Name);
        Assert.Equal("committer@example.com", commitInfo.Committer.Email);

        Assert.NotNull(commitInfo.Tree);
        Assert.Equal("tree123", commitInfo.Tree.Sha);
        Assert.Equal("https://api.github.com/repos/owner/repo/git/trees/tree123", commitInfo.Tree.Url);
    }

    [Fact]
    public void GitHubCommitter_Serialization_DeserializesCorrectly()
    {
        // Arrange
        string json = @"{
                ""name"": ""Test Committer"",
                ""email"": ""committer@example.com""
            }";

        // Act
        var committer = JsonSerializer.Deserialize<GitHubCommitter>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(committer);
        Assert.Equal("Test Committer", committer.Name);
        Assert.Equal("committer@example.com", committer.Email);
    }

    [Fact]
    public void GitHubFileCommitRequest_Serialization_SerializesCorrectly()
    {
        // Arrange
        var request = new GitHubFileCommitRequest
        {
            Message = "Commit message",
            Content = "base64content",
            Branch = "feature-branch",
            Sha = "existing-sha",
            Committer = new GitHubCommitter
            {
                Name = "Test Committer",
                Email = "committer@example.com"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Contains("\"message\":\"Commit message\"", json);
        Assert.Contains("\"content\":\"base64content\"", json);
        Assert.Contains("\"branch\":\"feature-branch\"", json);
        Assert.Contains("\"sha\":\"existing-sha\"", json);
        Assert.Contains("\"committer\":{", json);
        Assert.Contains("\"name\":\"Test Committer\"", json);
        Assert.Contains("\"email\":\"committer@example.com\"", json);
    }

    [Fact]
    public void GitHubFileContent_IsMarkdownFile_IdentifiesMarkdownFiles()
    {
        // Arrange
        var markdownFile1 = new GitHubFileContent { Path = "test.md" };
        var markdownFile2 = new GitHubFileContent { Path = "test.markdown" };
        var textFile = new GitHubFileContent { Path = "test.txt" };
        var noExtension = new GitHubFileContent { Path = "test" };

        // Act & Assert
        Assert.True(markdownFile1.IsMarkdownFile());
        Assert.True(markdownFile2.IsMarkdownFile());
        Assert.False(textFile.IsMarkdownFile());
        Assert.False(noExtension.IsMarkdownFile());
    }

    [Fact]
    public void GitHubFileContent_GetDecodedContent_DecodesBase64Content()
    {
        // Arrange
        string originalContent = "This is test content";
        string base64Content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(originalContent));

        var fileContent = new GitHubFileContent
        {
            Content = base64Content,
            Encoding = "base64"
        };

        // Act
        string decodedContent = fileContent.GetDecodedContent();

        // Assert
        Assert.Equal(originalContent, decodedContent);
    }

    [Fact]
    public void GitHubFileContent_GetDecodedContent_HandlesEmptyContent()
    {
        // Arrange
        var fileContent = new GitHubFileContent
        {
            Content = "",
            Encoding = "base64"
        };

        // Act
        string decodedContent = fileContent.GetDecodedContent();

        // Assert
        Assert.Equal(string.Empty, decodedContent);
    }

    [Fact]
    public void GitHubFileContent_GetDecodedContent_HandlesNonBase64Encoding()
    {
        // Arrange
        var fileContent = new GitHubFileContent
        {
            Content = "Plain text content",
            Encoding = "utf-8"
        };

        // Act
        string decodedContent = fileContent.GetDecodedContent();

        // Assert
        Assert.Equal("Plain text content", decodedContent);
    }

    [Fact]
    public void GitHubItem_IsProperties_IdentifyItemTypes()
    {
        // Arrange
        var fileItem = new GitHubItem { Type = "file" };
        var dirItem = new GitHubItem { Type = "dir" };
        var otherItem = new GitHubItem { Type = "other" };

        var markdownFile = new GitHubItem
        {
            Type = "file",
            Name = "readme.md"
        };

        var nonMarkdownFile = new GitHubItem
        {
            Type = "file",
            Name = "script.js"
        };

        // Act & Assert
        Assert.True(fileItem.IsFile);
        Assert.False(fileItem.IsDirectory);

        Assert.True(dirItem.IsDirectory);
        Assert.False(dirItem.IsFile);

        Assert.False(otherItem.IsFile);
        Assert.False(otherItem.IsDirectory);

        Assert.True(markdownFile.IsMarkdownFile);
        Assert.False(nonMarkdownFile.IsMarkdownFile);
    }

    [Fact]
    public void GitHubPullRequest_Serialization_DeserializesCorrectly()
    {
        // Arrange
        string json = @"{
                ""id"": 123456,
                ""number"": 42,
                ""html_url"": ""https://github.com/owner/repo/pull/42"",
                ""title"": ""Pull request title"",
                ""body"": ""Pull request description"",
                ""state"": ""open"",
                ""head"": {
                    ""ref"": ""feature-branch"",
                    ""sha"": ""head-sha"",
                    ""label"": ""owner:feature-branch""
                },
                ""base"": {
                    ""ref"": ""main"",
                    ""sha"": ""base-sha"",
                    ""label"": ""owner:main""
                }
            }";

        // Act
        var pullRequest = JsonSerializer.Deserialize<GitHubPullRequest>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(pullRequest);
        Assert.Equal(123456, pullRequest.Id);
        Assert.Equal(42, pullRequest.Number);
        Assert.Equal("https://github.com/owner/repo/pull/42", pullRequest.Url);
        Assert.Equal("Pull request title", pullRequest.Title);
        Assert.Equal("Pull request description", pullRequest.Body);
        Assert.Equal("open", pullRequest.State);

        Assert.NotNull(pullRequest.Head);
        Assert.Equal("feature-branch", pullRequest.Head.Ref);
        Assert.Equal("head-sha", pullRequest.Head.Sha);
        Assert.Equal("owner:feature-branch", pullRequest.Head.Label);

        Assert.NotNull(pullRequest.Base);
        Assert.Equal("main", pullRequest.Base.Ref);
        Assert.Equal("base-sha", pullRequest.Base.Sha);
        Assert.Equal("owner:main", pullRequest.Base.Label);
    }

    [Fact]
    public void GitHubRepository_Serialization_DeserializesCorrectly()
    {
        // Arrange
        string json = @"{
                ""id"": 12345678,
                ""name"": ""repo-name"",
                ""full_name"": ""owner/repo-name"",
                ""html_url"": ""https://github.com/owner/repo-name"",
                ""description"": ""Repository description"",
                ""private"": true,
                ""default_branch"": ""main""
            }";

        // Act
        var repository = JsonSerializer.Deserialize<GitHubRepository>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(repository);
        Assert.Equal(12345678, repository.Id);
        Assert.Equal("repo-name", repository.Name);
        Assert.Equal("owner/repo-name", repository.FullName);
        Assert.Equal("https://github.com/owner/repo-name", repository.HtmlUrl);
        Assert.Equal("Repository description", repository.Description);
        Assert.True(repository.Private);
        Assert.Equal("main", repository.DefaultBranch);
    }

    [Fact]
    public void GitHubSearchResult_Serialization_DeserializesCorrectly()
    {
        // Arrange
        string json = @"{
                ""total_count"": 2,
                ""incomplete_results"": false,
                ""items"": [
                    {
                        ""name"": ""file1.md"",
                        ""path"": ""path/to/file1.md"",
                        ""sha"": ""sha1"",
                        ""size"": 1024,
                        ""url"": ""https://api.github.com/repos/owner/repo/contents/path/to/file1.md"",
                        ""html_url"": ""https://github.com/owner/repo/blob/main/path/to/file1.md"",
                        ""download_url"": ""https://raw.githubusercontent.com/owner/repo/main/path/to/file1.md"",
                        ""type"": ""file""
                    },
                    {
                        ""name"": ""file2.md"",
                        ""path"": ""path/to/file2.md"",
                        ""sha"": ""sha2"",
                        ""size"": 2048,
                        ""url"": ""https://api.github.com/repos/owner/repo/contents/path/to/file2.md"",
                        ""html_url"": ""https://github.com/owner/repo/blob/main/path/to/file2.md"",
                        ""download_url"": ""https://raw.githubusercontent.com/owner/repo/main/path/to/file2.md"",
                        ""type"": ""file""
                    }
                ]
            }";

        // Act
        var searchResult = JsonSerializer.Deserialize<GitHubSearchResult>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(searchResult);
        Assert.Equal(2, searchResult.TotalCount);
        Assert.False(searchResult.IncompleteResults);
        Assert.Equal(2, searchResult.Items.Count);

        Assert.Equal("file1.md", searchResult.Items[0].Name);
        Assert.Equal("path/to/file1.md", searchResult.Items[0].Path);
        Assert.Equal("file", searchResult.Items[0].Type);

        Assert.Equal("file2.md", searchResult.Items[1].Name);
        Assert.Equal("path/to/file2.md", searchResult.Items[1].Path);
        Assert.Equal("file", searchResult.Items[1].Type);
    }

    // Test implementation of abstract GitHubApiResponse
    private class TestGitHubApiResponse : GitHubApiResponse
    {
    }
}