using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Admin.Repositories;

/// <summary>
/// Repository for managing content using the selected provider
/// </summary>
public class AdminContentRepository : IContentRepository
{
    private readonly IContentProvider _provider;
    private readonly ILogger<AdminContentRepository> _logger;

    public AdminContentRepository(
        IContentProviderManager providerManager,
        ILogger<AdminContentRepository> logger)
    {
        _provider = providerManager.GetDefaultProvider() ??
            throw new InvalidOperationException("No default content provider configured");
        _logger = logger;
    }

    public Task<ContentItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ContentItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ContentItem>> FindByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ContentItem>> GetByDirectoryAsync(string directoryId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ContentItem>> GetTranslationsAsync(string contentId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ContentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ContentItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ContentItem> SaveAsync(ContentItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}