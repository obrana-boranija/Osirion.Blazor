namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Service for GitHub authentication
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Gets whether the user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current access token if authenticated
    /// </summary>
    string? AccessToken { get; }

    /// <summary>
    /// Gets the username of the authenticated user
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Authenticates with GitHub using OAuth flow
    /// </summary>
    Task<bool> AuthenticateWithGitHubAsync(string code);

    /// <summary>
    /// Sets an access token directly
    /// </summary>
    Task<bool> SetAccessTokenAsync(string token);

    /// <summary>
    /// Signs out
    /// </summary>
    Task SignOutAsync();
}