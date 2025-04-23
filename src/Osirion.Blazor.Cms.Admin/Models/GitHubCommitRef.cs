using System.Text.Json.Serialization;

/// <summary>
/// Represents a commit reference in a GitHub branch
/// </summary>
public class GitHubCommitRef
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}