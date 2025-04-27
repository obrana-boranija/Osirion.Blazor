using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;


namespace Osirion.Blazor.Cms.Admin.Components.Repository;
public partial class BranchSelector(IGitHubAdminService GitHubService, CmsAdminState AdminState)
{
    [Parameter]
    public string Title { get; set; } = "Select Branch";

    [Parameter]
    public string SelectPrompt { get; set; } = "-- Select a branch --";

    [Parameter]
    public bool AllowCreateBranch { get; set; } = true;

    [Parameter]
    public EventCallback<GitHubBranch> OnBranchChange { get; set; }

    private List<GitHubBranch> Branches { get; set; } = new();
    private GitHubBranch? SelectedBranch => AdminState.SelectedBranch;
    private string SelectedBranchName => SelectedBranch?.Name ?? string.Empty;

    private bool IsLoading { get; set; }
    private string? ErrorMessage { get; set; }

    private bool IsCreatingNewBranch { get; set; }
    private bool IsCreatingBranch { get; set; }
    private string NewBranchName { get; set; } = string.Empty;
    private string BaseBranchName { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        AdminState.StateChanged += StateHasChanged;

        if (AdminState.SelectedRepository != null)
        {
            // Refresh branches if we have a repository selected
            // This handles both fresh loads and persistence scenarios
            await RefreshBranches();

            // If we have a branch selected (from persistence), load its content
            if (AdminState.SelectedBranch != null)
            {
                GitHubService.SetBranch(AdminState.SelectedBranch.Name);

                // Only reload content if we don't already have it
                if (!AdminState.CurrentItems.Any())
                {
                    await LoadContent();
                }
            }
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (AdminState.SelectedRepository != null && Branches.Count == 0 && !IsLoading)
        {
            await RefreshBranches();
        }
    }

    public void Dispose()
    {
        AdminState.StateChanged -= StateHasChanged;
    }

    private async Task RefreshBranches()
    {
        if (AdminState.SelectedRepository == null)
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            Branches = await GitHubService.GetBranchesAsync(AdminState.SelectedRepository.Name);

            if (Branches.Count > 0)
            {
                // Set default branch if one isn't selected
                BaseBranchName = AdminState.SelectedRepository.DefaultBranch;
            }

            AdminState.SetBranches(Branches);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load branches: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OnBranchSelected(ChangeEventArgs e)
    {
        var branchName = e.Value?.ToString() ?? string.Empty;

        if (string.IsNullOrEmpty(branchName))
        {
            // Reset branch selection
            AdminState.SelectBranch(new GitHubBranch());
            return;
        }

        var branch = Branches.FirstOrDefault(b => b.Name == branchName);
        if (branch != null)
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                AdminState.SelectBranch(branch);
                GitHubService.SetBranch(branch.Name);

                // Automatically load content after selecting a branch
                var contents = await GitHubService.GetRepositoryContentsAsync();
                AdminState.SetCurrentPath("", contents);

                if (OnBranchChange.HasDelegate)
                {
                    await OnBranchChange.InvokeAsync(branch);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load content: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private async Task LoadContent()
    {
        if (AdminState.SelectedRepository == null || AdminState.SelectedBranch == null)
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var contents = await GitHubService.GetRepositoryContentsAsync(AdminState.CurrentPath);
            AdminState.SetCurrentPath(AdminState.CurrentPath, contents);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load content: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SetCreatingNewBranch(bool isCreating)
    {
        IsCreatingNewBranch = isCreating;

        if (isCreating && AdminState.SelectedRepository != null)
        {
            // Set default base branch
            BaseBranchName = AdminState.SelectedRepository.DefaultBranch;
        }
        else
        {
            // Reset form
            NewBranchName = string.Empty;
        }
    }

    private async Task CreateBranch()
    {
        if (string.IsNullOrWhiteSpace(NewBranchName) || string.IsNullOrWhiteSpace(BaseBranchName))
        {
            return;
        }

        IsCreatingBranch = true;
        ErrorMessage = null;

        try
        {
            var newBranch = await GitHubService.CreateBranchAsync(NewBranchName, BaseBranchName);

            // Refresh branches
            await RefreshBranches();

            // Select the new branch
            AdminState.SelectBranch(newBranch);
            GitHubService.SetBranch(newBranch.Name);

            // Load contents for the new branch
            try
            {
                var contents = await GitHubService.GetRepositoryContentsAsync();
                AdminState.SetCurrentPath("", contents);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load content: {ex.Message}";
            }

            if (OnBranchChange.HasDelegate)
            {
                await OnBranchChange.InvokeAsync(newBranch);
            }

            // Close the form
            SetCreatingNewBranch(false);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create branch: {ex.Message}";
        }
        finally
        {
            IsCreatingBranch = false;
        }
    }

    private string GetBranchSelectorClass()
    {
        return $"osirion-admin-branch-selector {CssClass}".Trim();
    }
}