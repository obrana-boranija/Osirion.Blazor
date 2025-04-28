using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Sets a provider as the default provider
/// </summary>
public class DefaultProviderSetter : IDefaultProviderSetter
{
    private readonly string _providerId;
    private readonly bool _isDefault;

    public DefaultProviderSetter(string providerId, bool isDefault)
    {
        _providerId = providerId ?? throw new ArgumentNullException(nameof(providerId));
        _isDefault = isDefault;
    }

    public void SetDefault(IServiceProvider serviceProvider)
    {
        if (!_isDefault) return;

        var registry = serviceProvider.GetRequiredService<IContentProviderRegistry>();
        registry.SetDefaultProvider(_providerId);
    }
}