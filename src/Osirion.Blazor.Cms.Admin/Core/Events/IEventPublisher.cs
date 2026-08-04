namespace Osirion.Blazor.Cms.Admin.Core.Events;

/// <summary>
/// Interface for publishing events
/// </summary>
public interface IEventPublisher
{
    /// <summary>Publishes an event to its subscribed handlers.</summary>
    void Publish<TEvent>(TEvent @event) where TEvent : class;
}
