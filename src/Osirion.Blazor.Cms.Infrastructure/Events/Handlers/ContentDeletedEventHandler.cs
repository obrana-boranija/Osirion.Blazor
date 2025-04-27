using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osirion.Blazor.Cms.Infrastructure.Events.Handlers;

/// <summary>
/// Handles ContentDeletedEvent
/// </summary>
public class ContentDeletedEventHandler : IDomainEventHandler<ContentDeletedEvent>
{
    private readonly ILogger<ContentDeletedEventHandler> _logger;

    public ContentDeletedEventHandler(ILogger<ContentDeletedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task HandleAsync(ContentDeletedEvent domainEvent)
    {
        _logger.LogInformation(
            "Content item deleted: ID {ContentId}, Path: {Path}, Provider: {ProviderId}, Time: {OccurredOn}",
            domainEvent.ContentId,
            domainEvent.Path,
            domainEvent.ProviderId,
            domainEvent.OccurredOn);

        return Task.CompletedTask;
    }
}