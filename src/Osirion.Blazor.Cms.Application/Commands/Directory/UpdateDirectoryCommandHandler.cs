// src/Osirion.Blazor.Cms.Application/Commands/Directory/UpdateDirectoryCommandHandler.cs

using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Application.Commands.Directory;

/// <summary>
/// Handler for UpdateDirectoryCommand
/// </summary>
public class UpdateDirectoryCommandHandler : ICommandHandler<UpdateDirectoryCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<UpdateDirectoryCommandHandler> _logger;

    public UpdateDirectoryCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDomainEventDispatcher eventDispatcher,
        ILogger<UpdateDirectoryCommandHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(UpdateDirectoryCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating directory: {Id}", command.Id);

        // Create UnitOfWork for the specified provider or default
        using var unitOfWork = command.ProviderId != null
            ? _unitOfWorkFactory.Create(command.ProviderId)
            : _unitOfWorkFactory.CreateForDefaultProvider();

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get existing directory
            var existingDirectory = await unitOfWork.DirectoryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (existingDirectory == null)
            {
                throw new DirectoryNotFoundException(command.Id);
            }

            // Update properties
            existingDirectory.SetName(command.Name);

            if (!string.IsNullOrEmpty(command.Description))
                existingDirectory.SetDescription(command.Description);

            existingDirectory.SetOrder(command.Order);

            // Update provider-specific ID if provided
            if (!string.IsNullOrEmpty(command.ProviderSpecificId))
                existingDirectory.SetProviderSpecificId(command.ProviderSpecificId);

            // Save the updated directory
            var savedDirectory = await unitOfWork.DirectoryRepository.SaveAsync(existingDirectory, cancellationToken);

            // Commit the transaction
            await unitOfWork.CommitAsync(cancellationToken);

            // Dispatch domain event
            await _eventDispatcher.DispatchAsync(new DirectoryUpdatedEvent(
                savedDirectory.Id,
                savedDirectory.Name,
                savedDirectory.Path,
                unitOfWork.ProviderId));

            _logger.LogInformation("Directory updated successfully: {Id}", savedDirectory.Id);
        }
        catch (DirectoryNotFoundException)
        {
            // Rethrow not found exceptions
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating directory: {Id}", command.Id);

            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}