using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Factory for creating GitHub API client instances for different providers
/// </summary>
public interface IGitHubApiClientFactory
{
    /// <summary>
    /// Gets a GitHub API client for the specified provider
    /// </summary>
    /// <remarks>
    /// Every call returns a new instance owned by the caller. Calling SetRepository, SetBranch or
    /// SetAccessToken on it never affects a client returned to any other caller.
    /// </remarks>
    /// <param name="providerName">The name of the provider</param>
    /// <returns>A new GitHub API client</returns>
    IGitHubApiClient GetClient(string providerName);

    /// <summary>
    /// Gets a GitHub API client for the default provider
    /// </summary>
    /// <remarks>
    /// Every call returns a new instance owned by the caller, exactly as <see cref="GetClient(string)"/> does.
    /// </remarks>
    /// <returns>A new GitHub API client</returns>
    IGitHubApiClient GetDefaultClient();

    /// <summary>
    /// Gets all configured provider names
    /// </summary>
    /// <returns>Collection of provider names</returns>
    IEnumerable<string> GetProviderNames();

    /// <summary>
    /// Checks if a provider exists
    /// </summary>
    /// <param name="providerName">The name of the provider</param>
    /// <returns>True if the provider exists, false otherwise</returns>
    bool ProviderExists(string providerName);

    /// <summary>
    /// Gets the configuration options for a specific provider
    /// </summary>
    /// <param name="providerName">The name of the provider</param>
    /// <returns>The provider options, or null if not found</returns>
    GitHubProviderOptions? GetProviderOptions(string providerName);

    /// <summary>
    /// Gets all configured provider options
    /// </summary>
    /// <returns>Collection of all provider options with their names</returns>
    IEnumerable<KeyValuePair<string, GitHubProviderOptions>> GetAllProviderOptions();
}
