using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;

/// <summary>Manages branch selection and branch creation for the repository editor.</summary>
public class BranchSelectorViewModel
{
    private readonly RepositoryService _repositoryService;
    private readonly CmsState _appState;
    private readonly CmsEventMediator _eventMediator;

    /// <summary>Gets the available branches.</summary>
    public List<GitHubBranch> Branches { get; private set; } = new();
    /// <summary>Gets the selected branch.</summary>
    public GitHubBranch? SelectedBranch => _appState.SelectedBranch;
    /// <summary>Gets whether branch data is loading.</summary>
    public bool IsLoading { get; private set; }
    /// <summary>Gets the current error message.</summary>
    public string? ErrorMessage { get; private set; }

    // New branch creation
    /// <summary>Gets whether the new-branch form is visible.</summary>
    public bool IsCreatingNewBranch { get; private set; }
    /// <summary>Gets whether a branch is being created.</summary>
    public bool IsCreatingBranch { get; private set; }
    /// <summary>Gets or sets the new branch name.</summary>
    public string NewBranchName { get; set; } = string.Empty;
    /// <summary>Gets or sets the base branch name.</summary>
    public string BaseBranchName { get; set; } = string.Empty;

    /// <summary>Occurs when the view-model state changes.</summary>
    public event Action? StateChanged;

    /// <summary>Initializes a new branch selector view-model.</summary>
    public BranchSelectorViewModel(
        RepositoryService repositoryService,
        CmsState appState,
        CmsEventMediator eventMediator)
    {
        _repositoryService = repositoryService;
        _appState = appState;
        _eventMediator = eventMediator;

        _appState.StateChanged += OnAppStateChanged;
        _eventMediator.Subscribe<RepositorySelectedEvent>(OnRepositorySelected);
    }

    /// <summary>Refreshes the branches for the selected repository.</summary>
    public async Task RefreshBranchesAsync()
    {
        if (_appState.SelectedRepository is null)
            return;

        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            Branches = await _repositoryService.GetBranchesAsync(_appState.SelectedRepository.Name);

            if (Branches.Count > 0 && string.IsNullOrWhiteSpace(BaseBranchName))
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

    /// <summary>Selects a branch and publishes the selection.</summary>
    public async Task SelectBranchAsync(string branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName))
        {
            _appState.SelectBranch(null);
            return;
        }

        var branch = Branches.Find(b => b.Name == branchName);
        if (branch is not null)
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

    /// <summary>Shows or hides the new-branch form.</summary>
    public void SetCreatingNewBranch(bool isCreating)
    {
        IsCreatingNewBranch = isCreating;

        if (isCreating && _appState.SelectedRepository is not null)
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

    /// <summary>Creates and selects a new branch.</summary>
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

    /// <summary>Performs the NotifyStateChanged operation.</summary>
    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
