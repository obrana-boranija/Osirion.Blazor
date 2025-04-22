namespace Osirion.Blazor.Theming;

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
    /// Tailwind CSS framework integration
    /// </summary>
    Tailwind = 2,

    /// <summary>
    /// Fluent UI for Blazor framework integration
    /// </summary>
    FluentUI = 3,

    /// <summary>
    /// MudBlazor framework integration
    /// </summary>
    MudBlazor = 4,

    /// <summary>
    /// Radzen Blazor framework integration
    /// </summary>
    Radzen = 5
}