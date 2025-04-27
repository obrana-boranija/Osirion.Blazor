namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Represents a registration of a content provider that can be processed during application startup
/// </summary>
public interface IProviderRegistration
{
    /// <summary>
    /// Registers the provider with the factory
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    void Register(IServiceProvider serviceProvider);

    /// <summary>
    /// Gets the provider ID
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Gets whether this provider should be set as the default
    /// </summary>
    bool IsDefault { get; }
}