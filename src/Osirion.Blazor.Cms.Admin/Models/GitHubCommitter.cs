using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Admin.Models;

/// <summary>
/// Committer information for Git operations
/// </summary>
public class GitHubCommitter
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}