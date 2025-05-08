namespace Osirion.Blazor.Cms.Admin.Configuration;

/// <summary>
/// Configuration options for authentication
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// Gets or sets whether GitHub OAuth is enabled
    /// </summary>
    public bool EnableGitHubOAuth { get; set; } = false;

    /// <summary>
    /// Gets or sets the GitHub OAuth client ID
    /// </summary>
    public string GitHubClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GitHub OAuth client secret
    /// </summary>
    public string GitHubClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GitHub OAuth redirect URI
    /// </summary>
    public string GitHubRedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether personal access tokens are allowed
    /// </summary>
    public bool AllowPersonalAccessTokens { get; set; } = true;
}