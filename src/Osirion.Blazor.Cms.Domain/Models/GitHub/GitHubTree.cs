using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// A Git tree returned by the GitHub Git Trees API
/// </summary>
public class GitHubTree
{
    /// <summary>Gets or sets the SHA of the tree.</summary>
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    /// <summary>Gets or sets whether GitHub cut the listing off (more than 100,000 entries or 7 MB).</summary>
    [JsonPropertyName("truncated")]
    public bool Truncated { get; set; }

    /// <summary>Gets or sets the entries of the tree.</summary>
    [JsonPropertyName("tree")]
    public List<GitHubTreeEntry> Entries { get; set; } = [];
}

/// <summary>
/// One entry (file or directory) of a <see cref="GitHubTree"/>
/// </summary>
public class GitHubTreeEntry
{
    /// <summary>Gets or sets the path relative to the repository root, with <c>/</c> separators.</summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>Gets or sets the object type: <c>blob</c>, <c>tree</c> or <c>commit</c>.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the SHA of the object. For a blob it equals the SHA the contents API reports for the file.</summary>
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    /// <summary>Gets a value indicating whether the entry is a file.</summary>
    [JsonIgnore]
    public bool IsFile => Type == "blob";

    /// <summary>Gets a value indicating whether the entry is a directory.</summary>
    [JsonIgnore]
    public bool IsDirectory => Type == "tree";
}
