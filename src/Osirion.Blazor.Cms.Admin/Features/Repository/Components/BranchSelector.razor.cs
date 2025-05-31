using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.Components;

public partial class BranchSelector : IDisposable
{
    [Parameter]
    public string Title { get; set; } = "Select Branch";

    [Parameter]
    public string SelectPrompt { get; set; } = "-- Select a branch --";

    [Parameter]
    public bool AllowCreateBranch { get; set; } = true;

    [Parameter]
    public EventCallback<GitHubBranch> OnBranchChange { get; set; }

    protected override void OnInitialized()
    {
        ViewModel.StateChanged += StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await RefreshBranches();
    }

    public void Dispose()
    {
        ViewModel.StateChanged -= StateHasChanged;
    }

    private async Task RefreshBranches()
    {
        await ExecuteAsync(async () =>
        {
            await ViewModel.RefreshBranchesAsync();
        });
    }

    private async Task OnBranchSelected(ChangeEventArgs e)
    {
        var branchName = e.Value?.ToString() ?? string.Empty;

        await ExecuteAsync(async () =>
        {
            await ViewModel.SelectBranchAsync(branchName);

            if (OnBranchChange.HasDelegate && ViewModel.SelectedBranch is not null)
            {
                await OnBranchChange.InvokeAsync(ViewModel.SelectedBranch);
            }
        });
    }

    private async Task CreateBranch()
    {
        await ExecuteAsync(async () =>
        {
            await ViewModel.CreateBranchAsync();

            if (OnBranchChange.HasDelegate && ViewModel.SelectedBranch is not null)
            {
                await OnBranchChange.InvokeAsync(ViewModel.SelectedBranch);
            }
        });
    }
}