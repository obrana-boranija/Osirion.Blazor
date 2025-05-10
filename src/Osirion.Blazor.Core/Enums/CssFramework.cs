namespace Osirion.Blazor.Components;

/// <summary>
/// Supported CSS frameworks for integration
/// </summary>
public enum CssFramework
{
    /// <summary>
    /// No framework integration (use Osirion default styles)
    /// </summary>
    None = 0,

    /// <summary>
    /// Bootstrap framework integration
    /// </summary>
    Bootstrap = 1,

    /// <summary>
    /// Fluent UI for Blazor framework integration
    /// </summary>
    FluentUI = 2,

    /// <summary>
    /// MudBlazor framework integration
    /// </summary>
    MudBlazor = 3,

    /// <summary>
    /// Radzen Blazor framework integration
    /// </summary>
    Radzen = 4
}