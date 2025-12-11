using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.Utilities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Osirion.Blazor.Cms.Infrastructure.Repositories;

/// <summary>
/// Base implementation for content repositories with simplified caching
/// </summary>
public abstract class BaseContentRepository : RepositoryBase<ContentItem, string>, IContentRepository
{
    protected readonly IMarkdownProcessor MarkdownProcessor;
    protected readonly SemaphoreSlim CacheLock = new(1, 1);

    // Simplified cache - no time-based expiration
    protected Dictionary<string, ContentItem> ItemCache = new();
    protected bool CacheLoaded = false;

    // Configuration properties set by derived classes
    protected bool EnableLocalization = false;
    protected string DefaultLocale = "en";
    protected string ContentPath = string.Empty;
    protected List<string> SupportedLocales = new() { "en" };

    // Flag to track if update is in progress to avoid multiple webhooks hammering the system
    private bool _updateInProgress = false;

    private string[] _excludedExtensionsFromMarkdownProcessing = [".txt", ".json", ".yml", ".log", ".bak"];

    protected BaseContentRepository(
        string providerId,
        IMarkdownProcessor markdownProcessor,
        ILogger logger)
        : base(providerId, logger)
    {
        MarkdownProcessor = markdownProcessor ?? throw new ArgumentNullException(nameof(markdownProcessor));
    }

    protected abstract Task<Dictionary<string, ContentItem>> LoadItemsIntoCache(CancellationToken cancellationToken);

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);
            return ItemCache.Values.ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting all items", ProviderId);
            return Array.Empty<ContentItem>();
        }
    }

    /// <inheritdoc/>
    public override async Task<ContentItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (ItemCache.TryGetValue(id, out var item))
            {
                return item;
            }

            return null;
        }
        catch (Exception ex)
        {
            LogError(ex, "getting item by ID", id);
            return null;
        }
    }

    public async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        // Check if an update is already in progress to avoid multiple simultaneous refreshes
        if (_updateInProgress)
        {
            Logger.LogDebug("Cache refresh already in progress for provider {ProviderId}, skipping", ProviderId);
            return;
        }

        bool lockTaken = false;
        try
        {
            // Use a shorter timeout for webhooks - we don't want to block too long
            lockTaken = await CacheLock.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
            if (!lockTaken)
            {
                Logger.LogWarning("Could not acquire lock for cache refresh - another operation in progress for provider {ProviderId}", ProviderId);
                return;
            }

            _updateInProgress = true;

            // Log cache status before refresh
            Logger.LogInformation("Refreshing cache for provider {ProviderId}. Current cache has {ItemCount} items",
                ProviderId, ItemCache?.Count ?? 0);

            // Force reload the cache
            await LoadAndAssignCache(cancellationToken);

            Logger.LogInformation("Cache refreshed for provider {ProviderId}. New cache has {ItemCount} items",
                ProviderId, ItemCache.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error refreshing content cache for provider {ProviderId}", ProviderId);

            // Even if loading fails, ensure we have a valid cache
            if (ItemCache is null)
            {
                ItemCache = new Dictionary<string, ContentItem>();
            }
        }
        finally
        {
            _updateInProgress = false;

            if (lockTaken)
            {
                CacheLock.Release();
            }
        }
    }

    /// <summary>
    /// Helper method to load items into cache and properly assign them
    /// </summary>
    private async Task LoadAndAssignCache(CancellationToken cancellationToken)
    {
        try
        {
            var startTime = DateTime.UtcNow;
            Logger.LogInformation("Loading content cache for provider {ProviderId}", ProviderId);

            // Call the abstract method implemented by derived classes
            var newCache = await LoadItemsIntoCache(cancellationToken);

            // Ensure we always have a non-null cache
            if (newCache is null)
            {
                Logger.LogWarning("LoadItemsIntoCache returned null for provider {ProviderId}, using empty dictionary", ProviderId);
                newCache = new Dictionary<string, ContentItem>();
            }

            // Atomically update the cache
            ItemCache = newCache;
            CacheLoaded = true;

            Logger.LogInformation("Cache loaded successfully for provider {ProviderId} in {Duration}ms. Items count: {ItemCount}",
                ProviderId, (DateTime.UtcNow - startTime).TotalMilliseconds, newCache.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load items into cache for provider {ProviderId}", ProviderId);

            // Always ensure the cache is not null
            if (ItemCache is null)
            {
                ItemCache = new Dictionary<string, ContentItem>();
                CacheLoaded = true; // Mark as loaded even if empty to avoid infinite loading attempts
            }

            throw; // Rethrow to let the caller handle it
        }
    }

    /// <summary>
    /// Ensures the cache is loaded (simplified version without time-based expiration)
    /// </summary>
    protected virtual async Task EnsureCacheIsLoaded(CancellationToken cancellationToken, bool forceRefresh = false)
    {
        // Skip if we have a loaded cache and aren't forcing refresh
        if (!forceRefresh && CacheLoaded)
        {
            return;
        }

        // Skip if an update is already in progress
        if (_updateInProgress)
        {
            Logger.LogDebug("Cache update already in progress for provider {ProviderId}, using existing cache", ProviderId);
            return;
        }

        bool lockTaken = false;
        try
        {
            // Try to acquire the lock with a reasonable timeout
            lockTaken = await CacheLock.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);
            if (!lockTaken)
            {
                Logger.LogWarning("Timeout waiting for cache lock in EnsureCacheIsLoaded for provider {ProviderId}", ProviderId);
                return; // Use whatever cache we have, even if not loaded
            }

            // Mark update as in progress to prevent webhook contention
            _updateInProgress = true;

            // Double-check cache validity after acquiring lock
            if (!forceRefresh && CacheLoaded)
            {
                return; // Another thread loaded the cache while we were waiting
            }

            // Load and assign the cache
            await LoadAndAssignCache(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error ensuring cache is loaded for provider {ProviderId}", ProviderId);

            // Always ensure cache is not null
            if (ItemCache is null)
            {
                ItemCache = new Dictionary<string, ContentItem>();
                CacheLoaded = true; // Mark as loaded to avoid infinite retry
            }
        }
        finally
        {
            _updateInProgress = false;

            if (lockTaken)
            {
                CacheLock.Release();
            }
        }
    }

    /// <inheritdoc/>
    public async Task<ContentItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            var normalizedPath = UrlGenerator.NormalizePath(path);
            return ItemCache.Values.FirstOrDefault(c => UrlGenerator.NormalizePath(c.Path) == normalizedPath);
        }
        catch (Exception ex)
        {
            LogError(ex, "getting item by path", path);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<ContentItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            return ItemCache.Values.FirstOrDefault(c => c.Url == url);
        }
        catch (Exception ex)
        {
            LogError(ex, "getting item by URL", url);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>> FindByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            var filteredItems = ItemCache.Values.AsQueryable();
            var someList = filteredItems.ToList();

            // Apply query filters
            filteredItems = ApplyQueryFilters(filteredItems, query);

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
            LogError(ex, "finding items by query", ProviderId);
            return Array.Empty<ContentItem>();
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>> GetByDirectoryAsync(string directoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            return ItemCache.Values
                .Where(item => item.Directory is not null && item.Directory.Id == directoryId)
                .ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting items by directory", directoryId);
            return Array.Empty<ContentItem>();
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>> GetTranslationsAsync(string contentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            return new List<ContentItem>();

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            return ItemCache.Values
                .Where(item => item.ContentId == contentId)
                .ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting translations", contentId);
            return Array.Empty<ContentItem>();
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            return ItemCache.Values
                .SelectMany(item => item.Tags)
                .GroupBy(tag => tag.ToLowerInvariant())
                .Select(group => ContentTag.Create(
                    name: group.First(),
                    slug: UrlGenerator.GenerateSlug(group.First()),
                    count: group.Count()))
                .OrderBy(tag => tag.Name)
                .ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting tags", ProviderId);
            return Array.Empty<ContentTag>();
        }
    }

    /// <inheritdoc/>
    public override async Task<ContentItem> SaveAsync(ContentItem entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        // Use default commit message for compatibility with version-controlled systems
        var commitMessage = string.IsNullOrWhiteSpace(entity.ProviderSpecificId)
            ? $"Create {Path.GetFileName(entity.Path)}"
            : $"Update {Path.GetFileName(entity.Path)}";

        return await SaveWithCommitMessageAsync(entity, commitMessage, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var commitMessage = $"Delete content {id}";
        await DeleteWithCommitMessageAsync(id, commitMessage, cancellationToken);
    }

    /// <summary>
    /// Saves a content item with a commit message
    /// </summary>
    public abstract Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a content item with a commit message
    /// </summary>
    public abstract Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a markdown file to create a content item
    /// </summary>
    protected async Task ProcessMarkdownAsync(string markdown, ContentItem contentItem, CancellationToken cancellationToken)
    {
        try
        {
            var result = MarkdownProcessor.ExtractFrontMatterAndContent(markdown);
            var html =  IsExcludedFromMarkdownProcessing(contentItem.Path) ? result.Content : await MarkdownProcessor.RenderToHtmlAsync(result.Content);

            contentItem.Metadata = result.FrontMatter;
            contentItem.SetOriginalMarkdown(result.Content);
            contentItem.SetContent(html);
            contentItem.SetTitle(contentItem.Metadata?.Title ?? contentItem.Title);
            contentItem.SetAuthor(contentItem.Metadata?.Author ?? contentItem.Author);
            contentItem.SetDescription(contentItem.Metadata?.Description ?? contentItem.Description);
            contentItem.SetLocale(contentItem.Metadata?.Lang ?? contentItem.Locale);
            contentItem.SetContentId(contentItem.Metadata?.Id ?? contentItem.ContentId);
            contentItem.SetSlug(contentItem.Metadata?.Slug ?? contentItem.Slug);
            contentItem.SetFeaturedImage(contentItem.Metadata?.FeaturedImage ?? contentItem.FeaturedImageUrl);
            contentItem.SetFeatured(contentItem.Metadata?.IsFeatured ?? contentItem.IsFeatured);
            contentItem.SetOrderIndex(contentItem.Metadata?.Order ?? contentItem.OrderIndex);

            var createdDate = contentItem.DateCreated;
            var metadataDate = contentItem.Metadata?.Date;

            if (!string.IsNullOrWhiteSpace(metadataDate) &&
                DateTime.TryParse(
                    metadataDate,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var parsedDate))
            {
                createdDate = parsedDate;
            }

            contentItem.SetCreatedDate(createdDate);

            if (contentItem.Metadata?.Categories is not null && contentItem.Metadata.Categories.Any())
            {
                foreach (var category in contentItem.Metadata.Categories)
                {
                    contentItem.AddCategory(category);
                }
            }

            if (contentItem.Metadata?.Tags is not null && contentItem.Metadata.Tags.Any())
            {
                foreach (var tag in contentItem.Metadata.Tags)
                {
                    contentItem.AddTag(tag);
                }
            }
            
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing markdown for {Path}", contentItem.Path);
            throw;
        }
    }

    /// <summary>
    /// Generates markdown content with front matter for saving
    /// </summary>
    protected string GenerateMarkdownWithFrontMatter(ContentItem entity)
    {
        var markdown = new StringBuilder();

        // Add front matter
        markdown.AppendLine("---");

        // Basic metadata
        if (!string.IsNullOrWhiteSpace(entity.Title))
            markdown.AppendLine($"title: \"{EscapeYamlString(entity.Title)}\"");

        if (!string.IsNullOrWhiteSpace(entity.Author))
            markdown.AppendLine($"author: \"{EscapeYamlString(entity.Author)}\"");

        if (!string.IsNullOrWhiteSpace(entity.Description))
            markdown.AppendLine($"description: \"{EscapeYamlString(entity.Description)}\"");

        // Date created (in ISO format)
        markdown.AppendLine($"date: {entity.DateCreated:yyyy-MM-dd}");

        // Last modified date
        if (entity.LastModified.HasValue)
            markdown.AppendLine($"last_modified: {entity.LastModified.Value:yyyy-MM-dd}");

        // Content ID for localization
        if (!string.IsNullOrWhiteSpace(entity.ContentId))
            markdown.AppendLine($"content_id: \"{entity.ContentId}\"");

        if (!string.IsNullOrWhiteSpace(entity.Locale))
            markdown.AppendLine($"locale: \"{entity.Locale}\"");

        // Slug
        if (!string.IsNullOrWhiteSpace(entity.Slug))
            markdown.AppendLine($"slug: \"{entity.Slug}\"");

        // Featured status and image
        if (entity.IsFeatured)
            markdown.AppendLine("featured: true");

        if (!string.IsNullOrWhiteSpace(entity.FeaturedImageUrl))
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
        //foreach (var kvp in entity.Metadata)
        //{
        //    // Skip properties we've already handled
        //    if (IsStandardFrontMatterKey(kvp.Key))
        //        continue;

        //    if (kvp.Value is string strValue)
        //        markdown.AppendLine($"{kvp.Key}: \"{EscapeYamlString(strValue)}\"");
        //    else if (kvp.Value is bool boolValue)
        //        markdown.AppendLine($"{kvp.Key}: {boolValue.ToString().ToLowerInvariant()}");
        //    else if (kvp.Value is int intValue)
        //        markdown.AppendLine($"{kvp.Key}: {intValue}");
        //    else if (kvp.Value is double doubleValue)
        //        markdown.AppendLine($"{kvp.Key}: {doubleValue}");
        //    else if (kvp.Value is DateTime dateValue)
        //        markdown.AppendLine($"{kvp.Key}: {dateValue:yyyy-MM-dd}");
        //    else
        //        markdown.AppendLine($"{kvp.Key}: \"{kvp.Value}\"");
        //}

        markdown.AppendLine("---");
        markdown.AppendLine();

        // Add original markdown content if available
        if (!string.IsNullOrWhiteSpace(entity.OriginalMarkdown))
        {
            markdown.Append(entity.OriginalMarkdown);
        }
        // Otherwise try to convert HTML to markdown
        else if (!string.IsNullOrWhiteSpace(entity.Content))
        {
            var plainText = MarkdownProcessor.ConvertHtmlToMarkdownAsync(entity.Content, CancellationToken.None)
                .GetAwaiter().GetResult();
            markdown.Append(plainText);
        }

        return markdown.ToString();
    }

    /// <summary>
    /// Determines whether a key is a standard front matter key
    /// </summary>
    protected bool IsStandardFrontMatterKey(string key)
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

    /// <summary>
    /// Escapes special characters in a YAML string
    /// </summary>
    protected string EscapeYamlString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    /// <summary>
    /// Applies query filters to a queryable collection of content items
    /// </summary>
    protected IQueryable<ContentItem> ApplyQueryFilters(IQueryable<ContentItem> items, ContentQuery query)
    {
        var filteredItems = items;

        if (!string.IsNullOrWhiteSpace(query.Path))
        {
            filteredItems = filteredItems.Where(item => UrlGenerator.NormalizePath(item.Path).Contains(UrlGenerator.NormalizePath(query.Path), StringComparison.OrdinalIgnoreCase));
        }


        if (!string.IsNullOrWhiteSpace(query.Directory))
        {
            var normalizedDirectory = UrlGenerator.NormalizePath(query.Directory);
            filteredItems = filteredItems.Where(item =>
                item.Directory != null && UrlGenerator.NormalizePath(item.Directory.Name).Equals(UrlGenerator.NormalizePath(query.Directory), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.DirectoryId))
        {
            filteredItems = filteredItems.Where(item =>
                item.Directory != null && item.Directory.Id == query.DirectoryId);
        }

        if (!string.IsNullOrWhiteSpace(query.Slug))
        {
            filteredItems = filteredItems.Where(item =>
                item.Slug != null && UrlGenerator.NormalizePath(item.Slug).Equals(UrlGenerator.NormalizePath(query.Slug), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Url))
        {
            filteredItems = filteredItems.Where(item =>
                item.Url != null && UrlGenerator.NormalizePath(item.Url).Equals(UrlGenerator.NormalizePath(query.Url), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            filteredItems = filteredItems.Where(item =>
                item.Categories.Any(c => c.Equals(query.Category, StringComparison.OrdinalIgnoreCase)));
        }

        if (query.Categories is not null && query.Categories.Any())
        {
            filteredItems = filteredItems.Where(item =>
                query.Categories.All(c =>
                    item.Categories.Any(itemCat =>
                        itemCat.Equals(c, StringComparison.OrdinalIgnoreCase))));
        }

        if (!string.IsNullOrWhiteSpace(query.Tag))
        {
            filteredItems = filteredItems.Where(item =>
                item.Tags.Any(t => t.Equals(query.Tag.Replace("-", " "), StringComparison.OrdinalIgnoreCase)));
        }

        if (query.Tags is not null && query.Tags.Any())
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

        if (!string.IsNullOrWhiteSpace(query.Author))
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

        if (!string.IsNullOrWhiteSpace(query.SearchQuery))
        {
            var searchTerms = query.SearchQuery.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            filteredItems = filteredItems.Where(item =>
                searchTerms.Any(term =>
                    (item.Title != null && item.Title.ToLower().Contains(term)) ||
                    (item.Description != null && item.Description.ToLower().Contains(term)) ||
                    (item.Content != null && item.Content.ToLower().Contains(term)) ||
                    item.Categories.Any(c => c.ToLower().Contains(term)) ||
                    item.Tags.Any(t => t.ToLower().Contains(term))
                )
            );
        }

        if (!string.IsNullOrWhiteSpace(query.Locale))
        {
            filteredItems = filteredItems.Where(item =>
                item.Locale.Equals(query.Locale, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.LocalizationId))
        {
            filteredItems = filteredItems.Where(item =>
                item.ContentId.Equals(query.LocalizationId, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.ProviderId))
        {
            filteredItems = filteredItems.Where(item =>
                item.ProviderId.Equals(query.ProviderId, StringComparison.OrdinalIgnoreCase));
        }

        if (query.IncludeIds is not null && query.IncludeIds.Any())
        {
            filteredItems = filteredItems.Where(item =>
                query.IncludeIds.Contains(item.Id));
        }

        if (query.ExcludeIds is not null && query.ExcludeIds.Any())
        {
            filteredItems = filteredItems.Where(item =>
                !query.ExcludeIds.Contains(item.Id));
        }

        return filteredItems;
    }

    /// <summary>
    /// Applies sorting to a queryable collection of content items
    /// </summary>
    protected IQueryable<ContentItem> ApplySorting(
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

    private bool IsExcludedFromMarkdownProcessing(string fileName)
    {
        return _excludedExtensionsFromMarkdownProcessing.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}