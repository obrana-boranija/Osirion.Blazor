// src/Osirion.Blazor.Cms.Application/Commands/Directory/MoveDirectoryCommandHandler.cs

using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Application.Commands.Directory;

/// <summary>
/// Handler for MoveDirectoryCommand
/// </summary>
public class MoveDirectoryCommandHandler : ICommandHandler<MoveDirectoryCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<MoveDirectoryCommandHandler> _logger;

    public MoveDirectoryCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDomainEventDispatcher eventDispatcher,
        ILogger<MoveDirectoryCommandHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(MoveDirectoryCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Moving directory: {Id} to parent: {NewParentId}",
            command.Id, command.NewParentId ?? "root");

        // Create UnitOfWork for the specified provider or default
        using var unitOfWork = command.ProviderId != null
            ? _unitOfWorkFactory.Create(command.ProviderId)
            : _unitOfWorkFactory.CreateForDefaultProvider();

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Move the directory
            var movedDirectory = await unitOfWork.DirectoryRepository.MoveAsync(
                command.Id,
                command.NewParentId,
                cancellationToken);

            // Commit the transaction
            await unitOfWork.CommitAsync(cancellationToken);

            // Log success
            _logger.LogInformation("Directory moved successfully: {Id} to path: {Path}",
                movedDirectory.Id, movedDirectory.Path);

            // Dispatch domain event for directory updated
            await _eventDispatcher.DispatchAsync(new DirectoryUpdatedEvent(
                movedDirectory.Id,
                movedDirectory.Name,
                movedDirectory.Path,
                unitOfWork.ProviderId));
        }
        catch (DirectoryNotFoundException)
        {
            // Rethrow not found exceptions
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving directory: {Id}", command.Id);

            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}