using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Options;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Controls where the page scrolls after a Blazor enhanced navigation. It renders no visible output, only an inline
/// script that listens for the <c>enhancedload</c> event of the <c>Blazor</c> object, so place it after the Blazor
/// script. It needs no interactive render mode.
/// </summary>
public partial class EnhancedNavigation
{

    /// <summary>
    /// Gets or sets how the page scrolls to the top: <c>Auto</c>, <c>Instant</c> or <c>Smooth</c>. When not set, the
    /// configured <see cref="EnhancedNavigationOptions.Behavior"/> applies, which is <c>Auto</c> by default.
    /// </summary>
    [Parameter]
    public ScrollBehavior? Behavior { get; set; }

    /// <summary>
    /// Gets or sets whether the page scrolls to the top after an enhanced navigation. When not set, the configured
    /// <see cref="EnhancedNavigationOptions.ResetScrollOnNavigation"/> applies, which is <c>true</c> by default.
    /// </summary>
    [Parameter]
    public bool? ResetScrollOnNavigation { get; set; }

    /// <summary>
    /// Gets or sets whether an enhanced update that leaves the URL unchanged keeps the scroll position. It matters only
    /// while <see cref="ResetScrollOnNavigation"/> is on. When not set, the configured
    /// <see cref="EnhancedNavigationOptions.PreserveScrollForSamePageNavigation"/> applies, which is <c>true</c> by
    /// default.
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
