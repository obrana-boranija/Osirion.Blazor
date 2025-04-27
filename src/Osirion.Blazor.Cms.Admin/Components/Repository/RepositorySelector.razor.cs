using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Core.Providers.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.GitHub.Models;

namespace Osirion.Blazor.Cms.Admin.Components.Repository;

public partial class RepositorySelector(IGitHubAdminService gitHubService, CmsAdminState adminState, NavigationManager navigationManager)
{
    [Parameter]
    public string Title { get; set; } = "Select Repository";

    [Parameter]
    public string SelectPrompt { get; set; } = "-- Select a repository --";

    [Parameter]
    public EventCallback<GitHubRepository> OnRepositoryChange { get; set; }

    private List<GitHubRepository> Repositories { get; set; } = new();
    private GitHubRepository? SelectedRepository => adminState.SelectedRepository;
    private string SelectedRepositoryName => SelectedRepository?.Name ?? string.Empty;
    private bool IsLoading { get; set; }
    private string? ErrorMessage { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Initialize state persistence if available
            if (adminState is CmsAdminStatePersistent persistentState)
            {
                await persistentState.InitializeAsync();

                // If we have a selected repository, refresh repositories
                if (adminState.SelectedRepository != null)
                {
                    gitHubService.SetRepository(adminState.SelectedRepository.Name);

                    // Refresh repositories to get the complete list
                    await RefreshRepositories(false);

                    // Force a re-render to reflect the updated state
                    StateHasChanged();
                }
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public void Dispose()
    {
        adminState.StateChanged -= StateHasChanged;
    }

    private async Task RefreshRepositories(bool resetSelection = true)
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            Repositories = await gitHubService.GetRepositoriesAsync();

            if (resetSelection)
            {
                adminState.SetRepositories(Repositories);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load repositories: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OnRepositorySelected(ChangeEventArgs e)
    {
        var repositoryName = e.Value?.ToString() ?? string.Empty;

        if (string.IsNullOrEmpty(repositoryName))
        {
            adminState.Reset();
            return;
        }

        var repository = Repositories.FirstOrDefault(r => r.Name == repositoryName);
        if (repository != null)
        {
            IsLoading = true;
            adminState.SelectRepository(repository);
            gitHubService.SetRepository(repository.Name);

            try
            {
                // Automatically load branches for the selected repository
                var branches = await gitHubService.GetBranchesAsync(repository.Name);
                adminState.SetBranches(branches);

                // If default branch exists, select it
                var defaultBranch = branches.FirstOrDefault(b => b.Name == repository.DefaultBranch);
                if (defaultBranch != null)
                {
                    adminState.SelectBranch(defaultBranch);
                    gitHubService.SetBranch(defaultBranch.Name);

                    // Automatically load content for the default branch
                    var contents = await gitHubService.GetRepositoryContentsAsync();
                    adminState.SetCurrentPath("", contents);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load branches: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }

            if (OnRepositoryChange.HasDelegate)
            {
                await OnRepositoryChange.InvokeAsync(repository);
            }
        }
    }

    private void OpenRepositoryLink()
    {
        if (SelectedRepository != null && !string.IsNullOrEmpty(SelectedRepository.HtmlUrl))
        {
            navigationManager.NavigateTo(SelectedRepository.HtmlUrl, true);
        }
    }

    private string GetRepositorySelectorClass()
    {
        return $"osirion-admin-repository-selector {CssClass}".Trim();
    }
}
