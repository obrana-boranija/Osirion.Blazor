using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services.State;

namespace Osirion.Blazor.Cms.Admin.Common.Base;

public abstract class EditableComponentBase : LoadableComponentBase
{
    [Inject]
    protected CmsApplicationState ApplicationState { get; set; } = default!;

    protected bool IsDirty { get; set; }
    protected bool IsSaving { get; set; }

    protected async Task SaveWithConfirmationAsync(Func<Task> saveAction)
    {
        IsSaving = true;

        try
        {
            await saveAction();
            IsDirty = false;
            ApplicationState.SetStatusMessage("Changes saved successfully.");
        }
        catch (Exception ex)
        {
            ApplicationState.SetErrorMessage($"Error saving changes: {ex.Message}");
            throw;
        }
        finally
        {
            IsSaving = false;
            StateHasChanged();
        }
    }

    protected void MarkAsDirty()
    {
        IsDirty = true;
    }

    // Add confirmation dialog logic for discarding changes
    protected async Task<bool> ConfirmDiscardChangesAsync()
    {
        if (!IsDirty)
            return true;

        // In a real implementation, you'd show a confirmation dialog
        // For simplicity, we'll just return true
        return true;
    }
}