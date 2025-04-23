namespace Osirion.Blazor.Cms.Admin.Options;

/// <summary>
/// Options for GitHub authentication
/// </summary>
public class GitHubAuthOptions
{
    /// <summary>
    /// Gets or sets the GitHub OAuth client ID
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the GitHub OAuth client secret
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets whether GitHub OAuth authentication is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the redirect URI for GitHub OAuth
    /// </summary>
    public string? RedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the OAuth scopes to request
    /// </summary>
    public string Scopes { get; set; } = "repo";
}