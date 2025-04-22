using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Markdig;
using Osirion.Blazor.Content.Models;
using Osirion.Blazor.Content.Options;
using Osirion.Blazor.Content.Providers.Internal;
using Osirion.Blazor.Content.Exceptions;

namespace Osirion.Blazor.Content.Providers;

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
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri("https://api.github.com/");
        }

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("OsirionBlazor", "2.0"));

        if (!string.IsNullOrEmpty(_options.ApiToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.ApiToken);
        }
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("content:all");

        //return await GetOrCreateCachedAsync(cacheKey, async ct =>
        //{
        //    var contents = await GetRepositoryContentsAsync(_options.ContentPath, ct);
        //    var contentItems = new List<ContentItem>();

        //    await ProcessContentsRecursivelyAsync(contents, contentItems, null, ct);

        //    return contentItems.AsReadOnly();
        //}, cancellationToken) ?? Array.Empty<ContentItem>();

        return null;
    }

    /// <inheritdoc/>
    public override async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        var items = await GetAllItemsAsync(cancellationToken);
        return items.FirstOrDefault(item =>
            item.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
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

        return contentItem;
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