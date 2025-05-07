using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Shared.Events;

namespace Osirion.Blazor.Cms.Admin.Shared.Components;

/// <summary>
/// Base class for all CMS Admin components with common functionality
/// </summary>
public abstract class BaseComponent : OsirionComponentBase, IDisposable
{
    [Inject]
    protected IEventPublisher EventPublisher { get; set; } = null!;

    [Inject]
    protected IEventSubscriber EventSubscriber { get; set; } = null!;

    protected bool IsLoading { get; private set; }
    protected string? ErrorMessage { get; set; }

    /// <summary>
    /// Executes an operation with loading state management
    /// </summary>
    protected async Task ExecuteWithLoadingAsync(Func<Task> action)
    {
        IsLoading = true;
        ErrorMessage = null;
        StateHasChanged();

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            EventPublisher.Publish(new ErrorOccurredEvent(ex.Message, ex));
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Executes an operation with loading state management and returns a result
    /// </summary>
    protected async Task<TResult?> ExecuteWithLoadingAsync<TResult>(Func<Task<TResult>> action)
    {
        IsLoading = true;
        ErrorMessage = null;
        StateHasChanged();

        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            EventPublisher.Publish(new ErrorOccurredEvent(ex.Message, ex));
            return default;
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Shows a notification message
    /// </summary>
    protected void ShowNotification(string message, StatusType type = StatusType.Info)
    {
        EventPublisher.Publish(new StatusNotificationEvent(message, type));
    }

    /// <summary>
    /// Shows an error message and optionally logs the exception
    /// </summary>
    protected void ShowError(string message, Exception? exception = null)
    {
        ErrorMessage = message;
        EventPublisher.Publish(new ErrorOccurredEvent(message, exception));
    }

    /// <summary>
    /// Cleans up any subscriptions when the component is disposed
    /// </summary>
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}