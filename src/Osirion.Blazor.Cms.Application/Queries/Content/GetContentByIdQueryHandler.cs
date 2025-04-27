using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Handler for GetContentByIdQuery
/// </summary>
public class GetContentByIdQueryHandler : IQueryHandler<GetContentByIdQuery, ContentItem?>
{
    private readonly IContentProviderManager _providerManager;
    private readonly ILogger<GetContentByIdQueryHandler> _logger;

    public GetContentByIdQueryHandler(
        IContentProviderManager providerManager,
        ILogger<GetContentByIdQueryHandler> logger)
    {
        _providerManager = providerManager;
        _logger = logger;
    }

    public async Task<ContentItem?> HandleAsync(GetContentByIdQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting content by ID: {Id}", query.Id);

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
            return await provider.GetItemByIdAsync(query.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content by ID: {Id}", query.Id);
            throw;
        }
    }
}