using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.Components;

/// <summary>Defines the RepositorySelector type.</summary>
public partial class RepositorySelector : IDisposable
{
    /// <summary>Gets or sets the Title value.</summary>
    [Parameter]
    public string Title { get; set; } = "Select Repository";

    /// <summary>Gets or sets the SelectPrompt value.</summary>
    [Parameter]
    public string SelectPrompt { get; set; } = "-- Select a repository --";

    /// <summary>Gets or sets the OnRepositoryChange value.</summary>
    [Parameter]
    public EventCallback<GitHubRepository> OnRepositoryChange { get; set; }

    private string SelectedRepositoryName => ViewModel.SelectedRepository?.Name ?? string.Empty;

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        ViewModel.StateChanged += StateHasChanged;
    }

    /// <summary>Initializes the component state and required services.</summary>
    protected override async Task OnInitializedAsync()
    {
        await RefreshRepositories();
    }

    /// <summary>Releases resources held by the component or service.</summary>
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
