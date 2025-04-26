using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Core.Providers.GitHub.Models;

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