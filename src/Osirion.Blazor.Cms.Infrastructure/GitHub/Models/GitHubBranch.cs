using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub.Models;

/// <summary>
/// Represents a branch in a GitHub repository
/// </summary>
public class GitHubBranch
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("commit")]
    public GitHubCommitRef Commit { get; set; } = new();

    [JsonPropertyName("protected")]
    public bool Protected { get; set; }
}