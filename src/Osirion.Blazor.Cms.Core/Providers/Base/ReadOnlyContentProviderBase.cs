using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Interfaces;

namespace Osirion.Blazor.Cms.Providers.Base;

/// <summary>
/// Base class for read-only content providers
/// </summary>
public abstract class ReadOnlyContentProviderBase : ContentProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyContentProviderBase"/> class.
    /// </summary>
    protected ReadOnlyContentProviderBase(
        IMemoryCache cacheService,
        ILogger logger)
        : base(cacheService, logger)
    {
    }

    /// <inheritdoc/>
    public override bool IsReadOnly => true;
}