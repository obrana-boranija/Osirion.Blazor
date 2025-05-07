using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Services.State
{
    public class StateManager
    {
        private readonly CmsApplicationState _state;
        private readonly IStateStorageService _storageService;

        public StateManager(CmsApplicationState state, IStateStorageService storageService)
        {
            _state = state;
            _storageService = storageService;

            // Subscribe to state changes
            _state.StateChanged += OnStateChanged;
        }

        public async Task InitializeAsync()
        {
            if (!_storageService.IsInitialized)
            {
                await _storageService.InitializeAsync();
            }

            if (_storageService.IsInitialized)
            {
                await LoadStateAsync();
            }
        }

        private async Task LoadStateAsync()
        {
            // Load state from storage
            var selectedRepository = await _storageService.GetStateAsync<string>("selectedRepository");
            var selectedBranch = await _storageService.GetStateAsync<string>("selectedBranch");
            var currentPath = await _storageService.GetStateAsync<string>("currentPath");

            // Apply loaded state
            // In a real implementation, we would need to load the full objects
        }

        private async void OnStateChanged()
        {
            if (_storageService.IsInitialized)
            {
                await SaveStateAsync();
            }
        }

        private async Task SaveStateAsync()
        {
            // Save repository ID if selected
            if (_state.SelectedRepository != null)
            {
                await _storageService.SaveStateAsync("selectedRepository", _state.SelectedRepository.Name);
            }
            else
            {
                await _storageService.RemoveStateAsync("selectedRepository");
            }

            // Save branch name if selected
            if (_state.SelectedBranch != null)
            {
                await _storageService.SaveStateAsync("selectedBranch", _state.SelectedBranch.Name);
            }
            else
            {
                await _storageService.RemoveStateAsync("selectedBranch");
            }

            // Save current path
            if (!string.IsNullOrEmpty(_state.CurrentPath))
            {
                await _storageService.SaveStateAsync("currentPath", _state.CurrentPath);
            }
            else
            {
                await _storageService.RemoveStateAsync("currentPath");
            }
        }
    }
}