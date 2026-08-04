using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;

namespace Osirion.Blazor.Cms.Infrastructure.Events.Handlers;

/// <summary>
/// Handles DirectoryUpdatedEvent
/// </summary>
public class DirectoryUpdatedEventHandler : IDomainEventHandler<DirectoryUpdatedEvent>
{
    private readonly ILogger<DirectoryUpdatedEventHandler> _logger;

    /// <summary>Performs the DirectoryUpdatedEventHandler operation.</summary>
    public DirectoryUpdatedEventHandler(ILogger<DirectoryUpdatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>Performs the Handle operation asynchronously.</summary>
    public Task HandleAsync(DirectoryUpdatedEvent domainEvent)
    {
        _logger.LogInformation(
            "Directory updated: ID {DirectoryId}, Name: {Name}, Path: {Path}, Provider: {ProviderId}, Time: {OccurredOn}",
            domainEvent.DirectoryId,
            domainEvent.Name,
            domainEvent.Path,
            domainEvent.ProviderId,
            domainEvent.OccurredOn);

        return Task.CompletedTask;
    }
}
