namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Services;

/// <summary>
/// Keeps a GitHub sign-in that has started, from the moment the browser leaves for GitHub until GitHub sends it
/// back with an authorization code.
/// </summary>
/// <remarks>
/// The default implementation encrypts the request with ASP.NET Core Data Protection and keeps it in the
/// browser's session storage, so it belongs to one browser tab. Because it uses the browser, call it only during
/// an interactive render.
/// </remarks>
public interface IGitHubOAuthStateStore
{
    /// <summary>
    /// Stores <paramref name="request"/>, replacing any sign-in that has not completed yet.
    /// </summary>
    /// <param name="request">The sign-in that is about to leave for GitHub</param>
    Task SaveAsync(GitHubOAuthRequest request);

    /// <summary>
    /// Gets the pending sign-in and removes it, so a sign-in can complete at most once.
    /// </summary>
    /// <returns>The pending sign-in, or <see langword="null"/> when there is none or it cannot be decrypted</returns>
    Task<GitHubOAuthRequest?> TakeAsync();
}

/// <summary>
/// A GitHub OAuth sign-in that has been sent to GitHub but has not completed yet.
/// </summary>
/// <param name="State">The unguessable value sent as the OAuth <c>state</c>, which GitHub sends back unchanged</param>
/// <param name="CodeVerifier">The PKCE code verifier whose S256 challenge was sent to GitHub</param>
/// <param name="RedirectUri">The redirect URI sent to GitHub, which the code exchange must repeat</param>
/// <param name="ReturnUrl">The local path to open once the sign-in completes</param>
public sealed record GitHubOAuthRequest(string State, string CodeVerifier, string RedirectUri, string ReturnUrl);
