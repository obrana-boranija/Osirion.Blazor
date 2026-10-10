using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Services;

/// <summary>
/// Builds the pieces of a GitHub OAuth authorization request that protect it: the <c>state</c> value against
/// cross-site request forgery and the PKCE (S256) code challenge against code interception.
/// </summary>
/// <remarks>
/// GitHub OAuth apps accept <c>code_challenge</c> with <c>code_challenge_method=S256</c> only, and require the
/// matching <c>code_verifier</c> at the token exchange:
/// https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps
/// </remarks>
internal static class GitHubOAuth
{
    private const string AuthorizeEndpoint = "https://github.com/login/oauth/authorize";
    private const string Scope = "repo";
    private const int RandomValueByteCount = 32;

    /// <summary>
    /// Creates an unguessable value, used both as the OAuth state and as the PKCE code verifier.
    /// </summary>
    /// <returns>32 random bytes, base64url encoded without padding (43 characters)</returns>
    internal static string CreateRandomValue()
    {
        return Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(RandomValueByteCount));
    }

    /// <summary>
    /// Creates the S256 code challenge of <paramref name="codeVerifier"/>.
    /// </summary>
    /// <returns>The base64url encoded SHA-256 hash of the verifier, without padding</returns>
    internal static string CreateCodeChallenge(string codeVerifier)
    {
        return Base64Url.EncodeToString(SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier)));
    }

    /// <summary>
    /// Builds the URL that sends the browser to GitHub to authorize the app.
    /// </summary>
    internal static string BuildAuthorizeUrl(string clientId, GitHubOAuthRequest request)
    {
        return $"{AuthorizeEndpoint}" +
               $"?client_id={Uri.EscapeDataString(clientId)}" +
               $"&redirect_uri={Uri.EscapeDataString(request.RedirectUri)}" +
               $"&scope={Scope}" +
               $"&state={Uri.EscapeDataString(request.State)}" +
               $"&code_challenge={Uri.EscapeDataString(CreateCodeChallenge(request.CodeVerifier))}" +
               "&code_challenge_method=S256";
    }

    /// <summary>
    /// Compares the state GitHub sent back with the state the sign-in started with, in constant time.
    /// </summary>
    /// <returns><see langword="true"/> only when <paramref name="actual"/> equals <paramref name="expected"/></returns>
    internal static bool StatesMatch(string expected, string? actual)
    {
        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(actual))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(actual));
    }
}
