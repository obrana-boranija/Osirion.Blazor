using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Represents a pull request
/// </summary>
public class GitHubPullRequest
{
    /// <summary>Gets or sets the Id value.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>Gets or sets the Number value.</summary>
    [JsonPropertyName("number")]
    public int Number { get; set; }

    /// <summary>Gets or sets the Url value.</summary>
    [JsonPropertyName("html_url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>Gets or sets the Title value.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the Body value.</summary>
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    /// <summary>Gets or sets the State value.</summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>Performs the Head operation.</summary>
    [JsonPropertyName("head")]
    public GitHubRef Head { get; set; } = new();

    /// <summary>Performs the Base operation.</summary>
    [JsonPropertyName("base")]
    public GitHubRef Base { get; set; } = new();
}
