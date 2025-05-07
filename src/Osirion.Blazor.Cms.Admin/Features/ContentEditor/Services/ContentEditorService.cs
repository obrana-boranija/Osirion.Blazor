using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Shared.Events;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;

public class ContentEditorService : IContentEditorService
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<ContentEditorService> _logger;

    public ContentEditorService(
        IContentRepositoryAdapter repositoryAdapter,
        IEventPublisher eventPublisher,
        ILogger<ContentEditorService> logger)
    {
        _repositoryAdapter = repositoryAdapter;
        _eventPublisher = eventPublisher;
        _logger = logger;
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
            _eventPublisher.Publish(new ErrorOccurredEvent($"Failed to load file: {path}", ex));
            throw;
        }
    }

    public async Task<GitHubFileCommitResponse> SaveBlogPostAsync(BlogPost post, string commitMessage)
    {
        try
        {
            _logger.LogInformation("Saving blog post: {Path}", post.FilePath);
            var content = post.ToMarkdown();
            var message = string.IsNullOrEmpty(commitMessage)
                ? $"Update {Path.GetFileName(post.FilePath)}"
                : commitMessage;

            var response = await _repositoryAdapter.SaveContentAsync(
                post.FilePath,
                content,
                message,
                post.Sha);

            _logger.LogInformation("Blog post saved successfully: {Path}", post.FilePath);
            _eventPublisher.Publish(new ContentSavedEvent(post.FilePath));
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving blog post: {Path}", post.FilePath);
            _eventPublisher.Publish(new ErrorOccurredEvent($"Failed to save file: {post.FilePath}", ex));
            throw;
        }
    }

    public async Task<GitHubFileCommitResponse> DeleteBlogPostAsync(string path, string sha)
    {
        try
        {
            _logger.LogInformation("Deleting blog post: {Path}", path);
            var message = $"Delete {Path.GetFileName(path)}";
            var response = await _repositoryAdapter.DeleteFileAsync(path, message, sha);

            _logger.LogInformation("Blog post deleted successfully: {Path}", path);
            _eventPublisher.Publish(new ContentDeletedEvent(path));
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blog post: {Path}", path);
            _eventPublisher.Publish(new ErrorOccurredEvent($"Failed to delete file: {path}", ex));
            throw;
        }
    }

    public BlogPost CreateNewBlogPost(string path = "", string title = "New Post")
    {
        _logger.LogInformation("Creating new blog post with title: {Title}", title);

        var metadata = FrontMatter.Create(
            title,
            "Enter description here...",
            DateTime.Now);

        var content = $"## {title}\n\nStart writing your content here...";

        var filePath = string.IsNullOrEmpty(path)
            ? "new-post.md"
            : $"{path.TrimEnd('/')}/new-post.md";

        return new BlogPost
        {
            Metadata = metadata,
            Content = content,
            FilePath = filePath
        };
    }

    public string GenerateFileNameFromTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return "new-post.md";
        }

        // Convert to kebab case (lowercase with hyphens)
        var fileName = Regex.Replace(title.ToLowerInvariant(), @"[^a-z0-9\s-]", "");
        fileName = Regex.Replace(fileName, @"\s+", "-");
        fileName = Regex.Replace(fileName, @"-{2,}", "-");
        fileName = fileName.Trim('-');

        // Ensure it ends with .md
        if (!fileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".md";
        }

        return fileName;
    }
}