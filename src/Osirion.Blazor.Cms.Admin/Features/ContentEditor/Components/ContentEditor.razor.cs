using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components;

public partial class ContentEditor : EditableComponent
{
    [Inject]
    private ContentEditorViewModel ViewModel { get; set; } = null!;

    [Parameter]
    public bool IsMetadataPanelVisible { get; set; } = true;

    [Parameter]
    public bool IsPreviewVisible { get; set; } = true;

    [Parameter]
    public bool AskForCommitMessage { get; set; } = true;

    [Parameter]
    public EventCallback<BlogPost> OnSaveComplete { get; set; }

    [Parameter]
    public EventCallback OnDiscard { get; set; }

    private MarkdownEditorWithPreview? EditorPreviewRef;
    private string ActiveTab { get; set; } = "content";

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ViewModel.StateChanged += StateHasChanged;
    }

    public void Dispose()
    {
        ViewModel.StateChanged -= StateHasChanged;
    }

    private void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    private void TogglePreview()
    {
        IsPreviewVisible = !IsPreviewVisible;
    }

    private void UpdateContent(string content)
    {
        if (ViewModel.EditingPost != null)
        {
            ViewModel.EditingPost.Content = content;
            MarkAsDirty();
        }
    }

    private async Task SaveChanges()
    {
        await SaveWithConfirmationAsync(async () =>
        {
            await ViewModel.SavePostAsync();

            if (OnSaveComplete.HasDelegate && ViewModel.EditingPost != null)
            {
                await OnSaveComplete.InvokeAsync(ViewModel.EditingPost);
            }
        });
    }

    private async Task DiscardChanges()
    {
        if (await ConfirmDiscardChangesAsync())
        {
            ViewModel.DiscardChanges();

            if (OnDiscard.HasDelegate)
            {
                await OnDiscard.InvokeAsync();
            }
        }
    }
}