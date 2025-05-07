namespace Osirion.Blazor.Cms.Admin.Core.Events;

/// <summary>
/// Interface for subscribing to events
/// </summary>
public interface IEventSubscriber
{
    void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
    void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
}