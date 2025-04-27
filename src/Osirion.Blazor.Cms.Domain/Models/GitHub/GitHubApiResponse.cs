using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Base class for GitHub API responses
/// </summary>
public abstract class GitHubApiResponse
{
    /// <summary>
    /// Gets or sets the response status
    /// </summary>
    [JsonIgnore]
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets any error message
    /// </summary>
    [JsonIgnore]
    public string? ErrorMessage { get; set; }
}