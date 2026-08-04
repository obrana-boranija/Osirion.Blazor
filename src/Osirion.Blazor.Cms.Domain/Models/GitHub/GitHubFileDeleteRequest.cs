using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Model for delete file commit request
/// </summary>
public class GitHubFileDeleteRequest
{
    /// <summary>Gets or sets the Message value.</summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>Gets or sets the Sha value.</summary>
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    /// <summary>Gets or sets the Branch value.</summary>
    [JsonPropertyName("branch")]
    public string? Branch { get; set; }

    /// <summary>Performs the Committer operation.</summary>
    [JsonPropertyName("committer")]
    public GitHubCommitter? Committer { get; set; }
}
