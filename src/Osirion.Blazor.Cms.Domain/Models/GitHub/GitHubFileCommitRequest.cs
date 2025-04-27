using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

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