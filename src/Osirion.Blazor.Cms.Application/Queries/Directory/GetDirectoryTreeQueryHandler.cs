using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Application.Queries.Directory;

/// <summary>
/// Handler for GetDirectoryTreeQuery
/// </summary>
public class GetDirectoryTreeQueryHandler : IQueryHandler<GetDirectoryTreeQuery, IReadOnlyList<DirectoryItem>>
{
    private readonly IContentProviderManager _providerManager;
    private readonly ILogger<GetDirectoryTreeQueryHandler> _logger;

    public GetDirectoryTreeQueryHandler(
        IContentProviderManager providerManager,
        ILogger<GetDirectoryTreeQueryHandler> logger)
    {
        _providerManager = providerManager ?? throw new ArgumentNullException(nameof(providerManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<DirectoryItem>> HandleAsync(GetDirectoryTreeQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting directory tree with locale: {Locale}", query.Locale ?? "all");

        var provider = query.ProviderId is not null
            ? _providerManager.GetProvider(query.ProviderId)
            : _providerManager.GetDefaultProvider();

        if (provider is null)
        {
            _logger.LogWarning("Provider not found: {ProviderId}", query.ProviderId ?? "default");
            return Array.Empty<DirectoryItem>();
        }

        try
        {
            return await provider.GetDirectoriesAsync(query.Locale, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting directory tree");
            throw;
        }
    }
}