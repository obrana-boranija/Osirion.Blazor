using Microsoft.Extensions.Options;
using Osirion.Blazor.Navigation.Options;

namespace Osirion.Blazor.Navigation.Services;

/// <summary>
/// Manages enhanced navigation behavior
/// </summary>
public class EnhancedNavigationManager
{
    private readonly EnhancedNavigationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnhancedNavigationManager"/> class.
    /// </summary>
    /// <param name="options">The navigation options</param>
    public EnhancedNavigationManager(IOptions<EnhancedNavigationOptions> options)
    {
        _options = options?.Value ?? new EnhancedNavigationOptions();
    }

    /// <summary>
    /// Gets the scroll behavior to use
    /// </summary>
    public ScrollBehavior Behavior => _options.Behavior;

    /// <summary>
    /// Gets whether to reset scroll position on navigation
    /// </summary>
    public bool ResetScrollOnNavigation => _options.ResetScrollOnNavigation;

    /// <summary>
    /// Gets whether to preserve scroll position for same-page navigation
    /// </summary>
    public bool PreserveScrollForSamePageNavigation => _options.PreserveScrollForSamePageNavigation;

    /// <summary>
    /// Event that is raised when navigation options change
    /// </summary>
    public event EventHandler? OptionsChanged;

    /// <summary>
    /// Updates the navigation options
    /// </summary>
    /// <param name="options">The new options</param>
    public void UpdateOptions(EnhancedNavigationOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        _options.Behavior = options.Behavior;
        _options.ResetScrollOnNavigation = options.ResetScrollOnNavigation;
        _options.PreserveScrollForSamePageNavigation = options.PreserveScrollForSamePageNavigation;

        OnOptionsChanged();
    }

    /// <summary>
    /// Raises the OptionsChanged event
    /// </summary>
    protected virtual void OnOptionsChanged()
    {
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }
}