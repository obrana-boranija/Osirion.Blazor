using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Admin.Services;

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

/// <summary>
/// Implementation of IStateStorageService using memory for server-side rendering
/// and localStorage for client-side interactions
/// </summary>
public class SSRFriendlyStorageService : IStateStorageService, IDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly Dictionary<string, string> _memoryStore = new();
    private readonly NavigationManager _navigationManager;
    private bool _isInitialized = false;
    private bool _isClientSide = false;

    public SSRFriendlyStorageService(IJSRuntime jsRuntime, NavigationManager navigationManager)
    {
        _jsRuntime = jsRuntime;
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    public bool IsInitialized => _isInitialized;

    public async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            try
            {
                _isClientSide = await _jsRuntime.InvokeAsync<bool>("eval", "typeof window !== 'undefined'");
                _isInitialized = true;
            }
            catch
            {
                // If JSInterop fails, we're likely in server-side prerendering
                _isClientSide = false;
                _isInitialized = false;
            }
        }
    }

    public async Task SaveStateAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);

        // Always update memory store
        _memoryStore[key] = json;

        // If we're in client-side mode, also update localStorage
        if (_isClientSide && _isInitialized)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
            }
            catch
            {
                // Fail silently if localStorage is not available
            }
        }
    }

    public async Task<T?> GetStateAsync<T>(string key)
    {
        // Try to get from memory first
        if (_memoryStore.TryGetValue(key, out var memoryJson))
        {
            return JsonSerializer.Deserialize<T>(memoryJson);
        }

        // If we're in client-side mode, try localStorage
        if (_isClientSide && _isInitialized)
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);

                if (!string.IsNullOrEmpty(json))
                {
                    // Save to memory for future use
                    _memoryStore[key] = json;
                    return JsonSerializer.Deserialize<T>(json);
                }
            }
            catch
            {
                // Fail silently if localStorage is not available
            }
        }

        return default;
    }

    public async Task RemoveStateAsync(string key)
    {
        // Remove from memory
        _memoryStore.Remove(key);

        // If we're in client-side mode, also remove from localStorage
        if (_isClientSide && _isInitialized)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            }
            catch
            {
                // Fail silently if localStorage is not available
            }
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // This ensures we keep the state across navigation
        // No need to do anything special, as we're already storing in memory
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}