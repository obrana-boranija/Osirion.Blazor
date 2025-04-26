using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Core.Providers.GitHub.Models;

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