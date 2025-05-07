using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Admin.Core.Events;

namespace Osirion.Blazor.Cms.Admin.Shared.Components;

/// <summary>
/// Base class for components that provide editing capabilities
/// </summary>
public abstract class EditableComponent : BaseComponent
{
    [Inject]
    protected IJSRuntime JS { get; set; } = null!;

    protected bool IsDirty { get; set; }
    protected bool IsSaving { get; set; }
    protected bool HasConfirmedNavigation { get; set; }

    /// <summary>
    /// Marks the component's content as modified
    /// </summary>
    protected void MarkAsDirty()
    {
        IsDirty = true;
        StateHasChanged();
    }

    /// <summary>
    /// Marks the component's content as saved/clean
    /// </summary>
    protected void MarkAsClean()
    {
        IsDirty = false;
        StateHasChanged();
    }

    /// <summary>
    /// Performs a save operation with confirmation if needed
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
    /// Shows a confirmation dialog before discarding changes
    /// </summary>
    /// <returns>True if the user confirmed, or if there are no changes</returns>
    protected async Task<bool> ConfirmDiscardChangesAsync()
    {
        if (!IsDirty)
            return true;

        try
        {
            // In a real implementation, this would use a dialog service
            // For now, we'll use the browser's confirm dialog
            return await JS.InvokeAsync<bool>("confirm", "You have unsaved changes. Are you sure you want to discard them?");
        }
        catch
        {
            // If there's an error with JS interop, default to allowing the action
            return true;
        }
    }

    /// <summary>
    /// Prevents navigation when there are unsaved changes
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
            // If there's an error with JS interop, default to allowing navigation
            return true;
        }
    }

    /// <summary>
    /// Registers browser beforeunload event to prevent accidental navigation with unsaved changes
    /// </summary>
    protected async Task RegisterBeforeUnloadAsync()
    {
        await JS.InvokeVoidAsync("window.osirion.registerBeforeUnload");
    }

    /// <summary>
    /// Unregisters browser beforeunload event
    /// </summary>
    protected async Task UnregisterBeforeUnloadAsync()
    {
        await JS.InvokeVoidAsync("window.osirion.unregisterBeforeUnload");
    }

    /// <summary>
    /// Override of OnAfterRenderAsync to register browser events
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await EnsureOsirionJsModuleAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Ensures the Osirion JS module is initialized
    /// </summary>
    private async Task EnsureOsirionJsModuleAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("eval", @"
                window.osirion = window.osirion || {};
                window.osirion.registerBeforeUnload = function() {
                    window.addEventListener('beforeunload', window.osirion.beforeUnloadHandler);
                };
                window.osirion.unregisterBeforeUnload = function() {
                    window.removeEventListener('beforeunload', window.osirion.beforeUnloadHandler);
                };
                window.osirion.beforeUnloadHandler = function(e) {
                    e.preventDefault();
                    e.returnValue = '';
                    return '';
                };
            ");
        }
        catch
        {
            // Ignore errors, this is a progressive enhancement
        }
    }

    /// <summary>
    /// Override dispose to clean up resources
    /// </summary>
    public override async void Dispose()
    {
        try
        {
            await UnregisterBeforeUnloadAsync();
        }
        catch
        {
            // Ignore errors during cleanup
        }

        base.Dispose();
    }
}