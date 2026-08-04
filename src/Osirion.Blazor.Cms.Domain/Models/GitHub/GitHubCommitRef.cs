using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Represents a commit reference in a GitHub branch
/// </summary>
public class GitHubCommitRef
{
    /// <summary>Gets or sets the Sha value.</summary>
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    /// <summary>Gets or sets the Url value.</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}
