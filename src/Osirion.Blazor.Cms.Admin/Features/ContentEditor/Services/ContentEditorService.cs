﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;

/// <summary>
/// Service for editing content in the admin interface
/// </summary>
public class ContentEditorService : IContentEditorService
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly IAdminContentService _contentService;
    private readonly IEventPublisher _eventPublisher;
    private readonly CmsAdminOptions _options;
    private readonly ILogger<ContentEditorService> _logger;

    public ContentEditorService(
        IContentRepositoryAdapter repositoryAdapter,
        IAdminContentService contentService,
        IEventPublisher eventPublisher,
        IOptions<CmsAdminOptions> options,
        ILogger<ContentEditorService> logger)
    {
        _repositoryAdapter = repositoryAdapter ?? throw new ArgumentNullException(nameof(repositoryAdapter));
        _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a blog post by path
    /// </summary>
    public async Task<BlogPost> GetBlogPostAsync(string path)
    {
        try
        {
            _logger.LogInformation("Fetching blog post: {Path}", path);

            // First try to get from the repository adapter directly
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

    /// <summary>
    /// Saves a blog post
    /// </summary>
    public async Task<GitHubFileCommitResponse> SaveBlogPostAsync(BlogPost post, string commitMessage)
    {
        try
        {
            _logger.LogInformation("Saving blog post: {Path}", post.FilePath);

            // Validate the post if front matter validation is enabled
            if (_options.ContentRules.EnforceFrontMatterValidation)
            {
                ValidateBlogPost(post);
            }

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

    /// <summary>
    /// Deletes a blog post
    /// </summary>
    public async Task<GitHubFileCommitResponse> DeleteBlogPostAsync(string path, string sha)
    {
        if (!_options.ContentRules.AllowFileDeletion)
        {
            var message = "File deletion is not allowed by content rules";
            _logger.LogWarning(message);
            _eventPublisher.Publish(new ErrorOccurredEvent(message));
            throw new InvalidOperationException(message);
        }

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

    /// <summary>
    /// Creates a new blog post with default content
    /// </summary>
    public BlogPost CreateNewBlogPost(string path = "", string title = "New Post")
    {
        _logger.LogInformation("Creating new blog post with title: {Title}", title);

        var metadata = FrontMatter.Create(
            title,
            "Enter description here...",
            DateTime.Now);

        var content = $"## {title}\n\nStart writing your content here...";

        var fileName = GenerateFileNameFromTitle(title);

        var filePath = string.IsNullOrEmpty(path)
            ? fileName
            : $"{path.TrimEnd('/')}/{fileName}";

        return new BlogPost
        {
            Metadata = metadata,
            Content = content,
            FilePath = filePath
        };
    }

    /// <summary>
    /// Generates a file name from a title
    /// </summary>
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

    /// <summary>
    /// Converts a ContentItem to a BlogPost
    /// </summary>
    public BlogPost ConvertToBlogPost(ContentItem item)
    {
        var frontMatter = new FrontMatter
        {
            Title = item.Title,
            Description = item.Description,
            Author = item.Author,
            Date = item.DateCreated.ToString("yyyy-MM-dd"),
            Tags = item.Tags.ToList(),
            Categories = item.Categories.ToList(),
            Slug = item.Slug,
            IsFeatured = item.IsFeatured,
            Published = item.IsPublished
        };

        return new BlogPost
        {
            Metadata = frontMatter,
            Content = item.Content,
            FilePath = item.Path,
            Sha = item.Sha
        };
    }

    /// <summary>
    /// Validates a blog post
    /// </summary>
    private void ValidateBlogPost(BlogPost post)
    {
        // Check for required front matter fields
        foreach (var field in _options.ContentRules.RequiredFrontMatterFields)
        {
            var propertyInfo = typeof(FrontMatter).GetProperty(field, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (propertyInfo == null)
            {
                continue;
            }

            var value = propertyInfo.GetValue(post.Metadata);

            if (value == null || (value is string strValue && string.IsNullOrWhiteSpace(strValue)))
            {
                throw new ValidationException($"Required field '{field}' is missing or empty in front matter");
            }
        }

        // Check for content
        if (string.IsNullOrWhiteSpace(post.Content))
        {
            throw new ValidationException("Content cannot be empty");
        }

        // Check file extension
        var extension = Path.GetExtension(post.FilePath);

        if (!_options.ContentRules.AllowedFileExtensions.Contains(extension.ToLowerInvariant()))
        {
            throw new ValidationException($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", _options.ContentRules.AllowedFileExtensions)}");
        }

        // Auto-generate slug if enabled and not provided
        if (_options.ContentRules.AutoGenerateSlugs && string.IsNullOrWhiteSpace(post.Metadata.Slug))
        {
            post.Metadata.Slug = GenerateSlugFromTitle(post.Metadata.Title);
        }
    }

    /// <summary>
    /// Generates a slug from a title
    /// </summary>
    private string GenerateSlugFromTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return "post";
        }

        // Similar to file name generation but without extension
        var slug = Regex.Replace(title.ToLowerInvariant(), @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-{2,}", "-");
        slug = slug.Trim('-');

        return slug;
    }
}

/// <summary>
/// Exception thrown when blog post validation fails
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}