using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Response from file commit operations
/// </summary>
public class GitHubFileCommitResponse : GitHubApiResponse
{
    /// <summary>Performs the Content operation.</summary>
    [JsonPropertyName("content")]
    public GitHubFileContent Content { get; set; } = new();

    /// <summary>Performs the Commit operation.</summary>
    [JsonPropertyName("commit")]
    public GitHubCommitInfo Commit { get; set; } = new();
}
