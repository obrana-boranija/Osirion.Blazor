using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Utilities;
using System.Globalization;
using System.Text;

namespace Osirion.Blazor.Cms.Infrastructure.Repositories;

/// <summary>
/// Base implementation for content repositories with simplified caching
/// </summary>
public abstract class BaseContentRepository : RepositoryBase<ContentItem, string>, IContentRepository
{
    /// <summary>Gets the markdown processor used by the repository.</summary>
    protected readonly IMarkdownProcessor MarkdownProcessor;
    /// <summary>Gets the content query filter.</summary>
    protected readonly IContentQueryFilter QueryFilter;
    /// <summary>Gets the lock protecting cache updates.</summary>
    protected readonly SemaphoreSlim CacheLock = new(1, 1);

    // Simplified cache - no time-based expiration
    /// <summary>Gets or sets the cached content items.</summary>
    protected volatile Dictionary<string, ContentItem> ItemCache = new();
    /// <summary>Gets or sets whether the cache has been loaded.</summary>
    protected volatile bool CacheLoaded = false;

    // Configuration properties set by derived classes
    /// <summary>Gets or sets whether localization is enabled.</summary>
    protected bool EnableLocalization = false;
    /// <summary>Gets or sets the default content locale.</summary>
    protected string DefaultLocale = "en";
    /// <summary>Gets or sets the content path.</summary>
    protected string ContentPath = string.Empty;
    /// <summary>Gets or sets the supported content locales.</summary>
    protected List<string> SupportedLocales = new() { "en" };

    // Single-flight load state: the load that is running (null when idle) and whether another run was requested meanwhile
    private readonly object _loadGate = new();
    private Task? _loadInFlight;
    private bool _rerunRequested;

    private volatile bool _lastRefreshSucceeded = true;
    private DateTime _coldRetryNotBeforeUtc = DateTime.MinValue;
    private static readonly TimeSpan ColdLoadRetryDelay = TimeSpan.FromSeconds(15);

    private string[] _excludedExtensionsFromMarkdownProcessing = [".txt", ".json", ".yml", ".log", ".bak"];

    /// <summary>Initializes a repository with its content services.</summary>
    protected BaseContentRepository(
        string providerId,
        IMarkdownProcessor markdownProcessor,
        IContentQueryFilter queryFilter,
        ILogger logger)
        : base(providerId, logger)
    {
        MarkdownProcessor = markdownProcessor ?? throw new ArgumentNullException(nameof(markdownProcessor));
        QueryFilter = queryFilter ?? throw new ArgumentNullException(nameof(queryFilter));
    }

    /// <summary>Loads content items into the repository cache.</summary>
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

    /// <summary>Gets whether the most recent cache load succeeded. A failed load keeps the previous cache.</summary>
    public bool LastRefreshSucceeded => _lastRefreshSucceeded;

    /// <summary>
    /// Refreshes the repository cache from its content provider.
    /// </summary>
    /// <remarks>
    /// Refreshes are single-flight. A request that arrives while a load is running does not start a second
    /// concurrent load and is not dropped: it schedules exactly one more run after the current one, so a webhook
    /// for a newer commit is always reflected. The returned task completes when that run has finished.
    /// A failed run keeps the previous cache and is logged.
    /// </remarks>
    public Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        return RequestLoadAsync(forceRefresh: true, cancellationToken);
    }

    /// <summary>
    /// Ensures the cache is loaded. Concurrent callers on a cold cache all wait for the same load, so none of
    /// them reads an empty cache while the first load is still running.
    /// </summary>
    protected virtual Task EnsureCacheIsLoaded(CancellationToken cancellationToken, bool forceRefresh = false)
    {
        if (!forceRefresh && CacheLoaded)
        {
            return Task.CompletedTask;
        }

        return RequestLoadAsync(forceRefresh, cancellationToken);
    }

    private Task RequestLoadAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        Task flight;
        lock (_loadGate)
        {
            if (_loadInFlight is null)
            {
                if (!forceRefresh && CacheLoaded)
                {
                    return Task.CompletedTask;
                }

                // A cold load that just failed is not retried by every reader; a refresh request still is.
                if (!forceRefresh && DateTime.UtcNow < _coldRetryNotBeforeUtc)
                {
                    return Task.CompletedTask;
                }

                var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                _loadInFlight = completion.Task;
                _rerunRequested = false;
                _ = Task.Run(() => RunLoadLoopAsync(completion));
            }
            else if (forceRefresh)
            {
                // The running load may have read the source before the change that triggered this request.
                _rerunRequested = true;
            }

            flight = _loadInFlight;
        }

        return flight.WaitAsync(cancellationToken);
    }

    private async Task RunLoadLoopAsync(TaskCompletionSource completion)
    {
        try
        {
            while (true)
            {
                try
                {
                    // The load is shared by every waiter, so it must not observe one caller's cancellation.
                    await LoadAndAssignCache(CancellationToken.None).ConfigureAwait(false);
                    _lastRefreshSucceeded = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error loading content cache for provider {ProviderId}; keeping the previous cache", ProviderId);
                    _lastRefreshSucceeded = false;

                    if (!CacheLoaded)
                    {
                        lock (_loadGate)
                        {
                            _coldRetryNotBeforeUtc = DateTime.UtcNow + ColdLoadRetryDelay;
                        }
                    }
                }

                lock (_loadGate)
                {
                    if (!_rerunRequested)
                    {
                        _loadInFlight = null;
                        break;
                    }

                    _rerunRequested = false;
                }
            }
        }
        finally
        {
            completion.TrySetResult();
        }
    }

    /// <summary>
    /// Loads items into the cache and assigns them. A failed load leaves the previous cache untouched.
    /// </summary>
    private async Task LoadAndAssignCache(CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        Logger.LogInformation("Loading content cache for provider {ProviderId}. Current cache has {ItemCount} items",
            ProviderId, ItemCache.Count);

        var newCache = await LoadItemsIntoCache(cancellationToken).ConfigureAwait(false);

        if (newCache is null)
        {
            Logger.LogWarning("LoadItemsIntoCache returned null for provider {ProviderId}, using empty dictionary", ProviderId);
            newCache = new Dictionary<string, ContentItem>();
        }

        ItemCache = newCache;
        CacheLoaded = true;

        Logger.LogInformation("Cache loaded successfully for provider {ProviderId} in {Duration}ms. Items count: {ItemCount}",
            ProviderId, (DateTime.UtcNow - startTime).TotalMilliseconds, newCache.Count);
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
    public async Task<IReadOnlyList<ContentItem>?> FindByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            var filteredItems = ItemCache.Values.AsQueryable();

            // Apply query filters, sorting, and pagination - all handled by QueryFilter
            filteredItems = QueryFilter.ApplyFilters(filteredItems, query);

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
    /// Creates the id of the content item stored at <paramref name="path"/> in this provider.
    /// </summary>
    /// <remarks>
    /// The id is a hash of the path and the provider id that is the same in every process and on every server
    /// instance, so caches, links and lookups by id survive restarts and scale-out.
    /// </remarks>
    /// <param name="path">The item's path relative to the provider root, with <c>/</c> separators.</param>
    /// <returns>A 16-character lowercase hexadecimal id.</returns>
    protected string CreateItemId(string path)
    {
        return IdGenerator.GenerateStableId(path, ProviderId);
    }

    /// <summary>
    /// Gives an item whose front matter has no <c>id</c> a localization id derived from its path without the locale
    /// segment and the file extension, so all translations of the same content share it.
    /// </summary>
    /// <remarks>
    /// The provider id is left out on purpose: the admin writes this id into the front matter when it saves an item,
    /// and translations that were never saved must still compute the same value if the provider id changes.
    /// </remarks>
    /// <param name="contentItem">The item to complete.</param>
    protected void AssignDefaultLocalizationId(ContentItem contentItem)
    {
        if (!EnableLocalization || !string.IsNullOrWhiteSpace(contentItem.ContentId))
            return;

        var pathWithoutLocale = UrlGenerator.RemoveLocaleFromPath(contentItem.Path, ContentPath, EnableLocalization, SupportedLocales);
        var pathWithoutExtension = Path.ChangeExtension(pathWithoutLocale, null);
        if (string.IsNullOrWhiteSpace(pathWithoutExtension))
            return;

        contentItem.SetContentId(IdGenerator.GenerateStableId(pathWithoutExtension));
    }

    /// <summary>
    /// Processes a markdown file to create a content item
    /// </summary>
    protected async Task ProcessMarkdownAsync(string markdown, ContentItem contentItem, CancellationToken cancellationToken)
    {
        try
        {
            var result = MarkdownProcessor.ExtractFrontMatterAndContent(markdown);
            // Files excluded from markdown rendering are stored as HTML, so they are sanitized as HTML
            var html = IsExcludedFromMarkdownProcessing(contentItem.Path)
                ? MarkdownProcessor.SanitizeHtml(result.Content)
                : await MarkdownProcessor.RenderToHtmlAsync(result.Content, sanitizeHtml: true);

            contentItem.Metadata = result.FrontMatter;
            // Keep the source next to the sanitized HTML so a save writes the original markdown back,
            // not a conversion of the sanitized HTML (SetContent alone resets the source to null)
            contentItem.SetContent(html, result.Content);
            
            // Map all FrontMatter properties to ContentItem (in order of FrontMatter class)
            contentItem.SetContentId(contentItem.Metadata?.Id ?? contentItem.ContentId);
            contentItem.SetOrderIndex(contentItem.Metadata?.Order ?? contentItem.OrderIndex);
            // Layout and Permalink are stored in Metadata only
            contentItem.SetTitle(contentItem.Metadata?.Title ?? contentItem.Title);
            contentItem.SetDescription(contentItem.Metadata?.Description ?? contentItem.Description);
            contentItem.SetAuthor(contentItem.Metadata?.Author ?? contentItem.Author);
            contentItem.SetFeaturedImage(contentItem.Metadata?.FeaturedImage ?? contentItem.FeaturedImageUrl);
            contentItem.SetFeatured(contentItem.Metadata?.IsFeatured ?? contentItem.IsFeatured);
            contentItem.SetSlug(contentItem.Metadata?.Slug ?? contentItem.Slug);
            contentItem.SetLocale(contentItem.Metadata?.Lang ?? contentItem.Locale);
            
            // Map Published to Status and IsPublished
            var isPublished = contentItem.Metadata?.Published ?? true;
            contentItem.IsPublished = isPublished;
            contentItem.SetStatus(isPublished ? ContentStatus.Published : ContentStatus.Draft);
            
            // CustomFields and SeoProperties are stored in Metadata only

            // Parse and set date
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
            contentItem.SetLastModifiedDate(contentItem.LastModified ?? createdDate);

            // Add categories
            if (contentItem.Metadata?.Categories is not null && contentItem.Metadata.Categories.Any())
            {
                foreach (var category in contentItem.Metadata.Categories)
                {
                    contentItem.AddCategory(category);
                }
            }

            // Add tags
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
    /// <remarks>Blocks the calling thread while the HTML is converted. Use <see cref="GenerateMarkdownWithFrontMatterAsync"/>.</remarks>
    [Obsolete("Use GenerateMarkdownWithFrontMatterAsync, which does not block the calling thread.")]
    protected string GenerateMarkdownWithFrontMatter(ContentItem entity)
    {
        return GenerateMarkdownWithFrontMatterAsync(entity, CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Generates markdown content with front matter for saving
    /// </summary>
    /// <param name="entity">The item to serialize.</param>
    /// <param name="cancellationToken">Cancels the HTML to markdown conversion.</param>
    protected async Task<string> GenerateMarkdownWithFrontMatterAsync(ContentItem entity, CancellationToken cancellationToken = default)
    {
        var markdown = new StringBuilder();

        // Use FrontMatter.ToYaml() if Metadata exists, otherwise build manually
        if (entity.Metadata is not null)
        {
            // Update metadata with current entity values
            entity.Metadata.Id = entity.ContentId;
            entity.Metadata.Order = entity.OrderIndex;
            entity.Metadata.Title = entity.Title;
            entity.Metadata.Description = entity.Description;
            entity.Metadata.Author = entity.Author;
            entity.Metadata.Date = entity.DateCreated.ToString("yyyy-MM-dd");
            entity.Metadata.FeaturedImage = entity.FeaturedImageUrl;
            entity.Metadata.IsFeatured = entity.IsFeatured;
            entity.Metadata.Slug = entity.Slug;
            entity.Metadata.Lang = entity.Locale;
            entity.Metadata.Published = entity.IsPublished;
            entity.Metadata.Categories = entity.Categories.ToList();
            entity.Metadata.Tags = entity.Tags.ToList();

            // Generate YAML using FrontMatter's built-in method
            markdown.Append(entity.Metadata.ToYaml());
        }
        else
        {
            // Fallback: Build frontmatter manually
            markdown.AppendLine("---");
            
            if (!string.IsNullOrWhiteSpace(entity.ContentId))
                markdown.AppendLine($"id: \"{EscapeYamlString(entity.ContentId)}\"");
            
            if (entity.OrderIndex > 0)
                markdown.AppendLine($"order: {entity.OrderIndex}");
            
            if (!string.IsNullOrWhiteSpace(entity.Title))
                markdown.AppendLine($"title: \"{EscapeYamlString(entity.Title)}\"");
            
            if (!string.IsNullOrWhiteSpace(entity.Description))
                markdown.AppendLine($"description: \"{EscapeYamlString(entity.Description)}\"");
            
            if (!string.IsNullOrWhiteSpace(entity.Author))
                markdown.AppendLine($"author: \"{EscapeYamlString(entity.Author)}\"");
            
            markdown.AppendLine($"date: {entity.DateCreated:yyyy-MM-dd}");
            
            if (!string.IsNullOrWhiteSpace(entity.FeaturedImageUrl))
                markdown.AppendLine($"featured_image: \"{EscapeYamlString(entity.FeaturedImageUrl)}\"");
            
            if (entity.Categories.Count > 0)
            {
                markdown.AppendLine("categories:");
                foreach (var category in entity.Categories)
                    markdown.AppendLine($"  - \"{EscapeYamlString(category)}\"");
            }
            
            if (entity.Tags.Count > 0)
            {
                markdown.AppendLine("tags:");
                foreach (var tag in entity.Tags)
                    markdown.AppendLine($"  - \"{EscapeYamlString(tag)}\"");
            }
            
            if (entity.IsFeatured)
                markdown.AppendLine("is_featured: true");
            
            if (!entity.IsPublished)
                markdown.AppendLine("published: false");
            
            if (!string.IsNullOrWhiteSpace(entity.Slug))
                markdown.AppendLine($"slug: \"{EscapeYamlString(entity.Slug)}\"");
            
            if (!string.IsNullOrWhiteSpace(entity.Locale))
                markdown.AppendLine($"lang: \"{EscapeYamlString(entity.Locale)}\"");
            
            markdown.AppendLine("---");
            markdown.AppendLine();
        }

        // Add original markdown content if available
        if (!string.IsNullOrWhiteSpace(entity.OriginalMarkdown))
        {
            markdown.Append(entity.OriginalMarkdown);
        }
        // Otherwise try to convert HTML to markdown
        else if (!string.IsNullOrWhiteSpace(entity.Content))
        {
            var plainText = await MarkdownProcessor.ConvertHtmlToMarkdownAsync(entity.Content, cancellationToken).ConfigureAwait(false);
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
        ArgumentNullException.ThrowIfNull(value);

        var escaped = new StringBuilder(value.Length + 8);
        foreach (var character in value)
        {
            switch (character)
            {
                case '\\':
                    escaped.Append("\\\\");
                    break;
                case '"':
                    escaped.Append("\\\"");
                    break;
                case '\n':
                    escaped.Append("\\n");
                    break;
                case '\r':
                    escaped.Append("\\r");
                    break;
                case '\t':
                    escaped.Append("\\t");
                    break;
                // Other control characters and the Unicode line separators would end the quoted scalar or its line
                case var control when char.IsControl(control) || (int)control is 0x2028 or 0x2029:
                    escaped.Append("\\u").Append(((int)control).ToString("X4", CultureInfo.InvariantCulture));
                    break;
                default:
                    escaped.Append(character);
                    break;
            }
        }

        return escaped.ToString();
    }

    private bool IsExcludedFromMarkdownProcessing(string fileName)
    {
        return _excludedExtensionsFromMarkdownProcessing.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}
