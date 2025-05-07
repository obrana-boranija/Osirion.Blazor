using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;

public class ContentEditorService
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<ContentEditorService> _logger;

    public ContentEditorService(
        IContentRepositoryAdapter repositoryAdapter,
        CmsEventMediator eventMediator,
        ILogger<ContentEditorService> logger)
    {
        _repositoryAdapter = repositoryAdapter;
        _eventMediator = eventMediator;
        _logger = logger;
    }

    public async Task<GitHubFileCommitResponse> SaveContentAsync(BlogPost post, string commitMessage)
    {
        try
        {
            _logger.LogInformation("Saving content: {Path}", post.FilePath);

            var content = post.ToMarkdown();
            var message = string.IsNullOrEmpty(commitMessage)
                ? $"Update {Path.GetFileName(post.FilePath)}"
                : commitMessage;

            var response = await _repositoryAdapter.SaveContentAsync(
                post.FilePath,
                content,
                message,
                post.Sha);

            _logger.LogInformation("Content saved successfully: {Path}", post.FilePath);

            _eventMediator.Publish(new ContentSavedEvent(post.FilePath));
            _eventMediator.Publish(new StatusNotificationEvent(
                $"File saved successfully: {post.FilePath}", StatusType.Success));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving content: {Path}", post.FilePath);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to save file: {post.FilePath}", ex));
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

    public BlogPost CreateNewBlogPost(string path = "", string title = "New Post")
    {
        _logger.LogInformation("Creating new blog post with title: {Title}", title);

        return new BlogPost
        {
            Metadata = FrontMatter.Create(
                title,
                "Enter description here",
                DateTime.Now),
            Content = "## New Post\n\nStart writing your content here...",
            FilePath = string.IsNullOrEmpty(path) ?
                "new-post.md" :
                $"{path}/new-post.md"
        };
    }

    public async Task<GitHubFileCommitResponse> DeleteBlogPostAsync(string path, string sha)
    {
        try
        {
            _logger.LogInformation("Deleting blog post: {Path}", path);

            var message = $"Delete {Path.GetFileName(path)}";
            var response = await _repositoryAdapter.DeleteFileAsync(path, message, sha);

            _logger.LogInformation("Blog post deleted successfully: {Path}", path);

            _eventMediator.Publish(new ContentDeletedEvent(path));
            _eventMediator.Publish(new StatusNotificationEvent(
                $"File deleted successfully: {path}", StatusType.Success));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blog post: {Path}", path);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to delete file: {path}", ex));
            throw;
        }
    }

    public string GenerateFileNameFromTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return "new-post";
        }

        // Convert to lowercase and remove invalid characters
        var fileName = new string(title.ToLowerInvariant()
            .Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-')
            .ToArray());

        // Replace spaces with dashes
        fileName = fileName.Replace(" ", "-");

        // Remove consecutive dashes
        while (fileName.Contains("--"))
        {
            fileName = fileName.Replace("--", "-");
        }

        // Ensure it ends with .md
        if (!fileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            fileName = $"{fileName}.md";
        }

        return fileName;
    }
}