// src/Osirion.Blazor.Cms.Domain/Common/Entity.cs
using Osirion.Blazor.Cms.Domain.Events;
using System.Collections.ObjectModel;

namespace Osirion.Blazor.Cms.Domain.Common
{
    /// <summary>Provides identity, domain-event, and equality behavior for domain entities.</summary>
    public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : notnull
    {
        private List<IDomainEvent>? _domainEvents;

        /// <summary>Gets the entity identifier.</summary>
        public TId Id { get; protected set; } = default!;

        /// <summary>Initializes an entity without assigning an identifier.</summary>
        protected Entity() { }

        /// <summary>Initializes an entity with the specified identifier.</summary>
        /// <param name="id">The entity identifier.</param>
        protected Entity(TId id)
        {
            Id = id;
        }

        /// <summary>Gets the domain events raised by the entity.</summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents
        {
            get
            {
                return _domainEvents is not null
                    ? new ReadOnlyCollection<IDomainEvent>(_domainEvents)
                    : Array.Empty<IDomainEvent>();
            }
        }

        /// <summary>Adds a domain event to the entity.</summary>
        /// <param name="domainEvent">The event to add.</param>
        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(domainEvent);
        }

        /// <summary>Removes a domain event from the entity.</summary>
        /// <param name="domainEvent">The event to remove.</param>
        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents?.Remove(domainEvent);
        }

        /// <summary>Removes all domain events raised by the entity.</summary>
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Entity<TId> entity && Equals(entity);
        }

        /// <inheritdoc />
        public bool Equals(Entity<TId>? other)
        {
            return other is not null &&
                   EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        /// <summary>Determines whether two entities have equal identifiers.</summary>
        public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        {
            return EqualityComparer<Entity<TId>>.Default.Equals(left, right);
        }

        /// <summary>Determines whether two entities have different identifiers.</summary>
        public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        {
            return !(left == right);
        }
    }
}
