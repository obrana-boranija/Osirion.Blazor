using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Application.Queries.Directory;

/// <summary>
/// Handler for GetDirectoryByPathQuery
/// </summary>
public class GetDirectoryByPathQueryHandler : IQueryHandler<GetDirectoryByPathQuery, DirectoryItem?>
{
    private readonly IContentProviderManager _providerManager;
    private readonly ILogger<GetDirectoryByPathQueryHandler> _logger;

    public GetDirectoryByPathQueryHandler(
        IContentProviderManager providerManager,
        ILogger<GetDirectoryByPathQueryHandler> logger)
    {
        _providerManager = providerManager ?? throw new ArgumentNullException(nameof(providerManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DirectoryItem?> HandleAsync(GetDirectoryByPathQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting directory by path: {Path}", query.Path);

        var provider = query.ProviderId is not null
            ? _providerManager.GetProvider(query.ProviderId)
            : _providerManager.GetDefaultProvider();

        if (provider is null)
        {
            _logger.LogWarning("Provider not found: {ProviderId}", query.ProviderId ?? "default");
            return null;
        }

        try
        {
            return await provider.GetDirectoryByPathAsync(query.Path, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting directory by path: {Path}", query.Path);
            throw;
        }
    }
}