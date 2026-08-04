using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.Components;

/// <summary>Defines the BranchSelector type.</summary>
public partial class BranchSelector : IDisposable
{
    /// <summary>Gets or sets the Title value.</summary>
    [Parameter]
    public string Title { get; set; } = "Select Branch";

    /// <summary>Gets or sets the SelectPrompt value.</summary>
    [Parameter]
    public string SelectPrompt { get; set; } = "-- Select a branch --";

    /// <summary>Gets or sets the AllowCreateBranch value.</summary>
    [Parameter]
    public bool AllowCreateBranch { get; set; } = true;

    /// <summary>Gets or sets the OnBranchChange value.</summary>
    [Parameter]
    public EventCallback<GitHubBranch> OnBranchChange { get; set; }

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        ViewModel.StateChanged += StateHasChanged;
    }

    /// <summary>Initializes the component state and required services.</summary>
    protected override async Task OnInitializedAsync()
    {
        await RefreshBranches();
    }

    /// <summary>Releases resources held by the component or service.</summary>
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
