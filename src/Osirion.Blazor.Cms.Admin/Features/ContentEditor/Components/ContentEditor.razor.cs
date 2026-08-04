using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components;

    /// <summary>Defines the public member type.</summary>
public partial class ContentEditor : IDisposable
{
    /// <summary>Performs the ViewModel operation.</summary>
    [Inject]
    public ContentEditorViewModel ViewModel { get; set; } = null!;

    /// <summary>Gets or sets the IsPreviewVisible value.</summary>
    [Parameter]
    public bool IsPreviewVisible { get; set; } = true;

    /// <summary>Gets or sets the OnSaveComplete value.</summary>
    [Parameter]
    public EventCallback<ContentItem> OnSaveComplete { get; set; }

    /// <summary>Gets or sets the OnDiscard value.</summary>
    [Parameter]
    public EventCallback OnDiscard { get; set; }

    private string ActiveTab { get; set; } = "content";
    private bool IsDirty { get; set; } = false;

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        // Subscribe to changes from ViewModel
        ViewModel.StateChanged += StateHasChanged;

        // If ViewModel.EditingPost is null but AdminState has a post,
        // we need to initialize ViewModel from AdminState
        if (ViewModel.EditingPost is null && AdminState.EditingPost is not null)
        {
            ViewModel.InitializeFromState(AdminState.EditingPost, AdminState.IsCreatingNewFile);
        }

        // Subscribe to content-related events from EventBus
        EventSubscriber.Subscribe<ContentSelectedEvent>(OnContentSelected);
        EventSubscriber.Subscribe<CreateNewContentEvent>(OnCreateNewContent);
    }

    /// <summary>Releases resources held by the component or service.</summary>
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
        if (!string.IsNullOrWhiteSpace(e.Path))
        {
            ViewModel.LoadPostAsync(e.Path).ConfigureAwait(false);
        }
    }

    // Handle create new content event
    private void OnCreateNewContent(CreateNewContentEvent e)
    {
        if (AdminState.EditingPost is not null && AdminState.IsCreatingNewFile)
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

            if (OnSaveComplete.HasDelegate && ViewModel.EditingPost is not null)
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
