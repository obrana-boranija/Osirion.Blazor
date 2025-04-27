using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Registration information for a content provider
/// </summary>
public class ProviderRegistration : IProviderRegistration
{
    private readonly Func<IServiceProvider, IContentProvider> _factory;

    /// <summary>
    /// Initializes a new instance of the ProviderRegistration class
    /// </summary>
    public ProviderRegistration(
        string providerId,
        Func<IServiceProvider, IContentProvider> factory,
        bool isDefault)
    {
        ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        IsDefault = isDefault;
    }

    /// <inheritdoc/>
    public string ProviderId { get; }

    /// <inheritdoc/>
    public bool IsDefault { get; }

    /// <inheritdoc/>
    public void Register(IServiceProvider serviceProvider)
    {
        var factory = serviceProvider.GetRequiredService<IContentProviderFactory>();
        factory.RegisterProvider(_factory);
    }
}
