using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Services;

/// <summary>
/// Keeps the pending GitHub sign-in in browser session storage, encrypted with Data Protection.
/// </summary>
internal sealed class ProtectedGitHubOAuthStateStore : IGitHubOAuthStateStore
{
    /// <summary>
    /// The browser storage key of the encrypted pending sign-in.
    /// </summary>
    internal const string StorageKey = "osirion_cms_admin_github_oauth";

    private readonly ProtectedSessionStorage _storage;
    private readonly ILogger<ProtectedGitHubOAuthStateStore> _logger;

    public ProtectedGitHubOAuthStateStore(
        ProtectedSessionStorage storage,
        ILogger<ProtectedGitHubOAuthStateStore> logger)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task SaveAsync(GitHubOAuthRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _storage.SetAsync(StorageKey, request);
    }

    /// <inheritdoc/>
    public async Task<GitHubOAuthRequest?> TakeAsync()
    {
        try
        {
            var result = await _storage.GetAsync<GitHubOAuthRequest>(StorageKey);
            return result.Success ? result.Value : null;
        }
        catch (CryptographicException ex)
        {
            _logger.LogWarning(ex, "The pending GitHub sign-in could not be decrypted and was discarded");
            return null;
        }
        finally
        {
            // Single use: a replayed callback finds nothing to match
            await _storage.DeleteAsync(StorageKey);
        }
    }
}
