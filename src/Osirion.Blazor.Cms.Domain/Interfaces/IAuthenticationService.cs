namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Interface for authentication services
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Gets whether the user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current access token
    /// </summary>
    string? AccessToken { get; }

    /// <summary>
    /// Gets the authenticated username
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Event raised when authentication state changes
    /// </summary>
    event Action<bool>? AuthenticationChanged;

    /// <summary>
    /// Authenticates with GitHub using an OAuth code
    /// </summary>
    /// <param name="code">The OAuth code</param>
    /// <returns>True if authentication was successful</returns>
    Task<bool> AuthenticateWithGitHubAsync(string code);

    /// <summary>
    /// Sets and validates an access token
    /// </summary>
    /// <param name="token">The access token to set</param>
    /// <returns>True if the token is valid</returns>
    Task<bool> SetAccessTokenAsync(string token);

    /// <summary>
    /// Signs out the current user
    /// </summary>
    Task SignOutAsync();
}