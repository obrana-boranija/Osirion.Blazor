using Osirion.Blazor.Cms.Admin.Core.Events;

namespace Osirion.Blazor.Cms.Admin.Shared.Components;

/// <summary>
/// Base component with loading state management
/// </summary>
public abstract class LoadableComponentBase : BaseComponent
{
    /// <summary>Gets or sets the IsProcessing value.</summary>
    protected bool IsProcessing { get; private set; }

    /// <summary>Performs the ExecuteWithLoading operation asynchronously.</summary>
    protected new async Task ExecuteWithLoadingAsync(Func<Task> action)
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

    /// <summary>Executes an operation and returns a fallback value when it fails.</summary>
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
