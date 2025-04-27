using Microsoft.Extensions.Configuration;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Providers.Base;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

public class GitHubContentProvider : BaseContentProvider,
          IReadContentProvider,
          IDirectoryContentProvider,
          IQueryContentProvider
{
    private readonly IGitHubApiClient _apiClient;
    private readonly GitHubContentRepository _contentRepository;
    private readonly GitHubDirectoryRepository _directoryRepository;
    private readonly GitHubOptions _options;

    public GitHubContentProvider(IConfiguration config)
        : base(config) { }

    public string ProviderId => $"github-{RootFolder.GetHashCode()}";

    ///// <inheritdoc/>
    //public override string ProviderId => _options.ProviderId ?? $"github-{_options.Owner}-{_options.Repository}";

    ///// <inheritdoc/>
    //public override string DisplayName => $"GitHub: {_options.Owner}/{_options.Repository}";

    ///// <inheritdoc/>
    //public override bool IsReadOnly => string.IsNullOrEmpty(_options.ApiToken);

    ///// <inheritdoc/>
    //public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    //{
    //    return await _contentRepository.GetAllAsync(cancellationToken);
    //}

    ///// <inheritdoc/>
    //public override async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    //{
    //    return await _contentRepository.GetByPathAsync(path, cancellationToken);
    //}

    ///// <inheritdoc/>
    //public override async Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default)
    //{
    //    return await _contentRepository.GetByUrlAsync(url, cancellationToken);
    //}

    ///// <inheritdoc/>
    //public override async Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    //{
    //    return await _contentRepository.FindByQueryAsync(query, cancellationToken);
    //}

    ///// <inheritdoc/>
    //public override async Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    //{
    //    return await _directoryRepository.GetByLocaleAsync(locale, cancellationToken);
    //}

    ///// <inheritdoc/>
    //public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    //{
    //    Logger.LogInformation("Initializing GitHub content provider: {Owner}/{Repository}",
    //        _options.Owner, _options.Repository);

    //    // Configure API client
    //    _apiClient.SetRepository(_options.Owner, _options.Repository);
    //    _apiClient.SetBranch(_options.Branch);

    //    if (!string.IsNullOrEmpty(_options.ApiToken))
    //    {
    //        _apiClient.SetAccessToken(_options.ApiToken);
    //    }

    //    // Pre-load some data in the background
    //    _ = Task.Run(async () => {
    //        try
    //        {
    //            await _contentRepository.GetAllAsync(cancellationToken);
    //            await _directoryRepository.GetAllAsync(cancellationToken);
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.LogError(ex, "Error pre-loading content for GitHub provider");
    //        }
    //    }, cancellationToken);

    //    await base.InitializeAsync(cancellationToken);
    //}

    ///// <inheritdoc/>
    //public override async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    //{
    //    await _contentRepository.RefreshCacheAsync(cancellationToken);
    //    await _directoryRepository.RefreshCacheAsync(cancellationToken);
    //    await base.RefreshCacheAsync(cancellationToken);
    //}

    public Task<ContentItem> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ContentItem>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetDirectoriesAsync(string path)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ContentItem>> QueryAsync(ContentQuery filter)
    {
        throw new NotImplementedException();
    }
}