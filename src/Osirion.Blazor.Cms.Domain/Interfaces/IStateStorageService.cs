namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Service for persisting admin state across page navigations
/// </summary>
public interface IStateStorageService
{
    /// <summary>
    /// Saves state data to browser storage
    /// </summary>
    Task SaveStateAsync<T>(string key, T value);

    /// <summary>
    /// Retrieves state data from browser storage
    /// </summary>
    Task<T?> GetStateAsync<T>(string key);

    /// <summary>
    /// Removes state data from browser storage
    /// </summary>
    Task RemoveStateAsync(string key);

    /// <summary>
    /// Indicates if the service is ready to be used
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Initializes the service for use after component rendering
    /// </summary>
    Task InitializeAsync();
}