using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;

namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

public interface IContentRepositoryAdapterFactory
{
    IContentRepositoryAdapter CreateAdapter(string providerType);
    IContentRepositoryAdapter CreateDefaultAdapter();
}