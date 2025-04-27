using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Application.Commands.Content;

/// <summary>
/// Handler for CreateContentCommand
/// </summary>
public class CreateContentCommandHandler : ICommandHandler<CreateContentCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<CreateContentCommandHandler> _logger;

    public CreateContentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<CreateContentCommandHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task HandleAsync(CreateContentCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating content with title: {Title}", command.Title);

        // Create UnitOfWork for the specified provider or default
        using var unitOfWork = command.ProviderId != null
            ? _unitOfWorkFactory.Create(command.ProviderId)
            : _unitOfWorkFactory.CreateForDefaultProvider();

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Create the content entity
            var contentItem = ContentItem.Create(
                id: Guid.NewGuid().ToString("N"),
                title: command.Title,
                content: command.Content,
                path: command.Path,
                providerId: unitOfWork.ProviderId);

            // Set additional properties
            if (!string.IsNullOrEmpty(command.Author))
                contentItem.SetAuthor(command.Author);

            if (!string.IsNullOrEmpty(command.Description))
                contentItem.SetDescription(command.Description);

            if (!string.IsNullOrEmpty(command.Slug))
                contentItem.SetSlug(command.Slug);

            if (!string.IsNullOrEmpty(command.Locale))
                contentItem.SetLocale(command.Locale);

            if (!string.IsNullOrEmpty(command.ContentId))
                contentItem.SetContentId(command.ContentId);

            contentItem.SetFeatured(command.IsFeatured);

            // Add tags and categories
            foreach (var tag in command.Tags)
                contentItem.AddTag(tag);

            foreach (var category in command.Categories)
                contentItem.AddCategory(category);

            // Save the content
            var commitMessage = command.CommitMessage ?? $"Create content: {command.Title}";
            var savedContent = await unitOfWork.ContentRepository.SaveWithCommitMessageAsync(
                contentItem,
                commitMessage,
                cancellationToken);

            // Commit the transaction
            await unitOfWork.CommitAsync(cancellationToken);

            // Log success
            _logger.LogInformation("Content created successfully: {Id}", savedContent.Id);

            // Note: In a real application, you would probably want to publish a domain event here
            // For example:
            // await _domainEventDispatcher.DispatchAsync(new ContentCreatedEvent(savedContent.Id, savedContent.Title, savedContent.Path, unitOfWork.ProviderId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating content: {Title}", command.Title);

            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}