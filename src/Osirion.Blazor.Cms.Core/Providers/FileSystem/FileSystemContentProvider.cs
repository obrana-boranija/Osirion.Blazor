using Markdig;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Enums;
using Osirion.Blazor.Cms.Models;
using Osirion.Blazor.Cms.Options;
using Osirion.Blazor.Cms.Providers.Base;
using Osirion.Blazor.Core.Extensions;

namespace Osirion.Blazor.Cms.Core.Providers.FileSystem;

/// <summary>
/// Content provider for local file system
/// </summary>
public class FileSystemContentProvider : ContentProviderBase
{
    private readonly FileSystemContentOptions _options;
    private readonly MarkdownPipeline _markdownPipeline;
    private FileSystemWatcher? _fileWatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemContentProvider"/> class.
    /// </summary>
    public FileSystemContentProvider(
        IOptions<FileSystemContentOptions> options,
        ILogger<FileSystemContentProvider> logger,
        IMemoryCache memoryCache)
        : base(memoryCache, logger) // Corrected parameter order to match base class constructor
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();
    }

    /// <inheritdoc/>
    public override string ProviderId => _options.ProviderId ?? $"filesystem-{_options.BasePath.GetHashCode():x}";

    /// <inheritdoc/>
    public override string DisplayName => $"FileSystem: {_options.BasePath}";

    /// <inheritdoc/>
    public override bool IsReadOnly => true;

    /// <inheritdoc/>
    protected override TimeSpan CacheDuration => TimeSpan.FromMinutes(_options.CacheDurationMinutes);

    /// <inheritdoc/>
    public override Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_options.BasePath))
        {
            if (_options.CreateDirectoriesIfNotExist)
            {
                try
                {
                    Directory.CreateDirectory(_options.BasePath);
                }
                catch (Exception ex)
                {
                    throw new DirectoryNotFoundException($"Content directory not found and could not be created: {_options.BasePath}. Error: {ex.Message}");
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"Content directory not found: {_options.BasePath}");
            }
        }

        if (_options.WatchForChanges)
        {
            SetupFileWatcher();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("content:all");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var contentItems = new List<ContentItem>();
            await ProcessDirectoryAsync(_options.BasePath, contentItems, ct);
            return contentItems.AsReadOnly();
        }, cancellationToken) ?? new List<ContentItem>().AsReadOnly();
    }

    /// <inheritdoc/>
    public override async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        var items = await GetAllItemsAsync(cancellationToken);

        // Normalize path for comparison
        var normalizedPath = NormalizePath(path);

        return items.FirstOrDefault(item =>
            NormalizePath(item.Path).Equals(normalizedPath, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public override async Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var items = await GetAllItemsAsync(cancellationToken);
        return items.FirstOrDefault(item => item.Url.Equals(url, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        var items = await GetAllItemsAsync(cancellationToken);
        var filteredItems = items.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(query.Directory))
        {
            filteredItems = filteredItems.Where(item =>
                item.Path.StartsWith(query.Directory, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(query.Category))
        {
            filteredItems = filteredItems.Where(item =>
                item.Categories.Any(c => c.Equals(query.Category, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrEmpty(query.Tag))
        {
            filteredItems = filteredItems.Where(item =>
                item.Tags.Any(t => t.Equals(query.Tag, StringComparison.OrdinalIgnoreCase)));
        }

        if (query.IsFeatured.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.IsFeatured == query.IsFeatured.Value);
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

        if (query.Skip.HasValue)
        {
            filteredItems = filteredItems.Skip(query.Skip.Value);
        }

        if (query.Take.HasValue)
        {
            filteredItems = filteredItems.Take(query.Take.Value);
        }

        return filteredItems.ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey($"directories:{locale ?? "all"}");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var rootDirectories = new List<DirectoryItem>();
            await ScanDirectoriesAsync(_options.BasePath, rootDirectories, null, locale, ct);
            return rootDirectories.AsReadOnly();
        }, cancellationToken) ?? new List<DirectoryItem>().AsReadOnly();
    }

    private async Task ProcessDirectoryAsync(string directory, List<ContentItem> contentItems, CancellationToken cancellationToken)
    {
        var searchOption = _options.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        foreach (var pattern in _options.IncludePatterns)
        {
            // Convert glob pattern to a regex pattern
            var regexPattern = GlobPatternToRegex(pattern);

            foreach (var file in Directory.EnumerateFiles(directory, "*.*", searchOption))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Skip files that match exclude patterns
                if (_options.ExcludePatterns.Any(ep => MatchesGlobPattern(file, ep)))
                    continue;

                // Check if file matches the include pattern
                if (MatchesGlobPattern(file, pattern))
                {
                    try
                    {
                        var contentItem = await ProcessMarkdownFileAsync(file, cancellationToken);
                        if (contentItem != null)
                        {
                            contentItems.Add(contentItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log but continue processing other files
                        Logger.LogError(ex, "Error processing file: {FileName}", file);
                    }
                }
            }
        }
    }

    private async Task<ContentItem?> ProcessMarkdownFileAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            if (string.IsNullOrEmpty(content))
                return null;

            var relativePath = Path.GetRelativePath(_options.BasePath, filePath);
            var fileInfo = new FileInfo(filePath);

            var contentItem = new ContentItem
            {
                Id = relativePath.GetHashCode().ToString("x"),
                Path = NormalizePath(relativePath),
                ProviderId = ProviderId,
                ProviderSpecificId = filePath,
                DateCreated = fileInfo.CreationTimeUtc,
                LastModified = fileInfo.LastWriteTimeUtc
            };

            // Extract locale from path if localization is enabled
            if (_options.EnableLocalization)
            {
                var pathSegments = relativePath.Split('/', '\\');
                if (pathSegments.Length > 0 && _options.SupportedLocales.Contains(pathSegments[0]))
                {
                    contentItem.Locale = pathSegments[0];
                }
                else
                {
                    contentItem.Locale = _options.DefaultLocale;
                }
            }
            else
            {
                contentItem.Locale = _options.DefaultLocale;
            }

            // Parse markdown and extract front matter
            var frontMatterEndIndex = content.IndexOf("---", 4);
            if (frontMatterEndIndex > 0)
            {
                var frontMatter = content.Substring(4, frontMatterEndIndex - 4).Trim();
                ParseFrontMatter(frontMatter, contentItem);

                var markdownContent = content.Substring(frontMatterEndIndex + 3).Trim();
                contentItem.Content = Markdown.ToHtml(markdownContent, _markdownPipeline);
                contentItem.OriginalMarkdown = markdownContent;
            }
            else
            {
                contentItem.Content = Markdown.ToHtml(content, _markdownPipeline);
                contentItem.OriginalMarkdown = content;
            }

            // Set defaults if not provided
            if (string.IsNullOrEmpty(contentItem.Title))
            {
                contentItem.Title = Path.GetFileNameWithoutExtension(filePath);
            }

            if (string.IsNullOrEmpty(contentItem.Slug))
            {
                contentItem.Slug = contentItem.Title.ToUrlSlug();
            }

            // Generate URL
            contentItem.Url = GenerateUrl(contentItem.Path, contentItem.Slug, _options.ContentRoot);

            return contentItem;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to process markdown file: {FilePath}", filePath);
            return null;
        }
    }

    private async Task ScanDirectoriesAsync(
        string directoryPath,
        List<DirectoryItem> parentDirectories,
        DirectoryItem? parentDirectory,
        string? locale,
        CancellationToken cancellationToken)
    {
        try
        {
            var dirInfo = new DirectoryInfo(directoryPath);
            if (!dirInfo.Exists)
                return;

            // Get the relative path from the base path
            var relativePath = Path.GetRelativePath(_options.BasePath, directoryPath);
            if (relativePath == ".")
                relativePath = "";

            // Check locale filter
            var currentLocale = _options.DefaultLocale;
            var pathSegments = relativePath.Split('/', '\\');
            if (_options.EnableLocalization && pathSegments.Length > 0 && _options.SupportedLocales.Contains(pathSegments[0]))
            {
                currentLocale = pathSegments[0];
            }

            // Skip if locale doesn't match the filter
            if (locale != null && currentLocale != locale)
                return;

            // Create directory item for this directory
            var directory = new DirectoryItem
            {
                Id = relativePath.GetHashCode().ToString("x"),
                Name = dirInfo.Name,
                Path = NormalizePath(relativePath),
                Locale = currentLocale,
                Parent = parentDirectory
            };

            // Add metadata from _index.md if exists
            await ProcessDirectoryMetadataAsync(directory, directoryPath, cancellationToken);

            // Add to parent collection
            parentDirectories.Add(directory);

            // Process subdirectories if we're including them
            if (_options.IncludeSubdirectories)
            {
                foreach (var subdirInfo in dirInfo.GetDirectories())
                {
                    // Skip directories that match exclude patterns
                    if (_options.ExcludePatterns.Any(pattern => MatchesGlobPattern(subdirInfo.FullName, pattern)))
                        continue;

                    await ScanDirectoriesAsync(
                        subdirInfo.FullName,
                        directory.Children,
                        directory,
                        locale,
                        cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error scanning directory: {DirectoryPath}", directoryPath);
        }
    }

    private async Task ProcessDirectoryMetadataAsync(DirectoryItem directory, string directoryPath, CancellationToken cancellationToken)
    {
        var indexPath = Path.Combine(directoryPath, "_index.md");
        if (File.Exists(indexPath))
        {
            try
            {
                var content = await File.ReadAllTextAsync(indexPath, cancellationToken);
                var frontMatterEndIndex = content.IndexOf("---", 4);
                if (frontMatterEndIndex > 0)
                {
                    var frontMatter = content.Substring(4, frontMatterEndIndex - 4).Trim();
                    var metadata = ParseDirectoryFrontMatter(frontMatter);

                    // Apply metadata to directory
                    if (metadata.TryGetValue("title", out var title))
                        directory.Name = title;

                    if (metadata.TryGetValue("description", out var description))
                        directory.Description = description;

                    if (metadata.TryGetValue("order", out var orderStr) && int.TryParse(orderStr, out var order))
                        directory.Order = order;

                    // Add other metadata as properties
                    foreach (var key in metadata.Keys.Where(k => k != "title" && k != "description" && k != "order"))
                    {
                        directory.Metadata[key] = metadata[key];
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing directory metadata: {IndexPath}", indexPath);
            }
        }
    }

    private void SetupFileWatcher()
    {
        try
        {
            _fileWatcher = new FileSystemWatcher(_options.BasePath)
            {
                IncludeSubdirectories = _options.IncludeSubdirectories,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName
            };

            // Add filters based on supported extensions
            foreach (var extension in _options.SupportedExtensions)
            {
                _fileWatcher.Filters.Add($"*{extension}");
            }

            _fileWatcher.Changed += OnFileChanged;
            _fileWatcher.Created += OnFileChanged;
            _fileWatcher.Deleted += OnFileChanged;
            _fileWatcher.Renamed += OnFileChanged;

            _fileWatcher.EnableRaisingEvents = true;

            Logger.LogInformation("File watcher set up for path: {BasePath}", _options.BasePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to set up file watcher for {BasePath}", _options.BasePath);
        }
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        Logger.LogInformation("File system change detected: {ChangeType} - {Path}", e.ChangeType, e.FullPath);

        // Invalidate cache
        RefreshCacheAsync().ConfigureAwait(false);
    }

    private string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim('/');
    }

    private Dictionary<string, string> ParseDirectoryFrontMatter(string frontMatter)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var lines = frontMatter.Split('\n');
        foreach (var line in lines)
        {
            var parts = line.Split(':', 2);
            if (parts.Length != 2)
                continue;

            var key = parts[0].Trim().ToLowerInvariant();
            var value = parts[1].Trim();

            // Remove quotes if present
            if (value.StartsWith("\"") && value.EndsWith("\"") ||
                value.StartsWith("'") && value.EndsWith("'"))
            {
                value = value.Substring(1, value.Length - 2);
            }

            result[key] = value;
        }

        return result;
    }

    private void ParseFrontMatter(string frontMatter, ContentItem contentItem)
    {
        foreach (var line in frontMatter.Split('\n'))
        {
            var parts = line.Split(':', 2);
            if (parts.Length != 2)
                continue;

            var key = parts[0].Trim().ToLowerInvariant();
            var value = parts[1].Trim();

            // Remove quotes if present
            if (value.StartsWith("\"") && value.EndsWith("\"") ||
                value.StartsWith("'") && value.EndsWith("'"))
            {
                value = value.Substring(1, value.Length - 2);
            }

            switch (key)
            {
                case "title":
                    contentItem.Title = value;
                    break;
                case "author":
                    contentItem.Author = value;
                    break;
                case "date":
                    if (DateTime.TryParse(value, out var date))
                        contentItem.DateCreated = date;
                    break;
                case "description":
                    contentItem.Description = value;
                    break;
                case "tags":
                    ParseListItems(value).ForEach(contentItem.AddTag);
                    break;
                case "categories":
                case "category":
                    ParseListItems(value).ForEach(contentItem.AddCategory);
                    break;
                case "slug":
                    contentItem.Slug = value;
                    break;
                case "featured":
                case "isfeatured":
                    contentItem.IsFeatured = bool.TryParse(value, out var featured) && featured;
                    break;
                case "featuredimage":
                case "feature_image":
                    contentItem.FeaturedImageUrl = value;
                    break;
                case "content_id":
                case "localization_id":
                    contentItem.ContentId = value;
                    break;
                case "locale":
                case "language":
                    contentItem.Locale = value;
                    break;
                case "meta_title":
                case "seo_title":
                    contentItem.Seo.MetaTitle = value;
                    break;
                case "meta_description":
                case "seo_description":
                    contentItem.Seo.MetaDescription = value;
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
    }

    private List<string> ParseListItems(string value)
    {
        var result = new List<string>();

        // Handle YAML arrays with brackets
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            value = value.Substring(1, value.Length - 2);
            var items = value.Split(',');

            foreach (var item in items)
            {
                var trimmedItem = item.Trim();
                if (trimmedItem.StartsWith("\"") && trimmedItem.EndsWith("\"") ||
                    trimmedItem.StartsWith("'") && trimmedItem.EndsWith("'"))
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

        // Handle comma or semicolon separated values
        foreach (var item in value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedItem = item.Trim();
            if (!string.IsNullOrEmpty(trimmedItem))
            {
                result.Add(trimmedItem);
            }
        }

        return result;
    }

    private string GenerateUrl(string path, string slug, string? rootPath = null)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return slug;

        // Remove filename from path
        var directoryPath = string.Join("/", segments.Take(segments.Length - 1));

        // Remove locale prefix if using localization
        if (_options.EnableLocalization && segments.Length > 0 && _options.SupportedLocales.Contains(segments[0]))
        {
            directoryPath = string.Join("/", segments.Skip(1).Take(segments.Length - 2));
        }

        // Combine with slug
        var url = string.IsNullOrEmpty(directoryPath) ? slug : $"{directoryPath}/{slug}";

        // Add root path if specified
        if (!string.IsNullOrEmpty(rootPath))
        {
            url = $"{rootPath.TrimEnd('/')}/{url}";
        }

        return url;
    }

    /// <summary>
    /// Determines if a path matches a glob pattern
    /// </summary>
    private bool MatchesGlobPattern(string path, string pattern)
    {
        var regex = new System.Text.RegularExpressions.Regex(
            "^" +
            System.Text.RegularExpressions.Regex.Escape(pattern)
                .Replace("\\*\\*", ".*")      // ** matches any number of directories
                .Replace("\\*", "[^/\\\\]*")  // * matches any number of characters except path separators
                .Replace("\\?", "[^/\\\\]")   // ? matches a single character except path separators
            + "$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return regex.IsMatch(path);
    }

    /// <summary>
    /// Converts a glob pattern to a regex pattern
    /// </summary>
    private string GlobPatternToRegex(string pattern)
    {
        return "^" +
            System.Text.RegularExpressions.Regex.Escape(pattern)
                .Replace("\\*\\*", ".*")      // ** matches any number of directories
                .Replace("\\*", "[^/\\\\]*")  // * matches any number of characters except path separators
                .Replace("\\?", "[^/\\\\]")   // ? matches a single character except path separators
            + "$";
    }

    /// <summary>
    /// Disposes of the resources used by the provider
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fileWatcher?.Dispose();
        }

        base.Dispose(disposing);
    }
}