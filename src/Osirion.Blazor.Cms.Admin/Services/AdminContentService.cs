using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Interfaces;
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
    private readonly ILogger<AdminContentService> _logger;

    public AdminContentService(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILogger<AdminContentService> logger)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _logger = logger;
    }

    public async Task<ContentItem?> GetContentByIdAsync(string id, string? providerId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting content by ID: {Id}", id);

            var query = new GetContentByIdQuery
            {
                Id = id,
                ProviderId = providerId
            };

            // Explicitly cast the query to IQuery<ContentItem> to satisfy the nullability constraint
            var result = await _queryDispatcher.DispatchAsync<IQuery<ContentItem>, ContentItem>((IQuery<ContentItem>)query, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content by ID: {Id}", id);
            throw;
        }
    }

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

            // Fix CS0311 and CS0246 by ensuring the correct type is used
            var result = await _queryDispatcher.DispatchAsync<SearchContentQuery, IReadOnlyList<ContentItem>>(query, cancellationToken);
            return result ?? Array.Empty<ContentItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching content: {@Query}", searchQuery);
            throw;
        }
    }

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
                CommitMessage = commitMessage
            };

            // Fix CS0305 by removing the second type argument
            await _commandDispatcher.DispatchAsync(command, cancellationToken);

            // Assuming the content ID is generated and returned in the command object
            if (!string.IsNullOrEmpty(command.ContentId))
            {
                // Get the created content by ID
                return await GetContentByIdAsync(command.ContentId, providerId, cancellationToken);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating content with title: {Title}, path: {Path}", title, path);
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
                CommitMessage = commitMessage
            };

            // Fix CS0305 by removing the second type argument
            await _commandDispatcher.DispatchAsync(command, cancellationToken);

            // Assuming the content ID is generated and returned in the command object
            return await GetContentByIdAsync(id, providerId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating content with ID: {Id}, title: {Title}", id, title);
            throw;
        }
    }

    public async Task DeleteContentAsync(
        string id,
        string? providerId = null,
        string? commitMessage = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting content with ID: {Id}", id);

            var command = new DeleteContentCommand
            {
                Id = id,
                ProviderId = providerId,
                CommitMessage = commitMessage
            };

            await _commandDispatcher.DispatchAsync(command, cancellationToken);

            _logger.LogInformation("Content with ID: {Id} deleted successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting content with ID: {Id}", id);
            throw;
        }
    }
}
