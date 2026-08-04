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
    /// <summary>Publishes events raised by the admin component.</summary>
    [Inject] protected IEventPublisher EventPublisher { get; set; } = null!;
    /// <summary>Provides access to event subscription operations.</summary>
    [Inject] protected IEventSubscriber EventSubscriber { get; set; } = null!;
    /// <summary>Provides navigation services for the admin component.</summary>
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    /// <summary>Provides shared CMS admin state.</summary>
    [Inject] protected CmsState AdminState { get; set; } = null!;

    /// <summary>Gets or sets the IsLoading value.</summary>
    protected bool IsLoading { get; set; }
    /// <summary>Gets or sets the ErrorMessage value.</summary>
    protected string? ErrorMessage { get; set; }

    /// <summary>Performs the Execute operation asynchronously.</summary>
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

    /// <summary>Performs the ExecuteWithLoading operation asynchronously.</summary>
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
