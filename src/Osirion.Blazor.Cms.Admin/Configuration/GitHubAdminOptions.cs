namespace Osirion.Blazor.Cms.Admin.Configuration;

/// <summary>
/// Configuration options for GitHub integration
/// </summary>
public class GitHubAdminOptions
{
    /// <summary>
    /// Gets or sets the GitHub owner (user or organization)
    /// </summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the repository name
    /// </summary>
    public string Repository { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default branch
    /// </summary>
    public string DefaultBranch { get; set; } = "main";

    /// <summary>
    /// Gets or sets the content path within the repository
    /// </summary>
    public string ContentPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default content provider
    /// </summary>
    public string DefaultContentProvider { get; set; } = "GitHub";
}