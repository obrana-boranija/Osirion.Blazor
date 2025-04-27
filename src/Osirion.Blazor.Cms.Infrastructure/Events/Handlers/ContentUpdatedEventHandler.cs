using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;

namespace Osirion.Blazor.Cms.Infrastructure.Events.Handlers;

/// <summary>
/// Handles ContentUpdatedEvent
/// </summary>
public class ContentUpdatedEventHandler : IDomainEventHandler<ContentUpdatedEvent>
{
    private readonly ILogger<ContentUpdatedEventHandler> _logger;

    public ContentUpdatedEventHandler(ILogger<ContentUpdatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task HandleAsync(ContentUpdatedEvent domainEvent)
    {
        _logger.LogInformation(
            "Content item updated: ID {ContentId}, Title: {Title}, Path: {Path}, Provider: {ProviderId}, Time: {OccurredOn}",
            domainEvent.ContentId,
            domainEvent.Title,
            domainEvent.Path,
            domainEvent.ProviderId,
            domainEvent.OccurredOn);

        return Task.CompletedTask;
    }
}