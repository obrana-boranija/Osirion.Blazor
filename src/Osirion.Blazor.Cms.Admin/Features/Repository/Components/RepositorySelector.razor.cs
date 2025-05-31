using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.Components;

public partial class RepositorySelector : IDisposable
{
    [Parameter]
    public string Title { get; set; } = "Select Repository";

    [Parameter]
    public string SelectPrompt { get; set; } = "-- Select a repository --";

    [Parameter]
    public EventCallback<GitHubRepository> OnRepositoryChange { get; set; }

    private string SelectedRepositoryName => ViewModel.SelectedRepository?.Name ?? string.Empty;

    protected override void OnInitialized()
    {
        ViewModel.StateChanged += StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await RefreshRepositories();
    }

    public void Dispose()
    {
        ViewModel.StateChanged -= StateHasChanged;
    }

    private async Task RefreshRepositories()
    {
        await ExecuteAsync(async () =>
        {
            await ViewModel.LoadRepositoriesAsync();
        });
    }

    private async Task OnRepositorySelected(ChangeEventArgs e)
    {
        var repositoryName = e.Value?.ToString() ?? string.Empty;

        await ExecuteAsync(async () =>
        {
            await ViewModel.SelectRepositoryAsync(repositoryName);

            if (OnRepositoryChange.HasDelegate && ViewModel.SelectedRepository is not null)
            {
                await OnRepositoryChange.InvokeAsync(ViewModel.SelectedRepository);
            }
        });
    }
}