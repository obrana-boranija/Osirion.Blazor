namespace Osirion.Blazor.Cms.Admin.Core.Events;

/// <summary>
/// Interface for publishing events
/// </summary>
public interface IEventPublisher
{
    void Publish<TEvent>(TEvent @event) where TEvent : class;
}