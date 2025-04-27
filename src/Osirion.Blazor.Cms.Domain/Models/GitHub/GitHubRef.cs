using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Git reference information
/// </summary>
public class GitHubRef
{
    [JsonPropertyName("ref")]
    public string Ref { get; set; } = string.Empty;

    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}