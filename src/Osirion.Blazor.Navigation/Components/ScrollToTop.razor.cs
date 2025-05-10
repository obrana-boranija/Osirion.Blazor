using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Options;

namespace Osirion.Blazor.Navigation.Components;

public partial class ScrollToTop
{
    /// <summary>
    /// Gets or sets the position of the button
    /// </summary>
    [Parameter]
    public Position Position { get; set; } = Position.BottomRight;

    /// <summary>
    /// Gets or sets the scroll behavior
    /// </summary>
    [Parameter]
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Smooth;

    /// <summary>
    /// Gets or sets the visibility threshold in pixels
    /// </summary>
    [Parameter]
    public int VisibilityThreshold { get; set; } = 300;

    /// <summary>
    /// Gets or sets the button text
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the button title
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Scroll to top";

    /// <summary>
    /// Gets or sets custom icon markup
    /// </summary>
    [Parameter]
    public string? CustomIcon { get; set; }

    /// <summary>
    /// Enable Scroll to Top button
    /// </summary>
    [Parameter]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets the effective options, merging parameters with configured options
    /// </summary>
    private ScrollToTopOptions EffectiveOptions
    {
        get
        {
            var options = Options?.Value ?? new ScrollToTopOptions();
            return new ScrollToTopOptions
            {
                Position = Position != Position.BottomRight ? Position : options.Position,
                Behavior = Behavior != ScrollBehavior.Smooth ? Behavior : options.Behavior,
                VisibilityThreshold = VisibilityThreshold != 300 ? VisibilityThreshold : options.VisibilityThreshold,
                Text = Text ?? options.Text,
                Title = Title != "Scroll to top" ? Title : options.Title,
                CssClass = CssClass ?? options.CssClass,
                CustomIcon = CustomIcon ?? options.CustomIcon,
                Enabled = (bool)(Enabled is null ? options.Enabled : Enabled)
            };
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    /// <summary>
    /// Gets the CSS class for the button based on position and options
    /// </summary>
    protected override string GetButtonClass()
    {
        var positionClass = EffectiveOptions.Position switch
        {
            Position.TopLeft => "top-left",
            Position.TopRight => "top-right",
            Position.BottomLeft => "bottom-left",
            Position.BottomRight => "bottom-right",
            _ => "bottom-right"
        };

        // Use both class naming conventions for better compatibility
        return $"{base.GetButtonClass()} osirion-scroll-to-top {positionClass} {EffectiveOptions.CssClass}".Trim();
    }

    private string GetScript()
    {
        return $@"
            <script id=""osirion-scroll-to-top-script"">
                (function() {{
                    function initScrollToTop() {{
                        const scrollButton = document.getElementById('osirion-scroll-to-top');
                        if (!scrollButton) return;

                        function updateButtonVisibility() {{
                            scrollButton.style.display = window.scrollY > {EffectiveOptions.VisibilityThreshold} ? 'flex' : 'none';
                        }}

                        updateButtonVisibility();

                        window.addEventListener('scroll', updateButtonVisibility);

                        if (!scrollButton.hasAttribute('data-initialized')) {{
                            scrollButton.setAttribute('data-initialized', 'true');
                            scrollButton.addEventListener('click', function(e) {{
                                e.preventDefault();
                                window.scrollTo({{
                                    top: 0,
                                    left: 0,
                                    behavior: '{(EffectiveOptions.Behavior.ToString().ToLowerInvariant())}'
                                }});
                            }});
                        }}
                    }}

                    if (document.readyState === 'loading') {{
                        document.addEventListener('DOMContentLoaded', initScrollToTop);
                    }} else {{
                        initScrollToTop();
                    }}
                }})();
            </script>
        ";
    }
}
