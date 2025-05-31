using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Core.State;

/// <summary>
/// Handles persistence of application state
/// </summary>
public class StatePersistenceService : IDisposable
{
    private readonly CmsState _state;
    private readonly IStateStorageService _storageService;
    private readonly ILogger<StatePersistenceService> _logger;
    private const string STATE_KEY = "osirion_cms_admin_state";
    private bool _isInitialized = false;

    public StatePersistenceService(
        CmsState state,
        IStateStorageService storageService,
        ILogger<StatePersistenceService> logger)
    {
        _state = state;
        _storageService = storageService;
        _logger = logger;

        // Subscribe to state changes
        _state.StateChanged += OnStateChanged;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            await _storageService.InitializeAsync();

            if (_storageService.IsInitialized)
            {
                var serializedState = await _storageService.GetStateAsync<string>(STATE_KEY);

                if (!string.IsNullOrWhiteSpace(serializedState))
                {
                    _state.DeserializeFrom(serializedState);
                    _logger.LogInformation("State loaded from storage successfully");
                }

                _isInitialized = true;
            }
            else
            {
                _logger.LogWarning("Storage service could not be initialized");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing state persistence");
        }
    }

    private async void OnStateChanged()
    {
        await SaveStateAsync();
    }

    private async Task SaveStateAsync()
    {
        if (!_storageService.IsInitialized)
            return;

        try
        {
            var serializedState = _state.Serialize();
            await _storageService.SaveStateAsync(STATE_KEY, serializedState);
            _logger.LogDebug("State saved to storage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving state to storage");
        }
    }

    public async Task ResetStateAsync()
    {
        try
        {
            if (_storageService.IsInitialized)
            {
                await _storageService.RemoveStateAsync(STATE_KEY);
            }

            _state.Reset();
            _logger.LogInformation("State reset completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting state");
        }
    }

    public void Dispose()
    {
        _state.StateChanged -= OnStateChanged;
        _logger.LogDebug("State persistence service disposed");
    }
}