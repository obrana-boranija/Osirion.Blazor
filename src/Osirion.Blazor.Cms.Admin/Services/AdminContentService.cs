using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Application.Queries.Content;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Admin.Services;

/// <summary>
/// Service for managing content in the admin interface
/// </summary>
public class AdminContentService : IAdminContentService
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<AdminContentService> _logger;

    public AdminContentService(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        CmsEventMediator eventMediator,
        ILogger<AdminContentService> logger)
    {
        _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        _queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        _eventMediator = eventMediator ?? throw new ArgumentNullException(nameof(eventMediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets content by ID
    /// </summary>
    public async Task<ContentItem?> GetContentByIdAsync(
        string id,
        string? providerId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting content by ID: {Id}", id);

            var query = new GetContentByIdQuery
            {
                Id = id,
                ProviderId = providerId
            };

            var result = await _queryDispatcher.DispatchAsync<GetContentByIdQuery, ContentItem?>(query, cancellationToken);

            if (result is not null)
            {
                _logger.LogInformation("Content found with ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Content not found with ID: {Id}", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content by ID: {Id}", id);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to load content: {ex.Message}", ex));
            throw;
        }
    }

    /// <summary>
    /// Searches for content based on query parameters
    /// </summary>
    public async Task<IReadOnlyList<ContentItem>> SearchContentAsync(
        ContentQuery searchQuery,
        string? providerId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching content with query: {@Query}", searchQuery);

            var query = new SearchContentQuery
            {
                Query = searchQuery,
                ProviderId = providerId
            };

            var result = await _queryDispatcher.DispatchAsync<SearchContentQuery, IReadOnlyList<ContentItem>>(query, cancellationToken);

            _logger.LogInformation("Found {Count} content items matching query", result?.Count ?? 0);

            return result ?? Array.Empty<ContentItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching content: {@Query}", searchQuery);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to search content: {ex.Message}", ex));
            throw;
        }
    }

    /// <summary>
    /// Creates new content
    /// </summary>
    public async Task<ContentItem?> CreateContentAsync(
    string title,
    string content,
    string path,
    string? author = null,
    string? description = null,
    string? slug = null,
    List<string>? tags = null,
    List<string>? categories = null,
    bool isFeatured = false,
    string? providerId = null,
    string? commitMessage = null,
    CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating content with title: {Title}, path: {Path}", title, path);

            var command = new CreateContentCommand
            {
                Title = title,
                Content = content,
                Path = path,
                Author = author,
                Description = description,
                Slug = slug,
                Tags = tags ?? new List<string>(),
                Categories = categories ?? new List<string>(),
                IsFeatured = isFeatured,
                ProviderId = providerId,
                CommitMessage = commitMessage ?? $"Create {Path.GetFileName(path)}"
            };

            // Adjusted to remove the second type argument and handle the result without relying on CreateContentResult
            await _commandDispatcher.DispatchAsync(command, cancellationToken);

            _logger.LogInformation("Content created successfully with title: {Title}, path: {Path}", title, path);
            _eventMediator.Publish(new ContentSavedEvent(path));
            _eventMediator.Publish(new StatusNotificationEvent($"Content created successfully", StatusType.Success));

            // Get and return the created content
            return await GetContentByIdAsync(command.ContentId ?? string.Empty, providerId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating content with title: {Title}, path: {Path}", title, path);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to create content: {ex.Message}", ex));
            throw;
        }
    }

    public async Task<ContentItem?> UpdateContentAsync(
        string id,
        string title,
        string content,
        string path,
        string? author = null,
        string? description = null,
        string? slug = null,
        List<string>? tags = null,
        List<string>? categories = null,
        bool isFeatured = false,
        string? providerId = null,
        string? commitMessage = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating content with ID: {Id}, title: {Title}", id, title);

            var command = new UpdateContentCommand
            {
                Id = id,
                Title = title,
                Content = content,
                Path = path,
                Author = author,
                Description = description,
                Slug = slug,
                Tags = tags ?? new List<string>(),
                Categories = categories ?? new List<string>(),
                IsFeatured = isFeatured,
                ProviderId = providerId,
                CommitMessage = commitMessage ?? $"Update {Path.GetFileName(path)}"
            };

            await _commandDispatcher.DispatchAsync(command, cancellationToken);

            _logger.LogInformation("Content updated successfully with ID: {Id}", id);
            _eventMediator.Publish(new ContentSavedEvent(path));
            _eventMediator.Publish(new StatusNotificationEvent($"Content updated successfully", StatusType.Success));

            // Get and return the updated content
            return await GetContentByIdAsync(id, providerId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating content with ID: {Id}, title: {Title}", id, title);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to update content: {ex.Message}", ex));
            throw;
        }
    }

    /// <summary>
    /// Deletes content by ID
    /// </summary>
    public async Task DeleteContentAsync(
        string id,
        string? providerId = null,
        string? commitMessage = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting content with ID: {Id}", id);

            // Get the content first to know the path for event publishing
            var content = await GetContentByIdAsync(id, providerId, cancellationToken);

            var command = new DeleteContentCommand
            {
                Id = id,
                ProviderId = providerId,
                CommitMessage = commitMessage ?? $"Delete content {id}"
            };

            await _commandDispatcher.DispatchAsync(command, cancellationToken);

            _logger.LogInformation("Content with ID: {Id} deleted successfully", id);

            if (content is not null)
            {
                _eventMediator.Publish(new ContentDeletedEvent(content.Path));
            }

            _eventMediator.Publish(new StatusNotificationEvent($"Content deleted successfully", StatusType.Success));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting content with ID: {Id}", id);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to delete content: {ex.Message}", ex));
            throw;
        }
    }
}