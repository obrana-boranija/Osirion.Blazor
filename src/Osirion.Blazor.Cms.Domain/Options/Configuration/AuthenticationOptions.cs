namespace Osirion.Blazor.Cms.Domain.Options.Configuration;

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
    /// Gets or sets the GitHub OAuth redirect URI. When empty, the admin login page of the site
    /// (<c>{BaseUri}osirion/login</c>) is used.
    /// </summary>
    public string GitHubRedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether personal access tokens are allowed
    /// </summary>
    public bool AllowPersonalAccessTokens { get; set; } = true;

    /// <summary>
    /// Gets or sets a server personal access token (PAT) that admin services use on the server when no user is
    /// signed in with GitHub. It never signs anyone in and is never sent to the browser.
    /// </summary>
    public string? PersonalAccessToken { get; set; }
}
