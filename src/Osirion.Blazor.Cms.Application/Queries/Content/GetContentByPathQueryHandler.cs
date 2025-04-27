using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Handler for GetContentByPathQuery
/// </summary>
public class GetContentByPathQueryHandler : IQueryHandler<GetContentByPathQuery, ContentItem?>
{
    private readonly IContentProviderManager _providerManager;
    private readonly ILogger<GetContentByPathQueryHandler> _logger;

    public GetContentByPathQueryHandler(
        IContentProviderManager providerManager,
        ILogger<GetContentByPathQueryHandler> logger)
    {
        _providerManager = providerManager;
        _logger = logger;
    }

    public async Task<ContentItem?> HandleAsync(GetContentByPathQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting content by path: {Path}", query.Path);

        var provider = query.ProviderId != null
            ? _providerManager.GetProvider(query.ProviderId)
            : _providerManager.GetDefaultProvider();

        if (provider == null)
        {
            _logger.LogWarning("Provider not found: {ProviderId}", query.ProviderId ?? "default");
            return null;
        }

        try
        {
            return await provider.GetItemByPathAsync(query.Path, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content by path: {Path}", query.Path);
            throw;
        }
    }
}