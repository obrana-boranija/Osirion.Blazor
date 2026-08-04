using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Git reference information
/// </summary>
public class GitHubRef
{
    /// <summary>Gets or sets the Ref value.</summary>
    [JsonPropertyName("ref")]
    public string Ref { get; set; } = string.Empty;

    /// <summary>Gets or sets the Sha value.</summary>
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    /// <summary>Gets or sets the Label value.</summary>
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}
