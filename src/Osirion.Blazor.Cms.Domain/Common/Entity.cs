// src/Osirion.Blazor.Cms.Domain/Common/Entity.cs
using Osirion.Blazor.Cms.Domain.Events;
using System.Collections.ObjectModel;

namespace Osirion.Blazor.Cms.Domain.Common
{
    public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : notnull
    {
        private List<IDomainEvent>? _domainEvents;

        public TId Id { get; protected set; } = default!;

        protected Entity() { }

        protected Entity(TId id)
        {
            Id = id;
        }

        public IReadOnlyCollection<IDomainEvent> DomainEvents
        {
            get
            {
                return _domainEvents is not null
                    ? new ReadOnlyCollection<IDomainEvent>(_domainEvents)
                    : Array.Empty<IDomainEvent>();
            }
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents?.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public override bool Equals(object? obj)
        {
            return obj is Entity<TId> entity && Equals(entity);
        }

        public bool Equals(Entity<TId>? other)
        {
            return other is not null &&
                   EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        {
            return EqualityComparer<Entity<TId>>.Default.Equals(left, right);
        }

        public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        {
            return !(left == right);
        }
    }
}