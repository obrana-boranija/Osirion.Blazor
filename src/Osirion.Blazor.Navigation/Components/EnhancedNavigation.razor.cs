using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Options;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>Defines the EnhancedNavigation type.</summary>
public partial class EnhancedNavigation
{

    /// <summary>
    /// Gets or sets the scroll behavior. When not set, the configured <see cref="EnhancedNavigationOptions.Behavior"/> applies
    /// </summary>
    [Parameter]
    public ScrollBehavior? Behavior { get; set; }

    /// <summary>
    /// Gets or sets whether to reset scroll on navigation. When not set, the configured <see cref="EnhancedNavigationOptions.ResetScrollOnNavigation"/> applies
    /// </summary>
    [Parameter]
    public bool? ResetScrollOnNavigation { get; set; }

    /// <summary>
    /// Gets or sets whether to preserve scroll for same-page navigation. When not set, the configured <see cref="EnhancedNavigationOptions.PreserveScrollForSamePageNavigation"/> applies
    /// </summary>
    [Parameter]
    public bool? PreserveScrollForSamePageNavigation { get; set; }

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
                Behavior = Behavior ?? options.Behavior,
                ResetScrollOnNavigation = ResetScrollOnNavigation ?? options.ResetScrollOnNavigation,
                PreserveScrollForSamePageNavigation = PreserveScrollForSamePageNavigation ?? options.PreserveScrollForSamePageNavigation
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
