using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Admin.Models;

/// <summary>
/// Information about a commit
/// </summary>
public class GitHubCommitInfo
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;
}