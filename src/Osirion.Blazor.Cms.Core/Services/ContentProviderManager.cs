using Osirion.Blazor.Cms.Interfaces;

namespace Osirion.Blazor.Cms.Services;

public class ContentProviderManager : IContentProviderManager
{
    private readonly IEnumerable<IContentProvider> _providers;

    public ContentProviderManager(IEnumerable<IContentProvider> providers)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
    }

    public IContentProvider? GetDefaultProvider() => _providers.FirstOrDefault();

    public IContentProvider? GetProvider(string providerId) =>
        _providers.FirstOrDefault(p => p.ProviderId == providerId);

    public IEnumerable<IContentProvider> GetAllProviders() => _providers;
}