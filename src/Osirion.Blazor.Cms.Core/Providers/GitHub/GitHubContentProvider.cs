using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Core.Providers.Base;
using Osirion.Blazor.Cms.Core.Providers.Interfaces;
using Osirion.Blazor.Cms.Exceptions;
using Osirion.Blazor.Cms.Models;
using Osirion.Blazor.Cms.Options;
using Osirion.Blazor.Cms.Providers;
using Osirion.Blazor.Cms.Providers.GitHub.Models;
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

    /// <summary>
    /// Initializes a new instance of the GitHubContentProvider
    /// </summary>
    public GitHubContentProvider(
        IGitHubApiClient apiClient,
        IContentParser contentParser,
        IContentCacheService cacheService,
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
    }

    /// <inheritdoc/>
    public override string ProviderId => _options.ProviderId ?? $"github-{_options.Owner}-{_options.Repository}";

    /// <inheritdoc/>
    public override string DisplayName => $"GitHub: {_options.Owner}/{_options.Repository}";

    /// <inheritdoc/>
    public override bool SupportsWriting => !string.IsNullOrEmpty(_options.ApiToken);

    /// <inheritdoc/>
    protected override TimeSpan CacheDuration => TimeSpan.FromMinutes(_options.CacheDurationMinutes);

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("content:all");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var contents = await _apiClient.GetRepositoryContentsAsync(_options.ContentPath, ct);
            var contentItems = new List<ContentItem>();

            await ProcessContentsRecursivelyAsync(contents, contentItems, null, ct);

            return contentItems.AsReadOnly();
        }, cancellationToken) ?? Array.Empty<ContentItem>();
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
            catch (ContentProviderException)
            {
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
    public override async Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        // Generate a cache key based on query parameters
        var queryCacheKey = GenerateQueryCacheKey(query);
        var cacheKey = GetCacheKey($"content:query:{queryCacheKey}");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var allItems = await GetAllItemsAsync(ct);
            var filteredItems = allItems.AsQueryable();

            // Directory filtering
            if (!string.IsNullOrEmpty(query.Directory))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Directory?.Path.Contains(query.Directory, StringComparison.OrdinalIgnoreCase) == true);
            }

            // DirectoryId filtering
            if (!string.IsNullOrEmpty(query.DirectoryId))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Directory?.Id == query.DirectoryId);
            }

            // Category filtering
            if (!string.IsNullOrEmpty(query.Category))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Categories.Any(c => c.Contains(query.Category, StringComparison.OrdinalIgnoreCase)));
            }

            // Tag filtering
            if (!string.IsNullOrEmpty(query.Tag))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Tags.Any(t => t.Contains(query.Tag, StringComparison.OrdinalIgnoreCase)));
            }

            // Featured items filtering
            if (query.IsFeatured.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.IsFeatured == query.IsFeatured.Value);
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

            // Apply sorting
            filteredItems = query.SortBy switch
            {
                SortField.Title => query.SortDirection == SortDirection.Ascending ?
                    filteredItems.OrderBy(item => item.Title) :
                    filteredItems.OrderByDescending(item => item.Title),
                SortField.Author => query.SortDirection == SortDirection.Ascending ?
                    filteredItems.OrderBy(item => item.Author) :
                    filteredItems.OrderByDescending(item => item.Author),
                SortField.LastModified => query.SortDirection == SortDirection.Ascending ?
                    filteredItems.OrderBy(item => item.LastModified ?? item.DateCreated) :
                    filteredItems.OrderByDescending(item => item.LastModified ?? item.DateCreated),
                _ => query.SortDirection == SortDirection.Ascending ?
                    filteredItems.OrderBy(item => item.DateCreated) :
                    filteredItems.OrderByDescending(item => item.DateCreated)
            };

            // Apply pagination
            if (query.Skip.HasValue)
            {
                filteredItems = filteredItems.Skip(query.Skip.Value);
            }

            if (query.Take.HasValue)
            {
                filteredItems = filteredItems.Take(query.Take.Value);
            }

            return filteredItems.ToList().AsReadOnly();
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        string cacheKey = locale != null ?
            GetCacheKey($"directories:{locale}") :
            GetCacheKey("directories:all");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var rootDirectories = new List<DirectoryItem>();
            var allDirectories = new Dictionary<string, DirectoryItem>();

            // Start from the content path
            var contents = await _apiClient.GetRepositoryContentsAsync(_options.ContentPath, ct);

            // Process the directory structure recursively
            await ProcessDirectoriesRecursivelyAsync(
                contents,
                rootDirectories,
                allDirectories,
                null,
                ct);

            // Process directory metadata (_index.md files)
            await ProcessDirectoryMetadataAsync(allDirectories, ct);

            // Filter by locale if requested
            if (!string.IsNullOrEmpty(locale))
            {
                return rootDirectories
                    .Where(d => d.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .AsReadOnly();
            }

            return rootDirectories.AsReadOnly();
        }, cancellationToken) ?? Array.Empty<DirectoryItem>();
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
                    AvailableLocales = new List<string> { _options.DefaultLocale }
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
                localizationInfo.AvailableLocales.AddRange(locales);

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
                localizationInfo.AvailableLocales.Add(_options.DefaultLocale);
            }

            return localizationInfo;
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<ContentItem> CreateContentAsync(ContentItem item, string? commitMessage = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(item.Path))
            throw new ArgumentException("Content path cannot be empty", nameof(item));

        // Generate a default commit message if not provided
        var message = commitMessage ?? $"Create {Path.GetFileName(item.Path)}";

        // Generate the file content
        string fileContent = _contentParser.GenerateMarkdownWithFrontMatter(item);

        // Create the file in GitHub
        var response = await _apiClient.CreateOrUpdateFileAsync(
            item.Path,
            fileContent,
            message,
            null,
            cancellationToken);

        if (!response.Success)
            throw new ContentProviderException($"Failed to create content: {response.ErrorMessage}");

        // Update the item with provider-specific information
        item.ProviderSpecificId = response.Content.Sha;
        item.ProviderId = ProviderId;

        // Invalidate cache
        await RefreshCacheAsync(cancellationToken);

        return item;
    }

    /// <inheritdoc/>
    public override async Task<ContentItem> UpdateContentAsync(ContentItem item, string? commitMessage = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(item.Path))
            throw new ArgumentException("Content path cannot be empty", nameof(item));

        if (string.IsNullOrEmpty(item.ProviderSpecificId))
            throw new ArgumentException("Content SHA is required for updates", nameof(item));

        // Generate a default commit message if not provided
        var message = commitMessage ?? $"Update {Path.GetFileName(item.Path)}";

        // Generate the file content
        string fileContent = _contentParser.GenerateMarkdownWithFrontMatter(item);

        // Update the file in GitHub
        var response = await _apiClient.CreateOrUpdateFileAsync(
            item.Path,
            fileContent,
            message,
            item.ProviderSpecificId,
            cancellationToken);

        if (!response.Success)
            throw new ContentProviderException($"Failed to update content: {response.ErrorMessage}");

        // Update the item with new SHA
        item.ProviderSpecificId = response.Content.Sha;

        // Invalidate cache
        await RefreshCacheAsync(cancellationToken);

        return item;
    }

    /// <inheritdoc/>
    public override async Task DeleteContentAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default)
    {
        // Get the item first to determine path and SHA
        var item = await GetItemByIdAsync(id, cancellationToken);
        if (item == null)
            throw new ContentProviderException($"Content with ID {id} not found");

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

        // Invalidate cache
        await RefreshCacheAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<DirectoryItem> CreateDirectoryAsync(DirectoryItem directory, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(directory.Path))
            throw new ArgumentException("Directory path cannot be empty", nameof(directory));

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
                frontMatter.AppendLine($"title: \"{directory.Name}\"");

            if (!string.IsNullOrEmpty(directory.Description))
                frontMatter.AppendLine($"description: \"{directory.Description}\"");

            if (!string.IsNullOrEmpty(directory.Locale))
                frontMatter.AppendLine($"locale: \"{directory.Locale}\"");

            if (directory.Order != 0)
                frontMatter.AppendLine($"order: {directory.Order}");

            frontMatter.AppendLine("---");

            await _apiClient.CreateOrUpdateFileAsync(
                indexPath,
                frontMatter.ToString(),
                $"Create metadata for {directory.Name} directory",
                null,
                cancellationToken);
        }

        // Invalidate cache
        await RefreshCacheAsync(cancellationToken);

        return directory;
    }

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
                    //logger.LogError(ex, "Error processing file: {FileName}", item.Name);
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
        catch
        {
            // Use current date if history fails
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
            catch
            {
                // Index file might not exist, continue without metadata
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

        // Add other metadata
        foreach (var (key, value) in frontMatter)
        {
            if (!new[] { "id", "title", "description", "order", "locale" }.Contains(key))
            {
                directory.Metadata[key] = value;
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

        // Try to extract locale from path format like "en/blog" or "es/articles"
        var segments = path.Split('/');
        if (segments.Length >= 2 && IsValidLocale(segments[0]))
        {
            return segments[0];
        }

        // No valid locale found, return default
        return _options.DefaultLocale;
    }

    private bool IsValidLocale(string locale)
    {
        // Simple validation for common locale codes (2-letter language code)
        // Could be expanded to support more complex locale codes like en-US
        return locale.Length == 2 && locale.All(char.IsLetter);
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

        // Step 1: Remove string a from the beginning of string b
        if (!skipPrefixRemoval && path.StartsWith(skipSegment!))
        {
            // Only remove 'a' if it's followed by a slash or is the entire string
            if (path.Length == skipSegment!.Length || path[skipSegment.Length] == '/')
            {
                // If a is followed by a slash, remove both a and the slash
                // If a is the entire string, this will result in an empty string
                path = path.Length > skipSegment.Length ? path.Substring(skipSegment.Length + 1) : "";
            }
        }

        // Step 2: Remove the last slash-delimited segment from string b
        int lastSlashIndex = path.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            path = path.Substring(0, lastSlashIndex);
        }
        else
        {
            // If there's no slash, b is a single segment, so clear it
            path = "";
        }

        // Step 3: Append string c, with a slash if b is not empty
        if (!string.IsNullOrEmpty(path))
        {
            return path + "/" + slug;
        }
        else
        {
            return slug;
        }
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

        if (query.Skip.HasValue)
            keyParts.Add($"skip:{query.Skip.Value}");

        if (query.Take.HasValue)
            keyParts.Add($"take:{query.Take.Value}");

        return string.Join("|", keyParts);
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
}