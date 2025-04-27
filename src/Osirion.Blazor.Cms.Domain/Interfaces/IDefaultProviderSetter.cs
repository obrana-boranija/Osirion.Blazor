namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Sets a default provider during application startup
/// </summary>
public interface IDefaultProviderSetter
{
    /// <summary>
    /// Sets the default provider
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    void SetDefault(IServiceProvider serviceProvider);
}