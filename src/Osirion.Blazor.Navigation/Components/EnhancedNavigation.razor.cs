using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Options;

namespace Osirion.Blazor.Navigation.Components;

public partial class EnhancedNavigation
{

    /// <summary>
    /// Gets or sets the scroll behavior
    /// </summary>
    [Parameter]
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Auto;

    /// <summary>
    /// Gets or sets whether to reset scroll on navigation
    /// </summary>
    [Parameter]
    public bool ResetScrollOnNavigation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to preserve scroll for same-page navigation
    /// </summary>
    [Parameter]
    public bool PreserveScrollForSamePageNavigation { get; set; } = true;

    /// <summary>
    /// Gets the effective options, merging parameters with configured options
    /// </summary>
    private EnhancedNavigationOptions EffectiveOptions
    {
        get
        {
            var options = Options?.Value ?? new EnhancedNavigationOptions();
            return new EnhancedNavigationOptions
            {
                Behavior = Behavior != ScrollBehavior.Auto ? Behavior : options.Behavior,
                ResetScrollOnNavigation = ResetScrollOnNavigation,
                PreserveScrollForSamePageNavigation = PreserveScrollForSamePageNavigation
            };
        }
    }

    private string GetScript()
    {
        var behaviorString = EffectiveOptions.Behavior switch
        {
            ScrollBehavior.Smooth => "smooth",
            ScrollBehavior.Instant => "instant",
            _ => "auto"
        };

        return $@"
            <script>
                (function() {{
                    let currentUrl = window.location.href;

                    // Check if enhanced navigation is supported
                    if (typeof Blazor !== 'undefined' && 'addEventListener' in Blazor) {{
                        Blazor.addEventListener('enhancedload', () => {{
                            let newUrl = window.location.href;

                            if ({EffectiveOptions.ResetScrollOnNavigation.ToString().ToLower()}) {{
                                if ({EffectiveOptions.PreserveScrollForSamePageNavigation.ToString().ToLower()}) {{
                                    if (currentUrl !== newUrl) {{
                                        window.scrollTo({{ top: 0, left: 0, behavior: '{behaviorString}' }});
                                    }}
                                }} else {{
                                    window.scrollTo({{ top: 0, left: 0, behavior: '{behaviorString}' }});
                                }}
                            }}

                            currentUrl = newUrl;
                        }});
                    }}
                }})();
            </script>
        ";
    }
}
