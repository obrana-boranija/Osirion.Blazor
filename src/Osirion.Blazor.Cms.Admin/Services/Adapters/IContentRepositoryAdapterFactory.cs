namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

public interface IContentRepositoryAdapterFactory
{
    IContentRepositoryAdapter CreateAdapter(string providerType);
    IContentRepositoryAdapter CreateDefaultAdapter();
}