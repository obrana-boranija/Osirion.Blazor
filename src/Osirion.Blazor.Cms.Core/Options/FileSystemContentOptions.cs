namespace Osirion.Blazor.Cms.Options;

/// <summary>
/// Configuration options for GitHub content provider
/// </summary>
public class GitHubContentOptions : ContentProviderOptions
{
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
    /// Gets or sets the GitHub API URL (defaults to api.github.com)
    /// </summary>
    public string ApiUrl { get; set; } = "https://api.github.com";

    /// <summary>
    /// Gets or sets whether write operations are allowed
    /// </summary>
    public bool AllowWrite { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to allow branch creation
    /// </summary>
    public bool AllowBranchCreation { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to allow pull requests
    /// </summary>
    public bool AllowPullRequests { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to allow file deletion
    /// </summary>
    public bool AllowFileDelete { get; set; } = false;

    /// <summary>
    /// Gets or sets the committer name for commits
    /// </summary>
    public string? CommitterName { get; set; }

    /// <summary>
    /// Gets or sets the committer email for commits
    /// </summary>
    public string? CommitterEmail { get; set; }

    /// <summary>
    /// Gets or sets whether to fetch commit history
    /// </summary>
    public bool FetchHistory { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of history items to fetch
    /// </summary>
    public int MaxHistoryItems { get; set; } = 10;

    /// <summary>
    /// Gets or sets the request timeout in seconds
    /// </summary>
    public int RequestTimeoutSeconds { get; set; } = 30;
}