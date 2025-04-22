namespace Osirion.Blazor.Cms.Services;

public interface IContentProviderManager
{
    IContentProvider? GetDefaultProvider();
    IContentProvider? GetProvider(string providerId);
    IEnumerable<IContentProvider> GetAllProviders();
}