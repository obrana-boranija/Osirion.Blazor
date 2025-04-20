using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Options;

namespace Osirion.Blazor.Components.Navigation;

/// <summary>
/// A component that provides a button to scroll back to the top of the page
/// </summary>
public partial class ScrollToTop
{
    /// <summary>
    /// Gets or sets the options for the scroll-to-top button
    /// </summary>
    [Parameter]
    public ScrollToTopOptions? Options { get; set; }

    /// <summary>
    /// Gets or sets the options from DI
    /// </summary>
    [Inject]
    public IOptions<ScrollToTopOptions>? ConfiguredOptions { get; set; }

    /// <summary>
    /// Gets or sets the behavior of the scroll animation
    /// </summary>
    [Parameter]
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Smooth;

    /// <summary>
    /// Gets or sets the visibility threshold in pixels
    /// </summary>
    [Parameter]
    public int VisibilityThreshold { get; set; } = 300;

    /// <summary>
    /// Gets or sets the position of the button
    /// </summary>
    [Parameter]
    public ButtonPosition Position { get; set; } = ButtonPosition.BottomRight;

    /// <summary>
    /// Gets or sets the title attribute for the button
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Scroll to top";

    /// <summary>
    /// Gets or sets the text to display on the button
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the custom CSS class for the button
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets the custom SVG icon markup
    /// </summary>
    [Parameter]
    public string? CustomIcon { get; set; }

    /// <summary>
    /// Gets the effective options, merging parameters with configured options
    /// </summary>
    private ScrollToTopOptions EffectiveOptions => Options ?? ConfiguredOptions?.Value ?? new ScrollToTopOptions();

    /// <summary>
    /// Gets the behavior string for the script
    /// </summary>
    private string BehaviorString => Behavior switch
    {
        ScrollBehavior.Smooth => "smooth",
        ScrollBehavior.Instant => "instant",
        ScrollBehavior.Auto => "auto",
        _ => "smooth"
    };

    /// <summary>
    /// Gets the CSS classes for the button based on the position parameter
    /// </summary>
    protected string GetButtonClass()
    {
        var position = Position;
        var behavior = Behavior;

        // Use options if parameters not explicitly set
        if (Options != null || ConfiguredOptions?.Value != null)
        {
            position = Position != ButtonPosition.BottomRight ? Position : EffectiveOptions.Position;
            behavior = Behavior != ScrollBehavior.Smooth ? Behavior : EffectiveOptions.Behavior;
        }

        var positionClass = position switch
        {
            ButtonPosition.BottomLeft => "bottom-left",
            ButtonPosition.TopRight => "top-right",
            ButtonPosition.TopLeft => "top-left",
            _ => "bottom-right"
        };

        return $"scroll-to-top {positionClass} {CssClass}".Trim();
    }

    /// <summary>
    /// Gets the inline JavaScript for the scroll-to-top functionality
    /// </summary>
    private string GetScript()
    {
        var threshold = VisibilityThreshold;
        var behavior = BehaviorString;

        // Use options if parameters not explicitly set
        if (Options != null || ConfiguredOptions?.Value != null)
        {
            threshold = VisibilityThreshold != 300 ? VisibilityThreshold : EffectiveOptions.VisibilityThreshold;
            behavior = Behavior != ScrollBehavior.Smooth ? BehaviorString : EffectiveOptions.BehaviorString;
        }

        return $@"
            <script>
                document.addEventListener('DOMContentLoaded', function() {{
                    // Initialize on DOM content loaded
                    initScrollToTop();
                }});

                // Initialize as soon as possible for quicker response
                initScrollToTop();

                function initScrollToTop() {{
                    // Set initial button visibility
                    const scrollButton = document.getElementById('osirion-scroll-to-top');
                    if (!scrollButton) return;
                    
                    // Handle initial state
                    updateScrollButtonVisibility(scrollButton, {threshold});
                    
                    // Handle window scroll for button visibility
                    window.addEventListener('scroll', function() {{
                        updateScrollButtonVisibility(scrollButton, {threshold});
                    }});
                    
                    // Handle button click - attach only once
                    if (!scrollButton.hasAttribute('data-initialized')) {{
                        scrollButton.setAttribute('data-initialized', 'true');
                        scrollButton.addEventListener('click', function(e) {{
                            e.preventDefault();
                            window.scrollTo({{ top: 0, left: 0, behavior: '{behavior}' }});
                        }});
                    }}
                }}

                function updateScrollButtonVisibility(button, threshold) {{
                    if (button) {{
                        button.style.display = window.scrollY > threshold ? 'flex' : 'none';
                    }}
                }}
            </script>
        ";
    }
}