using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components;

public partial class ContentEditor : BaseComponent
{
    [Inject]
    public ContentEditorViewModel ViewModel { get; set; } = null!;

    [Parameter]
    public bool IsPreviewVisible { get; set; } = true;

    [Parameter]
    public EventCallback<BlogPost> OnSaveComplete { get; set; }

    [Parameter]
    public EventCallback OnDiscard { get; set; }

    private string ActiveTab { get; set; } = "content";
    private bool IsDirty { get; set; } = false;

    protected override void OnInitialized()
    {
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

    private async Task SaveChanges()
    {
        await ExecuteAsync(async () =>
        {
            await ViewModel.SavePostAsync();
            IsDirty = false;

            if (OnSaveComplete.HasDelegate && ViewModel.EditingPost != null)
            {
                await OnSaveComplete.InvokeAsync(ViewModel.EditingPost);
            }
        });
    }

    private async Task DiscardChanges()
    {
        ViewModel.DiscardChanges();
        IsDirty = false;

        if (OnDiscard.HasDelegate)
        {
            await OnDiscard.InvokeAsync();
        }
    }

    private void OnContentChanged()
    {
        IsDirty = true;
    }
}