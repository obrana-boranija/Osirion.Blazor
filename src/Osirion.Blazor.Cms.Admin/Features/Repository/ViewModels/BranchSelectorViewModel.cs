using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;

public class BranchSelectorViewModel
{
    private readonly RepositoryService _repositoryService;
    private readonly CmsApplicationState _appState;
    private readonly CmsEventMediator _eventMediator;

    public List<GitHubBranch> Branches { get; private set; } = new();
    public GitHubBranch? SelectedBranch => _appState.SelectedBranch;
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    // New branch creation
    public bool IsCreatingNewBranch { get; private set; }
    public bool IsCreatingBranch { get; private set; }
    public string NewBranchName { get; set; } = string.Empty;
    public string BaseBranchName { get; set; } = string.Empty;

    public event Action? StateChanged;

    public BranchSelectorViewModel(
        RepositoryService repositoryService,
        CmsApplicationState appState,
        CmsEventMediator eventMediator)
    {
        _repositoryService = repositoryService;
        _appState = appState;
        _eventMediator = eventMediator;

        _appState.StateChanged += OnAppStateChanged;
        _eventMediator.Subscribe<RepositorySelectedEvent>(OnRepositorySelected);
    }

    public async Task RefreshBranchesAsync()
    {
        if (_appState.SelectedRepository == null)
            return;

        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            Branches = await _repositoryService.GetBranchesAsync(_appState.SelectedRepository.Name);

            if (Branches.Count > 0 && string.IsNullOrEmpty(BaseBranchName))
            {
                // Set default base branch
                BaseBranchName = _appState.SelectedRepository.DefaultBranch;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load branches: {ex.Message}";
            _appState.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task SelectBranchAsync(string branchName)
    {
        if (string.IsNullOrEmpty(branchName))
        {
            _appState.SelectBranch(null);
            return;
        }

        var branch = Branches.Find(b => b.Name == branchName);
        if (branch != null)
        {
            IsLoading = true;
            NotifyStateChanged();

            try
            {
                // Set the selected branch in state
                _appState.SelectBranch(branch);

                // Configure the repository adapter
                _repositoryService.SetBranch(branch.Name);

                // Publish branch selected event
                _eventMediator.Publish(new BranchSelectedEvent(branch));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to select branch: {ex.Message}";
                _appState.SetErrorMessage(ErrorMessage);
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }
    }

    public void SetCreatingNewBranch(bool isCreating)
    {
        IsCreatingNewBranch = isCreating;

        if (isCreating && _appState.SelectedRepository != null)
        {
            // Set default base branch
            BaseBranchName = _appState.SelectedRepository.DefaultBranch;
        }
        else
        {
            // Reset form
            NewBranchName = string.Empty;
        }

        NotifyStateChanged();
    }

    public async Task CreateBranchAsync()
    {
        if (string.IsNullOrWhiteSpace(NewBranchName) || string.IsNullOrWhiteSpace(BaseBranchName))
            return;

        IsCreatingBranch = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            var newBranch = await _repositoryService.CreateBranchAsync(NewBranchName, BaseBranchName);

            // Refresh branches list
            await RefreshBranchesAsync();

            // Select the new branch
            await SelectBranchAsync(newBranch.Name);

            // Close the form
            SetCreatingNewBranch(false);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create branch: {ex.Message}";
            _appState.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsCreatingBranch = false;
            NotifyStateChanged();
        }
    }

    private async void OnRepositorySelected(RepositorySelectedEvent e)
    {
        // Clear branches when repository changes
        Branches.Clear();
        _appState.SelectBranch(null);

        // Load branches for the new repository
        await RefreshBranchesAsync();
    }

    private void OnAppStateChanged()
    {
        NotifyStateChanged();
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}