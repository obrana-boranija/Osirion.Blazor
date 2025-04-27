using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Application.Commands.Directory;

/// <summary>
/// Handler for DeleteDirectoryCommand
/// </summary>
public class DeleteDirectoryCommandHandler : ICommandHandler<DeleteDirectoryCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<DeleteDirectoryCommandHandler> _logger;

    public DeleteDirectoryCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDomainEventDispatcher eventDispatcher,
        ILogger<DeleteDirectoryCommandHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(DeleteDirectoryCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting directory: {Id}, Recursive: {Recursive}", command.Id, command.Recursive);

        // Create UnitOfWork for the specified provider or default
        using var unitOfWork = command.ProviderId != null
            ? _unitOfWorkFactory.Create(command.ProviderId)
            : _unitOfWorkFactory.CreateForDefaultProvider();

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // First, verify the directory exists to get its path for event publishing later
            var existingDirectory = await unitOfWork.DirectoryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (existingDirectory == null)
            {
                throw new DirectoryNotFoundException(command.Id);
            }

            var directoryPath = existingDirectory.Path;

            // Delete the directory (recursively if specified)
            var commitMessage = command.CommitMessage ?? $"Delete directory: {command.Id}";
            await unitOfWork.DirectoryRepository.DeleteRecursiveAsync(
                command.Id,
                commitMessage,
                cancellationToken);

            // Commit the transaction
            await unitOfWork.CommitAsync(cancellationToken);

            // Log success
            _logger.LogInformation("Directory deleted successfully: {Id}", command.Id);

            // Dispatch domain event
            await _eventDispatcher.DispatchAsync(new DirectoryDeletedEvent(
                command.Id,
                directoryPath,
                unitOfWork.ProviderId,
                command.Recursive));
        }
        catch (DirectoryNotFoundException)
        {
            // Rethrow not found exceptions
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting directory: {Id}", command.Id);

            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}