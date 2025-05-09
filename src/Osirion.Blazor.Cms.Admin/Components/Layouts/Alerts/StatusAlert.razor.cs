using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts.Alerts;
public partial class StatusAlert
{
    [Parameter]
    public string? Message { get; set; }

    [Parameter]
    public AlertType Type { get; set; } = AlertType.Info;

    [Parameter]
    public EventCallback OnDismiss { get; set; }

    private string AlertClass => Type switch
    {
        AlertType.Success => "alert-success",
        AlertType.Error => "alert-danger",
        AlertType.Warning => "alert-warning",
        AlertType.Info => "alert-info",
        _ => "alert-info"
    };

    public enum AlertType
    {
        Success,
        Error,
        Warning,
        Info
    }
}
