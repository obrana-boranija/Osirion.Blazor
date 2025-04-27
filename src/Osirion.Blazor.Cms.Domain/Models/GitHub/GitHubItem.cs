using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Represents a file or directory in a GitHub repository
/// </summary>
public class GitHubItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

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