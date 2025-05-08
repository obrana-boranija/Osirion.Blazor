namespace Osirion.Blazor.Cms.Admin.Core.Configuration;

/// <summary>
/// Configuration options for GitHub integration
/// </summary>
public class GitHubOptions
{
    /// <summary>
    /// Gets or sets the GitHub owner (user or organization)
    /// </summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default repository name
    /// </summary>
    public string DefaultRepository { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content path within the repository
    /// </summary>
    public string ContentPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default branch to use
    /// </summary>
    public string DefaultBranch { get; set; } = "main";

    /// <summary>
    /// Gets or sets the GitHub API URL (defaults to public GitHub)
    /// </summary>
    public string ApiUrl { get; set; } = "https://api.github.com";

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

    /// <summary>
    /// Gets or sets whether to use SSL for GitHub API
    /// </summary>
    public bool UseSSL { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout in seconds for GitHub API requests
    /// </summary>
    public int TimeoutSeconds { get; set; } = 100;

    /// <summary>
    /// Gets or sets the allowed file extensions (empty means all extensions are allowed)
    /// </summary>
    public List<string> AllowedFileExtensions { get; set; } = new() { ".md", ".markdown" };

    /// <summary>
    /// Gets or sets the path to restrict all operations to (for security)
    /// </summary>
    public string? SecurityRootPath { get; set; }
}