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

    /// <summary>Initializes a repository using the configured default provider.</summary>
    public AdminContentRepository(
        IContentProviderManager providerManager,
        ILogger<AdminContentRepository> logger)
    {
        _provider = providerManager.GetDefaultProvider() ??
            throw new InvalidOperationException("No default content provider configured");
        _logger = logger;
    }

    /// <summary>Gets content by path.</summary>
    public Task<ContentItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Gets content by URL.</summary>
    public Task<ContentItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Finds content matching a query.</summary>
    public Task<IReadOnlyList<ContentItem>?> FindByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Gets content in a directory.</summary>
    public Task<IReadOnlyList<ContentItem>> GetByDirectoryAsync(string directoryId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Gets translations for content.</summary>
    public Task<IReadOnlyList<ContentItem>> GetTranslationsAsync(string contentId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Gets content tags.</summary>
    public Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Saves content with a commit message.</summary>
    public Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Deletes content with a commit message.</summary>
    public Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Refreshes the content cache.</summary>
    public Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Gets all content.</summary>
    public Task<IReadOnlyList<ContentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Gets content by identifier.</summary>
    public Task<ContentItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Saves content.</summary>
    public Task<ContentItem> SaveAsync(ContentItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>Deletes content by identifier.</summary>
    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
