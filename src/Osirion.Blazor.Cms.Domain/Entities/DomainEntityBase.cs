using Osirion.Blazor.Cms.Domain.Common;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Extensions;

namespace Osirion.Blazor.Cms.Domain.Entities;

/// <summary>
/// Base class for domain entities with event support
/// </summary>
/// <typeparam name="TId">Type of entity identifier</typeparam>
public abstract class DomainEntityBase<TId> : DomainEntity<TId> where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the domain events raised by this entity
    /// </summary>
    public new IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to be dispatched when the entity is saved
    /// </summary>
    /// <param name="domainEvent">The domain event</param>
    protected new void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to remove</param>
    protected new void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events
    /// </summary>
    public new void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
