using Osirion.Blazor.Cms.Domain.Interfaces;

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
    /// Gets the current username
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Event raised when authentication state changes
    /// </summary>
    event Action<bool> AuthenticationChanged;

    /// <summary>
    /// Initializes the authentication service and restores authentication state
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Authenticates with GitHub using an OAuth code
    /// </summary>
    Task<bool> AuthenticateWithGitHubAsync(string code);

    /// <summary>
    /// Sets and validates an access token
    /// </summary>
    Task<bool> SetAccessTokenAsync(string token);

    /// <summary>
    /// Signs the user out
    /// </summary>
    Task SignOutAsync();
}