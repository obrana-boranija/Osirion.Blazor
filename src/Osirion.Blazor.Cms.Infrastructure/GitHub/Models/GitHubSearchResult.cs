using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub.Models;

/// <summary>
/// Response model for GitHub file search operations
/// </summary>
public class GitHubSearchResult : GitHubApiResponse
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("incomplete_results")]
    public bool IncompleteResults { get; set; }

    [JsonPropertyName("items")]
    public List<GitHubItem> Items { get; set; } = new();
}