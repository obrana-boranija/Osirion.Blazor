namespace Osirion.Blazor.Navigation.Options;

/// <summary>
/// Configuration options for enhanced navigation
/// </summary>
public class EnhancedNavigationOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "EnhancedNavigation";

    /// <summary>
    /// Gets or sets the default scroll behavior
    /// </summary>
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Auto;

    /// <summary>
    /// Gets or sets whether to reset scroll position on navigation
    /// </summary>
    public bool ResetScrollOnNavigation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to preserve scroll position for same-page navigation
    /// </summary>
    public bool PreserveScrollForSamePageNavigation { get; set; } = true;
}