using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Implementation of IStateStorageService using memory for server-side rendering
/// and localStorage for client-side interactions
/// </summary>
public class LocalStorageService : IStateStorageService, IDisposable
{
    private readonly ILocalStorageService _localStorage;
    private readonly Dictionary<string, string> _memoryStore = new();
    private readonly NavigationManager _navigationManager;
    private bool _isInitialized = false;
    private bool _isClientSide = false;

    public LocalStorageService(
        ILocalStorageService localStorage,
        NavigationManager navigationManager)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
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
                // Check if we're on the client
                await _localStorage.ContainKeyAsync("test");
                _isClientSide = true;
                _isInitialized = true;
            }
            catch
            {
                // If accessing localStorage fails, we're in server prerendering
                _isClientSide = true;
                _isInitialized = true;
            }
        }
    }

    public async Task SaveStateAsync<T>(string key, T value)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(value);

        // Always update memory store
        _memoryStore[key] = json;

        // If we're in client-side mode, also update localStorage
        //if (_isClientSide && _isInitialized)
        //{
            try
            {
                await _localStorage.SetItemAsStringAsync(key, json);
            }
            catch
            {
                // Fail silently if localStorage is not available
            }
        //}
    }

    public async Task<T?> GetStateAsync<T>(string key)
    {
        // Try to get from memory first
        if (_memoryStore.TryGetValue(key, out var memoryJson))
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(memoryJson);
        }

        // If we're in client-side mode, try localStorage
        //if (_isClientSide && _isInitialized)
        //{
            try
            {
                var exists = await _localStorage.ContainKeyAsync(key);
                if (exists)
                {
                    var json = await _localStorage.GetItemAsStringAsync(key);
                    if (!string.IsNullOrEmpty(json))
                    {
                        // Save to memory for future use
                        _memoryStore[key] = json;
                        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
                    }
                }
            }
            catch
            {
                // Fail silently if localStorage is not available
            }
        //}

        return default;
    }

    public async Task RemoveStateAsync(string key)
    {
        // Remove from memory
        _memoryStore.Remove(key);

        // If we're in client-side mode, also remove from localStorage
        //if (_isClientSide && _isInitialized)
        //{
            try
            {
                await _localStorage.RemoveItemAsync(key);
            }
            catch
            {
                // Fail silently if localStorage is not available
            }
        //}
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