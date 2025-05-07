namespace Osirion.Blazor.Cms.Admin.Shared.Components;

/// <summary>
/// Base class for components that edit data
/// </summary>
public abstract class EditingComponent : BaseComponent
{
    protected bool IsDirty { get; set; }
    protected bool IsSaving { get; set; }

    protected void MarkAsDirty()
    {
        IsDirty = true;
    }

    protected async Task SaveChangesAsync(Func<Task> saveAction, Func<Task>? onSuccess = null)
    {
        IsSaving = true;
        ErrorMessage = null;

        try
        {
            await saveAction();
            IsDirty = false;

            if (onSuccess != null)
            {
                await onSuccess();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
            StateHasChanged();
        }
    }

    protected async Task<bool> ConfirmDiscardChangesAsync()
    {
        if (!IsDirty)
            return true;

        // In later real implementation, we would show a confirmation dialog
        // For simplicity, we'll just return true
        return true;
    }
}