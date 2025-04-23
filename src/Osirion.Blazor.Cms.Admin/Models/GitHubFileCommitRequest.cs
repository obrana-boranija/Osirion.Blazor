using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Admin.Models;

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

    /// <summary>
    /// Creates a new GitHubFileCommitRequest instance
    /// </summary>
    public static GitHubFileCommitRequest Create(
        string content,
        string message,
        string? branch = null,
        string? sha = null,
        string? committerName = null,
        string? committerEmail = null)
    {
        // Convert content to Base64
        var base64Content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content));

        var request = new GitHubFileCommitRequest
        {
            Content = base64Content,
            Message = message,
            Branch = branch,
            Sha = sha
        };

        // Add committer information if provided
        if (!string.IsNullOrEmpty(committerName) && !string.IsNullOrEmpty(committerEmail))
        {
            request.Committer = new GitHubCommitter
            {
                Name = committerName,
                Email = committerEmail
            };
        }

        return request;
    }
}