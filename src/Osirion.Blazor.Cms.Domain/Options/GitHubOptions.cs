using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Domain.Options;

/// <summary>
/// Configuration options for GitHub content provider
/// </summary>
public class GitHubOptions : ContentProviderOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "Osirion:Cms:GitHub";

    /// <summary>
    /// Gets or sets the GitHub repository owner/organization
    /// </summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GitHub repository name
    /// </summary>
    public string Repository { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch to use (default: main)
    /// </summary>
    public string Branch { get; set; } = "main";

    /// <summary>
    /// Gets or sets the GitHub API token for authentication
    /// </summary>
    public string ApiToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GitHub API URL (default: https://api.github.com)
    /// </summary>
    public string? ApiUrl { get; set; } = "https://api.github.com";

    /// <summary>
    /// Gets or sets the webhook secret for validating webhook requests
    /// </summary>
    public string? WebhookSecret { get; set; }

    /// <summary>
    /// Gets or sets whether polling is enabled for checking repository changes
    /// </summary>
    public bool EnablePolling { get; set; } = false;

    /// <summary>
    /// Gets or sets the polling interval in seconds (default: 86400 = 1 day)
    /// </summary>
    public int PollingIntervalSeconds { get; set; } = 86400;

    /// <summary>
    /// Gets or sets whether to use background cache updates (recommended for webhooks/polling)
    /// </summary>
    public bool UseBackgroundCacheUpdate { get; set; } = true;

    /// <summary>
    /// Gets or sets authentication options for GitHub
    /// </summary>
    public AuthenticationOptions Authentication { get; set; } = new();

    /// <summary>
    /// Gets or sets patterns for excluding files/directories
    /// </summary>
    public List<string> ExcludePatterns { get; set; } = new() { ".*", "_*" };
}