namespace Osirion.Blazor.Options;

/// <summary>
/// Configuration options for Osirion styling
/// </summary>
public class OsirionStyleOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "OsirionStyle";

    /// <summary>
    /// Gets or sets whether to use the default styles
    /// </summary>
    public bool UseStyles { get; set; } = true;

    /// <summary>
    /// Gets or sets custom CSS variables to override the default values
    /// </summary>
    public string? CustomVariables { get; set; }

    /// <summary>
    /// Gets or sets the CSS framework to integrate with
    /// </summary>
    public CssFramework FrameworkIntegration { get; set; } = CssFramework.None;
}