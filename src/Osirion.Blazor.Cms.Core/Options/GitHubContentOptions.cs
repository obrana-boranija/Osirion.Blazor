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
    /// Gets or sets whether localization is enabled
    /// </summary>
    /// <remarks>
    /// When enabled, the provider will extract locale information from directory paths 
    /// and group content by localization ID. When disabled, all content is treated as
    /// if it belonged to the default locale.
    /// </remarks>
    public bool EnableLocalization { get; set; } = false;

    /// <summary>
    /// Gets or sets the default locale to use when localization is disabled
    /// </summary>
    public string DefaultLocale { get; set; } = "en";
}