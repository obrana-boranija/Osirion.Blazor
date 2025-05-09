using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Admin.Shared.Components;

/// <summary>
/// Base component for all admin components with common functionality
/// </summary>
public abstract class BaseComponent : OsirionComponentBase
{
    [Inject] protected IEventPublisher EventPublisher { get; set; } = null!;
    [Inject] protected IEventSubscriber EventSubscriber { get; set; } = null!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject] protected CmsState AdminState { get; set; } = null!;

    protected bool IsLoading { get; set; }
    protected string? ErrorMessage { get; set; }

    protected async Task ExecuteAsync(Func<Task> action)
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

    protected async Task ExecuteWithLoadingAsync(Func<Task> action)
    {
        try
        {
            IsLoading = true;
            StateHasChanged();
            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            EventPublisher.Publish(new ErrorOccurredEvent(ErrorMessage, ex));
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}