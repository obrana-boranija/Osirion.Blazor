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
    /// <returns>The GitHub API client instance</returns>
    IGitHubApiClient GetClient(string providerName);

    /// <summary>
    /// Gets a GitHub API client for the default provider
    /// </summary>
    /// <returns>The GitHub API client instance</returns>
    IGitHubApiClient GetDefaultClient();

    /// <summary>
    /// Gets all available provider names
    /// </summary>
    /// <returns>Collection of provider names</returns>
    IEnumerable<string> GetProviderNames();

    /// <summary>
    /// Checks if a provider exists
    /// </summary>
    /// <param name="providerName">The name of the provider</param>
    /// <returns>True if the provider exists</returns>
    bool ProviderExists(string providerName);
}