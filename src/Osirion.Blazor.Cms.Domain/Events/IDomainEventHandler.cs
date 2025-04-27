namespace Osirion.Blazor.Cms.Domain.Events;

/// <summary>
/// Interface for domain event handlers
/// </summary>
/// <typeparam name="T">Type of domain event to handle</typeparam>
public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    /// <summary>
    /// Handles a domain event
    /// </summary>
    /// <param name="domainEvent">The event to handle</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task HandleAsync(T domainEvent);
}