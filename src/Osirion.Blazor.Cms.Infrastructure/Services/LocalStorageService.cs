using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Implementation of IStateStorageService using memory for server-side rendering
/// and localStorage for client-side interactions
/// </summary>
public class LocalStorageService : IStateStorageService, IDisposable
{
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<LocalStorageService> _logger;
    private readonly Dictionary<string, string> _memoryStore = new();
    private bool _isInitialized = false;
    private bool _isClientSide = false;

    public LocalStorageService(
        ILocalStorageService localStorage,
        NavigationManager navigationManager,
        ILogger<LocalStorageService> logger)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    public bool IsInitialized => _isInitialized;

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            // Try to detect if we're running on the client side
            // by attempting a simple localStorage operation
            await _localStorage.SetItemAsync("__osirion_storage_test", true);
            await _localStorage.RemoveItemAsync("__osirion_storage_test");

            _isClientSide = true;
            _isInitialized = true;
            _logger.LogInformation("LocalStorage initialized in client-side mode");

            // Try to preload important keys from localStorage to memory
            await PreloadKeysFromStorageAsync();
        }
        catch (Exception ex)
        {
            // If accessing localStorage fails, we're in server prerendering
            // or another environment where localStorage isn't available
            _isClientSide = false;
            _isInitialized = true; // Still mark as initialized, we'll use memory store
            _logger.LogWarning(ex, "LocalStorage not available, using memory-only storage");
        }
    }

    private async Task PreloadKeysFromStorageAsync()
    {
        if (!_isClientSide) return;

        try
        {
            // Load important authentication keys
            var authKeys = new[] {
                "github_auth_token",
                "github_username",
                "auth_status",
                "last_login_method"
            };

            foreach (var key in authKeys)
            {
                if (await _localStorage.ContainKeyAsync(key))
                {
                    var value = await _localStorage.GetItemAsStringAsync(key);
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        _memoryStore[key] = value;
                        _logger.LogDebug("Preloaded key from localStorage: {Key}", key);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error preloading keys from localStorage");
        }
    }

    public async Task SaveStateAsync<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key)) return;

        var json = System.Text.Json.JsonSerializer.Serialize(value);

        // Always update memory store
        _memoryStore[key] = json;
        _logger.LogDebug("Saved to memory store: {Key}", key);

        // If we're in client-side mode, also update localStorage
        if (_isClientSide)
        {
            try
            {
                await _localStorage.SetItemAsStringAsync(key, json);
                _logger.LogDebug("Saved to localStorage: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to save to localStorage: {Key}", key);
            }
        }
    }

    public async Task<T?> GetStateAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return default;

        // Try to get from memory first
        if (_memoryStore.TryGetValue(key, out var memoryJson))
        {
            _logger.LogDebug("Retrieved from memory store: {Key}", key);
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(memoryJson);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize from memory: {Key}", key);
            }
        }

        // If we're in client-side mode, try localStorage as fallback
        if (_isClientSide)
        {
            try
            {
                var exists = await _localStorage.ContainKeyAsync(key);
                if (exists)
                {
                    var json = await _localStorage.GetItemAsStringAsync(key);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        // Save to memory for future use
                        _memoryStore[key] = json;
                        _logger.LogDebug("Retrieved from localStorage: {Key}", key);
                        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve from localStorage: {Key}", key);
            }
        }

        return default;
    }

    public async Task RemoveStateAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return;

        // Remove from memory
        _memoryStore.Remove(key);
        _logger.LogDebug("Removed from memory store: {Key}", key);

        // If we're in client-side mode, also remove from localStorage
        if (_isClientSide)
        {
            try
            {
                await _localStorage.RemoveItemAsync(key);
                _logger.LogDebug("Removed from localStorage: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove from localStorage: {Key}", key);
            }
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // No action needed - we're keeping state in memory and localStorage
        // This ensures we keep the state across navigation
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}