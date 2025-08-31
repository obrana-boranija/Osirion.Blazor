using System.ComponentModel;

namespace Osirion.Blazor.Components;

/// <summary>
/// Defines alert/notification variants
/// </summary>
public enum Severity
{
    /// <summary>
    /// Success alert (typically green)
    /// </summary>
    [Description("Success alert")]
    Success,

    /// <summary>
    /// Error/danger alert (typically red)
    /// </summary>
    [Description("Error alert")]
    Error,

    /// <summary>
    /// Warning alert (typically yellow/orange)
    /// </summary>
    [Description("Warning alert")]
    Warning,

    /// <summary>
    /// Info alert (typically blue)
    /// </summary>
    [Description("Info alert")]
    Info
}