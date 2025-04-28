// src/Osirion.Blazor.Cms.Domain/Services/IContentProviderRegistry.cs
namespace Osirion.Blazor.Cms.Domain.Services;

/// <summary>
/// Registry for content providers
/// </summary>
public interface IContentProviderRegistry
{
    /// <summary>
    /// Gets all registered providers
    /// </summary>
    IEnumerable<IContentProvider> GetAllProviders();

    /// <summary>
    /// Gets the default provider
    /// </summary>
    IContentProvider? GetDefaultProvider();

    /// <summary>
    /// Gets a provider by ID
    /// </summary>
    IContentProvider? GetProvider(string providerId);

    /// <summary>
    /// Sets the default provider
    /// </summary>
    void SetDefaultProvider(string providerId);
}