namespace Osirion.Blazor.Cms.Domain.Common;

/// <summary>
/// Base class for domain entities with common behavior
/// </summary>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public abstract class DomainEntity<TId> : Entity<TId>
{
    /// <summary>
    /// Gets or sets the provider identifier that created this entity
    /// </summary>
    public string ProviderId { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider-specific identifier
    /// </summary>
    public string? ProviderSpecificId { get; protected set; }

    /// <summary>
    /// Sets the provider ID
    /// </summary>
    public void SetProviderId(string providerId)
    {
        ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
    }

    /// <summary>
    /// Sets the provider-specific ID
    /// </summary>
    public void SetProviderSpecificId(string? id)
    {
        ProviderSpecificId = id;
    }
}