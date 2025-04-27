using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;

namespace Osirion.Blazor.Cms.Infrastructure.Events.Handlers;

/// <summary>
/// Handles ContentStatusChangedEvent
/// </summary>
public class ContentStatusChangedEventHandler : IDomainEventHandler<ContentStatusChangedEvent>
{
    private readonly ILogger<ContentStatusChangedEventHandler> _logger;

    public ContentStatusChangedEventHandler(ILogger<ContentStatusChangedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task HandleAsync(ContentStatusChangedEvent domainEvent)
    {
        _logger.LogInformation(
            "Content item status changed: ID {ContentId}, Title: {Title}, Status: {PreviousStatus} -> {NewStatus}, Provider: {ProviderId}, Time: {OccurredOn}",
            domainEvent.ContentId,
            domainEvent.Title,
            domainEvent.PreviousStatus,
            domainEvent.NewStatus,
            domainEvent.ProviderId,
            domainEvent.OccurredOn);

        return Task.CompletedTask;
    }
}