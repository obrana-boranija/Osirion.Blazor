using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Extensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.Providers;

public class GitHubContentProvider : ContentProviderBase
{
    private readonly GitHubContentRepository _contentRepository;
    private readonly GitHubDirectoryRepository _directoryRepository;
    private readonly GitHubOptions _options;
    private readonly IGitHubApiClient _apiClient;

    public GitHubContentProvider(
        GitHubContentRepository contentRepository,
        GitHubDirectoryRepository directoryRepository,
        IOptions<GitHubOptions> options,
        IGitHubApiClient apiClient,
        IMemoryCache memoryCache,
        ILogger<GitHubContentProvider> logger)
        : base(memoryCache, logger)
    {
        _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        _directoryRepository = directoryRepository ?? throw new ArgumentNullException(nameof(directoryRepository));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public override string ProviderId => _options.ProviderId ?? $"github-{_options.Owner}-{_options.Repository}";

    public override string DisplayName => $"GitHub: {_options.Owner}/{_options.Repository}";

    public override bool IsReadOnly => string.IsNullOrEmpty(_options.ApiToken);

    public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _contentRepository.GetAllAsync(cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _contentRepository.GetByIdAsync(id, cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        return await _contentRepository.GetByPathAsync(path, cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        return await _contentRepository.GetByUrlAsync(url, cancellationToken);
    }

    public override async Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        return await _contentRepository.FindByQueryAsync(query, cancellationToken);
    }

    public override async Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        return await _directoryRepository.GetByLocaleAsync(locale, cancellationToken);
    }

    public override async Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        return await _directoryRepository.GetByPathAsync(path, cancellationToken);
    }

    public override async Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _directoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public override async Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        return await _directoryRepository.GetByUrlAsync(url, cancellationToken);
    }

    public override async Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        // Get all content items
        var allContent = await GetAllItemsAsync(cancellationToken);

        // Extract and group categories
        return allContent
            .SelectMany(item => item.Categories)
            .GroupBy(category => category.ToLowerInvariant())
            .Select(group => ContentCategory.Create(
                group.First(),
                slug: group.First().GenerateSlug(),
                count: group.Count()))
            .OrderBy(c => c.Name)
            .ToList();
    }

    public override async Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        // Get all content items
        var allContent = await GetAllItemsAsync(cancellationToken);

        // Extract and group tags
        return allContent
            .SelectMany(item => item.Tags)
            .GroupBy(tag => tag.ToLowerInvariant())
            .Select(group => ContentTag.Create(
                group.First(),
                slug: group.First().GenerateSlug(),
                count: group.Count()))
            .OrderBy(t => t.Name)
            .ToList();
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Initializing GitHub content provider: {Owner}/{Repository}",
            _options.Owner, _options.Repository);

        // Configure API client
        _apiClient.SetRepository(_options.Owner, _options.Repository);
        _apiClient.SetBranch(_options.Branch);

        if (!string.IsNullOrEmpty(_options.ApiToken))
        {
            _apiClient.SetAccessToken(_options.ApiToken);
        }

        // Pre-load some data in the background
        _ = Task.Run(async () => {
            try
            {
                await _contentRepository.RefreshCacheAsync(cancellationToken);
                await _directoryRepository.RefreshCacheAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error pre-loading content for GitHub provider");
            }
        }, cancellationToken);

        await base.InitializeAsync(cancellationToken);
    }

    public override async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await _contentRepository.RefreshCacheAsync(cancellationToken);
        await _directoryRepository.RefreshCacheAsync(cancellationToken);
        await base.RefreshCacheAsync(cancellationToken);
    }
}