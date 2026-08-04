using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Domain.Models.GitHub;

/// <summary>
/// Represents an author or committer of a GitHub commit
/// </summary>
public class GitHubAuthor
{
    /// <summary>Gets or sets the Name value.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the Email value.</summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the Date value.</summary>
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}
