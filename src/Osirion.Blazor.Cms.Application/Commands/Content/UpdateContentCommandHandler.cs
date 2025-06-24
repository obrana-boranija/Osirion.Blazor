using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Application.Commands.Content;

/// <summary>
/// Handler for UpdateContentCommand
/// </summary>
public class UpdateContentCommandHandler : ICommandHandler<UpdateContentCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateContentCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateContentCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWorkFactory"></param>
    /// <param name="logger"></param>
    public UpdateContentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateContentCommandHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateContentCommand to update existing content in the repository.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task HandleAsync(UpdateContentCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating content: {Id}", command.Id);

        // Create UnitOfWork for the specified provider or default
        using var unitOfWork = command.ProviderId is not null
            ? _unitOfWorkFactory.Create(command.ProviderId)
            : _unitOfWorkFactory.CreateForDefaultProvider();

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get existing content
            var existingContent = await unitOfWork.ContentRepository.GetByIdAsync(command.Id, cancellationToken);
            if (existingContent is null)
            {
                throw new ContentItemNotFoundException(command.Id, unitOfWork.ProviderId);
            }

            // Update properties
            existingContent.SetTitle(command.Title);
            existingContent.SetContent(command.Content);

            if (!string.IsNullOrWhiteSpace(command.Path) && existingContent.Path != command.Path)
                existingContent.SetPath(command.Path);

            if (!string.IsNullOrWhiteSpace(command.Author))
                existingContent.SetAuthor(command.Author);

            if (!string.IsNullOrWhiteSpace(command.Description))
                existingContent.SetDescription(command.Description);

            if (!string.IsNullOrWhiteSpace(command.Slug))
                existingContent.SetSlug(command.Slug);

            if (!string.IsNullOrWhiteSpace(command.Locale))
                existingContent.SetLocale(command.Locale);

            if (!string.IsNullOrWhiteSpace(command.ContentId))
                existingContent.SetContentId(command.ContentId);

            existingContent.SetFeatured(command.IsFeatured);

            // Update provider-specific ID if provided
            if (!string.IsNullOrWhiteSpace(command.ProviderSpecificId))
                existingContent.SetProviderSpecificId(command.ProviderSpecificId);

            // Update tags and categories
            existingContent.ClearTags();
            foreach (var tag in command.Tags)
                existingContent.AddTag(tag);

            existingContent.ClearCategories();
            foreach (var category in command.Categories)
                existingContent.AddCategory(category);

            // Save the updated content
            var commitMessage = command.CommitMessage ?? $"Update content: {command.Title}";
            var savedContent = await unitOfWork.ContentRepository.SaveWithCommitMessageAsync(
                existingContent,
                commitMessage,
                cancellationToken);

            // Commit the transaction
            await unitOfWork.CommitAsync(cancellationToken);

            // Log success
            _logger.LogInformation("Content updated successfully: {Id}", savedContent.Id);

            // Domain event would go here in a real application
        }
        catch (ContentItemNotFoundException)
        {
            // Rethrow not found exceptions
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating content: {Id}", command.Id);

            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}