using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Core.Interfaces;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Admin.Services;

/// <summary>
/// Implementation of IStateStorageService using browser's localStorage
/// </summary>
public class LocalStorageService : IStateStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigationManager;
    private bool _isInitialized = false;

    public LocalStorageService(IJSRuntime jsRuntime, NavigationManager navigationManager)
    {
        _jsRuntime = jsRuntime;
        _navigationManager = navigationManager;
    }

    public bool IsInitialized => _isInitialized;

    public async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            try
            {
                // Check if we're in browser context by testing if window is defined
                _isInitialized = await _jsRuntime.InvokeAsync<bool>("eval", "typeof window !== 'undefined'");
            }
            catch
            {
                // If we get an exception, we're likely in server-side prerendering
                _isInitialized = false;
            }
        }
    }

    public async Task SaveStateAsync<T>(string key, T value)
    {
        if (!_isInitialized)
            return;

        try
        {
            var json = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch
        {
            // Fail silently if localStorage is not available
        }
    }

    public async Task<T?> GetStateAsync<T>(string key)
    {
        if (!_isInitialized)
            return default;

        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            // Fail silently if localStorage is not available
            return default;
        }
    }

    public async Task RemoveStateAsync(string key)
    {
        if (!_isInitialized)
            return;

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