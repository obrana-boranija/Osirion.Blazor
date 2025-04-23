using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Admin.Models;

/// <summary>
/// Response from file commit operations
/// </summary>
public class GitHubFileCommitResponse
{
    [JsonPropertyName("content")]
    public GitHubFileContent Content { get; set; } = new();

    [JsonPropertyName("commit")]
    public GitHubCommitInfo Commit { get; set; } = new();
}