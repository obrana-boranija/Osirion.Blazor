using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Core.Providers.GitHub.Models;
using Osirion.Blazor.Cms.Core.Providers.Interfaces;
using Osirion.Blazor.Cms.Enums;
using Osirion.Blazor.Cms.Exceptions;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;
using Osirion.Blazor.Cms.Options;
using Osirion.Blazor.Cms.Providers.Base;
using Osirion.Blazor.Core.Extensions;
using System.Text;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Core.Providers.GitHub;

/// <summary>
/// Content provider for GitHub repositories with enhanced caching and error handling
/// </summary>
public class GitHubContentProvider : WritableContentProviderBase
{
    private readonly GitHubContentOptions _options;
    private readonly IGitHubApiClient _apiClient;
    private readonly IContentParser _contentParser;
    private readonly SemaphoreSlim _operationLock = new(1, 1);

    // In-memory tree cache for optimizing directory lookups
    private Dictionary<string, DirectoryItem>? _directoryCache;

    /// <summary>
    /// Initializes a new instance of the GitHubContentProvider
    /// </summary>
    public GitHubContentProvider(
        IGitHubApiClient apiClient,
        IContentParser contentParser,
        IMemoryCache cacheService,
        IOptions<GitHubContentOptions> options,
        ILogger<GitHubContentProvider> logger)
        : base(cacheService, logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _contentParser = contentParser ?? throw new ArgumentNullException(nameof(contentParser));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrEmpty(_options.Owner))
            throw new ArgumentException("GitHub owner is required", nameof(options));

        if (string.IsNullOrEmpty(_options.Repository))
            throw new ArgumentException("GitHub repository is required", nameof(options));

        // Configure the API client with repository settings
        _apiClient.SetRepository(_options.Owner, _options.Repository);

        if (!string.IsNullOrEmpty(_options.Branch))
        {
            _apiClient.SetBranch(_options.Branch);
        }

        if (!string.IsNullOrEmpty(_options.ApiToken))
        {
            _apiClient.SetAccessToken(_options.ApiToken);
        }
    }

    /// <inheritdoc/>
    public override string ProviderId => _options.ProviderId ?? $"github-{_options.Owner}-{_options.Repository}";

    /// <inheritdoc/>
    public override string DisplayName => $"GitHub: {_options.Owner}/{_options.Repository}";

    /// <inheritdoc/>
    public override bool IsReadOnly => string.IsNullOrEmpty(_options.ApiToken);

    /// <inheritdoc/>
    protected override TimeSpan CacheDuration => TimeSpan.FromMinutes(_options.CacheDurationMinutes);

    /// <inheritdoc/>
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // Nothing special to initialize, but this is a good place to verify the connection
        try
        {
            // Verify we can access the repository by trying to get the root contents
            await _apiClient.GetRepositoryContentsAsync("", cancellationToken);
            Logger.LogInformation("Successfully connected to GitHub repository: {Owner}/{Repository}",
                _options.Owner, _options.Repository);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize GitHub content provider for {Owner}/{Repository}",
                _options.Owner, _options.Repository);
            throw new ContentProviderException($"Failed to connect to GitHub repository: {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("content:all");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var contentItems = new List<ContentItem>();
            var contentPath = NormalizePath(_options.ContentPath);

            // Get all contents from the repository
            var contents = await _apiClient.GetRepositoryContentsAsync(contentPath, ct);

            // Process all contents recursively
            await ProcessContentsRecursivelyAsync(contents, contentItems, null, ct);

            Logger.LogInformation("Retrieved {Count} content items from GitHub repository", contentItems.Count);
            return contentItems.AsReadOnly();
        }, cancellationToken) ?? new List<ContentItem>().AsReadOnly();
    }

    /// <inheritdoc/>
    public override async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        // Normalize path for consistency
        path = NormalizePath(path);

        var cacheKey = GetCacheKey($"content:path:{path}");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            try
            {
                // First try direct fetch by path for performance
                var fileContent = await _apiClient.GetFileContentAsync(path, ct);
                return await ProcessMarkdownFileAsync(fileContent, ct);
            }
            catch (ContentProviderException ex)
            {
                Logger.LogWarning(ex, "Failed to get file directly, falling back to search: {Path}", path);

                // Fall back to searching in all items
                var allItems = await GetAllItemsAsync(ct);
                return allItems.FirstOrDefault(item =>
                    NormalizePath(item.Path) == path);
            }
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        // URLs are unique and case-sensitive
        var cacheKey = GetCacheKey($"content:url:{url}");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var items = await GetAllItemsAsync(ct);
            return items.FirstOrDefault(item => item.Url == url);
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        var allItems = await GetAllItemsAsync(cancellationToken);

        // Generate a cache key based on query parameters
        var queryCacheKey = GenerateQueryCacheKey(query);
        var cacheKey = GetCacheKey($"content:query:{queryCacheKey}");

        // Explicitly specify the return type to help the compiler
        return await GetOrCreateCachedAsync<IReadOnlyList<ContentItem>>(cacheKey, async ct =>
        {
            // First get all items - either this is already cached or will be cached
            var allItems = await GetAllItemsAsync(ct);
            var filteredItems = allItems.AsQueryable();

            // Directory filtering
            if (!string.IsNullOrEmpty(query.Directory))
            {
                var normalizedDirectory = NormalizePath(query.Directory);
                filteredItems = filteredItems.Where(item =>
                    NormalizePath(item.Path).StartsWith(normalizedDirectory, StringComparison.OrdinalIgnoreCase));
            }

            // DirectoryId filtering
            if (!string.IsNullOrEmpty(query.DirectoryId))
            {
                // Get the directory first to find its path
                var directory = await GetDirectoryByIdAsync(query.DirectoryId, query.Locale, ct);
                if (directory != null)
                {
                    var directoryPath = NormalizePath(directory.Path);
                    filteredItems = filteredItems.Where(item =>
                        NormalizePath(item.Path).StartsWith(directoryPath, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    // No matching directory, return empty list
                    return Array.Empty<ContentItem>();
                }
            }

            // Category filtering
            if (!string.IsNullOrEmpty(query.Category))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Categories.Any(c => c.Equals(query.Category, StringComparison.OrdinalIgnoreCase)));
            }

            // Multiple categories filtering (AND logic)
            if (query.Categories != null && query.Categories.Any())
            {
                filteredItems = filteredItems.Where(item =>
                    query.Categories.All(c =>
                        item.Categories.Any(itemCat =>
                            itemCat.Equals(c, StringComparison.OrdinalIgnoreCase))));
            }

            // Tag filtering
            if (!string.IsNullOrEmpty(query.Tag))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Tags.Any(t => t.Equals(query.Tag, StringComparison.OrdinalIgnoreCase)));
            }

            // Multiple tags filtering (AND logic)
            if (query.Tags != null && query.Tags.Any())
            {
                filteredItems = filteredItems.Where(item =>
                    query.Tags.All(t =>
                        item.Tags.Any(itemTag =>
                            itemTag.Equals(t, StringComparison.OrdinalIgnoreCase))));
            }

            // Featured items filtering
            if (query.IsFeatured.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.IsFeatured == query.IsFeatured.Value);
            }

            // Author filtering
            if (!string.IsNullOrEmpty(query.Author))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Author.Equals(query.Author, StringComparison.OrdinalIgnoreCase));
            }

            // Status filtering
            if (query.Status.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.Status == query.Status.Value);
            }

            // Date filtering
            if (query.DateFrom.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.DateCreated >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.DateCreated <= query.DateTo.Value);
            }

            // Full-text search
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

            // Locale filtering
            if (!string.IsNullOrEmpty(query.Locale))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Locale.Equals(query.Locale, StringComparison.OrdinalIgnoreCase));
            }

            // Localization ID filtering
            if (!string.IsNullOrEmpty(query.LocalizationId))
            {
                filteredItems = filteredItems.Where(item =>
                    item.ContentId.Equals(query.LocalizationId, StringComparison.OrdinalIgnoreCase));
            }

            // Provider filtering
            if (!string.IsNullOrEmpty(query.ProviderId) && query.ProviderId != ProviderId)
            {
                // No items match this provider
                return Array.Empty<ContentItem>();
            }

            // Include/exclude specific IDs
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

            // Published state filtering
            if (!query.IncludeUnpublished)
            {
                filteredItems = filteredItems.Where(item =>
                    item.Status == ContentStatus.Published);
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

            Logger.LogInformation("Query returned {Count} items from {Total} total items",
                filteredItems.Count(), allItems.Count);

            return filteredItems.ToList().AsReadOnly();
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey($"directories:{locale ?? "all"}");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var directoryItems = new List<DirectoryItem>();
            var contentPath = NormalizePath(_options.ContentPath);

            // Get all contents from the repository
            var contents = await _apiClient.GetRepositoryContentsAsync(contentPath, ct);

            // Process all contents recursively to extract directories
            await ProcessDirectoriesRecursivelyAsync(contents, directoryItems, new Dictionary<string, DirectoryItem>(), null, ct);

            Logger.LogInformation("Retrieved {Count} directory items from GitHub repository", directoryItems.Count);
            return directoryItems.AsReadOnly();
        }, cancellationToken) ?? new List<DirectoryItem>().AsReadOnly();
    }

    /// <inheritdoc/>
    public override async Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("localization:info");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            // If localization is disabled, return minimal info
            if (!_options.EnableLocalization)
            {
                return new LocalizationInfo
                {
                    DefaultLocale = _options.DefaultLocale,
                    AvailableLocales = new List<LocaleInfo>
                    {
                        new LocaleInfo
                        {
                            Code = _options.DefaultLocale,
                            Name = _options.DefaultLocale,
                            IsDefault = true
                        }
                    }
                };
            }

            var localizationInfo = new LocalizationInfo
            {
                DefaultLocale = _options.DefaultLocale
            };

            // Get all content items to analyze translations
            var allItems = await GetAllItemsAsync(ct);

            // Collect all available locales
            var locales = allItems
                .Select(item => item.Locale)
                .Where(l => !string.IsNullOrEmpty(l))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (locales.Count > 0)
            {
                // Add all locales with basic info
                foreach (var locale in locales)
                {
                    localizationInfo.AddLocale(
                        locale,
                        GetLocaleDisplayName(locale),
                        GetLocaleNativeName(locale));
                }

                // Mark default locale
                var defaultLocale = localizationInfo.AvailableLocales.FirstOrDefault(l =>
                    l.Code.Equals(_options.DefaultLocale, StringComparison.OrdinalIgnoreCase));

                if (defaultLocale != null)
                {
                    defaultLocale.IsDefault = true;
                }

                // Group by localization ID to build translations map
                var itemsByLocalizationId = allItems
                    .Where(item => !string.IsNullOrEmpty(item.ContentId))
                    .GroupBy(item => item.ContentId);

                foreach (var group in itemsByLocalizationId)
                {
                    var translations = new Dictionary<string, string>();

                    foreach (var item in group)
                    {
                        if (!string.IsNullOrEmpty(item.Locale))
                        {
                            translations[item.Locale] = item.Path;
                        }
                    }

                    if (translations.Count > 0)
                    {
                        localizationInfo.Translations[group.Key] = translations;
                    }
                }
            }
            else
            {
                // No locales found, use default
                localizationInfo.AvailableLocales.Add(new LocaleInfo
                {
                    Code = _options.DefaultLocale,
                    Name = GetLocaleDisplayName(_options.DefaultLocale),
                    NativeName = GetLocaleNativeName(_options.DefaultLocale),
                    IsDefault = true
                });
            }

            return localizationInfo;
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>> GetContentTranslationsAsync(string localizationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(localizationId))
            return Array.Empty<ContentItem>();

        var cacheKey = GetCacheKey($"translations:{localizationId}");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            // Create a query for all content with this localization ID
            var query = new ContentQuery { LocalizationId = localizationId };
            var translations = await GetItemsByQueryAsync(query, ct);

            Logger.LogInformation("Found {Count} translations for localization ID: {LocalizationId}",
                translations.Count, localizationId);

            return translations;
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await base.RefreshCacheAsync(cancellationToken);

        // Clear the in-memory directory cache
        _directoryCache = null;
    }

    /// <inheritdoc/>
    protected override async Task<ContentItem> CreateOrUpdateContentInternalAsync(
        ContentItem item,
        string? commitMessage,
        CancellationToken cancellationToken)
    {
        if (IsReadOnly)
            throw new ContentProviderException("This provider is in read-only mode. Set an API token to enable write operations.");

        if (string.IsNullOrEmpty(item.Path))
            throw new ArgumentException("Content path cannot be empty", nameof(item));

        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            // Validate content
            if (_options.ValidateContent)
            {
                var validationResult = _contentParser.ValidateContent(item);
                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join("; ", validationResult.GetAllErrors());

                    throw new ContentValidationException(
                        $"Content validation failed: {errorMessages}",
                        validationResult.Errors.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.ToArray()
                        )
                    );
                }
            }

            // Generate a default commit message if not provided
            var message = commitMessage ?? GenerateCommitMessage(item);

            // Generate the markdown content with front matter
            string markdownContent = _contentParser.GenerateMarkdownWithFrontMatter(item);

            // Get existing SHA if this is an update
            string? sha = null;
            if (!string.IsNullOrEmpty(item.ProviderSpecificId))
            {
                sha = item.ProviderSpecificId;
            }
            else if (!string.IsNullOrEmpty(item.Id))
            {
                // Try to get existing file to get its SHA
                try
                {
                    var existingItem = await GetItemByPathAsync(item.Path, cancellationToken);
                    if (existingItem != null)
                    {
                        sha = existingItem.ProviderSpecificId;
                    }
                }
                catch (ContentProviderException)
                {
                    // File doesn't exist, creating new - no SHA needed
                }
            }

            // Create or update the file in GitHub
            var response = await _apiClient.CreateOrUpdateFileAsync(
                item.Path,
                markdownContent,
                message,
                sha,
                cancellationToken);

            if (!response.Success)
                throw new ContentProviderException($"Failed to create/update content: {response.ErrorMessage}");

            // Update the item with provider-specific information
            item.ProviderSpecificId = response.Content.Sha;
            item.ProviderId = ProviderId;

            // If this is a new file and ID is empty, generate one
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = item.Path.GetHashCode().ToString("x");
            }

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);

            return item;
        }
        finally
        {
            _operationLock.Release();
        }
    }

    /// <inheritdoc/>
    public override async Task DeleteContentAsync(
        string id,
        string? commitMessage = null,
        CancellationToken cancellationToken = default)
    {
        if (IsReadOnly)
            throw new ContentProviderException("This provider is in read-only mode. Set an API token to enable write operations.");

        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            // Get the item first to determine path and SHA
            var item = await GetItemByIdAsync(id, cancellationToken);
            if (item == null)
                throw new ContentItemNotFoundException(id, ProviderId);

            if (string.IsNullOrEmpty(item.ProviderSpecificId))
                throw new ContentProviderException("Content SHA is required for deletion");

            // Generate a default commit message if not provided
            var message = commitMessage ?? $"Delete {Path.GetFileName(item.Path)}";

            // Delete the file in GitHub
            var response = await _apiClient.DeleteFileAsync(
                item.Path,
                message,
                item.ProviderSpecificId,
                cancellationToken);

            if (!response.Success)
                throw new ContentProviderException($"Failed to delete content: {response.ErrorMessage}");

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    /// <inheritdoc/>
    public override async Task<DirectoryItem> CreateDirectoryAsync(
        DirectoryItem directory,
        CancellationToken cancellationToken = default)
    {
        if (IsReadOnly)
            throw new ContentProviderException("This provider is in read-only mode. Set an API token to enable write operations.");

        if (string.IsNullOrEmpty(directory.Path))
            throw new ArgumentException("Directory path cannot be empty", nameof(directory));

        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            // GitHub doesn't support creating empty directories, so we create a placeholder file
            var placeholderPath = Path.Combine(directory.Path, ".gitkeep").Replace('\\', '/');
            var message = $"Create directory {directory.Name}";

            // Create the placeholder file
            var response = await _apiClient.CreateOrUpdateFileAsync(
                placeholderPath,
                "", // Empty content
                message,
                null,
                cancellationToken);

            if (!response.Success)
                throw new ContentProviderException($"Failed to create directory: {response.ErrorMessage}");

            // Create _index.md file if we have metadata
            if (!string.IsNullOrEmpty(directory.Name) || !string.IsNullOrEmpty(directory.Description))
            {
                var indexPath = Path.Combine(directory.Path, "_index.md").Replace('\\', '/');
                var frontMatter = new StringBuilder();
                frontMatter.AppendLine("---");

                if (!string.IsNullOrEmpty(directory.Id))
                    frontMatter.AppendLine($"id: \"{directory.Id}\"");

                if (!string.IsNullOrEmpty(directory.Name))
                    frontMatter.AppendLine($"title: \"{EscapeYamlString(directory.Name)}\"");

                if (!string.IsNullOrEmpty(directory.Description))
                    frontMatter.AppendLine($"description: \"{EscapeYamlString(directory.Description)}\"");

                if (!string.IsNullOrEmpty(directory.Locale))
                    frontMatter.AppendLine($"locale: \"{directory.Locale}\"");

                if (directory.Order != 0)
                    frontMatter.AppendLine($"order: {directory.Order}");

                // Add custom metadata
                if (directory.Metadata.Count > 0)
                {
                    foreach (var kvp in directory.Metadata)
                    {
                        if (kvp.Value is string strValue)
                            frontMatter.AppendLine($"{kvp.Key}: \"{EscapeYamlString(strValue)}\"");
                        else if (kvp.Value is bool boolValue)
                            frontMatter.AppendLine($"{kvp.Key}: {boolValue.ToString().ToLowerInvariant()}");
                        else if (kvp.Value is int intValue)
                            frontMatter.AppendLine($"{kvp.Key}: {intValue}");
                        else
                            frontMatter.AppendLine($"{kvp.Key}: \"{kvp.Value}\"");
                    }
                }

                frontMatter.AppendLine("---");

                await _apiClient.CreateOrUpdateFileAsync(
                    indexPath,
                    frontMatter.ToString(),
                    $"Create metadata for {directory.Name} directory",
                    null,
                    cancellationToken);
            }

            // If directory ID is not set, generate one
            if (string.IsNullOrEmpty(directory.Id))
            {
                directory.Id = directory.Path.GetHashCode().ToString("x");
            }

            // If ProviderId is not set, set it
            directory.ProviderId = ProviderId;

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);

            return directory;
        }
        finally
        {
            _operationLock.Release();
        }
    }

    /// <inheritdoc/>
    public override async Task<DirectoryItem> UpdateDirectoryAsync(
        DirectoryItem directory,
        CancellationToken cancellationToken = default)
    {
        if (IsReadOnly)
            throw new ContentProviderException("This provider is in read-only mode. Set an API token to enable write operations.");

        if (string.IsNullOrEmpty(directory.Path))
            throw new ArgumentException("Directory path cannot be empty", nameof(directory));

        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            // Update the _index.md file with metadata
            var indexPath = Path.Combine(directory.Path, "_index.md").Replace('\\', '/');
            var frontMatter = new StringBuilder();
            frontMatter.AppendLine("---");

            if (!string.IsNullOrEmpty(directory.Id))
                frontMatter.AppendLine($"id: \"{directory.Id}\"");

            if (!string.IsNullOrEmpty(directory.Name))
                frontMatter.AppendLine($"title: \"{EscapeYamlString(directory.Name)}\"");

            if (!string.IsNullOrEmpty(directory.Description))
                frontMatter.AppendLine($"description: \"{EscapeYamlString(directory.Description)}\"");

            if (!string.IsNullOrEmpty(directory.Locale))
                frontMatter.AppendLine($"locale: \"{directory.Locale}\"");

            if (directory.Order != 0)
                frontMatter.AppendLine($"order: {directory.Order}");

            // Add custom metadata
            if (directory.Metadata.Count > 0)
            {
                foreach (var kvp in directory.Metadata)
                {
                    if (kvp.Value is string strValue)
                        frontMatter.AppendLine($"{kvp.Key}: \"{EscapeYamlString(strValue)}\"");
                    else if (kvp.Value is bool boolValue)
                        frontMatter.AppendLine($"{kvp.Key}: {boolValue.ToString().ToLowerInvariant()}");
                    else if (kvp.Value is int intValue)
                        frontMatter.AppendLine($"{kvp.Key}: {intValue}");
                    else
                        frontMatter.AppendLine($"{kvp.Key}: \"{kvp.Value}\"");
                }
            }

            frontMatter.AppendLine("---");

            // Check if _index.md already exists
            string? sha = null;
            try
            {
                var fileContent = await _apiClient.GetFileContentAsync(indexPath, cancellationToken);
                sha = fileContent.Sha;
            }
            catch
            {
                // File doesn't exist, we'll create it
            }

            // Create or update the file
            var response = await _apiClient.CreateOrUpdateFileAsync(
                indexPath,
                frontMatter.ToString(),
                $"Update metadata for {directory.Name} directory",
                sha,
                cancellationToken);

            if (!response.Success)
                throw new ContentProviderException($"Failed to update directory: {response.ErrorMessage}");

            // If directory ID is not set, generate one
            if (string.IsNullOrEmpty(directory.Id))
            {
                directory.Id = directory.Path.GetHashCode().ToString("x");
            }

            // If ProviderId is not set, set it
            directory.ProviderId = ProviderId;

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);

            return directory;
        }
        finally
        {
            _operationLock.Release();
        }
    }

    /// <inheritdoc/>
    public override async Task DeleteDirectoryAsync(
        string id,
        bool recursive = false,
        string? commitMessage = null,
        CancellationToken cancellationToken = default)
    {
        if (IsReadOnly)
            throw new ContentProviderException("This provider is in read-only mode. Set an API token to enable write operations.");

        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            // Get the directory first
            var directory = await GetDirectoryByIdAsync(id, null, cancellationToken);
            if (directory == null)
                throw new Cms.Exceptions.DirectoryNotFoundException($"Directory with ID {id} not found");

            // If recursive is false and directory has content, throw exception
            if (!recursive && (directory.Items.Count > 0 || directory.Children.Count > 0))
                throw new ContentProviderException("Cannot delete non-empty directory without recursive flag");

            // Get all files in the directory including subdirectories if recursive
            var directoryPath = directory.Path;
            var contents = await GetAllContentsInPathAsync(directoryPath, recursive, cancellationToken);

            // Generate a default commit message if not provided
            var message = commitMessage ?? $"Delete directory {directory.Name}";

            // Delete all files in reverse order (deeper paths first)
            foreach (var file in contents.OrderByDescending(c => c.Path.Count(ch => ch == '/')))
            {
                await _apiClient.DeleteFileAsync(
                    file.Path,
                    message,
                    file.Sha,
                    cancellationToken);
            }

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    #region Helper Methods

    private async Task ProcessContentsRecursivelyAsync(
        List<GitHubItem> contents,
        List<ContentItem> contentItems,
        string? currentDirectory,
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
                        contentItems.Add(contentItem);
                    }
                }
                catch (Exception ex)
                {
                    // Log but continue processing other files
                    // We don't want one bad file to break everything
                    Logger.LogError(ex, "Error processing file: {FileName}", item.Name);
                }
            }
            else if (item.IsDirectory)
            {
                var subContents = await _apiClient.GetRepositoryContentsAsync(item.Path, cancellationToken);
                await ProcessContentsRecursivelyAsync(subContents, contentItems, item.Name, cancellationToken);
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

        // Create the content item
        var contentItem = new ContentItem
        {
            Id = fileContent.Path.GetHashCode().ToString("x"),
            Path = fileContent.Path,
            ProviderId = ProviderId,
            ProviderSpecificId = fileContent.Sha
        };

        // Get file history
        try
        {
            var (created, modified) = await _apiClient.GetFileHistoryAsync(fileContent.Path, cancellationToken);
            contentItem.DateCreated = created;
            contentItem.LastModified = modified;
        }
        catch (Exception ex)
        {
            // Use current date if history fails
            Logger.LogWarning(ex, "Failed to get file history for {Path}, using current date", fileContent.Path);
            contentItem.DateCreated = DateTime.UtcNow;
        }

        // Extract locale from path
        contentItem.Locale = ExtractLocaleFromPath(fileContent.Path);

        // Parse the markdown file
        await _contentParser.ParseMarkdownContentAsync(markdownContent, contentItem);

        // Set default title if not extracted from front matter
        if (string.IsNullOrEmpty(contentItem.Title))
        {
            contentItem.Title = Path.GetFileNameWithoutExtension(fileContent.Name);
        }

        // Set default slug if not provided
        if (string.IsNullOrEmpty(contentItem.Slug))
        {
            contentItem.Slug = contentItem.Title.ToUrlSlug();
        }

        // Generate URL
        contentItem.Url = GenerateUrl(contentItem.Path, contentItem.Slug, _options.ContentPath);

        // Set content ID if not present but localization is enabled
        if (string.IsNullOrEmpty(contentItem.ContentId) && _options.EnableLocalization)
        {
            // Use path without locale prefix and extension as content ID
            var pathWithoutLocale = RemoveLocaleFromPath(contentItem.Path);
            var pathWithoutExtension = Path.ChangeExtension(pathWithoutLocale, null);
            contentItem.ContentId = pathWithoutExtension.GetHashCode().ToString("x");
        }

        // Link to parent directory
        var directories = await GetDirectoriesAsync(cancellationToken: cancellationToken);
        var parentDir = FindParentDirectory(directories, fileContent.Path);
        if (parentDir != null)
        {
            contentItem.Directory = parentDir;
        }

        return contentItem;
    }

    private async Task ProcessDirectoriesRecursivelyAsync(
        List<GitHubItem> contents,
        List<DirectoryItem> parentDirectories,
        Dictionary<string, DirectoryItem> allDirectories,
        DirectoryItem? parentDirectory,
        CancellationToken cancellationToken)
    {
        // Group contents by directory
        var directoryContents = contents
            .Where(item => item.IsDirectory)
            .OrderBy(item => item.Name)
            .ToList();

        // Process each directory
        foreach (var dir in directoryContents)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Extract locale from path
            string locale = ExtractLocaleFromPath(dir.Path);

            var directory = new DirectoryItem
            {
                Path = dir.Path,
                Name = dir.Name,
                Locale = locale,
                Parent = parentDirectory,
                ProviderId = ProviderId,
                Id = dir.Path.GetHashCode().ToString("x") // Default ID until we process _index.md
            };

            // Add to collections
            parentDirectories.Add(directory);
            allDirectories[dir.Path] = directory;

            // Process subdirectories
            var subContents = await _apiClient.GetRepositoryContentsAsync(dir.Path, cancellationToken);
            await ProcessDirectoriesRecursivelyAsync(
                subContents,
                directory.Children,
                allDirectories,
                directory,
                cancellationToken);
        }
    }

    private async Task ProcessDirectoryMetadataAsync(
        Dictionary<string, DirectoryItem> allDirectories,
        CancellationToken cancellationToken)
    {
        foreach (var (path, directory) in allDirectories)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Look for _index.md file in this directory
            try
            {
                var indexFilePath = Path.Combine(path, "_index.md").Replace('\\', '/');
                var fileContent = await _apiClient.GetFileContentAsync(indexFilePath, cancellationToken);
                if (fileContent != null)
                {
                    string markdownContent = fileContent.GetDecodedContent();
                    if (!string.IsNullOrWhiteSpace(markdownContent))
                    {
                        // Extract front matter
                        var frontMatter = ExtractFrontMatter(markdownContent);
                        if (frontMatter.Count > 0)
                        {
                            // Apply metadata to directory
                            ApplyDirectoryMetadata(directory, frontMatter);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Index file might not exist, continue without metadata
                Logger.LogDebug(ex, "No _index.md found for directory {Path}", path);
            }
        }
    }

    private void ApplyDirectoryMetadata(DirectoryItem directory, Dictionary<string, string> frontMatter)
    {
        if (frontMatter.TryGetValue("id", out var id))
        {
            directory.Id = id;
        }

        if (frontMatter.TryGetValue("title", out var title))
        {
            directory.Name = title;
        }

        if (frontMatter.TryGetValue("description", out var description))
        {
            directory.Description = description;
        }

        if (frontMatter.TryGetValue("order", out var orderStr) && int.TryParse(orderStr, out var order))
        {
            directory.Order = order;
        }

        // Process locale if specified
        if (frontMatter.TryGetValue("locale", out var locale))
        {
            directory.Locale = locale;
        }

        // Process URL if specified
        if (frontMatter.TryGetValue("url", out var url))
        {
            directory.Url = url;
        }
        else if (string.IsNullOrEmpty(directory.Url))
        {
            // Generate URL from path
            directory.Url = GenerateDirectoryUrl(directory.Path, _options.ContentPath);
        }

        // Add other metadata
        foreach (var (key, value) in frontMatter)
        {
            if (!new[] { "id", "title", "description", "order", "locale", "url" }.Contains(key))
            {
                if (bool.TryParse(value, out var boolValue))
                {
                    directory.Metadata[key] = boolValue;
                }
                else if (int.TryParse(value, out var intValue))
                {
                    directory.Metadata[key] = intValue;
                }
                else if (double.TryParse(value, out var doubleValue))
                {
                    directory.Metadata[key] = doubleValue;
                }
                else
                {
                    directory.Metadata[key] = value;
                }
            }
        }
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

    private Dictionary<string, string> ExtractFrontMatter(string content)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Look for front matter between --- delimiters
        var match = Regex.Match(content, @"^\s*---\s*\n(.*?)\n\s*---\s*\n", RegexOptions.Singleline);
        if (match.Success && match.Groups.Count > 1)
        {
            var frontMatterContent = match.Groups[1].Value;
            var lines = frontMatterContent.Split('\n');

            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ':' }, 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // Remove quotes if present
                    if (value.StartsWith("\"") && value.EndsWith("\"") ||
                        value.StartsWith("'") && value.EndsWith("'"))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    result[key] = value;
                }
            }
        }

        return result;
    }

    private DirectoryItem? FindParentDirectory(IEnumerable<DirectoryItem> directories, string filePath)
    {
        string? fileDirectory = Path.GetDirectoryName(filePath)?.Replace('\\', '/');
        if (string.IsNullOrEmpty(fileDirectory))
            return null;

        // If we have a directory cache, use it for faster lookup
        if (_directoryCache != null && _directoryCache.TryGetValue(fileDirectory, out var cachedDirectory))
        {
            return cachedDirectory;
        }

        // Otherwise search through the hierarchy
        foreach (var directory in directories)
        {
            if (directory.Path.Equals(fileDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return directory;
            }

            var childResult = FindParentDirectory(directory.Children, filePath);
            if (childResult != null)
            {
                return childResult;
            }
        }

        return null;
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

    private string GenerateDirectoryUrl(string path, string? skipSegment = null)
    {
        // Similar to GenerateUrl but for directories (no slug to append)

        // Normalize path
        path = NormalizePath(path);

        // Remove skipSegment from beginning if present
        if (!string.IsNullOrWhiteSpace(skipSegment) && skipSegment != "/" && path.StartsWith(skipSegment))
        {
            if (path.Length == skipSegment.Length || path[skipSegment.Length] == '/')
            {
                path = path.Length > skipSegment.Length ? path.Substring(skipSegment.Length + 1) : "";
            }
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

        return path;
    }

    private string GenerateQueryCacheKey(ContentQuery query)
    {
        var keyParts = new List<string>();

        if (!string.IsNullOrEmpty(query.Directory))
            keyParts.Add($"dir:{query.Directory}");

        if (!string.IsNullOrEmpty(query.DirectoryId))
            keyParts.Add($"dirid:{query.DirectoryId}");

        if (!string.IsNullOrEmpty(query.Category))
            keyParts.Add($"cat:{query.Category}");

        if (!string.IsNullOrEmpty(query.Tag))
            keyParts.Add($"tag:{query.Tag}");

        if (query.IsFeatured.HasValue)
            keyParts.Add($"feat:{query.IsFeatured.Value}");

        if (!string.IsNullOrEmpty(query.Author))
            keyParts.Add($"author:{query.Author}");

        if (query.Status.HasValue)
            keyParts.Add($"status:{query.Status.Value}");

        if (query.DateFrom.HasValue)
            keyParts.Add($"from:{query.DateFrom.Value:yyyyMMdd}");

        if (query.DateTo.HasValue)
            keyParts.Add($"to:{query.DateTo.Value:yyyyMMdd}");

        if (!string.IsNullOrEmpty(query.SearchQuery))
            keyParts.Add($"q:{query.SearchQuery}");

        if (!string.IsNullOrEmpty(query.Locale))
            keyParts.Add($"loc:{query.Locale}");

        if (!string.IsNullOrEmpty(query.LocalizationId))
            keyParts.Add($"locid:{query.LocalizationId}");

        keyParts.Add($"sort:{query.SortBy}:{query.SortDirection}");

        if (query.IncludeUnpublished)
            keyParts.Add("unpub:true");

        if (query.Skip.HasValue)
            keyParts.Add($"skip:{query.Skip.Value}");

        if (query.Take.HasValue)
            keyParts.Add($"take:{query.Take.Value}");

        if (query.IncludeIds != null && query.IncludeIds.Any())
            keyParts.Add($"incids:{string.Join(",", query.IncludeIds)}");

        if (query.ExcludeIds != null && query.ExcludeIds.Any())
            keyParts.Add($"excids:{string.Join(",", query.ExcludeIds)}");

        if (query.Categories != null && query.Categories.Any())
            keyParts.Add($"cats:{string.Join(",", query.Categories)}");

        if (query.Tags != null && query.Tags.Any())
            keyParts.Add($"tags:{string.Join(",", query.Tags)}");

        return string.Join("|", keyParts);
    }

    private IQueryable<ContentItem> ApplySorting(
        IQueryable<ContentItem> items,
        SortField sortField,
        SortDirection direction)
    {
        return sortField switch
        {
            SortField.Title => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.Title) :
                items.OrderByDescending(item => item.Title),

            SortField.Author => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.Author) :
                items.OrderByDescending(item => item.Author),

            SortField.LastModified => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.LastModified ?? item.DateCreated) :
                items.OrderByDescending(item => item.LastModified ?? item.DateCreated),

            SortField.Created => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.DateCreated) :
                items.OrderByDescending(item => item.DateCreated),

            SortField.Order => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.OrderIndex) :
                items.OrderByDescending(item => item.OrderIndex),

            SortField.PublishDate => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.PublishDate) :
                items.OrderByDescending(item => item.PublishDate),

            SortField.Slug => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.Slug) :
                items.OrderByDescending(item => item.Slug),

            SortField.ReadTime => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.ReadTimeMinutes) :
                items.OrderByDescending(item => item.ReadTimeMinutes),

            _ => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.DateCreated) :
                items.OrderByDescending(item => item.DateCreated)
        };
    }

    private bool IsMarkdownFile(string fileName)
    {
        return _options.SupportedExtensions.Any(ext =>
            fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }

    private string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim('/');
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

    private string GenerateCommitMessage(ContentItem item)
    {
        var fileName = Path.GetFileName(item.Path);
        var isNew = string.IsNullOrEmpty(item.ProviderSpecificId);

        if (isNew)
        {
            return $"Create {fileName}";
        }
        else
        {
            return $"Update {fileName}";
        }
    }

    private async Task<List<GitHubItem>> GetAllContentsInPathAsync(
        string path,
        bool recursive,
        CancellationToken cancellationToken)
    {
        var result = new List<GitHubItem>();
        var contents = await _apiClient.GetRepositoryContentsAsync(path, cancellationToken);

        foreach (var item in contents)
        {
            if (item.IsFile)
            {
                result.Add(item);
            }
            else if (item.IsDirectory && recursive)
            {
                // Recursively get contents of subdirectory
                var subContents = await GetAllContentsInPathAsync(item.Path, true, cancellationToken);
                result.AddRange(subContents);
            }
        }

        return result;
    }

    private string GetLocaleDisplayName(string locale)
    {
        // This would ideally use a proper culture/locale library
        // For now, just return the locale code with first letter capitalized
        if (string.IsNullOrEmpty(locale))
            return "Unknown";

        if (locale.Length == 2)
        {
            // Just language code
            return char.ToUpper(locale[0]) + locale.Substring(1);
        }

        if (locale.Length == 5 && locale[2] == '-')
        {
            // Language-region format
            var language = char.ToUpper(locale[0]) + locale.Substring(1, 1);
            var region = locale.Substring(3).ToUpper();
            return $"{language} ({region})";
        }

        return locale;
    }

    private string GetLocaleNativeName(string locale)
    {
        // Would normally use a proper localization library
        // For now, just return the display name
        return GetLocaleDisplayName(locale);
    }

    #endregion
}