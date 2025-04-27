using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Application.Commands.Content;

/// <summary>
/// Handler for DeleteContentCommand
/// </summary>
public class DeleteContentCommandHandler : ICommandHandler<DeleteContentCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<DeleteContentCommandHandler> _logger;

    public DeleteContentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<DeleteContentCommandHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteContentCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting content: {Id}", command.Id);

        // Create UnitOfWork for the specified provider or default
        using var unitOfWork = command.ProviderId != null
            ? _unitOfWorkFactory.Create(command.ProviderId)
            : _unitOfWorkFactory.CreateForDefaultProvider();

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // First, verify the content exists to get its path for event publishing later
            var existingContent = await unitOfWork.ContentRepository.GetByIdAsync(command.Id, cancellationToken);
            if (existingContent == null)
            {
                throw new ContentItemNotFoundException(command.Id, unitOfWork.ProviderId);
            }

            var contentPath = existingContent.Path;

            // Delete the content
            var commitMessage = command.CommitMessage ?? $"Delete content: {command.Id}";
            await unitOfWork.ContentRepository.DeleteWithCommitMessageAsync(
                command.Id,
                commitMessage,
                cancellationToken);

            // Commit the transaction
            await unitOfWork.CommitAsync(cancellationToken);

            // Log success
            _logger.LogInformation("Content deleted successfully: {Id}", command.Id);

            // Domain event would go here in a real application
            // await _domainEventDispatcher.DispatchAsync(new ContentDeletedEvent(command.Id, contentPath, unitOfWork.ProviderId));
        }
        catch (ContentItemNotFoundException)
        {
            // Rethrow not found exceptions
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting content: {Id}", command.Id);

            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}