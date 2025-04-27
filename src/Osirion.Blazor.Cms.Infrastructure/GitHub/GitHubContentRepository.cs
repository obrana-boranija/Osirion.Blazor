using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub.Models;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Osirion.Blazor.Cms.Infrastructure.Options;
using Osirion.Blazor.Cms.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Repository implementation for GitHub content
/// </summary>
public class GitHubContentRepository : RepositoryBase<ContentItem, string>, IContentRepository
{
    private readonly IGitHubApiClient _apiClient;
    private readonly IMarkdownProcessor _markdownProcessor;
    private readonly GitHubOptions _options;
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    // In-memory cache for items
    private Dictionary<string, ContentItem>? _itemCache;
    private DateTime _cacheExpiration = DateTime.MinValue;

    public GitHubContentRepository(
        IGitHubApiClient apiClient,
        IMarkdownProcessor markdownProcessor,
        IOptions<GitHubOptions> options,
        ILogger<GitHubContentRepository> logger)
        : base(GetProviderId(options.Value), logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _markdownProcessor = markdownProcessor ?? throw new ArgumentNullException(nameof(markdownProcessor));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        // Configure API client
        _apiClient.SetRepository(_options.Owner, _options.Repository);
        _apiClient.SetBranch(_options.Branch);

        if (!string.IsNullOrEmpty(_options.ApiToken))
        {
            _apiClient.SetAccessToken(_options.ApiToken);
        }
    }

    private static string GetProviderId(GitHubOptions options)
    {
        return options.ProviderId ?? $"github-{options.Owner}-{options.Repository}";
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);
            return _itemCache?.Values.ToList() ?? new List<ContentItem>();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting all items");
            throw new ContentProviderException($"Failed to get all content items: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public override async Task<ContentItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (_itemCache != null && _itemCache.TryGetValue(id, out var item))
            {
                return item;
            }

            return null;
        }
        catch (Exception ex)
        {
            LogError(ex, "getting item by ID", id);
            throw new ContentProviderException($"Failed to get content item by ID: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<ContentItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            var normalizedPath = NormalizePath(path);
            return _itemCache?.Values.FirstOrDefault(c => NormalizePath(c.Path) == normalizedPath);
        }
        catch (Exception ex)
        {
            LogError(ex, "getting item by path", path);
            throw new ContentProviderException($"Failed to get content item by path: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<ContentItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            return _itemCache?.Values.FirstOrDefault(c => c.Url == url);
        }
        catch (Exception ex)
        {
            LogError(ex, "getting item by URL", url);
            throw new ContentProviderException($"Failed to get content item by URL: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>> FindByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (_itemCache == null)
                return new List<ContentItem>();

            var filteredItems = _itemCache.Values.AsQueryable();

            // Apply query filters
            if (!string.IsNullOrEmpty(query.Directory))
            {
                var normalizedDirectory = NormalizePath(query.Directory);
                filteredItems = filteredItems.Where(item =>
                    NormalizePath(GetDirectoryPath(item.Path)).StartsWith(normalizedDirectory, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(query.DirectoryId))
            {
                // This is a placeholder - we would need directory info to fully implement this
                // In practice, we would find the directory by ID then filter by its path
            }

            if (!string.IsNullOrEmpty(query.Category))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Categories.Any(c => c.Equals(query.Category, StringComparison.OrdinalIgnoreCase)));
            }

            if (query.Categories != null && query.Categories.Any())
            {
                filteredItems = filteredItems.Where(item =>
                    query.Categories.All(c =>
                        item.Categories.Any(itemCat =>
                            itemCat.Equals(c, StringComparison.OrdinalIgnoreCase))));
            }

            if (!string.IsNullOrEmpty(query.Tag))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Tags.Any(t => t.Equals(query.Tag, StringComparison.OrdinalIgnoreCase)));
            }

            if (query.Tags != null && query.Tags.Any())
            {
                filteredItems = filteredItems.Where(item =>
                    query.Tags.All(t =>
                        item.Tags.Any(itemTag =>
                            itemTag.Equals(t, StringComparison.OrdinalIgnoreCase))));
            }

            if (query.IsFeatured.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.IsFeatured == query.IsFeatured.Value);
            }

            if (!string.IsNullOrEmpty(query.Author))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Author.Equals(query.Author, StringComparison.OrdinalIgnoreCase));
            }

            if (query.Status.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.Status == query.Status.Value);
            }

            if (query.DateFrom.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.DateCreated >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.DateCreated <= query.DateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.SearchQuery))
            {
                var searchTerms = query.SearchQuery.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                filteredItems = filteredItems.Where(item =>
                    searchTerms.Any(term =>
                        item.Title.ToLower().Contains(term) ||
                        item.Description.ToLower().Contains(term) ||
                        item.Content.ToLower().Contains(term) ||
                        item.Categories.Any(c => c.ToLower().Contains(term)) ||
                        item.Tags.Any(t => t.ToLower().Contains(term))
                    )
                );
            }

            if (!string.IsNullOrEmpty(query.Locale))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Locale.Equals(query.Locale, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(query.LocalizationId))
            {
                filteredItems = filteredItems.Where(item =>
                    item.ContentId.Equals(query.LocalizationId, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(query.ProviderId) && query.ProviderId != ProviderId)
            {
                // No items match this provider
                return new List<ContentItem>();
            }

            if (query.IncludeIds != null && query.IncludeIds.Any())
            {
                filteredItems = filteredItems.Where(item =>
                    query.IncludeIds.Contains(item.Id));
            }

            if (query.ExcludeIds != null && query.ExcludeIds.Any())
            {
                filteredItems = filteredItems.Where(item =>
                    !query.ExcludeIds.Contains(item.Id));
            }

            // Apply sorting
            filteredItems = ApplySorting(filteredItems, query.SortBy, query.SortDirection);

            // Apply pagination
            if (query.Skip.HasValue)
            {
                filteredItems = filteredItems.Skip(query.Skip.Value);
            }

            if (query.Take.HasValue)
            {
                filteredItems = filteredItems.Take(query.Take.Value);
            }

            return filteredItems.ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "finding items by query");
            throw new ContentProviderException($"Failed to find content items by query: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>> GetByDirectoryAsync(string directoryId, CancellationToken cancellationToken = default)
    {
        // This is a placeholder - we would need to implement directory functionality first
        // For now, return an empty list
        return new List<ContentItem>();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>> GetTranslationsAsync(string contentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(contentId))
            return new List<ContentItem>();

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (_itemCache == null)
                return new List<ContentItem>();

            return _itemCache.Values
                .Where(item => item.ContentId == contentId)
                .ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting translations", contentId);
            throw new ContentProviderException($"Failed to get content translations: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (_itemCache == null)
                return new List<ContentTag>();

            return _itemCache.Values
                .SelectMany(item => item.Tags)
                .GroupBy(tag => tag.ToLowerInvariant())
                .Select(group => ContentTag.Create(
                    name: group.First(),
                    slug: GenerateSlug(group.First()),
                    count: group.Count()))
                .OrderBy(tag => tag.Name)
                .ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting tags");
            throw new ContentProviderException($"Failed to get content tags: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (_itemCache == null)
                return new List<ContentCategory>();

            return _itemCache.Values
                .SelectMany(item => item.Categories)
                .GroupBy(category => category.ToLowerInvariant())
                .Select(group => ContentCategory.Create(
                    name: group.First(),
                    slug: GenerateSlug(group.First()),
                    count: group.Count()))
                .OrderBy(category => category.Name)
                .ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting categories");
            throw new ContentProviderException($"Failed to get content categories: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public override async Task<ContentItem> SaveAsync(ContentItem entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Use default commit message
        var commitMessage = string.IsNullOrEmpty(entity.ProviderSpecificId)
            ? $"Create {Path.GetFileName(entity.Path)}"
            : $"Update {Path.GetFileName(entity.Path)}";

        return await SaveWithCommitMessageAsync(entity, commitMessage, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (string.IsNullOrEmpty(entity.Path))
            throw new ArgumentException("Path cannot be empty", nameof(entity));

        LogOperation("saving", entity.Id);

        try
        {
            // Generate content with front matter
            var content = GenerateMarkdownWithFrontMatter(entity);

            // Get SHA if updating
            string? sha = entity.ProviderSpecificId;

            // Create or update file
            var response = await _apiClient.CreateOrUpdateFileAsync(
                entity.Path,
                content,
                commitMessage,
                sha,
                cancellationToken);

            if (!response.Success)
            {
                throw new ContentProviderException($"Failed to save content: {response.ErrorMessage}", ProviderId);
            }

            // Update with provider-specific information
            entity.SetProviderSpecificId(response.Content.Sha);

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);

            return entity;
        }
        catch (Exception ex)
        {
            LogError(ex, "saving", entity.Id);
            throw new ContentProviderException($"Failed to save content item: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var commitMessage = $"Delete content {id}";
        await DeleteWithCommitMessageAsync(id, commitMessage, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        LogOperation("deleting", id);

        try
        {
            // Get item to get path and SHA
            var item = await GetByIdAsync(id, cancellationToken);
            if (item == null)
                throw new ContentItemNotFoundException(id, ProviderId);

            if (string.IsNullOrEmpty(item.ProviderSpecificId))
                throw new ContentProviderException($"Content has no SHA; cannot delete", ProviderId);

            // Delete file
            var response = await _apiClient.DeleteFileAsync(
                item.Path,
                commitMessage,
                item.ProviderSpecificId,
                cancellationToken);

            if (!response.Success)
            {
                throw new ContentProviderException($"Failed to delete content: {response.ErrorMessage}", ProviderId);
            }

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);
        }
        catch (ContentItemNotFoundException)
        {
            // Re-throw not found exception
            throw;
        }
        catch (Exception ex)
        {
            LogError(ex, "deleting", id);
            throw new ContentProviderException($"Failed to delete content item: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            _itemCache = null;
            _cacheExpiration = DateTime.MinValue;

            // Force reload
            await EnsureCacheIsLoaded(cancellationToken, forceRefresh: true);
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    #region Helper Methods

    private async Task EnsureCacheIsLoaded(CancellationToken cancellationToken, bool forceRefresh = false)
    {
        if (!forceRefresh && _itemCache != null && DateTime.UtcNow < _cacheExpiration)
        {
            return; // Cache is still valid
        }

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            // Double-check inside the lock
            if (!forceRefresh && _itemCache != null && DateTime.UtcNow < _cacheExpiration)
            {
                return; // Cache was populated while waiting for lock
            }

            // Load all content items
            var cache = new Dictionary<string, ContentItem>();
            var contentPath = NormalizePath(_options.ContentPath);

            // Get repository contents
            var contents = await _apiClient.GetRepositoryContentsAsync(contentPath, cancellationToken);

            // Process contents recursively
            await ProcessContentsRecursivelyAsync(contents, cache, cancellationToken);

            // Update cache
            _itemCache = cache;
            _cacheExpiration = DateTime.UtcNow.AddMinutes(_options.CacheDurationMinutes);
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private async Task ProcessContentsRecursivelyAsync(
        List<GitHubItem> contents,
        Dictionary<string, ContentItem> contentItems,
        CancellationToken cancellationToken)
    {
        foreach (var item in contents)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (item.IsFile && IsMarkdownFile(item.Name) && !item.Name.Equals("_index.md", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var fileContent = await _apiClient.GetFileContentAsync(item.Path, cancellationToken);
                    var contentItem = await ProcessMarkdownFileAsync(fileContent, cancellationToken);

                    if (contentItem != null)
                    {
                        contentItems[contentItem.Id] = contentItem;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing file: {FileName}", item.Name);
                }
            }
            else if (item.IsDirectory)
            {
                var subContents = await _apiClient.GetRepositoryContentsAsync(item.Path, cancellationToken);
                await ProcessContentsRecursivelyAsync(subContents, contentItems, cancellationToken);
            }
        }
    }

    private async Task<ContentItem?> ProcessMarkdownFileAsync(GitHubFileContent fileContent, CancellationToken cancellationToken)
    {
        if (fileContent == null || !fileContent.IsMarkdownFile())
            return null;

        // Skip _index.md files - they're for directory metadata
        if (fileContent.Name.Equals("_index.md", StringComparison.OrdinalIgnoreCase))
            return null;

        // Get file content
        string markdownContent = fileContent.GetDecodedContent();
        if (string.IsNullOrWhiteSpace(markdownContent))
            return null;

        // Get unique ID from path
        var id = fileContent.Path.GetHashCode().ToString("x");

        // Create the content item
        var contentItem = ContentItem.Create(
            id: id,
            title: Path.GetFileNameWithoutExtension(fileContent.Name),
            content: string.Empty, // Will be set in ProcessMarkdownAsync
            path: fileContent.Path,
            providerId: ProviderId);

        // Store the provider-specific ID (SHA)
        contentItem.SetProviderSpecificId(fileContent.Sha);

        // Get file history for dates
        try
        {
            var (created, modified) = await _apiClient.GetFileHistoryAsync(fileContent.Path, cancellationToken);

            // Use reflection or direct method to set dates (depends on ContentItem implementation)
            // For now, let's assume ContentItem has proper setters
            if (contentItem is ContentItem item)
            {
                // Set dates (implementation depends on your ContentItem class)
                // This is just an example assuming you have extension methods or internal setters
                contentItem.SetCreatedDate(created);
                if (modified.HasValue)
                {
                    contentItem.SetLastModifiedDate(modified.Value);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to get file history for {Path}, using current date", fileContent.Path);
        }

        // Extract locale from path if enabled
        if (_options.EnableLocalization)
        {
            var locale = ExtractLocaleFromPath(fileContent.Path);
            contentItem.SetLocale(locale);
        }
        else
        {
            contentItem.SetLocale(_options.DefaultLocale);
        }

        // Process the markdown content
        await ProcessMarkdownAsync(markdownContent, contentItem, cancellationToken);

        // Set URL
        var url = GenerateUrl(contentItem.Path, contentItem.Slug, _options.ContentPath);
        contentItem.SetUrl(url);

        // Set content ID if localization is enabled
        if (_options.EnableLocalization && string.IsNullOrEmpty(contentItem.ContentId))
        {
            var pathWithoutLocale = RemoveLocaleFromPath(contentItem.Path);
            var pathWithoutExtension = Path.ChangeExtension(pathWithoutLocale, null);
            contentItem.SetContentId(pathWithoutExtension.GetHashCode().ToString("x"));
        }

        return contentItem;
    }

    private async Task ProcessMarkdownAsync(string markdown, ContentItem contentItem, CancellationToken cancellationToken)
    {
        try
        {
            // Extract front matter and content
            var result = await _markdownProcessor.ExtractFrontMatterAsync(markdown, cancellationToken);

            // Set markdown content
            contentItem.SetOriginalMarkdown(result.Content);

            // Render HTML
            var html = await _markdownProcessor.RenderToHtmlAsync(result.Content, cancellationToken);
            contentItem.SetContent(html);

            // Process frontmatter
            if (result.FrontMatter != null)
            {
                ApplyFrontMatterToContentItem(result.FrontMatter, contentItem);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing markdown for {Path}", contentItem.Path);
            throw;
        }
    }

    private void ApplyFrontMatterToContentItem(Dictionary<string, string> frontMatter, ContentItem contentItem)
    {
        foreach (var kvp in frontMatter)
        {
            var key = kvp.Key.ToLowerInvariant();
            var value = kvp.Value;

            switch (key)
            {
                case "title":
                    contentItem.SetTitle(value);
                    break;
                case "author":
                    contentItem.SetAuthor(value);
                    break;
                case "description":
                    contentItem.SetDescription(value);
                    break;
                case "date":
                    if (DateTime.TryParse(value, out var date))
                        contentItem.SetCreatedDate(date);
                    break;
                case "last_modified":
                case "date_modified":
                    if (DateTime.TryParse(value, out var lastModified))
                        contentItem.SetLastModifiedDate(lastModified);
                    break;
                case "slug":
                    contentItem.SetSlug(value);
                    break;
                case "featured":
                case "is_featured":
                    contentItem.SetFeatured(bool.TryParse(value, out var featured) && featured);
                    break;
                case "featured_image":
                case "feature_image":
                case "image":
                    contentItem.SetFeaturedImage(value);
                    break;
                case "content_id":
                case "localization_id":
                    contentItem.SetContentId(value);
                    break;
                case "locale":
                case "language":
                    contentItem.SetLocale(value);
                    break;
                case "status":
                    if (Enum.TryParse<Domain.Enums.ContentStatus>(value, true, out var status))
                        contentItem.SetStatus(status);
                    break;
                case "categories":
                case "category":
                    // Process comma-separated list
                    foreach (var category in ParseListValue(value))
                    {
                        contentItem.AddCategory(category);
                    }
                    break;
                case "tags":
                case "tag":
                    // Process comma-separated list
                    foreach (var tag in ParseListValue(value))
                    {
                        contentItem.AddTag(tag);
                    }
                    break;
                default:
                    // Add as custom metadata
                    if (bool.TryParse(value, out var boolVal))
                        contentItem.SetMetadata(key, boolVal);
                    else if (int.TryParse(value, out var intVal))
                        contentItem.SetMetadata(key, intVal);
                    else if (double.TryParse(value, out var doubleVal))
                        contentItem.SetMetadata(key, doubleVal);
                    else
                        contentItem.SetMetadata(key, value);
                    break;
            }
        }

        // Ensure slug is set
        if (string.IsNullOrEmpty(contentItem.Slug) && !string.IsNullOrEmpty(contentItem.Title))
        {
            contentItem.SetSlug(GenerateSlug(contentItem.Title));
        }
    }

    private string GenerateMarkdownWithFrontMatter(ContentItem entity)
    {
        var markdown = new StringBuilder();

        // Add front matter
        markdown.AppendLine("---");

        // Basic metadata
        if (!string.IsNullOrEmpty(entity.Title))
            markdown.AppendLine($"title: \"{EscapeYamlString(entity.Title)}\"");

        if (!string.IsNullOrEmpty(entity.Author))
            markdown.AppendLine($"author: \"{EscapeYamlString(entity.Author)}\"");

        if (!string.IsNullOrEmpty(entity.Description))
            markdown.AppendLine($"description: \"{EscapeYamlString(entity.Description)}\"");

        // Date created (in ISO format)
        markdown.AppendLine($"date: {entity.DateCreated:yyyy-MM-dd}");

        // Last modified date
        if (entity.LastModified.HasValue)
            markdown.AppendLine($"last_modified: {entity.LastModified.Value:yyyy-MM-dd}");

        // Content ID for localization
        if (!string.IsNullOrEmpty(entity.ContentId))
            markdown.AppendLine($"content_id: \"{entity.ContentId}\"");

        if (!string.IsNullOrEmpty(entity.Locale))
            markdown.AppendLine($"locale: \"{entity.Locale}\"");

        // Slug
        if (!string.IsNullOrEmpty(entity.Slug))
            markdown.AppendLine($"slug: \"{entity.Slug}\"");

        // Featured status and image
        if (entity.IsFeatured)
            markdown.AppendLine("featured: true");

        if (!string.IsNullOrEmpty(entity.FeaturedImageUrl))
            markdown.AppendLine($"featured_image: \"{entity.FeaturedImageUrl}\"");

        // Categories
        if (entity.Categories.Count > 0)
        {
            markdown.AppendLine("categories:");
            foreach (var category in entity.Categories)
            {
                markdown.AppendLine($"  - \"{EscapeYamlString(category)}\"");
            }
        }

        // Tags
        if (entity.Tags.Count > 0)
        {
            markdown.AppendLine("tags:");
            foreach (var tag in entity.Tags)
            {
                markdown.AppendLine($"  - \"{EscapeYamlString(tag)}\"");
            }
        }

        // Add custom metadata
        foreach (var kvp in entity.Metadata)
        {
            // Skip properties we've already handled
            if (IsStandardFrontMatterKey(kvp.Key))
                continue;

            if (kvp.Value is string strValue)
                markdown.AppendLine($"{kvp.Key}: \"{EscapeYamlString(strValue)}\"");
            else if (kvp.Value is bool boolValue)
                markdown.AppendLine($"{kvp.Key}: {boolValue.ToString().ToLowerInvariant()}");
            else if (kvp.Value is int intValue)
                markdown.AppendLine($"{kvp.Key}: {intValue}");
            else if (kvp.Value is double doubleValue)
                markdown.AppendLine($"{kvp.Key}: {doubleValue}");
            else if (kvp.Value is DateTime dateValue)
                markdown.AppendLine($"{kvp.Key}: {dateValue:yyyy-MM-dd}");
            else
                markdown.AppendLine($"{kvp.Key}: \"{kvp.Value}\"");
        }

        markdown.AppendLine("---");
        markdown.AppendLine();

        // Add original markdown content if available
        if (!string.IsNullOrEmpty(entity.OriginalMarkdown))
        {
            markdown.Append(entity.OriginalMarkdown);
        }
        // Otherwise try to convert HTML to markdown
        else if (!string.IsNullOrEmpty(entity.Content))
        {
            var plainText = _markdownProcessor.ConvertHtmlToMarkdownAsync(entity.Content, CancellationToken.None)
                .GetAwaiter().GetResult();
            markdown.Append(plainText);
        }

        return markdown.ToString();
    }

    private string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim('/');
    }

    private string GetDirectoryPath(string filePath)
    {
        var lastSlashIndex = filePath.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            return filePath.Substring(0, lastSlashIndex);
        }
        return string.Empty;
    }

    private string GenerateSlug(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "untitled";

        // Convert to lowercase
        var slug = text.ToLowerInvariant();

        // Remove special characters
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Replace spaces with hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");

        // Remove consecutive hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-{2,}", "-");

        // Trim hyphens from ends
        slug = slug.Trim('-');

        return slug;
    }

    private bool IsMarkdownFile(string fileName)
    {
        return _options.SupportedExtensions.Any(ext =>
            fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }

    private string EscapeYamlString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    private IQueryable<ContentItem> ApplySorting(
        IQueryable<ContentItem> items,
        Domain.Enums.SortField sortField,
        Domain.Enums.SortDirection direction)
    {
        return sortField switch
        {
            Domain.Enums.SortField.Title => direction == Domain.Enums.SortDirection.Ascending ?
                items.OrderBy(item => item.Title) :
                items.OrderByDescending(item => item.Title),

            Domain.Enums.SortField.Author => direction == Domain.Enums.SortDirection.Ascending ?
                items.OrderBy(item => item.Author) :
                items.OrderByDescending(item => item.Author),

            Domain.Enums.SortField.LastModified => direction == Domain.Enums.SortDirection.Ascending ?
                items.OrderBy(item => item.LastModified ?? item.DateCreated) :
                items.OrderByDescending(item => item.LastModified ?? item.DateCreated),

            Domain.Enums.SortField.Created => direction == Domain.Enums.SortDirection.Ascending ?
                items.OrderBy(item => item.DateCreated) :
                items.OrderByDescending(item => item.DateCreated),

            Domain.Enums.SortField.Order => direction == Domain.Enums.SortDirection.Ascending ?
                items.OrderBy(item => item.OrderIndex) :
                items.OrderByDescending(item => item.OrderIndex),

            Domain.Enums.SortField.PublishDate => direction == Domain.Enums.SortDirection.Ascending ?
                items.OrderBy(item => item.PublishDate) :
                items.OrderByDescending(item => item.PublishDate),

            Domain.Enums.SortField.Slug => direction == Domain.Enums.SortDirection.Ascending ?
                items.OrderBy(item => item.Slug) :
                items.OrderByDescending(item => item.Slug),

            Domain.Enums.SortField.ReadTime => direction == Domain.Enums.SortDirection.Ascending ?
                items.OrderBy(item => item.ReadTimeMinutes) :
                items.OrderByDescending(item => item.ReadTimeMinutes),

            _ => direction == Domain.Enums.SortDirection.Ascending ?
                items.OrderBy(item => item.DateCreated) :
                items.OrderByDescending(item => item.DateCreated)
        };
    }

    private IEnumerable<string> ParseListValue(string value)
    {
        var result = new List<string>();

        if (string.IsNullOrWhiteSpace(value))
            return result;

        // Handle YAML array format [item1, item2]
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            value = value.Substring(1, value.Length - 2);
        }

        // Split by comma or semicolon and process each item
        foreach (var item in value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedItem = item.Trim();

            // Remove quotes if present
            if ((trimmedItem.StartsWith("\"") && trimmedItem.EndsWith("\"")) ||
                (trimmedItem.StartsWith("'") && trimmedItem.EndsWith("'")))
            {
                trimmedItem = trimmedItem.Substring(1, trimmedItem.Length - 2);
            }

            if (!string.IsNullOrEmpty(trimmedItem))
            {
                result.Add(trimmedItem);
            }
        }

        return result;
    }

    private bool IsStandardFrontMatterKey(string key)
    {
        var standardKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "title", "author", "description", "date", "last_modified", "date_modified",
            "slug", "featured", "is_featured", "featured_image", "feature_image", "image",
            "content_id", "localization_id", "locale", "language", "status",
            "categories", "category", "tags", "tag"
        };

        return standardKeys.Contains(key);
    }

    private string ExtractLocaleFromPath(string path)
    {
        // If localization is disabled, always return default locale
        if (!_options.EnableLocalization)
        {
            return _options.DefaultLocale;
        }

        // Check if content path is set and remove it from the beginning
        var contentPath = NormalizePath(_options.ContentPath);
        if (!string.IsNullOrEmpty(contentPath) && path.StartsWith(contentPath))
        {
            // Only remove if it's followed by a slash or is the entire path
            if (path.Length == contentPath.Length || path[contentPath.Length] == '/')
            {
                // Remove content path prefix
                path = path.Length > contentPath.Length
                    ? path.Substring(contentPath.Length + 1)
                    : "";
            }
        }

        // Try to extract locale from path format like "en/blog" or "es/articles"
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && IsValidLocale(segments[0]))
        {
            return segments[0];
        }

        // No valid locale found, return default
        return _options.DefaultLocale;
    }

    private string RemoveLocaleFromPath(string path)
    {
        if (!_options.EnableLocalization)
            return path;

        // Check if content path is set and remove it
        var contentPath = NormalizePath(_options.ContentPath);
        if (!string.IsNullOrEmpty(contentPath) && path.StartsWith(contentPath))
        {
            // Only remove if it's followed by a slash or is the entire path
            if (path.Length == contentPath.Length || path[contentPath.Length] == '/')
            {
                // Remove content path prefix
                path = path.Length > contentPath.Length
                    ? path.Substring(contentPath.Length + 1)
                    : "";
            }
        }

        // Check if first segment is a locale
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && IsValidLocale(segments[0]))
        {
            // Remove locale segment
            return string.Join("/", segments.Skip(1));
        }

        return path;
    }

    private bool IsValidLocale(string locale)
    {
        // Check against supported locales list if defined
        if (_options.SupportedLocales.Count > 0)
        {
            return _options.SupportedLocales.Contains(locale, StringComparer.OrdinalIgnoreCase);
        }

        // Fallback to simple validation: 2-letter language code or language-region format
        return (locale.Length == 2 && locale.All(char.IsLetter)) ||
               (locale.Length == 5 && locale[2] == '-' &&
                locale.Substring(0, 2).All(char.IsLetter) &&
                locale.Substring(3, 2).All(char.IsLetter));
    }

    private string GenerateUrl(string path, string slug, string? skipSegment = null)
    {
        // Check if pathToTrim is null, whitespace, or just "/"
        bool skipPrefixRemoval = string.IsNullOrWhiteSpace(skipSegment) || skipSegment == "/";

        // Normalize path
        path = NormalizePath(path);

        // Step 1: Remove skipSegment from the beginning of the path if present
        if (!skipPrefixRemoval && path.StartsWith(skipSegment!))
        {
            // Only remove 'skipSegment' if it's followed by a slash or is the entire string
            if (path.Length == skipSegment.Length || path[skipSegment.Length] == '/')
            {
                // If skipSegment is followed by a slash, remove both skipSegment and the slash
                // If skipSegment is the entire string, this will result in an empty string
                path = path.Length > skipSegment.Length ? path.Substring(skipSegment.Length + 1) : "";
            }
        }

        // Step 2: Remove the filename from the path
        int lastSlashIndex = path.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            path = path.Substring(0, lastSlashIndex);
        }
        else
        {
            // If there's no slash, path is a single segment, so clear it
            path = "";
        }

        // If using localization, check if the first segment is a locale and remove it
        if (_options.EnableLocalization && path.Length > 0)
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0 && IsValidLocale(segments[0]))
            {
                // Remove the locale segment
                path = string.Join("/", segments.Skip(1));
            }
        }

        // Step 3: Append slug, with a slash if path is not empty
        if (!string.IsNullOrEmpty(path))
        {
            return path + "/" + slug;
        }
        else
        {
            return slug;
        }
    }

    #endregion
}