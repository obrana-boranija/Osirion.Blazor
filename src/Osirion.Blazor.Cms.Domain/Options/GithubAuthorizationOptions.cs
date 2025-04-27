namespace Osirion.Blazor.Cms.Domain.Options;

/// <summary>
/// Configuration options for GitHub integration
/// </summary>
public class GithubAuthorizationOptions
{
    /// <summary>
    /// The section name in the configuration file
    /// </summary>
    public const string Section = "Osirion:Cms:GitHub:Authorization";

    /// <summary>
    /// Gets or sets the GitHub ClientId
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GitHub ClientSecret
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}
