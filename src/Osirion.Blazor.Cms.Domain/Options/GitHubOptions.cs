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
    /// Gets or sets the GitHub API URL
    /// </summary>
    public string? ApiUrl { get; set; } = "https://api.github.com";

    // Webhook and polling settings

    /// <summary>
    /// Gets or sets the webhook secret for validating GitHub webhooks
    /// </summary>
    public string? WebhookSecret { get; set; }

    /// <summary>
    /// Gets or sets whether to enable periodic polling of the repository
    /// </summary>
    public bool EnablePolling { get; set; } = true;

    /// <summary>
    /// Gets or sets the polling interval in seconds
    /// </summary>
    public int PollingIntervalSeconds { get; set; } = 300; // 5 minutes by default

    /// <summary>
    /// Gets or sets whether to update the cache in the background
    /// </summary>
    public bool UseBackgroundCacheUpdate { get; set; } = true;
}