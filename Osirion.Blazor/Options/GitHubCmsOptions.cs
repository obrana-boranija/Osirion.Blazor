namespace Osirion.Blazor.Options;

/// <summary>
/// Configuration options for GitHub CMS
/// </summary>
public class GitHubCmsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "GitHubCms";

    /// <summary>
    /// Gets or sets the GitHub owner
    /// </summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GitHub repository
    /// </summary>
    public string Repository { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content path
    /// </summary>
    public string ContentPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch
    /// </summary>
    public string Branch { get; set; } = "main";

    /// <summary>
    /// Gets or sets the GitHub API token
    /// </summary>
    public string? ApiToken { get; set; }

    /// <summary>
    /// Gets or sets the cache duration in minutes
    /// </summary>
    public int CacheDurationMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets the supported file extensions
    /// </summary>
    public List<string> SupportedExtensions { get; set; } = new() { ".md", ".markdown" };

    /// <summary>
    /// Gets or sets whether to use the default styles
    /// </summary>
    public bool? UseStyles { get; set; }

    /// <summary>
    /// Gets or sets custom CSS variables to override the default values
    /// </summary>
    public string? CustomVariables { get; set; }
}