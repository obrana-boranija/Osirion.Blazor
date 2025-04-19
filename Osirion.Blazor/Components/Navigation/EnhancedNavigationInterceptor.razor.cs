using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components.Navigation;

public partial class EnhancedNavigationInterceptor
{
    [Parameter]
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Auto;

    private string BehaviorString => Behavior switch
    {
        ScrollBehavior.Smooth => "smooth",
        ScrollBehavior.Instant => "instant",
        ScrollBehavior.Auto => "auto",
        _ => "auto"
    };

    private string GetScript()
    {
        return $@"
            <script>
                (function() {{
                    let currentUrl = window.location.href;
                    Blazor.addEventListener('enhancedload', () => {{
                        let newUrl = window.location.href;
                        if (currentUrl != newUrl) {{
                            window.scrollTo({{ top: 0, left: 0, behavior: '{BehaviorString}' }});
                        }}
                        currentUrl = newUrl;
                    }});
                }})();
            </script>
        ";
    }
}
