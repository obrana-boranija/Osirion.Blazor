using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components.Editor;

public partial class CommitPanel
{
    [Parameter]
    public string Title { get; set; } = "Commit Changes";

    [Parameter]
    public string CommitButtonText { get; set; } = "Commit";

    [Parameter]
    public bool ShowDescription { get; set; } = false;

    [Parameter]
    public string CommitMessage { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> CommitMessageChanged { get; set; }

    [Parameter]
    public string CommitDescription { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> CommitDescriptionChanged { get; set; }

    [Parameter]
    public bool IsCommitting { get; set; } = false;

    [Parameter]
    public EventCallback<bool> IsCommittingChanged { get; set; }

    [Parameter]
    public string ErrorMessage { get; set; } = string.Empty;

    [Parameter]
    public EventCallback OnCommitClicked { get; set; }

    [Parameter]
    public EventCallback OnCancelClicked { get; set; }

    private async Task OnCommit()
    {
        if (string.IsNullOrWhiteSpace(CommitMessage))
            return;

        await IsCommittingChanged.InvokeAsync(true);

        if (OnCommitClicked.HasDelegate)
        {
            await OnCommitClicked.InvokeAsync();
        }
    }

    private async Task OnCancel()
    {
        if (OnCancelClicked.HasDelegate)
        {
            await OnCancelClicked.InvokeAsync();
        }
    }
}