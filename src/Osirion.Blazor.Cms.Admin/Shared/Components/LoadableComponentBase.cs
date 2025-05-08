using Osirion.Blazor.Cms.Admin.Core.Events;

namespace Osirion.Blazor.Cms.Admin.Shared.Components;

/// <summary>
/// Base component with loading state management
/// </summary>
public abstract class LoadableComponentBase : BaseComponent
{
    protected bool IsProcessing { get; private set; }

    protected async Task ExecuteWithLoadingAsync(Func<Task> action)
    {
        IsProcessing = true;
        ErrorMessage = null;
        StateHasChanged();

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            EventPublisher.Publish(new ErrorOccurredEvent(ErrorMessage, ex));
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    protected T HandleOperation<T>(Func<T> action, T defaultValue)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            EventPublisher.Publish(new ErrorOccurredEvent(ErrorMessage, ex));
            return defaultValue;
        }
    }
}