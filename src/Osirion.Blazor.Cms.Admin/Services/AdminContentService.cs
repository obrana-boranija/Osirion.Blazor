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

    public AdminContentService(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    public async Task<ContentItem?> GetContentByIdAsync(string id, string? providerId = null, CancellationToken cancellationToken = default)
    {
        var query = new GetContentByIdQuery
        {
            Id = id,
            ProviderId = providerId
        };

        return await _queryDispatcher.DispatchAsync<GetContentByIdQuery, ContentItem?>(query, cancellationToken);
    }

    public async Task<IReadOnlyList<ContentItem>> SearchContentAsync(ContentQuery searchQuery, string? providerId = null, CancellationToken cancellationToken = default)
    {
        var query = new SearchContentQuery
        {
            Query = searchQuery,
            ProviderId = providerId
        };

        return await _queryDispatcher.DispatchAsync<SearchContentQuery, IReadOnlyList<ContentItem>>(query, cancellationToken);
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

        await _commandDispatcher.DispatchAsync(command, cancellationToken);

        // Since we don't have the ID yet, we'll need to search for the content by path
        var searchQuery = new GetContentByPathQuery
        {
            Path = path,
            ProviderId = providerId
        };

        return await _queryDispatcher.DispatchAsync<GetContentByPathQuery, ContentItem?>(searchQuery, cancellationToken);
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

        await _commandDispatcher.DispatchAsync(command, cancellationToken);

        // Get the updated content
        var query = new GetContentByIdQuery
        {
            Id = id,
            ProviderId = providerId
        };

        return await _queryDispatcher.DispatchAsync<GetContentByIdQuery, ContentItem?>(query, cancellationToken);
    }

    public async Task DeleteContentAsync(
        string id,
        string? providerId = null,
        string? commitMessage = null,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteContentCommand
        {
            Id = id,
            ProviderId = providerId,
            CommitMessage = commitMessage
        };

        await _commandDispatcher.DispatchAsync(command, cancellationToken);
    }
}