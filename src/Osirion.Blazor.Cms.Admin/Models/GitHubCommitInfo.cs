using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Admin.Models;

/// <summary>
/// Information about a commit
/// </summary>
public class GitHubCommitInfo
{
    /// <summary>
    /// Gets or sets the SHA hash of the commit
    /// </summary>
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL of the commit
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTML URL of the commit
    /// </summary>
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;
}