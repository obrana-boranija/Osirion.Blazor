namespace Osirion.Blazor.Cms.Domain.Events;

/// <summary>
/// Interface for dispatching domain events
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches a domain event to its handlers
    /// </summary>
    /// <typeparam name="T">Type of domain event</typeparam>
    /// <param name="domainEvent">The event to dispatch</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DispatchAsync<T>(T domainEvent) where T : IDomainEvent;
}