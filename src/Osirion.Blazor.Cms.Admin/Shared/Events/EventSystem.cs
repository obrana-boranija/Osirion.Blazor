using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Cms.Admin.Shared.Events;

/// <summary>
/// Interface for publishing events
/// </summary>
public interface IEventPublisher
{
    void Publish<TEvent>(TEvent @event) where TEvent : class;
}

/// <summary>
/// Interface for subscribing to events
/// </summary>
public interface IEventSubscriber
{
    void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
    void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
}

/// <summary>
/// Centralized event bus for application-wide events
/// </summary>
public class EventBus : IEventPublisher, IEventSubscriber
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();
    private readonly ILogger<EventBus> _logger;

    public EventBus(ILogger<EventBus> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Subscribe to an event type
    /// </summary>
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Delegate>();
        }

        _handlers[eventType].Add(handler);
        _logger.LogDebug("Handler subscribed for event type: {EventType}", eventType.Name);
    }

    /// <summary>
    /// Unsubscribe from an event type
    /// </summary>
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (_handlers.ContainsKey(eventType))
        {
            _handlers[eventType].Remove(handler);
            _logger.LogDebug("Handler unsubscribed from event type: {EventType}", eventType.Name);
        }
    }

    /// <summary>
    /// Publish an event to all subscribers
    /// </summary>
    public void Publish<TEvent>(TEvent @event) where TEvent : class
    {
        var eventType = typeof(TEvent);
        _logger.LogDebug("Publishing event: {EventType}", eventType.Name);

        if (!_handlers.ContainsKey(eventType))
        {
            return;
        }

        // Create a copy of handlers to avoid modification issues if a handler unsubscribes
        var handlers = _handlers[eventType].ToList();

        foreach (var handler in handlers)
        {
            try
            {
                ((Action<TEvent>)handler)(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling event {EventType}", eventType.Name);
            }
        }
    }
}