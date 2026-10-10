namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Services;

/// <summary>
/// Keeps the GitHub access token a user signed in with, so the user does not have to enter it again after a page
/// reload. The admin login page restores it.
/// </summary>
/// <remarks>
/// The default implementation encrypts the token with ASP.NET Core Data Protection, binds it to the signed-in
/// host user and keeps it in the browser's local storage. Because it uses the browser, call it only during an
/// interactive render, for example from <c>OnAfterRenderAsync</c> or an event handler. The personal access
/// token configured for the server is never stored here.
/// </remarks>
public interface IGitHubTokenStore
{
    /// <summary>
    /// Gets the token stored for the current host user.
    /// </summary>
    /// <returns>
    /// The token, or <see langword="null"/> when none is stored, when the current host user has no stable
    /// identity, or when the stored value cannot be decrypted for this user (for example after a key ring change)
    /// </returns>
    Task<string?> GetTokenAsync();

    /// <summary>
    /// Stores <paramref name="token"/> for the current host user.
    /// </summary>
    /// <param name="token">The GitHub access token the user signed in with</param>
    /// <returns>
    /// <see langword="true"/> when the token was stored; <see langword="false"/> when the current host user has no
    /// stable identity, in which case nothing is stored and the token lasts only for the current session
    /// </returns>
    Task<bool> SaveTokenAsync(string token);

    /// <summary>
    /// Removes the stored token.
    /// </summary>
    Task RemoveTokenAsync();
}
