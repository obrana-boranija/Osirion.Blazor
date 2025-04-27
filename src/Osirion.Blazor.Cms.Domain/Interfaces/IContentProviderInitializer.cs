namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Initializes content providers
/// </summary>
public interface IContentProviderInitializer
{
    /// <summary>
    /// Initializes the content providers
    /// </summary>
    Task InitializeProvidersAsync(CancellationToken cancellationToken = default);
}