namespace Osirion.Blazor.Cms.Admin.Core.Events;

/// <summary>
/// Interface for subscribing to events
/// </summary>
public interface IEventSubscriber
{
    /// <summary>Subscribes a handler for an event type.</summary>
    void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
    /// <summary>Unsubscribes a handler from an event type.</summary>
    void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
}
