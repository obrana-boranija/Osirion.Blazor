using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Navigation.Options;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>Defines the ScrollToTop type.</summary>
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
    /// Gets or sets custom icon markup.
    /// </summary>
    /// <remarks>
    /// The markup is passed through the Osirion HTML sanitizer before it is rendered, so scripts, event
    /// handlers and unsafe URLs are removed. The sanitizer also removes <c>svg</c> elements, so an SVG
    /// icon set here is dropped: use <see cref="IconCssClass"/> for an icon font or a CSS mask icon, or
    /// leave both unset for the built-in arrow. When no sanitizer is registered the markup is ignored.
    /// </remarks>
    [Parameter]
    public string? CustomIcon { get; set; }

    /// <summary>
    /// Gets or sets the CSS class of an icon element rendered inside the button, for example an icon
    /// font class. It is used when <see cref="CustomIcon"/> yields no markup.
    /// </summary>
    [Parameter]
    public string? IconCssClass { get; set; }

    /// <summary>
    /// Enable Scroll to Top button
    /// </summary>
    [Parameter]
    public bool? Enabled { get; set; }

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    // Generated once per instance: a fixed id is duplicated when two buttons are on the page, and the
    // script must address the button it was rendered with.
    private readonly string _instanceId = Guid.NewGuid().ToString("N")[..8];

    private string ButtonId => $"osirion-scroll-to-top-{_instanceId}";

    private string? SanitizedCustomIcon
    {
        get
        {
            if (string.IsNullOrWhiteSpace(EffectiveOptions.CustomIcon))
            {
                return null;
            }

            var sanitizer = ServiceProvider.GetService<IHtmlContentSanitizer>();
            return sanitizer?.Sanitize(EffectiveOptions.CustomIcon);
        }
    }

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
                CssClass = Class ?? options.CssClass,
                CustomIcon = CustomIcon ?? options.CustomIcon,
                IconCssClass = IconCssClass ?? options.IconCssClass,
                Enabled = Enabled ?? options.Enabled
            };
        }
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

    // Every value interpolated below is an int or an enum name, never user text. Both listeners are
    // registered once per button, behind the data-initialized guard: a script that runs again (a
    // re-render, an enhanced navigation) must not stack another window scroll listener.
    private string GetScript()
    {
        return $@"
            <script>
                (function() {{
                    function initScrollToTop() {{
                        const scrollButton = document.getElementById('{ButtonId}');
                        if (!scrollButton || scrollButton.hasAttribute('data-initialized')) return;
                        scrollButton.setAttribute('data-initialized', 'true');

                        function updateButtonVisibility() {{
                            scrollButton.style.display = window.scrollY > {EffectiveOptions.VisibilityThreshold} ? 'flex' : 'none';
                        }}

                        updateButtonVisibility();
                        window.addEventListener('scroll', updateButtonVisibility);

                        scrollButton.addEventListener('click', function(e) {{
                            e.preventDefault();
                            window.scrollTo({{
                                top: 0,
                                left: 0,
                                behavior: '{(EffectiveOptions.Behavior.ToString().ToLowerInvariant())}'
                            }});
                        }});
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
