namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Factory interface for creating repositories
/// </summary>
public interface IRepositoryFactory
{
    /// <summary>
    /// Creates a content repository for the specified provider
    /// </summary>
    /// <param name="providerId">Provider ID</param>
    /// <returns>Content repository instance</returns>
    IContentRepository CreateContentRepository(string providerId);

    /// <summary>
    /// Creates a directory repository for the specified provider
    /// </summary>
    /// <param name="providerId">Provider ID</param>
    /// <returns>Directory repository instance</returns>
    IDirectoryRepository CreateDirectoryRepository(string providerId);

    /// <summary>
    /// Gets the default provider ID
    /// </summary>
    /// <returns>Default provider ID</returns>
    string GetDefaultProviderId();

    /// <summary>
    /// Gets all available provider IDs
    /// </summary>
    /// <returns>Available provider IDs</returns>
    IEnumerable<string> GetAvailableProviderIds();
}