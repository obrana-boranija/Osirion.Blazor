using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Represents a file or directory in a GitHub repository
/// </summary>
public class GitHubItem
{
    /// <summary>Gets or sets the Name value.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the Path value.</summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>Gets or sets the Sha value.</summary>
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    /// <summary>Gets or sets the Size value.</summary>
    [JsonPropertyName("size")]
    public int Size { get; set; }

    /// <summary>Gets or sets the Url value.</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>Gets or sets the HtmlUrl value.</summary>
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets the DownloadUrl value.</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>Gets or sets the Type value.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets whether this item is a file
    /// </summary>
    [JsonIgnore]
    public bool IsFile => Type == "file";

    /// <summary>
    /// Gets whether this item is a directory
    /// </summary>
    [JsonIgnore]
    public bool IsDirectory => Type == "dir";

    /// <summary>
    /// Gets whether this item is a markdown file
    /// </summary>
    [JsonIgnore]
    public bool IsMarkdownFile => IsFile && (Name.EndsWith(".md") || Name.EndsWith(".markdown"));
}
