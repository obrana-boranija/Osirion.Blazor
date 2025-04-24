using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Markdig;
using Osirion.Blazor.Cms.Models;
using Osirion.Blazor.Cms.Options;
using Osirion.Blazor.Cms.Providers.Internal;
using Osirion.Blazor.Cms.Exceptions;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Providers;

/// <summary>
/// Content provider for GitHub repositories
/// </summary>
public class GitHubContentProvider : ContentProviderBase
{
    private readonly HttpClient _httpClient;
    private readonly GitHubContentOptions _options;
    private readonly MarkdownPipeline _markdownPipeline;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubContentProvider"/> class.
    /// </summary>
    public GitHubContentProvider(
       HttpClient httpClient,
       IOptions<GitHubContentOptions> options,
       ILogger<GitHubContentProvider> logger,
       IMemoryCache memoryCache)
       : base(logger, memoryCache)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();
    }

    /// <inheritdoc/>
    public override Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // No initialization needed
        return Task.CompletedTask;
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
    public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("content:all");
        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var contents = await GetRepositoryContentsAsync(_options.ContentPath, ct);
            var contentItems = new List<ContentItem>();
            await ProcessContentsRecursivelyAsync(contents, contentItems, null, ct);
            return contentItems.AsReadOnly();
        }, cancellationToken) ?? Array.Empty<ContentItem>().ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public override async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        var items = await GetAllItemsAsync(cancellationToken);
        return items.FirstOrDefault(item =>
            item.Path.Contains(path, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        var items = await GetAllItemsAsync(cancellationToken);
        var filteredItems = items.AsQueryable();

        if (!string.IsNullOrEmpty(query.Directory))
        {
            filteredItems = filteredItems.Where(item =>
                item.Path.StartsWith(query.Directory, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(query.Category))
        {
            filteredItems = filteredItems.Where(item =>
                item.Categories.Any(c => c.Contains(query.Category, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrEmpty(query.Tag))
        {
            filteredItems = filteredItems.Where(item =>
                item.Tags.Any(t => t.Contains(query.Tag, StringComparison.OrdinalIgnoreCase)));
        }

        if (query.IsFeatured.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.IsFeatured == query.IsFeatured.Value);
        }

        if (query.DateFrom.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.Date >= query.DateFrom.Value);
        }

        if (query.DateTo.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.Date <= query.DateTo.Value);
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
                filteredItems.OrderBy(item => item.LastModified ?? item.Date) :
                filteredItems.OrderByDescending(item => item.LastModified ?? item.Date),
            _ => query.SortDirection == SortDirection.Ascending ?
                filteredItems.OrderBy(item => item.Date) :
                filteredItems.OrderByDescending(item => item.Date)
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

    private async Task<List<GitHubContent>> GetRepositoryContentsAsync(string path, CancellationToken cancellationToken)
    {
        var url = $"repos/{_options.Owner}/{_options.Repository}/contents/{path}?ref={_options.Branch}";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<List<GitHubContent>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<GitHubContent>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch repository contents at path: {Path}", path);
            throw new ContentProviderException("Failed to fetch GitHub content", ex);
        }
    }

    private async Task ProcessContentsRecursivelyAsync(
        List<GitHubContent> contents,
        List<ContentItem> contentItems,
        string? currentDirectory,
        CancellationToken cancellationToken)
    {
        foreach (var item in contents)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (item.Type == "file" && _options.SupportedExtensions.Any(ext => item.Name.EndsWith(ext)))
            {
                try
                {
                    var contentItem = await ProcessMarkdownFileAsync(item, currentDirectory, cancellationToken);
                    if (contentItem != null)
                    {
                        contentItems.Add(contentItem);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file: {FileName}", item.Name);
                }
            }
            else if (item.Type == "dir")
            {
                var subContents = await GetRepositoryContentsAsync(item.Path, cancellationToken);
                await ProcessContentsRecursivelyAsync(subContents, contentItems, item.Name, cancellationToken);
            }
        }
    }

    private async Task<ContentItem?> ProcessMarkdownFileAsync(
        GitHubContent item,
        string? directory,
        CancellationToken cancellationToken)
    {
        var content = await GetFileContentAsync(item.Url, cancellationToken);
        if (string.IsNullOrEmpty(content))
            return null;

        // Skip _index.md files as they're for directory metadata
        if (item.Name == "_index.md")
            return null;

        var contentItem = new ContentItem
        {
            Id = item.Path.GetHashCode().ToString("x"),
            Path = item.Path,
            ProviderId = ProviderId,
            ProviderSpecificId = item.Sha
        };

        var fileInfo = await GetFileInfoAsync(item.Path, cancellationToken);
        contentItem.Date = fileInfo.CreatedDate;
        contentItem.LastModified = fileInfo.LastModifiedDate;

        // Extract locale from path
        contentItem.Locale = ExtractLocaleFromPath(item.Path);

        // Parse markdown and extract front matter
        var frontMatterEndIndex = content.IndexOf("---", 4);
        if (frontMatterEndIndex > 0)
        {
            var frontMatter = content.Substring(4, frontMatterEndIndex - 4).Trim();
            ParseFrontMatter(frontMatter, contentItem);

            var markdownContent = content.Substring(frontMatterEndIndex + 3).Trim();
            contentItem.Content = Markdig.Markdown.ToHtml(markdownContent, _markdownPipeline);
        }
        else
        {
            contentItem.Content = Markdig.Markdown.ToHtml(content, _markdownPipeline);
        }

        // Set defaults if not provided
        if (string.IsNullOrEmpty(contentItem.Title))
        {
            contentItem.Title = Path.GetFileNameWithoutExtension(item.Name);
        }

        if (string.IsNullOrEmpty(contentItem.Slug))
        {
            contentItem.Slug = GenerateSlug(contentItem.Title);
        }

        // Set parent directory reference
        var directoryStructure = await BuildDirectoryStructureAsync(cancellationToken);
        var parentDir = FindParentDirectory(directoryStructure, item.Path);
        if (parentDir != null)
        {
            contentItem.Directory = parentDir;
            parentDir.Items.Add(contentItem);
        }

        return contentItem;
    }

    private DirectoryItem? FindParentDirectory(IEnumerable<DirectoryItem> directories, string filePath)
    {
        string? fileDirectory = Path.GetDirectoryName(filePath)?.Replace('\\', '/');
        if (string.IsNullOrEmpty(fileDirectory))
            return null;

        return FindDirectoryByPath(directories, fileDirectory);
    }

    private async Task<string> GetFileContentAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var fileContent = JsonSerializer.Deserialize<GitHubFileContent>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (fileContent != null && !string.IsNullOrEmpty(fileContent.Content))
            {
                var base64Content = fileContent.Content.Replace("\n", "");
                byte[] bytes = Convert.FromBase64String(base64Content);
                return Encoding.UTF8.GetString(bytes);
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file content from URL: {Url}", url);
            throw new ContentProviderException("Failed to get file content", ex);
        }
    }

    private async Task<GitHubFileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken)
    {
        var url = $"repos/{_options.Owner}/{_options.Repository}/commits?path={path}&page=1&per_page=1";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var commitsJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var commits = JsonSerializer.Deserialize<List<GitHubCommit>>(commitsJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<GitHubCommit>();

            var lastCommit = commits.FirstOrDefault();

            // Get creation date - fetch all commits
            url = $"repos/{_options.Owner}/{_options.Repository}/commits?path={path}&per_page=100";
            response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            commitsJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var allCommits = JsonSerializer.Deserialize<List<GitHubCommit>>(commitsJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<GitHubCommit>();

            var firstCommit = allCommits.LastOrDefault();

            return new GitHubFileInfo
            {
                Path = path,
                CreatedDate = firstCommit?.Commit?.Author?.Date ?? DateTime.UtcNow,
                LastModifiedDate = lastCommit?.Commit?.Author?.Date ?? DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file info for path: {Path}", path);
            return new GitHubFileInfo
            {
                Path = path,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
        }
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
                // Basic metadata
                case "title":
                    contentItem.Title = value;
                    break;
                case "author":
                    contentItem.Author = value;
                    break;
                case "date":
                    if (DateTime.TryParse(value, out var date))
                        contentItem.Date = date;
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

                // Localization
                case "locale":
                    contentItem.Locale = value;
                    break;
                case "localization_id":
                    contentItem.LocalizationId = value;
                    break;

                // SEO metadata
                case "meta_title":
                    contentItem.Seo.MetaTitle = value;
                    break;
                case "meta_description":
                    contentItem.Seo.MetaDescription = value;
                    break;
                case "canonical_url":
                    contentItem.Seo.CanonicalUrl = value;
                    break;
                case "robots":
                    contentItem.Seo.Robots = value;
                    break;
                case "og_title":
                    contentItem.Seo.OgTitle = value;
                    break;
                case "og_description":
                    contentItem.Seo.OgDescription = value;
                    break;
                case "og_image":
                    contentItem.Seo.OgImageUrl = value;
                    break;
                case "og_type":
                    contentItem.Seo.OgType = value;
                    break;
                case "twitter_card":
                    contentItem.Seo.TwitterCard = value;
                    break;
                case "twitter_title":
                    contentItem.Seo.TwitterTitle = value;
                    break;
                case "twitter_description":
                    contentItem.Seo.TwitterDescription = value;
                    break;
                case "twitter_image":
                    contentItem.Seo.TwitterImageUrl = value;
                    break;
                case "schema_type":
                    contentItem.Seo.SchemaType = value;
                    break;
                case "json_ld":
                    contentItem.Seo.JsonLd = value;
                    break;

                // Other metadata
                default:
                    contentItem.Metadata[key] = value;
                    break;
            }
        }

        // Set defaults for SEO fields if not provided
        if (string.IsNullOrEmpty(contentItem.Seo.MetaTitle))
        {
            contentItem.Seo.MetaTitle = contentItem.Title;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.MetaDescription))
        {
            contentItem.Seo.MetaDescription = contentItem.Description;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.OgTitle))
        {
            contentItem.Seo.OgTitle = contentItem.Seo.MetaTitle;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.OgDescription))
        {
            contentItem.Seo.OgDescription = contentItem.Seo.MetaDescription;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.TwitterTitle))
        {
            contentItem.Seo.TwitterTitle = contentItem.Seo.OgTitle;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.TwitterDescription))
        {
            contentItem.Seo.TwitterDescription = contentItem.Seo.OgDescription;
        }
    }

    private async Task<IReadOnlyList<DirectoryItem>> BuildDirectoryStructureAsync(CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey("directories:all");
        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var rootDirectories = new List<DirectoryItem>();
            var allDirectories = new Dictionary<string, DirectoryItem>();

            // Get all content to analyze directory structure
            var contents = await GetRepositoryContentsAsync(_options.ContentPath, ct);
            await ProcessDirectoriesRecursivelyAsync(contents, rootDirectories, allDirectories, null, ct);

            // Process _index.md files for directory metadata
            await ProcessDirectoryMetadataAsync(allDirectories, ct);

            return rootDirectories.AsReadOnly();
        }, cancellationToken) ?? Array.Empty<DirectoryItem>().ToList().AsReadOnly();
    }

    private async Task ProcessDirectoriesRecursivelyAsync(
        List<GitHubContent> contents,
        List<DirectoryItem> parentDirectories,
        Dictionary<string, DirectoryItem> allDirectories,
        DirectoryItem? parentDirectory,
        CancellationToken cancellationToken)
    {
        // Group contents by directory
        var directoryContents = contents
            .Where(item => item.Type == "dir")
            .OrderBy(item => item.Name)
            .ToList();

        // Process each directory
        foreach (var dir in directoryContents)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Extract locale from path if present
            string locale = ExtractLocaleFromPath(dir.Path);

            var directory = new DirectoryItem
            {
                Path = dir.Path,
                Name = dir.Name,
                Locale = locale,
                Parent = parentDirectory
            };

            // Add to collections
            parentDirectories.Add(directory);
            allDirectories[dir.Path] = directory;

            // Process subdirectories
            var subContents = await GetRepositoryContentsAsync(dir.Path, cancellationToken);
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
                var indexFilePath = $"{path}/_index.md";
                var indexContents = await GetRepositoryContentsAsync(indexFilePath, cancellationToken);
                var indexFile = indexContents.FirstOrDefault(c => c.Name == "_index.md");

                if (indexFile != null)
                {
                    var indexContent = await GetFileContentAsync(indexFile.Url, cancellationToken);
                    if (!string.IsNullOrEmpty(indexContent))
                    {
                        // Parse front matter
                        var frontMatter = ExtractFrontMatter(indexContent);
                        if (frontMatter != null)
                        {
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
        else
        {
            // Generate an ID if not provided
            directory.Id = directory.Path.GetHashCode().ToString("x");
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

        // Process other metadata
        foreach (var (key, value) in frontMatter)
        {
            if (!new[] { "id", "title", "description", "order" }.Contains(key))
            {
                directory.Metadata[key] = value;
            }
        }
    }

    private string ExtractLocaleFromPath(string path)
    {
        // Extract locale from path format like "en/blog" or "es/articles"
        var segments = path.Split('/');
        if (segments.Length >= 2 && IsValidLocale(segments[0]))
        {
            return segments[0];
        }
        return "en"; // Default locale
    }

    private bool IsValidLocale(string locale)
    {
        // Simple validation for common locale codes
        return locale.Length == 2 && locale.All(char.IsLetter);
    }

    private Dictionary<string, string> ExtractFrontMatter(string content)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Look for front matter between --- delimiters
        var frontMatterRegex = new Regex(@"^\s*---\s*\n(.*?)\n\s*---\s*\n", RegexOptions.Singleline);
        var match = frontMatterRegex.Match(content);

        if (match.Success && match.Groups.Count > 1)
        {
            var frontMatterContent = match.Groups[1].Value;
            var lines = frontMatterContent.Split('\n');

            foreach (var line in lines)
            {
                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // Remove quotes if present
                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    result[key] = value;
                }
            }
        }

        return result;
    }

    public override async Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var allDirectories = await BuildDirectoryStructureAsync(cancellationToken);

        if (!string.IsNullOrEmpty(locale))
        {
            // Filter directories by locale
            return allDirectories
                .Where(d => d.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .AsReadOnly();
        }

        return allDirectories;
    }

    public override async Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        var allDirectories = await BuildDirectoryStructureAsync(cancellationToken);
        return FindDirectoryByPath(allDirectories, path);
    }

    public override async Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default)
    {
        var allDirectories = await BuildDirectoryStructureAsync(cancellationToken);
        return FindDirectoryById(allDirectories, id, locale);
    }

    public override async Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("localization:info");
        return await GetOrCreateCachedAsync(cacheKey, async ct =>
        {
            var localizationInfo = new LocalizationInfo
            {
                DefaultLocale = "en" // Default locale
            };

            // Get all content items to analyze translations
            var allItems = await GetAllItemsAsync(ct);

            // Group by localization ID and build translations map
            var itemsByLocalizationId = allItems
                .Where(item => !string.IsNullOrEmpty(item.LocalizationId))
                .GroupBy(item => item.LocalizationId);

            foreach (var group in itemsByLocalizationId)
            {
                var translations = new Dictionary<string, string>();

                foreach (var item in group)
                {
                    if (!string.IsNullOrEmpty(item.Locale))
                    {
                        translations[item.Locale] = item.Path;

                        if (!localizationInfo.AvailableLocales.Contains(item.Locale))
                        {
                            localizationInfo.AvailableLocales.Add(item.Locale);
                        }
                    }
                }

                if (translations.Count > 0)
                {
                    localizationInfo.Translations[group.Key] = translations;
                }
            }

            return localizationInfo;
        }, cancellationToken);
    }

    public override async Task<IReadOnlyList<ContentItem>> GetContentTranslationsAsync(string localizationId, CancellationToken cancellationToken = default)
    {
        var allItems = await GetAllItemsAsync(cancellationToken);

        return allItems
            .Where(item => item.LocalizationId == localizationId)
            .ToList()
            .AsReadOnly();
    }

    private DirectoryItem? FindDirectoryByPath(IEnumerable<DirectoryItem> directories, string path)
    {
        foreach (var directory in directories)
        {
            if (directory.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                return directory;
            }

            var childResult = FindDirectoryByPath(directory.Children, path);
            if (childResult != null)
            {
                return childResult;
            }
        }

        return null;
    }

    private DirectoryItem? FindDirectoryById(IEnumerable<DirectoryItem> directories, string id, string? locale = null)
    {
        foreach (var directory in directories)
        {
            if (directory.Id == id && (locale == null || directory.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase)))
            {
                return directory;
            }

            var childResult = FindDirectoryById(directory.Children, id, locale);
            if (childResult != null)
            {
                return childResult;
            }
        }

        return null;
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

    private class GitHubContent
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Sha { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    private class GitHubFileContent
    {
        public string Content { get; set; } = string.Empty;
    }

    private class GitHubCommit
    {
        public GitHubCommitDetail? Commit { get; set; }
    }

    private class GitHubCommitDetail
    {
        public GitHubCommitAuthor? Author { get; set; }
    }

    private class GitHubCommitAuthor
    {
        public DateTime Date { get; set; }
    }

    private class GitHubFileInfo
    {
        public string Path { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}