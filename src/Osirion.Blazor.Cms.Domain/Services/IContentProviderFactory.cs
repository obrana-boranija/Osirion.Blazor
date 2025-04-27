namespace Osirion.Blazor.Cms.Domain.Services;

/// <summary>
/// Factory interface for creating content providers
/// </summary>
public interface IContentProviderFactory
{
    /// <summary>
    /// Creates a provider instance by ID
    /// </summary>
    /// <param name="providerId">The ID of the provider to create</param>
    /// <returns>The created content provider</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no provider with the given ID is registered</exception>
    IContentProvider CreateProvider(string providerId);

    /// <summary>
    /// Gets all available provider types
    /// </summary>
    /// <returns>Collection of provider IDs</returns>
    IEnumerable<string> GetAvailableProviderTypes();

    /// <summary>
    /// Registers a provider factory function
    /// </summary>
    /// <typeparam name="T">Type of content provider</typeparam>
    /// <param name="factory">Factory function that creates the provider</param>
    void RegisterProvider<T>(Func<IServiceProvider, T> factory) where T : class, IContentProvider;

    /// <summary>
    /// Sets the default provider ID
    /// </summary>
    /// <param name="providerId">The ID of the provider to set as default</param>
    void SetDefaultProvider(string providerId);

    /// <summary>
    /// Gets the default provider ID
    /// </summary>
    /// <returns>The default provider ID or null if not set</returns>
    string? GetDefaultProviderId();
}