using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Common.Base;
using Osirion.Blazor.Cms.Admin.Common.Extensions;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components;

public partial class ContentEditor : EditableComponentBase
{
    [Inject]
    private ContentEditorViewModel ViewModel { get; set; } = default!;

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

    private MarkdownEditorPreview? EditorPreviewRef;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ViewModel.StateChanged += StateHasChanged;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    public void Dispose()
    {
        ViewModel.StateChanged -= StateHasChanged;
    }

    private void ToggleMetadataPanel()
    {
        IsMetadataPanelVisible = !IsMetadataPanelVisible;
    }

    private void TogglePreview()
    {
        IsPreviewVisible = !IsPreviewVisible;
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

    private string GetContentEditorClass()
    {
        return this.GetCssClassNames(CssClass);
    }
}