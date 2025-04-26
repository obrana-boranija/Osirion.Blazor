using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Providers.GitHub.Models;

/// <summary>
/// Base class for GitHub API responses
/// </summary>
public abstract class GitHubApiResponse
{
    /// <summary>
    /// Gets or sets the response status
    /// </summary>
    [JsonIgnore]
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets any error message
    /// </summary>
    [JsonIgnore]
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Represents a file or directory in a GitHub repository
/// </summary>
public class GitHubItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets whether this item is a file
    /// </summary>
    [JsonIgnore]
    public bool IsFile => Type == "file";

    /// <summary>
    /// Gets whether this item is a directory
    /// </summary>
    [JsonIgnore]
    public bool IsDirectory => Type == "dir";

    /// <summary>
    /// Gets whether this item is a markdown file
    /// </summary>
    [JsonIgnore]
    public bool IsMarkdownFile => IsFile && (Name.EndsWith(".md") || Name.EndsWith(".markdown"));
}

/// <summary>
/// Represents a file content object from GitHub
/// </summary>
public class GitHubFileContent
{
    /// <summary>
    /// Gets or sets the type of the content (file, dir, etc.)
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file encoding (usually base64 for files)
    /// </summary>
    [JsonPropertyName("encoding")]
    public string Encoding { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes
    /// </summary>
    [JsonPropertyName("size")]
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the file name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file path relative to the repository root
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the file (usually base64 encoded)
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SHA hash of the file
    /// </summary>
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL of the file in the GitHub API
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the download URL of the file
    /// </summary>
    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Git URL of the file
    /// </summary>
    [JsonPropertyName("git_url")]
    public string GitUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTML URL of the file
    /// </summary>
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    /// <summary>
    /// Checks if the file is a markdown file
    /// </summary>
    /// <returns>True if the file is a markdown file</returns>
    public bool IsMarkdownFile()
    {
        return Path.EndsWith(".md") || Path.EndsWith(".markdown");
    }

    /// <summary>
    /// Gets the decoded content as a string
    /// </summary>
    /// <returns>The decoded content</returns>
    public string GetDecodedContent()
    {
        if (string.IsNullOrWhiteSpace(Content))
        {
            return string.Empty;
        }

        if (Encoding.Equals("base64", StringComparison.OrdinalIgnoreCase))
        {
            // Replace any whitespace that might be in the base64 string
            var base64 = Content.Replace("\n", "").Replace("\r", "");
            var bytes = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        return Content;
    }
}

/// <summary>
/// Represents a branch in a GitHub repository
/// </summary>
public class GitHubBranch
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("commit")]
    public GitHubCommitRef Commit { get; set; } = new();

    [JsonPropertyName("protected")]
    public bool Protected { get; set; }
}

/// <summary>
/// Represents a commit reference in a GitHub branch
/// </summary>
public class GitHubCommitRef
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Information about a commit
/// </summary>
public class GitHubCommitInfo
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public GitHubAuthor? Author { get; set; }

    [JsonPropertyName("committer")]
    public GitHubAuthor? Committer { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("tree")]
    public GitHubCommitRef Tree { get; set; } = new();
}

/// <summary>
/// Represents an author or committer of a GitHub commit
/// </summary>
public class GitHubAuthor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}

/// <summary>
/// Request model for creating or updating a file in GitHub
/// </summary>
public class GitHubFileCommitRequest
{
    /// <summary>
    /// Gets or sets the commit message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file content (Base64 encoded)
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch to commit to
    /// </summary>
    [JsonPropertyName("branch")]
    public string? Branch { get; set; }

    /// <summary>
    /// Gets or sets the SHA of the file being replaced (for updates)
    /// </summary>
    [JsonPropertyName("sha")]
    public string? Sha { get; set; }

    /// <summary>
    /// Gets or sets the committer information
    /// </summary>
    [JsonPropertyName("committer")]
    public GitHubCommitter? Committer { get; set; }
}

/// <summary>
/// Committer information for Git operations
/// </summary>
public class GitHubCommitter
{
    /// <summary>
    /// Gets or sets the name of the committer
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email of the committer
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Response from file commit operations
/// </summary>
public class GitHubFileCommitResponse : GitHubApiResponse
{
    [JsonPropertyName("content")]
    public GitHubFileContent Content { get; set; } = new();

    [JsonPropertyName("commit")]
    public GitHubCommitInfo Commit { get; set; } = new();
}

/// <summary>
/// Model for delete file commit request
/// </summary>
public class GitHubFileDeleteRequest
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("branch")]
    public string? Branch { get; set; }

    [JsonPropertyName("committer")]
    public GitHubCommitter? Committer { get; set; }
}

/// <summary>
/// Response model for GitHub file search operations
/// </summary>
public class GitHubSearchResult : GitHubApiResponse
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("incomplete_results")]
    public bool IncompleteResults { get; set; }

    [JsonPropertyName("items")]
    public List<GitHubItem> Items { get; set; } = new();
}

/// <summary>
/// Represents a GitHub repository
/// </summary>
public class GitHubRepository
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("private")]
    public bool Private { get; set; }

    [JsonPropertyName("default_branch")]
    public string DefaultBranch { get; set; } = "main";
}