using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Shared.Components;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

    /// <summary>Defines the public member type.</summary>
public partial class CommitPanel : BaseComponent
{
    /// <summary>Gets or sets the Title value.</summary>
    [Parameter]
    public string Title { get; set; } = "Commit Changes";

    /// <summary>Gets or sets the CommitButtonText value.</summary>
    [Parameter]
    public string CommitButtonText { get; set; } = "Commit";

    /// <summary>Gets or sets the ShowDescription value.</summary>
    [Parameter]
    public bool ShowDescription { get; set; } = false;

    /// <summary>Gets or sets the CommitMessage value.</summary>
    [Parameter]
    public string CommitMessage { get; set; } = string.Empty;

    /// <summary>Gets or sets the CommitMessageChanged value.</summary>
    [Parameter]
    public EventCallback<string> CommitMessageChanged { get; set; }

    /// <summary>Gets or sets the CommitDescription value.</summary>
    [Parameter]
    public string CommitDescription { get; set; } = string.Empty;

    /// <summary>Gets or sets the CommitDescriptionChanged value.</summary>
    [Parameter]
    public EventCallback<string> CommitDescriptionChanged { get; set; }

    /// <summary>Gets or sets the IsCommitting value.</summary>
    [Parameter]
    public bool IsCommitting { get; set; } = false;

    /// <summary>Gets or sets the IsCommittingChanged value.</summary>
    [Parameter]
    public EventCallback<bool> IsCommittingChanged { get; set; }

    /// <summary>Gets or sets the OnCommitClicked value.</summary>
    [Parameter]
    public EventCallback OnCommitClicked { get; set; }

    /// <summary>Gets or sets the OnCancelClicked value.</summary>
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
