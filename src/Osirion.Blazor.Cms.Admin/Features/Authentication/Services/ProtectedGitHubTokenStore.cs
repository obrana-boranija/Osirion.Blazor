using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Services;

/// <summary>
/// Keeps the user's GitHub token in browser local storage, encrypted with a Data Protection purpose that names
/// the signed-in host user.
/// </summary>
/// <remarks>
/// Binding the purpose to the host user means a value copied from another user's browser, or written while
/// another user was signed in, cannot be decrypted here. Such a value, and one written under a key ring that no
/// longer exists, is treated as signed out and removed.
/// </remarks>
internal sealed class ProtectedGitHubTokenStore : IGitHubTokenStore
{
    /// <summary>
    /// The browser storage key of the encrypted token.
    /// </summary>
    internal const string StorageKey = "osirion_cms_admin_github_token";

    private const string PurposePrefix = "Osirion.Blazor.Cms.Admin.GitHubToken";

    private readonly ProtectedLocalStorage _storage;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILogger<ProtectedGitHubTokenStore> _logger;

    public ProtectedGitHubTokenStore(
        ProtectedLocalStorage storage,
        AuthenticationStateProvider authenticationStateProvider,
        ILogger<ProtectedGitHubTokenStore> logger)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _authenticationStateProvider = authenticationStateProvider ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<string?> GetTokenAsync()
    {
        var purpose = await GetPurposeAsync();
        if (purpose is null)
        {
            return null;
        }

        try
        {
            var result = await _storage.GetAsync<string>(purpose, StorageKey);
            return result.Success && !string.IsNullOrWhiteSpace(result.Value) ? result.Value : null;
        }
        catch (CryptographicException ex)
        {
            _logger.LogWarning(ex, "The stored GitHub token could not be decrypted for the current user and was removed");
            await _storage.DeleteAsync(StorageKey);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SaveTokenAsync(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var purpose = await GetPurposeAsync();
        if (purpose is null)
        {
            _logger.LogInformation("The GitHub token was not stored because the signed-in user has no stable identity");
            return false;
        }

        await _storage.SetAsync(purpose, StorageKey, token);
        return true;
    }

    /// <inheritdoc/>
    public async Task RemoveTokenAsync()
    {
        await _storage.DeleteAsync(StorageKey);
    }

    /// <summary>
    /// Gets the stable identifier of an authenticated host user: the name identifier claim, else the name.
    /// </summary>
    /// <returns>The identifier, or <see langword="null"/> when the user is anonymous or has neither value</returns>
    internal static string? GetUserId(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            userId = user.Identity.Name;
        }

        return string.IsNullOrWhiteSpace(userId) ? null : userId;
    }

    private async Task<string?> GetPurposeAsync()
    {
        var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var userId = GetUserId(authenticationState.User);

        return userId is null ? null : $"{PurposePrefix}:{userId}";
    }
}
