using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Admin.Common.Base;

public abstract class LoadableComponentBase : OsirionComponentBase
{
    [Parameter]
    public bool ShowLoadingIndicator { get; set; } = true;

    protected bool IsLoading { get; set; }
    protected string? ErrorMessage { get; set; }

    protected async Task ExecuteWithLoadingAsync(Func<Task> action)
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            OnError(ex);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    protected virtual void OnError(Exception ex) { }
}