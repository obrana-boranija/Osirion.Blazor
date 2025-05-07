using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Admin.Shared.Components;

/// <summary>
/// Base class for all CMS Admin components with common functionality
/// </summary>
public abstract class BaseComponent : OsirionComponentBase
{
    protected string GetComponentCssClass(string defaultClass)
    {
        return string.IsNullOrEmpty(CssClass)
            ? defaultClass
            : $"{defaultClass} {CssClass}";
    }

    protected bool IsLoading { get; set; }
    protected string? ErrorMessage { get; set; }

    protected async Task ExecuteAsync(Func<Task> action, Action<Exception>? onError = null)
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
            onError?.Invoke(ex);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}