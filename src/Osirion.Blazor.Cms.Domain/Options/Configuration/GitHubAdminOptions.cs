namespace Osirion.Blazor.Cms.Domain.Options.Configuration;

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
    /// Gets or sets the GitHub API URL
    /// </summary>
    public string? ApiUrl { get; set; } = "https://api.github.com";

    /// <summary>
    /// Gets or sets the content path within the repository
    /// </summary>
    public string ContentPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the committer name for commits
    /// </summary>
    public string? CommitterName { get; set; }

    /// <summary>
    /// Gets or sets the committer email for commits
    /// </summary>
    public string? CommitterEmail { get; set; }

    /// <summary>
    /// Gets or sets whether to allow creating new branches
    /// </summary>
    public bool AllowBranchCreation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow deleting files
    /// </summary>
    public bool AllowFileDelete { get; set; } = true;
}