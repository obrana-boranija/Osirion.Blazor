namespace Osirion.Blazor.Cms.Admin.Services.Events;

public class CmsEventMediator
{
    private readonly Dictionary<Type, List<object>> _handlers = new();

    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : ICmsEvent
    {
        var eventType = typeof(TEvent);

        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<object>();
        }

        _handlers[eventType].Add(handler);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : ICmsEvent
    {
        var eventType = typeof(TEvent);

        if (_handlers.ContainsKey(eventType))
        {
            _handlers[eventType].Remove(handler);
        }
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : ICmsEvent
    {
        var eventType = typeof(TEvent);

        if (_handlers.ContainsKey(eventType))
        {
            foreach (var handler in _handlers[eventType])
            {
                ((Action<TEvent>)handler)(@event);
            }
        }
    }
}