using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Information about a commit
/// </summary>
public class GitHubCommitInfo
{
    /// <summary>Gets or sets the Sha value.</summary>
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    /// <summary>Gets or sets the Url value.</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>Gets or sets the HtmlUrl value.</summary>
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    /// <summary>Performs the Author operation.</summary>
    [JsonPropertyName("author")]
    public GitHubAuthor? Author { get; set; }

    /// <summary>Performs the Committer operation.</summary>
    [JsonPropertyName("committer")]
    public GitHubAuthor? Committer { get; set; }

    /// <summary>Gets or sets the Message value.</summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>Performs the Tree operation.</summary>
    [JsonPropertyName("tree")]
    public GitHubCommitRef Tree { get; set; } = new();
}
