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
    /// Sets the provider to use for authentication
    /// </summary>
    /// <param name="providerName">Name of the provider</param>
    void SetProvider(string providerName);

    /// <summary>
    /// Initializes the authentication service and restores authentication state
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Authenticates with GitHub using an OAuth code
    /// </summary>
    /// <remarks>
    /// The code is exchanged without an OAuth <c>state</c> check, a PKCE code verifier or a redirect URI. A caller
    /// that accepts a <c>code</c> from a callback URL without checking its state can be signed in to an attacker's
    /// GitHub account (login CSRF). A code issued for a PKCE authorization request is sent without its verifier.
    /// </remarks>
    [Obsolete("A GitHub OAuth code cannot be verified here. Check the state of the callback, exchange the code with " +
        "IGitHubTokenProvider.ExchangeCodeForTokenAsync(code, clientId, clientSecret, redirectUri, codeVerifier, " +
        "cancellationToken), then call SetAccessTokenAsync. The Cms.Admin LoginViewModel does this.")]
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
