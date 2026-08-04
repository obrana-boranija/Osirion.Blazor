using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;

namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

/// <summary>Defines the IContentRepositoryAdapterFactory API contract.</summary>
public interface IContentRepositoryAdapterFactory
{
    /// <summary>Gets or sets the CreateAdapter value.</summary>
    IContentRepositoryAdapter CreateAdapter(string providerType);
    /// <summary>Performs the CreateDefaultAdapter operation.</summary>
    IContentRepositoryAdapter CreateDefaultAdapter();
}
