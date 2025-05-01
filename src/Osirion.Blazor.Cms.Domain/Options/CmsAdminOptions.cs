namespace Osirion.Blazor.Cms.Domain.Options;

/// <summary>
/// Configuration options for Osirion.Blazor.Cms.Admin
/// </summary>
public class CmsAdminOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "Osirion:Cms:GitHub:Admin";

    /// <summary>
    /// Gets or sets the owner (user or organization) of the repositories
    /// </summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default repository to use
    /// </summary>
    public string? DefaultRepository { get; set; }

    /// <summary>
    /// Gets or sets the default branch to use
    /// </summary>
    public string DefaultBranch { get; set; } = "main";

    /// <summary>
    /// Gets or sets whether to allow creating new branches
    /// </summary>
    public bool AllowBranchCreation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow creating pull requests
    /// </summary>
    public bool AllowPullRequests { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow deleting files
    /// </summary>
    public bool AllowFileDelete { get; set; } = true;

    /// <summary>
    /// Gets or sets the list of allowed repository names (empty means all repositories are allowed)
    /// </summary>
    public List<string> AllowedRepositories { get; set; } = new();

    /// <summary>
    /// Gets or sets the committer name to use for commits
    /// </summary>
    public string? CommitterName { get; set; }

    /// <summary>
    /// Gets or sets the committer email to use for commits
    /// </summary>
    public string? CommitterEmail { get; set; }

    /// <summary>
    /// Gets or sets the allowed file extensions (empty means all extensions are allowed)
    /// </summary>
    public List<string> AllowedFileExtensions { get; set; } = new() { ".md", ".markdown" };

    /// <summary>
    /// Gets or sets the GitHub API token (optional, for private repos or higher rate limits)
    /// </summary>
    public string? ApiToken { get; set; }
}