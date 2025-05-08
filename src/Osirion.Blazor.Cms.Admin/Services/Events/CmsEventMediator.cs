using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.Events;

namespace Osirion.Blazor.Cms.Admin.Services.Events;

/// <summary>
/// Event mediator for the CMS admin interface, providing a centralized event handling system
/// that implements both publisher and subscriber interfaces.
/// </summary>
public class CmsEventMediator : IEventPublisher, IEventSubscriber
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();
    private readonly ILogger<CmsEventMediator> _logger;

    public CmsEventMediator(ILogger<CmsEventMediator> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (_handlers.ContainsKey(eventType))
        {
            _handlers[eventType].Remove(handler);
            _logger.LogDebug("Handler unsubscribed from event type: {EventType}", eventType.Name);
        }
    }

    /// <inheritdoc/>
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