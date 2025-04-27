using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;

namespace Osirion.Blazor.Cms.Infrastructure.Events.Handlers;

/// <summary>
/// Handles DirectoryDeletedEvent
/// </summary>
public class DirectoryDeletedEventHandler : IDomainEventHandler<DirectoryDeletedEvent>
{
    private readonly ILogger<DirectoryDeletedEventHandler> _logger;

    public DirectoryDeletedEventHandler(ILogger<DirectoryDeletedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task HandleAsync(DirectoryDeletedEvent domainEvent)
    {
        _logger.LogInformation(
            "Directory deleted: ID {DirectoryId}, Path: {Path}, Recursive: {Recursive}, Provider: {ProviderId}, Time: {OccurredOn}",
            domainEvent.DirectoryId,
            domainEvent.Path,
            domainEvent.Recursive,
            domainEvent.ProviderId,
            domainEvent.OccurredOn);

        return Task.CompletedTask;
    }
}