namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Services;

/// <summary>
/// Restores the GitHub sign-in kept by <see cref="IGitHubTokenStore"/> for the current circuit, so a page reload on
/// any admin page does not lose it.
/// </summary>
/// <remarks>
/// The token store uses browser storage, so call <see cref="RestoreAsync"/> only during an interactive render,
/// for example from <c>OnAfterRenderAsync</c>.
/// </remarks>
public interface IGitHubSessionRestorer
{
    /// <summary>
    /// Signs in with the stored token when the circuit is not signed in yet. A stored token that GitHub no longer
    /// accepts is removed. Failures are logged and reported as <see langword="false"/>.
    /// </summary>
    /// <returns><see langword="true"/> when the circuit is signed in to GitHub after the call.</returns>
    Task<bool> RestoreAsync();
}
