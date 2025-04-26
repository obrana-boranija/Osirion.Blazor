using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Markdig;
using Osirion.Blazor.Cms.Models;
using Osirion.Blazor.Cms.Options;
using Osirion.Blazor.Cms.Providers.Internal;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Cms.Core.Providers.Base;

namespace Osirion.Blazor.Cms.Providers;

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
        : base(logger, memoryCache)
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
            throw new DirectoryNotFoundException($"Content directory not found: {_options.BasePath}");
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

        //var items = await GetOrCreateCachedAsync(cacheKey, async ct =>
        //{
        //    var contentItems = new List<ContentItem>();
        //    await ProcessDirectoryAsync(_options.BasePath, contentItems, ct);
        //    return contentItems.AsReadOnly();
        //}, cancellationToken) ?? Array.Empty<ContentItem>();

        return null;
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

        // Apply filters (same implementation as GitHubContentProvider)
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

    private async Task ProcessDirectoryAsync(string directory, List<ContentItem> contentItems, CancellationToken cancellationToken)
    {
        foreach (var file in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_options.SupportedExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
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
                    _logger.LogError(ex, "Error processing file: {FileName}", file);
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

            // Parse markdown and extract front matter
            var frontMatterEndIndex = content.IndexOf("---", 4);
            if (frontMatterEndIndex > 0)
            {
                var frontMatter = content.Substring(4, frontMatterEndIndex - 4).Trim();
                ParseFrontMatter(frontMatter, contentItem);

                var markdownContent = content.Substring(frontMatterEndIndex + 3).Trim();
                contentItem.Content = Markdown.ToHtml(markdownContent, _markdownPipeline);
            }
            else
            {
                contentItem.Content = Markdown.ToHtml(content, _markdownPipeline);
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

            contentItem.Url = GenerateUrl(contentItem.Path, contentItem.Slug, _options.BasePath);

            return contentItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process markdown file: {FilePath}", filePath);
            return null;
        }
    }

    private void SetupFileWatcher()
    {
        _fileWatcher = new FileSystemWatcher(_options.BasePath)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };

        foreach (var extension in _options.SupportedExtensions)
        {
            _fileWatcher.Filters.Add($"*{extension}");
        }

        _fileWatcher.Changed += OnFileChanged;
        _fileWatcher.Created += OnFileChanged;
        _fileWatcher.Deleted += OnFileChanged;
        _fileWatcher.Renamed += OnFileChanged;

        _fileWatcher.EnableRaisingEvents = true;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File system change detected: {ChangeType} - {Path}", e.ChangeType, e.FullPath);

        // Invalidate cache
        RefreshCacheAsync().GetAwaiter().GetResult();
    }

    private string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim('/');
    }

    private void ParseFrontMatter(string frontMatter, ContentItem contentItem)
    {
        foreach (var line in frontMatter.Split('\n'))
        {
            var parts = line.Split(':', 2);
            if (parts.Length != 2)
                continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            // Remove quotes if present
            if (value.StartsWith("\"") && value.EndsWith("\"") ||
                value.StartsWith("'") && value.EndsWith("'"))
            {
                value = value.Substring(1, value.Length - 2);
            }

            switch (key.ToLower())
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
                    contentItem.Tags = ParseList(value);
                    break;
                case "categories":
                case "category":
                    contentItem.Categories = ParseList(value);
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
                default:
                    contentItem.Metadata[key] = value;
                    break;
            }
        }
    }

    private List<string> ParseList(string value)
    {
        // Remove brackets if present
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            value = value.Substring(1, value.Length - 2);
        }

        return value
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim().Trim('\'').Trim('"'))
            .Where(v => !string.IsNullOrEmpty(v))
            .ToList();
    }

    /// <summary>
    /// Disposes of the resources used by the provider
    /// </summary>
    public void Dispose()
    {
        _fileWatcher?.Dispose();
    }
}