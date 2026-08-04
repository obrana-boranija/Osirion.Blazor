using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Represents a branch in a GitHub repository
/// </summary>
public class GitHubBranch
{
    /// <summary>Gets or sets the Name value.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Performs the Commit operation.</summary>
    [JsonPropertyName("commit")]
    public GitHubCommitRef Commit { get; set; } = new();

    /// <summary>Gets or sets the Protected value.</summary>
    [JsonPropertyName("protected")]
    public bool Protected { get; set; }
}
