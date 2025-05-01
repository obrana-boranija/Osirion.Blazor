namespace Osirion.Blazor.Cms.Domain.Options;

/// <summary>
/// Configuration options for GitHub integration
/// </summary>
public class GitHubOptions
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

    /// <summary>
    /// Gets or sets the unique provider ID
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets whether caching is enabled
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the cache duration in minutes
    /// </summary>
    public int CacheDurationMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets whether this is the default provider
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets whether localization is enabled
    /// </summary>
    public bool EnableLocalization { get; set; } = false;

    /// <summary>
    /// Gets or sets the default locale
    /// </summary>
    public string DefaultLocale { get; set; } = "en";

    /// <summary>
    /// Gets or sets the supported locales
    /// </summary>
    public List<string> SupportedLocales { get; set; } = new() { "en" };

    /// <summary>
    /// Gets or sets the supported file extensions
    /// </summary>
    public List<string> SupportedExtensions { get; set; } = new() { ".md", ".markdown" };

    /// <summary>
    /// Gets or sets whether to validate content on write operations
    /// </summary>
    public bool ValidateContent { get; set; } = true;
}