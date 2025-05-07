using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Shared.Events;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;

public class ContentEditorService
{
    private readonly IContentRepository _contentRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<ContentEditorService> _logger;

    public ContentEditorService(
        IContentRepository contentRepository,
        IEventPublisher eventPublisher,
        ILogger<ContentEditorService> logger)
    {
        _contentRepository = contentRepository;
        _eventPublisher = eventPublisher;
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

            var response = await _contentRepository.SaveContentAsync(
                post.FilePath,
                content,
                message,
                post.Sha);

            _logger.LogInformation("Content saved successfully: {Path}", post.FilePath);

            _eventPublisher.Publish(new ContentSavedEvent(post.FilePath));
            _eventPublisher.Publish(new StatusNotificationEvent(
                $"File saved successfully: {post.FilePath}", StatusType.Success));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving content: {Path}", post.FilePath);
            _eventPublisher.Publish(new ErrorOccurredEvent($"Failed to save file: {post.FilePath}", ex));
            throw new ContentEditorException($"Failed to save content: {ex.Message}", ex);
        }
    }

    public async Task<BlogPost> GetBlogPostAsync(string path)
    {
        try
        {
            _logger.LogInformation("Fetching blog post: {Path}", path);
            var blogPost = await _contentRepository.GetBlogPostAsync(path);
            _logger.LogInformation("Blog post fetched successfully: {Path}", path);
            return blogPost;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching blog post: {Path}", path);
            _eventPublisher.Publish(new ErrorOccurredEvent($"Failed to load file: {path}", ex));
            throw new ContentEditorException($"Failed to load content: {ex.Message}", ex);
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
            var response = await _contentRepository.DeleteFileAsync(path, message, sha);

            _logger.LogInformation("Blog post deleted successfully: {Path}", path);

            _eventPublisher.Publish(new ContentDeletedEvent(path));
            _eventPublisher.Publish(new StatusNotificationEvent(
                $"File deleted successfully: {path}", StatusType.Success));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blog post: {Path}", path);
            _eventPublisher.Publish(new ErrorOccurredEvent($"Failed to delete file: {path}", ex));
            throw new ContentEditorException($"Failed to delete content: {ex.Message}", ex);
        }
    }

    public string GenerateFileNameFromTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return "new-post";
        }

        // Convert to kebab case
        var fileName = Regex.Replace(title.ToLowerInvariant(), @"[^a-z0-9\s-]", "");
        fileName = Regex.Replace(fileName, @"\s+", "-");
        fileName = Regex.Replace(fileName, @"-{2,}", "-");
        fileName = fileName.Trim('-');

        // Ensure it ends with .md
        if (!fileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            fileName = $"{fileName}.md";
        }

        return fileName;
    }
}

// Custom exception for ContentEditor operations
public class ContentEditorException : Exception
{
    public ContentEditorException(string message) : base(message) { }
    public ContentEditorException(string message, Exception innerException) : base(message, innerException) { }
}