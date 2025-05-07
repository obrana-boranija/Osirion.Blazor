using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Cms.Admin.Core.Events;

/// <summary>
/// Implementation of the event bus
/// </summary>
public class EventBus : IEventPublisher, IEventSubscriber
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();
    private readonly ILogger<EventBus> _logger;

    public EventBus(ILogger<EventBus> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (_handlers.ContainsKey(eventType))
        {
            _handlers[eventType].Remove(handler);
            _logger.LogDebug("Handler unsubscribed from event type: {EventType}", eventType.Name);
        }
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : class
    {
        var eventType = typeof(TEvent);
        _logger.LogDebug("Publishing event: {EventType}", eventType.Name);

        if (!_handlers.ContainsKey(eventType))
        {
            return;
        }

        // Make a copy to avoid concurrent modification issues
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