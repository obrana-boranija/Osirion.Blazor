using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Handler for SearchContentQuery
/// </summary>
public class SearchContentQueryHandler : IQueryHandler<SearchContentQuery, IReadOnlyList<ContentItem>>
{
    private readonly IContentProviderManager _providerManager;
    private readonly ILogger<SearchContentQueryHandler> _logger;

    public SearchContentQueryHandler(
        IContentProviderManager providerManager,
        ILogger<SearchContentQueryHandler> logger)
    {
        _providerManager = providerManager;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ContentItem>> HandleAsync(SearchContentQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Searching content with query criteria");

        var provider = query.ProviderId != null
            ? _providerManager.GetProvider(query.ProviderId)
            : _providerManager.GetDefaultProvider();

        if (provider == null)
        {
            _logger.LogWarning("Provider not found: {ProviderId}", query.ProviderId ?? "default");
            return Array.Empty<ContentItem>();
        }

        try
        {
            return await provider.GetItemsByQueryAsync(query.Query, cancellationToken) ?? Array.Empty<ContentItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching content");
            throw;
        }
    }
}