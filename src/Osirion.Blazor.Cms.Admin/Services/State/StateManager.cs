using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Admin.Services.State;

/// <summary>
/// Manages persistence of application state across sessions
/// </summary>
public class StateManager : IDisposable
{
    private readonly CmsState _state;
    private readonly IStateStorageService _storageService;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<StateManager> _logger;
    private readonly CmsAdminOptions _options;
    private const string STATE_KEY = "osirion_cms_admin_state";
    private bool _isInitialized = false;

    public StateManager(
        CmsState state,
        IStateStorageService storageService,
        CmsEventMediator eventMediator,
        IOptions<CmsAdminOptions> options,
        ILogger<StateManager> logger)
    {
        _state = state;
        _storageService = storageService;
        _eventMediator = eventMediator;
        _options = options.Value;
        _logger = logger;

        // Subscribe to state changes
        _state.StateChanged += OnStateChanged;

        // Subscribe to events that should trigger state saving
        _eventMediator.Subscribe<RepositorySelectedEvent>(_ => SaveStateAsync());
        _eventMediator.Subscribe<BranchSelectedEvent>(_ => SaveStateAsync());
        _eventMediator.Subscribe<ThemeChangedEvent>(_ => SaveStateAsync());
        _eventMediator.Subscribe<StateResetRequestedEvent>(_ => ResetStateAsync());
    }

    /// <summary>
    /// Initializes the state manager and loads persisted state if available
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized || !_options.PersistUserSelections)
            return;

        try
        {
            await _storageService.InitializeAsync();

            if (_storageService.IsInitialized)
            {
                var serializedState = await _storageService.GetStateAsync<string>(STATE_KEY);

                if (!string.IsNullOrEmpty(serializedState))
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
            _logger.LogError(ex, "Error initializing state manager");
        }
    }

    /// <summary>
    /// Event handler for state changes
    /// </summary>
    private async void OnStateChanged()
    {
        await SaveStateAsync();
    }

    /// <summary>
    /// Saves the current state to persistence
    /// </summary>
    private async Task SaveStateAsync()
    {
        if (!_storageService.IsInitialized || !_options.PersistUserSelections)
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

    /// <summary>
    /// Resets the state and clears persistence
    /// </summary>
    private async Task ResetStateAsync()
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

    /// <summary>
    /// Disposes of resources
    /// </summary>
    public void Dispose()
    {
        _state.StateChanged -= OnStateChanged;
        _logger.LogDebug("State manager disposed");
    }
}