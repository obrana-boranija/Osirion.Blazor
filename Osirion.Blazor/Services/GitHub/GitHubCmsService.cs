using Markdig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Models.Cms;
using Osirion.Blazor.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Services.GitHub;

/// <summary>
/// Service for managing content from a GitHub repository
/// </summary>
public class GitHubCmsService : IGitHubCmsService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<GitHubCmsOptions> _options;
    private readonly ILogger<GitHubCmsService> _logger;
    private readonly MarkdownPipeline _markdownPipeline;

    // Static cache to improve performance
    private static List<ContentItem>? _cachedContent;
    private static DateTime _cacheExpiry = DateTime.MinValue;
    private static readonly SemaphoreSlim _cacheLock = new(1, 1);

    private GitHubCmsOptions Options => _options.Value;

    public GitHubCmsService(
        HttpClient httpClient,
        IOptions<GitHubCmsOptions> options,
        ILogger<GitHubCmsService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Setup GitHub API client
        _httpClient.BaseAddress = new Uri("https://api.github.com/");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OsirionCms", "1.0"));

        // Add GitHub token if provided
        if (!string.IsNullOrEmpty(Options.ApiToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Options.ApiToken);
        }

        // Setup Markdown processor
        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();
    }

    public async Task<List<ContentItem>> GetAllContentItemsAsync()
    {
        if (_cachedContent != null && DateTime.Now < _cacheExpiry)
        {
            return _cachedContent;
        }

        await _cacheLock.WaitAsync();
        try
        {
            // Double-check pattern
            if (_cachedContent != null && DateTime.Now < _cacheExpiry)
            {
                return _cachedContent;
            }

            _logger.LogInformation("Refreshing content from GitHub repository");
            var contentItems = new List<ContentItem>();

            // Get contents of the repository
            var contents = await GetRepositoryContentsAsync(Options.ContentPath);

            // Process all markdown files
            await ProcessContentsAsync(contents, contentItems);

            // Sort by date (newest first)
            contentItems = contentItems.OrderByDescending(c => c.Date).ToList();

            // Update cache
            _cachedContent = contentItems;
            _cacheExpiry = DateTime.Now.AddMinutes(Options.CacheDurationMinutes);
            _logger.LogInformation("Successfully cached {Count} content items from GitHub", contentItems.Count);

            return contentItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching content from GitHub");
            return _cachedContent ?? new List<ContentItem>();
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<ContentItem?> GetContentItemByPathAsync(string path)
    {
        var contentItems = await GetAllContentItemsAsync();
        return contentItems.FirstOrDefault(c => c.GitHubFilePath.Equals(path, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<ContentItem>> GetContentItemsByDirectoryAsync(string directory)
    {
        var contentItems = await GetAllContentItemsAsync();
        return contentItems.Where(c => c.Directory.Equals(directory, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<ContentCategory>> GetCategoriesAsync()
    {
        var contentItems = await GetAllContentItemsAsync();

        return contentItems
            .SelectMany(c => c.Categories)
            .GroupBy(category => category.ToLower())
            .Select(g => new ContentCategory
            {
                Name = g.First(),
                SlugUrl = g.Key.Replace(' ', '-'),
                ContentCount = g.Count()
            })
            .OrderBy(c => c.Name)
            .ToList();
    }

    public async Task<List<ContentItem>> GetContentItemsByCategoryAsync(string category)
    {
        var contentItems = await GetAllContentItemsAsync();
        return contentItems
            .Where(c => c.Categories.Any(cat =>
                cat.ToLower().Replace(' ', '-') == category.ToLower()))
            .ToList();
    }

    public async Task<List<ContentTag>> GetTagsAsync()
    {
        var contentItems = await GetAllContentItemsAsync();

        return contentItems
            .SelectMany(c => c.Tags)
            .GroupBy(tag => tag.ToLower())
            .Select(g => new ContentTag
            {
                Name = g.First(),
                SlugUrl = g.Key.Replace(' ', '-'),
                ContentCount = g.Count()
            })
            .OrderBy(t => t.Name)
            .ToList();
    }

    public async Task<List<ContentItem>> GetContentItemsByTagAsync(string tag)
    {
        var contentItems = await GetAllContentItemsAsync();
        return contentItems
            .Where(c => c.Tags.Any(t =>
                t.ToLower().Replace(' ', '-') == tag.ToLower()))
            .ToList();
    }

    public async Task<List<ContentItem>> GetFeaturedContentItemsAsync(int count = 3)
    {
        var contentItems = await GetAllContentItemsAsync();

        // First get items marked as featured
        var featured = contentItems
            .Where(c => c.IsFeatured)
            .Take(count)
            .ToList();

        // If we don't have enough featured items, add the most recent ones
        if (featured.Count < count)
        {
            featured.AddRange(
                contentItems
                    .Where(c => !c.IsFeatured && !featured.Contains(c))
                    .Take(count - featured.Count)
            );
        }

        return featured;
    }

    public async Task<List<ContentItem>> SearchContentItemsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<ContentItem>();

        var contentItems = await GetAllContentItemsAsync();
        var searchTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return contentItems
            .Where(c =>
                searchTerms.Any(term =>
                    c.Title.ToLower().Contains(term) ||
                    c.Description.ToLower().Contains(term) ||
                    c.Content.ToLower().Contains(term) ||
                    c.Categories.Any(cat => cat.ToLower().Contains(term)) ||
                    c.Tags.Any(tag => tag.ToLower().Contains(term))
                )
            )
            .ToList();
    }

    public async Task RefreshCacheAsync()
    {
        _cacheExpiry = DateTime.MinValue;
        await GetAllContentItemsAsync();
    }

    private async Task<List<GitHubContent>> GetRepositoryContentsAsync(string path)
    {
        var url = $"repos/{Options.Owner}/{Options.Repository}/contents/{path}?ref={Options.Branch}";
        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<GitHubContent>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<GitHubContent>();
    }

    private async Task ProcessContentsAsync(List<GitHubContent> contents, List<ContentItem> contentItems, string? currentDirectory = null)
    {
        foreach (var item in contents)
        {
            // Process markdown files
            if (item.Type == "file" && Options.SupportedExtensions.Any(ext => item.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var fileInfo = await GetFileInfoAsync(item.Path);
                    var contentItem = await ProcessMarkdownFileAsync(item, fileInfo, currentDirectory);

                    if (contentItem != null)
                    {
                        contentItems.Add(contentItem);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file {FileName}", item.Name);
                }
            }
            // Process directories recursively
            else if (item.Type == "dir")
            {
                var subContents = await GetRepositoryContentsAsync(item.Path);
                await ProcessContentsAsync(subContents, contentItems, item.Name);
            }
        }
    }

    private async Task<GitHubFileInfo> GetFileInfoAsync(string path)
    {
        // Get commit information for the file
        var url = $"repos/{Options.Owner}/{Options.Repository}/commits?path={path}&page=1&per_page=1";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var commitsJson = await response.Content.ReadAsStringAsync();
        var commits = JsonSerializer.Deserialize<List<GitHubCommit>>(commitsJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<GitHubCommit>();

        // Get all commits to find creation date
        url = $"repos/{Options.Owner}/{Options.Repository}/commits?path={path}&per_page=100";
        response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        commitsJson = await response.Content.ReadAsStringAsync();
        var allCommits = JsonSerializer.Deserialize<List<GitHubCommit>>(commitsJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<GitHubCommit>();

        // The last commit in the list is the oldest/first one
        var firstCommit = allCommits.LastOrDefault();
        var lastCommit = commits.FirstOrDefault();

        return new GitHubFileInfo
        {
            Path = path,
            CreatedDate = firstCommit?.Commit?.Author?.Date ?? DateTime.Now,
            LastUpdatedDate = lastCommit?.Commit?.Author?.Date ?? DateTime.Now
        };
    }

    private async Task<string> GetFileContentAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var fileContent = JsonSerializer.Deserialize<GitHubFileContent>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // GitHub API returns content as base64 encoded
        if (fileContent != null && !string.IsNullOrEmpty(fileContent.Content))
        {
            // Remove newlines from base64 string
            var base64Content = fileContent.Content.Replace("\n", "");
            byte[] bytes = Convert.FromBase64String(base64Content);
            return Encoding.UTF8.GetString(bytes);
        }

        return string.Empty;
    }

    private async Task<ContentItem?> ProcessMarkdownFileAsync(GitHubContent item, GitHubFileInfo fileInfo, string? directory)
    {
        var content = await GetFileContentAsync(item.Url);
        if (string.IsNullOrEmpty(content))
            return null;

        var contentItem = new ContentItem
        {
            GitHubFilePath = item.Path,
            Directory = directory ?? Path.GetDirectoryName(item.Path) ?? string.Empty
        };

        // Extract front matter
        var frontMatterEndIndex = content.IndexOf("---", 4);
        if (frontMatterEndIndex > 0)
        {
            var frontMatter = content.Substring(4, frontMatterEndIndex - 4).Trim();

            // Parse front matter
            foreach (var line in frontMatter.Split('\n'))
            {
                var parts = line.Split(':', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                // Remove quotes if present
                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Substring(1, value.Length - 2);
                }
                else if (value.StartsWith("'") && value.EndsWith("'"))
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
                    case "keywords":
                        contentItem.Keywords = ParseList(value);
                        break;
                    case "categories":
                    case "category":
                        contentItem.Categories = ParseList(value);
                        break;
                    case "slug":
                        contentItem.Slug = value;
                        break;
                    case "is_featured":
                    case "featured":
                        contentItem.IsFeatured = bool.TryParse(value, out var featured) && featured;
                        break;
                    case "featured_image":
                    case "image":
                        contentItem.FeaturedImageUrl = value;
                        break;
                }
            }

            // Extract main content (after front matter)
            var mainContent = content.Substring(frontMatterEndIndex + 3).Trim();

            // Convert markdown to HTML
            contentItem.Content = Markdig.Markdown.ToHtml(mainContent, _markdownPipeline);

            // Generate ID based on file path
            contentItem.Id = item.Path.GetHashCode().ToString("x");

            // Use filename as slug if not specified
            if (string.IsNullOrEmpty(contentItem.Slug))
            {
                contentItem.Slug = Path.GetFileNameWithoutExtension(item.Name);
            }

            // Use GitHub file dates
            contentItem.CreatedDate = fileInfo.CreatedDate;
            contentItem.LastUpdatedDate = fileInfo.LastUpdatedDate;

            // If date not specified in frontmatter, use created date
            if (contentItem.Date == default)
            {
                contentItem.Date = contentItem.CreatedDate;
            }

            // Estimate read time
            var wordCount = mainContent.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            contentItem.ReadTimeMinutes = Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));

            return contentItem;
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
}