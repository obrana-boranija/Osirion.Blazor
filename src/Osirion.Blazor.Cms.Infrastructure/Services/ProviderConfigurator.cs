using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Interface for provider configurators
/// </summary>
public interface IProviderConfigurator
{
}

/// <summary>
/// Configurator for content providers
/// </summary>
public class ProviderConfigurator<TProvider> : IProviderConfigurator
    where TProvider : class, IContentProvider
{
    private readonly Action<TProvider> _configure;

    /// <summary>
    /// Initializes a new instance of the ProviderConfigurator class
    /// </summary>
    public ProviderConfigurator(Action<TProvider> configure)
    {
        _configure = configure ?? throw new ArgumentNullException(nameof(configure));
    }

    /// <summary>
    /// Applies configuration to the provider
    /// </summary>
    public void Configure(TProvider provider)
    {
        _configure(provider);
    }
}