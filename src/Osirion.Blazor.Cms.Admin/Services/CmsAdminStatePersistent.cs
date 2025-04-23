using Osirion.Blazor.Cms.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osirion.Blazor.Cms.Admin.Services;

/// <summary>
/// Service for managing the state of the CMS admin interface with persistence
/// that works with SSR
/// </summary>
public class CmsAdminStatePersistent : CmsAdminState
{
    private readonly IStateStorageService _stateStorage;
    private bool _isInitialized = false;
    private readonly string _stateKey = "osirion_cms_admin_state";

    public CmsAdminStatePersistent(IStateStorageService stateStorage)
    {
        _stateStorage = stateStorage;
    }

    /// <summary>
    /// Initializes the state from storage if available
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized || !_stateStorage.IsInitialized)
            return;

        try
        {
            // First initialize the storage service (important for SSR)
            await _stateStorage.InitializeAsync();

            // Only proceed if the storage service is now initialized
            if (_stateStorage.IsInitialized)
            {
                var persistedState = await _stateStorage.GetStateAsync<PersistedState>(_stateKey);
                if (persistedState != null)
                {
                    // Restore selected repository
                    if (persistedState.SelectedRepository != null)
                    {
                        SelectedRepository = persistedState.SelectedRepository;
                    }

                    // Restore selected branch
                    if (persistedState.SelectedBranch != null)
                    {
                        SelectedBranch = persistedState.SelectedBranch;
                    }

                    // Restore current path
                    if (!string.IsNullOrEmpty(persistedState.CurrentPath))
                    {
                        CurrentPath = persistedState.CurrentPath;
                    }

                    // If we restored items, make sure to notify listeners
                    //StateChanged?.Invoke();
                    NotifyStateChanged();
                }

                _isInitialized = true;
            }
        }
        catch
        {
            // If we fail to initialize, just continue without persistence
            _isInitialized = false;
        }
    }

    /// <summary>
    /// Persists the current state to storage
    /// </summary>
    private async Task SaveStateAsync()
    {
        if (!_stateStorage.IsInitialized)
            return;

        try
        {
            var state = new PersistedState
            {
                SelectedRepository = SelectedRepository,
                SelectedBranch = SelectedBranch,
                CurrentPath = CurrentPath
            };

            await _stateStorage.SaveStateAsync(_stateKey, state);
        }
        catch
        {
            // If saving fails, continue without persistence
        }
    }

    // Override state-changing methods to add persistence

    public override void SelectRepository(GitHubRepository repository)
    {
        base.SelectRepository(repository);

        // Use ConfigureAwait(false) to avoid potential deadlocks
        // and allow the method to continue even if the task doesn't complete
        _ = SaveStateAsync().ConfigureAwait(false);
    }

    public override void SelectBranch(GitHubBranch branch)
    {
        base.SelectBranch(branch);
        _ = SaveStateAsync().ConfigureAwait(false);
    }

    public override void SetCurrentPath(string path, List<GitHubItem> items)
    {
        base.SetCurrentPath(path, items);
        _ = SaveStateAsync().ConfigureAwait(false);
    }

    public override void Reset()
    {
        base.Reset();
        _ = _stateStorage.RemoveStateAsync(_stateKey).ConfigureAwait(false);
    }

    // Class to represent the state data to be persisted
    private class PersistedState
    {
        public GitHubRepository? SelectedRepository { get; set; }
        public GitHubBranch? SelectedBranch { get; set; }
        public string CurrentPath { get; set; } = string.Empty;
    }
}