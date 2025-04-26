using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Core.Providers.GitHub.Models;

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