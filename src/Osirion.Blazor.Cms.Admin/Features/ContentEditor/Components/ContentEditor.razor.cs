using Microsoft.AspNetCore.Components;
using Octokit;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components;

public partial class ContentEditor : BaseComponent
{
    [Inject]
    public ContentEditorViewModel ViewModel { get; set; } = null!;

    [Inject]
    public CmsState AdminState { get; set; } = null!;

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
        // Subscribe to changes from both ViewModel and AdminState
        ViewModel.StateChanged += StateHasChanged;

        // If ViewModel.EditingPost is null but AdminState has a post,
        // we need to initialize ViewModel from AdminState
        if (ViewModel.EditingPost == null && AdminState.EditingPost != null)
        {
            ViewModel.InitializeFromState(AdminState.EditingPost, AdminState.IsCreatingNewFile);
        }

        // Subscribe to content-related events from EventBus
        EventSubscriber.Subscribe<ContentSelectedEvent>(OnContentSelected);
        EventSubscriber.Subscribe<CreateNewContentEvent>(OnCreateNewContent);
    }

    public void Dispose()
    {
        ViewModel.StateChanged -= StateHasChanged;

        // Unsubscribe from events
        EventSubscriber.Unsubscribe<ContentSelectedEvent>(OnContentSelected);
        EventSubscriber.Unsubscribe<CreateNewContentEvent>(OnCreateNewContent);
    }

    // Handle content selection event
    private void OnContentSelected(ContentSelectedEvent e)
    {
        if (!string.IsNullOrEmpty(e.Path))
        {
            ViewModel.LoadPostAsync(e.Path).ConfigureAwait(false);
        }
    }

    // Handle create new content event
    private void OnCreateNewContent(CreateNewContentEvent e)
    {
        if (AdminState.EditingPost != null && AdminState.IsCreatingNewFile)
        {
            // Initialize from AdminState
            ViewModel.InitializeFromState(AdminState.EditingPost, true);
        }
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