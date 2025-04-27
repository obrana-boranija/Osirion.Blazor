using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Cms.Infrastructure.Events.Handlers;

/// <summary>
/// Handles DirectoryCreatedEvent by logging information and potentially triggering other actions
/// </summary>
public class DirectoryCreatedEventHandler : Domain.Events.IDomainEventHandler<Domain.Events.DirectoryCreatedEvent>
{
    private readonly ILogger<DirectoryCreatedEventHandler> _logger;

    public DirectoryCreatedEventHandler(ILogger<DirectoryCreatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task HandleAsync(Domain.Events.DirectoryCreatedEvent domainEvent)
    {
        _logger.LogInformation(
            "Directory created: ID {DirectoryId}, Name: {Name}, Path: {Path}, Provider: {ProviderId}, Time: {OccurredOn}",
            domainEvent.DirectoryId,
            domainEvent.Name,
            domainEvent.Path,
            domainEvent.ProviderId,
            domainEvent.OccurredOn);

        // Here you could implement additional actions:
        // - Update navigation structures
        // - Create additional metadata files
        // - Update access control lists
        // - Trigger workflows

        return Task.CompletedTask;
    }
}