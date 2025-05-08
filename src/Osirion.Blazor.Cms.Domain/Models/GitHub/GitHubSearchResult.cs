using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Represents a search result from GitHub API
/// </summary>
public class GitHubSearchResult
{
    /// <summary>
    /// Gets or sets the total count of items found
    /// </summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the results are incomplete
    /// </summary>
    [JsonPropertyName("incomplete_results")]
    public bool IncompleteResults { get; set; }

    /// <summary>
    /// Gets or sets the items found
    /// </summary>
    [JsonPropertyName("items")]
    public List<GitHubItem> Items { get; set; } = new List<GitHubItem>();
}