namespace Osirion.Blazor.Cms.Application.Commands.Directory;

using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Handler for CreateDirectoryCommand
/// </summary>
public class CreateDirectoryCommandHandler : ICommandHandler<CreateDirectoryCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<CreateDirectoryCommandHandler> _logger;

    public CreateDirectoryCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDomainEventDispatcher eventDispatcher,
        ILogger<CreateDirectoryCommandHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(CreateDirectoryCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating directory: {Name}", command.Name);

        // Create UnitOfWork for the specified provider or default
        using var unitOfWork = command.ProviderId != null
            ? _unitOfWorkFactory.Create(command.ProviderId)
            : _unitOfWorkFactory.CreateForDefaultProvider();

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get parent directory if specified
            DirectoryItem? parentDirectory = null;
            if (!string.IsNullOrEmpty(command.ParentId))
            {
                parentDirectory = await unitOfWork.DirectoryRepository.GetByIdAsync(
                    command.ParentId,
                    cancellationToken);

                if (parentDirectory == null)
                {
                    throw new DirectoryNotFoundException(command.ParentId);
                }
            }

            // Determine full path
            string path;
            if (parentDirectory != null)
            {
                path = Path.Combine(parentDirectory.Path, command.Name).Replace('\\', '/');
            }
            else
            {
                path = command.Path.Replace('\\', '/');

                if (string.IsNullOrEmpty(path))
                {
                    path = command.Name;
                }
            }

            // Create the directory entity
            var directoryId = path.GetHashCode().ToString("x");
            var directory = DirectoryItem.Create(
                directoryId,
                path,
                command.Name,
                unitOfWork.ProviderId);

            // Set parent if available
            directory.SetParent(parentDirectory);

            // Set additional properties
            if (!string.IsNullOrEmpty(command.Description))
                directory.SetDescription(command.Description);

            if (!string.IsNullOrEmpty(command.Locale))
                directory.SetLocale(command.Locale);

            directory.SetOrder(command.Order);

            // Save the directory
            var savedDirectory = await unitOfWork.DirectoryRepository.SaveAsync(directory, cancellationToken);

            // Commit the transaction
            await unitOfWork.CommitAsync(cancellationToken);

            // Dispatch domain event
            await _eventDispatcher.DispatchAsync(new DirectoryCreatedEvent(
                savedDirectory.Id,
                savedDirectory.Name,
                savedDirectory.Path,
                unitOfWork.ProviderId));

            _logger.LogInformation("Directory created successfully: {Id}, {Path}", savedDirectory.Id, savedDirectory.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating directory: {Name}", command.Name);

            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}