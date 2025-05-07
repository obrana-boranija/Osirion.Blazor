using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Admin.Core.Events;

namespace Osirion.Blazor.Cms.Admin.Shared.Components;

/// <summary>
/// Base component for content editors
/// </summary>
public abstract class EditableComponent : BaseComponent
{
    [Inject]
    protected IJSRuntime JS { get; set; } = null!;

    protected bool IsDirty { get; set; }
    protected bool IsSaving { get; set; }
    protected bool HasConfirmedNavigation { get; set; }

    /// <summary>
    /// Marks content as modified
    /// </summary>
    protected void MarkAsDirty()
    {
        IsDirty = true;
        StateHasChanged();
    }

    /// <summary>
    /// Marks content as saved
    /// </summary>
    protected void MarkAsClean()
    {
        IsDirty = false;
        StateHasChanged();
    }

    /// <summary>
    /// Saves with loading state
    /// </summary>
    protected async Task SaveWithConfirmationAsync(Func<Task> saveAction)
    {
        if (IsSaving) return;

        IsSaving = true;
        ErrorMessage = null;
        StateHasChanged();

        try
        {
            await saveAction();
            IsDirty = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save: {ex.Message}";
            EventPublisher.Publish(new ErrorOccurredEvent(ErrorMessage, ex));
        }
        finally
        {
            IsSaving = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Confirms before discarding changes
    /// </summary>
    protected async Task<bool> ConfirmDiscardChangesAsync()
    {
        if (!IsDirty)
            return true;

        try
        {
            return await JS.InvokeAsync<bool>("confirm",
                "You have unsaved changes. Are you sure you want to discard them?");
        }
        catch
        {
            // If JS interop fails, allow the action
            return true;
        }
    }

    /// <summary>
    /// Confirms before navigation
    /// </summary>
    protected async Task<bool> ConfirmNavigationAsync(string message = "You have unsaved changes. Are you sure you want to leave?")
    {
        if (!IsDirty || HasConfirmedNavigation)
            return true;

        try
        {
            var result = await JS.InvokeAsync<bool>("confirm", message);
            if (result)
            {
                HasConfirmedNavigation = true;
            }
            return result;
        }
        catch
        {
            return true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RegisterBeforeUnloadHandlerAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task RegisterBeforeUnloadHandlerAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("eval", @"
                window.registerBeforeUnload = function() {
                    window.addEventListener('beforeunload', function(e) {
                        e.preventDefault();
                        e.returnValue = '';
                        return '';
                    });
                };
            ");

            await JS.InvokeVoidAsync("registerBeforeUnload");
        }
        catch
        {
            // Ignore errors, this is a progressive enhancement
        }
    }
}