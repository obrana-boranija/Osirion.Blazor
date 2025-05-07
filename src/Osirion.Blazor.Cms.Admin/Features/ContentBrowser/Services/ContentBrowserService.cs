using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;

public class ContentBrowserService
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<ContentBrowserService> _logger;

    public ContentBrowserService(
        IContentRepositoryAdapter repositoryAdapter,
        CmsEventMediator eventMediator,
        ILogger<ContentBrowserService> logger)
    {
        _repositoryAdapter = repositoryAdapter;
        _eventMediator = eventMediator;
        _logger = logger;
    }

    public async Task<List<GitHubItem>> GetContentsAsync(string path)
    {
        try
        {
            _logger.LogInformation("Fetching contents for path: {Path}", path);
            var contents = await _repositoryAdapter.GetContentsAsync(path);
            _logger.LogInformation("Retrieved {Count} items for path: {Path}", contents.Count, path);
            return contents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching contents for path: {Path}", path);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to load contents for {path}", ex));
            throw;
        }
    }

    public async Task<BlogPost> GetBlogPostAsync(string path)
    {
        try
        {
            _logger.LogInformation("Fetching blog post: {Path}", path);
            var blogPost = await _repositoryAdapter.GetBlogPostAsync(path);
            _logger.LogInformation("Blog post fetched successfully: {Path}", path);
            return blogPost;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching blog post: {Path}", path);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to load file: {path}", ex));
            throw;
        }
    }

    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string sha)
    {
        try
        {
            _logger.LogInformation("Deleting file: {Path}", path);

            var message = $"Delete {Path.GetFileName(path)}";
            var response = await _repositoryAdapter.DeleteFileAsync(path, message, sha);

            _logger.LogInformation("File deleted successfully: {Path}", path);

            _eventMediator.Publish(new ContentDeletedEvent(path));
            _eventMediator.Publish(new StatusNotificationEvent(
                $"File deleted successfully: {path}", StatusType.Success));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Path}", path);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to delete file: {path}", ex));
            throw;
        }
    }

    public async Task<List<GitHubItem>> SearchFilesAsync(string query)
    {
        try
        {
            _logger.LogInformation("Searching files with query: {Query}", query);
            var results = await _repositoryAdapter.SearchFilesAsync(query);
            _logger.LogInformation("Found {Count} items matching query: {Query}", query);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files with query: {Query}", query);
            _eventMediator.Publish(new ErrorOccurredEvent(
                $"Failed to search files with query '{query}'", ex));

            throw;
        }
    }

    public async Task<List<GitHubItem>> GetDirectoryTreeAsync(string path)
    {
        try
        {
            _logger.LogInformation("Fetching directory tree for path: {Path}", path);

            // Get all items recursively
            var items = new List<GitHubItem>();
            await FetchDirectoryItemsRecursively(path, items);

            _logger.LogInformation("Retrieved {Count} items in directory tree for path: {Path}",
                items.Count, path);

            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching directory tree for path: {Path}", path);
            _eventMediator.Publish(new ErrorOccurredEvent(
                $"Failed to load directory tree for {path}", ex));

            throw;
        }
    }

    private async Task FetchDirectoryItemsRecursively(string path, List<GitHubItem> items)
    {
        var contents = await _repositoryAdapter.GetContentsAsync(path);

        // Add all items from this directory
        items.AddRange(contents);

        // Recursively fetch subdirectories
        foreach (var item in contents.Where(i => i.IsDirectory))
        {
            await FetchDirectoryItemsRecursively(item.Path, items);
        }
    }
}