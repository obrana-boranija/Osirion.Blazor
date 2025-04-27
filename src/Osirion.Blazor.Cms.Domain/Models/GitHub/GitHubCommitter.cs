using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Committer information for Git operations
/// </summary>
public class GitHubCommitter
{
    /// <summary>
    /// Gets or sets the name of the committer
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email of the committer
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}