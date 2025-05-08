using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.Events;

namespace Osirion.Blazor.Cms.Admin.Services.Events;

/// <summary>
/// Mediator service for handling events in the CMS admin interface
/// </summary>
public class CmsEventMediator
{
    private readonly Dictionary<Type, List<object>> _handlers = new();
    private readonly ILogger<CmsEventMediator> _logger;
    private readonly List<ICmsEvent> _eventHistory = new();
    private readonly int _maxHistorySize = 100;

    public CmsEventMediator(ILogger<CmsEventMediator> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Subscribes to an event type
    /// </summary>
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : ICmsEvent
    {
        var eventType = typeof(TEvent);

        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<object>();
        }

        _handlers[eventType].Add(handler);
        _logger.LogDebug("Handler subscribed for event type: {EventType}", eventType.Name);
    }

    /// <summary>
    /// Unsubscribes from an event type
    /// </summary>
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : ICmsEvent
    {
        var eventType = typeof(TEvent);

        if (_handlers.ContainsKey(eventType))
        {
            _handlers[eventType].Remove(handler);
            _logger.LogDebug("Handler unsubscribed from event type: {EventType}", eventType.Name);
        }
    }

    /// <summary>
    /// Publishes an event
    /// </summary>
    public void Publish<TEvent>(TEvent @event) where TEvent : ICmsEvent
    {
        var eventType = typeof(TEvent);
        _logger.LogDebug("Publishing event: {EventType}", eventType.Name);

        // Add to history
        _eventHistory.Add(@event);
        if (_eventHistory.Count > _maxHistorySize)
        {
            _eventHistory.RemoveAt(0);
        }

        if (_handlers.ContainsKey(eventType))
        {
            foreach (var handler in _handlers[eventType])
            {
                try
                {
                    ((Action<TEvent>)handler)(@event);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in event handler for {EventType}", eventType.Name);
                }
            }
        }
    }

    /// <summary>
    /// Gets the event history
    /// </summary>
    public IReadOnlyList<ICmsEvent> GetEventHistory() => _eventHistory.AsReadOnly();

    /// <summary>
    /// Clears all event handlers
    /// </summary>
    public void ClearHandlers()
    {
        _handlers.Clear();
        _logger.LogDebug("All event handlers cleared");
    }
}