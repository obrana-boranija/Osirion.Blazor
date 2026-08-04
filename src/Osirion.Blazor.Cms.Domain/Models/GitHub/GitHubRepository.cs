using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Represents a GitHub repository
/// </summary>
public class GitHubRepository
{
    /// <summary>Performs the Id operation.</summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>Gets or sets the Name value.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the FullName value.</summary>
    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Gets or sets the HtmlUrl value.</summary>
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets the Description value.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the Private value.</summary>
    [JsonPropertyName("private")]
    public bool Private { get; set; }

    /// <summary>Gets or sets the DefaultBranch value.</summary>
    [JsonPropertyName("default_branch")]
    public string DefaultBranch { get; set; } = "main";
}
