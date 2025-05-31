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
    /// <param name="providerName">The name of the provider</param>
    /// <returns>The GitHub API client</returns>
    IGitHubApiClient GetClient(string providerName);

    /// <summary>
    /// Gets the default GitHub API client
    /// </summary>
    /// <returns>The default GitHub API client</returns>
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