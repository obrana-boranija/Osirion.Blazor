using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Default provider setter that uses a provider ID
/// </summary>
public class IdDefaultProviderSetter : IDefaultProviderSetter
{
    private readonly string _providerId;

    /// <summary>
    /// Initializes a new instance of the IdDefaultProviderSetter class
    /// </summary>
    public IdDefaultProviderSetter(string providerId)
    {
        _providerId = providerId ?? throw new ArgumentNullException(nameof(providerId));
    }

    /// <inheritdoc/>
    public void SetDefault(IServiceProvider serviceProvider)
    {
        var factory = serviceProvider.GetRequiredService<IContentProviderFactory>();
        factory.SetDefaultProvider(_providerId);
    }
}

/// <summary>
/// Default provider setter that uses a provider type
/// </summary>
public class TypeDefaultProviderSetter<TProvider> : IDefaultProviderSetter
    where TProvider : class, IContentProvider
{
    /// <inheritdoc/>
    public void SetDefault(IServiceProvider serviceProvider)
    {
        var factory = serviceProvider.GetRequiredService<IContentProviderFactory>();
        var provider = serviceProvider.GetRequiredService<TProvider>();
        factory.SetDefaultProvider(provider.ProviderId);
    }
}