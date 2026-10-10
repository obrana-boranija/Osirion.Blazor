using Osirion.Blazor.Cms.Admin.Core.Events;

namespace Osirion.Blazor.Cms.Admin.Tests.TestDoubles;

/// <summary>
/// Publish on the event buses is not virtual, so a substitute cannot observe it; the tests use
/// a real bus and record what reaches a subscriber instead.
/// </summary>
public static class EventRecorder
{
    public static List<TEvent> Record<TEvent>(IEventSubscriber subscriber) where TEvent : class
    {
        var events = new List<TEvent>();
        subscriber.Subscribe<TEvent>(events.Add);
        return events;
    }
}
