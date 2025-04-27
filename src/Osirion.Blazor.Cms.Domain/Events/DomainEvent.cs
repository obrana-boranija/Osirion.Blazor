namespace Osirion.Blazor.Cms.Domain.Events;

/// <summary>
/// Base interface for all domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    DateTime OccurredOn { get; }
}

/// <summary>
/// Base class for all domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}