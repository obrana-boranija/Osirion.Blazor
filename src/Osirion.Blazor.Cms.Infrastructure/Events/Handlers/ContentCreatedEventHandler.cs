using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;

namespace Osirion.Blazor.Cms.Infrastructure.Events.Handlers;

/// <summary>
/// Handles ContentCreatedEvent by logging information and potentially triggering other actions
/// </summary>
public class ContentCreatedEventHandler : IDomainEventHandler<ContentCreatedEvent>
{
    private readonly ILogger<ContentCreatedEventHandler> _logger;

    public ContentCreatedEventHandler(ILogger<ContentCreatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task HandleAsync(ContentCreatedEvent domainEvent)
    {
        _logger.LogInformation(
            "Content item created: ID {ContentId}, Title: {Title}, Path: {Path}, Provider: {ProviderId}, Time: {OccurredOn}",
            domainEvent.ContentId,
            domainEvent.Title,
            domainEvent.Path,
            domainEvent.ProviderId,
            domainEvent.OccurredOn);

        // Here we could implement additional actions:
        // - Send notifications
        // - Update indexes
        // - Generate cache entries
        // - Trigger workflows

        return Task.CompletedTask;
    }
}