using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts.Alerts;

/// <summary>
/// StatusAlert component for displaying status messages to the user
/// </summary>
public partial class StatusAlert
{
    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public string? Message { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public AlertType Type { get; set; } = AlertType.Info;

    /// <summary>
    /// 
    /// </summary>
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
