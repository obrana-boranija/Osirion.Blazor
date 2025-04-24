namespace Osirion.Blazor.Cms.Interfaces;

public interface IContentProviderManager
{
    IContentProvider? GetDefaultProvider();
    IContentProvider? GetProvider(string providerId);
    IEnumerable<IContentProvider> GetAllProviders();
}