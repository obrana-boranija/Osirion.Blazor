using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Admin.Services.State;

public class StateManager : IDisposable
{
    private readonly CmsApplicationState _state;
    private readonly IStateStorageService _storageService;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<StateManager> _logger;
    private bool _isInitialized = false;

    public StateManager(
        CmsApplicationState state,
        IStateStorageService storageService,
        CmsEventMediator eventMediator,
        ILogger<StateManager> logger)
    {
        _state = state;
        _storageService = storageService;
        _eventMediator = eventMediator;
        _logger = logger;

        // Subscribe to state changes
        _state.StateChanged += OnStateChanged;

        // Subscribe to events that should trigger state saving
        _eventMediator.Subscribe<RepositorySelectedEvent>(_ => SaveStateAsync());
        _eventMediator.Subscribe<BranchSelectedEvent>(_ => SaveStateAsync());
        _eventMediator.Subscribe<ThemeChangedEvent>(_ => SaveStateAsync());
        _eventMediator.Subscribe<StateResetRequestedEvent>(_ => ResetStateAsync());
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            if (!_storageService.IsInitialized)
            {
                await _storageService.InitializeAsync();
            }

            if (_storageService.IsInitialized)
            {
                await LoadStateAsync();
                _isInitialized = true;
                _logger.LogInformation("State manager initialized successfully");
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

    private async Task LoadStateAsync()
    {
        try
        {
            // Load repository
            var repositoryJson = await _storageService.GetStateAsync<string>(CmsConstants.StorageKeys.SelectedRepository);
            if (!string.IsNullOrEmpty(repositoryJson))
            {
                var repository = JsonSerializer.Deserialize<GitHubRepository>(repositoryJson);
                if (repository != null)
                {
                    _state.SelectRepository(repository);
                    _logger.LogDebug("Loaded repository from storage: {RepositoryName}", repository.Name);
                }
            }

            // Load branch
            var branchJson = await _storageService.GetStateAsync<string>(CmsConstants.StorageKeys.SelectedBranch);
            if (!string.IsNullOrEmpty(branchJson))
            {
                var branch = JsonSerializer.Deserialize<GitHubBranch>(branchJson);
                if (branch != null)
                {
                    _state.SelectBranch(branch);
                    _logger.LogDebug("Loaded branch from storage: {BranchName}", branch.Name);
                }
            }

            // Load current path
            var currentPath = await _storageService.GetStateAsync<string>(CmsConstants.StorageKeys.CurrentPath);
            if (!string.IsNullOrEmpty(currentPath))
            {
                _state.SetCurrentPath(currentPath, new List<GitHubItem>());
                _logger.LogDebug("Loaded current path from storage: {Path}", currentPath);
            }

            // Load theme
            var theme = await _storageService.GetStateAsync<string>(CmsConstants.StorageKeys.Theme);
            if (!string.IsNullOrEmpty(theme))
            {
                _eventMediator.Publish(new ThemeChangedEvent(theme));
                _logger.LogDebug("Loaded theme from storage: {Theme}", theme);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading state from storage");
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
            // Save repository
            if (_state.SelectedRepository != null)
            {
                var repositoryJson = JsonSerializer.Serialize(_state.SelectedRepository);
                await _storageService.SaveStateAsync(CmsConstants.StorageKeys.SelectedRepository, repositoryJson);
            }
            else
            {
                await _storageService.RemoveStateAsync(CmsConstants.StorageKeys.SelectedRepository);
            }

            // Save branch
            if (_state.SelectedBranch != null)
            {
                var branchJson = JsonSerializer.Serialize(_state.SelectedBranch);
                await _storageService.SaveStateAsync(CmsConstants.StorageKeys.SelectedBranch, branchJson);
            }
            else
            {
                await _storageService.RemoveStateAsync(CmsConstants.StorageKeys.SelectedBranch);
            }

            // Save current path
            if (!string.IsNullOrEmpty(_state.CurrentPath))
            {
                await _storageService.SaveStateAsync(CmsConstants.StorageKeys.CurrentPath, _state.CurrentPath);
            }
            else
            {
                await _storageService.RemoveStateAsync(CmsConstants.StorageKeys.CurrentPath);
            }

            _logger.LogDebug("State saved to storage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving state to storage");
        }
    }

    private async Task ResetStateAsync()
    {
        try
        {
            await _storageService.RemoveStateAsync(CmsConstants.StorageKeys.SelectedRepository);
            await _storageService.RemoveStateAsync(CmsConstants.StorageKeys.SelectedBranch);
            await _storageService.RemoveStateAsync(CmsConstants.StorageKeys.CurrentPath);

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
        if (_state != null)
        {
            _state.StateChanged -= OnStateChanged;
        }

        _logger.LogDebug("State manager disposed");
    }
}