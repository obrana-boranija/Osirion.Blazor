namespace Osirion.Blazor.Cms.Domain.Options;

/// <summary>
/// Configuration options for GitHub integration
/// </summary>
public class GitHubOptions : ContentProviderOptions
{
    /// <summary>
    /// The section name in the configuration file
    /// </summary>
    public const string Section = "Osirion:Cms:GitHub:Web";

    /// <summary>
    /// Gets or sets the GitHub owner (user or organization)
    /// </summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GitHub repository name
    /// </summary>
    public string Repository { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content path within the repository
    /// </summary>
    public string ContentPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch to read from
    /// </summary>
    public string Branch { get; set; } = "main";

    /// <summary>
    /// Gets or sets the GitHub API token (optional, for private repos or higher rate limits)
    /// </summary>
    public string? ApiToken { get; set; }

    /// <summary>
    /// Gets or sets the committer name for commits
    /// </summary>
    public string? CommitterName { get; set; }

    /// <summary>
    /// Gets or sets the committer email for commits
    /// </summary>
    public string? CommitterEmail { get; set; }

    /// <summary>
    /// Gets or sets the GitHub API URL
    /// </summary>
    public string? ApiUrl { get; set; } = "https://api.github.com";
}