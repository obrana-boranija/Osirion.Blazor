namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Provides functionality to exchange GitHub OAuth code for an access token
/// </summary>
public interface IGitHubTokenProvider
{
    /// <summary>
    /// Exchanges an OAuth code for an access token
    /// </summary>
    /// <param name="code">The OAuth code to exchange</param>
    /// <param name="clientId">The GitHub client ID</param>
    /// <param name="clientSecret">The GitHub client secret</param>
    /// <returns>The access token, or null if the exchange fails</returns>
    /// <remarks>
    /// Sends no redirect URI and no PKCE code verifier. When the authorization request carried a
    /// <c>code_challenge</c>, use the overload that takes the code verifier instead.
    /// </remarks>
    Task<string?> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret)
        => ExchangeCodeForTokenAsync(code, clientId, clientSecret, redirectUri: null, codeVerifier: null, CancellationToken.None);

    /// <summary>
    /// Exchanges an OAuth code for an access token
    /// </summary>
    /// <param name="code">The OAuth code to exchange</param>
    /// <param name="clientId">The GitHub client ID</param>
    /// <param name="clientSecret">The GitHub client secret</param>
    /// <param name="redirectUri">
    /// The redirect URI sent in the authorization request, which GitHub matches against the one the code was
    /// issued for; <see langword="null"/> to omit it
    /// </param>
    /// <param name="codeVerifier">
    /// The PKCE code verifier whose S256 challenge was sent in the authorization request; <see langword="null"/>
    /// when no challenge was sent
    /// </param>
    /// <param name="cancellationToken">A token that cancels the request</param>
    /// <returns>The access token, or null if GitHub rejects the exchange or the request fails</returns>
    Task<string?> ExchangeCodeForTokenAsync(
        string code,
        string clientId,
        string clientSecret,
        string? redirectUri,
        string? codeVerifier,
        CancellationToken cancellationToken = default);
}
