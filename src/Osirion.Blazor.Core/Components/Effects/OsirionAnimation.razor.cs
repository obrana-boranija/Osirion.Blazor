using Microsoft.AspNetCore.Components;
using System.Text;

namespace Osirion.Blazor.Components;

/// <summary>
/// A reusable animated container component that provides AOS.js-based scroll animations
/// compatible with all Blazor hosting models (SSR, Server, WebAssembly).
/// </summary>
public partial class OsirionAnimation
{
    #region Core Parameters

    /// <summary>
    /// The content to be animated.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the animation should be shown (default: true).
    /// </summary>
    [Parameter]
    public bool Show { get; set; } = true;

    /// <summary>
    /// The animation effect to apply.
    /// </summary>
    [Parameter]
    public AnimationEffect Effect { get; set; } = AnimationEffect.FadeUp;

    /// <summary>
    /// Animation duration preset.
    /// </summary>
    [Parameter]
    public AnimationSpeed Speed { get; set; } = AnimationSpeed.Normal;

    /// <summary>
    /// Animation easing function.
    /// </summary>
    [Parameter]
    public AnimationEasing Easing { get; set; } = AnimationEasing.EaseOut;

    /// <summary>
    /// Delay before animation starts (0-3000ms).
    /// </summary>
    [Parameter]
    public int Delay { get; set; } = 0;

    #endregion

    #region Optional Customization Parameters

    /// <summary>
    /// Whether the animation can only happen once (default: false).
    /// </summary>
    [Parameter]
    public bool Once { get; set; } = false;

    /// <summary>
    /// Whether to mirror animation on scroll out (default: false).
    /// </summary>
    [Parameter]
    public bool Mirror { get; set; } = false;

    /// <summary>
    /// Anchor element ID or selector for positioning.
    /// </summary>
    [Parameter]
    public string? Anchor { get; set; }

    /// <summary>
    /// Custom animation distance in pixels.
    /// </summary>
    [Parameter]
    public int? Distance { get; set; }

    /// <summary>
    /// Custom animation duration in milliseconds (50-3000).
    /// </summary>
    [Parameter]
    public int? Duration { get; set; }

    /// <summary>
    /// Whether to respect reduced motion preferences (default: true).
    /// </summary>
    [Parameter]
    public bool RespectReducedMotion { get; set; } = true;

    /// <summary>
    /// Offset from the original trigger point (in px or %).
    /// </summary>
    [Parameter]
    public string? Offset { get; set; }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets the computed CSS classes.
    /// </summary>
    protected string CssClasses => BuildCssClasses();

    /// <summary>
    /// Gets the merged attributes including AOS data attributes.
    /// </summary>
    protected Dictionary<string, object> AosAttributes => BuildAosAttributes();

    #endregion

    #region Lifecycle Methods

    /// <summary>
    /// Validates parameters when they are set.
    /// </summary>
    protected override void OnParametersSet()
    {
        if(Show)
        {
            ValidateParameters();
        }
        
        base.OnParametersSet();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validates component parameters and throws exceptions for invalid values.
    /// </summary>
    private void ValidateParameters()
    {
        if (Delay < 0 || Delay > 3000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Delay),
                Delay,
                "Delay must be between 0 and 3000ms.");
        }

        if (Distance.HasValue && Distance.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Distance),
                Distance,
                "Distance must be positive.");
        }

        if (Duration.HasValue && (Duration.Value < 50 || Duration.Value > 3000))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Duration),
                Duration,
                "Duration must be between 50 and 3000ms.");
        }

        if (!Enum.IsDefined(typeof(AnimationEffect), Effect))
        {
            throw new ArgumentException($"Invalid animation effect: {Effect}", nameof(Effect));
        }

        if (!Enum.IsDefined(typeof(AnimationSpeed), Speed))
        {
            throw new ArgumentException($"Invalid animation speed: {Speed}", nameof(Speed));
        }

        if (!Enum.IsDefined(typeof(AnimationEasing), Easing))
        {
            throw new ArgumentException($"Invalid animation easing: {Easing}", nameof(Easing));
        }
    }

    /// <summary>
    /// Builds the CSS classes string for the animated container.
    /// </summary>
    /// <returns>A space-separated string of CSS classes.</returns>
    private string BuildCssClasses()
    {
        var classes = new StringBuilder();

        // Reduced motion support
        if (RespectReducedMotion)
        {
            classes.Append("osirion-respect-motion ");
        }

        // Additional custom classes
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Append(Class);
        }

        return classes.ToString().Trim();
    }

    /// <summary>
    /// Builds the merged attributes including AOS data attributes and user attributes.
    /// </summary>
    /// <returns>A dictionary of merged attributes.</returns>
    private Dictionary<string, object> BuildAosAttributes()
    {
        var attributes = new Dictionary<string, object>();

        // Start with user-provided attributes (if any)
        if (Attributes != null)
        {
            foreach (var attr in Attributes)
            {
                attributes[attr.Key] = attr.Value;
            }
        }

        // Add custom style if provided
        if (!string.IsNullOrWhiteSpace(Style))
        {
            attributes["style"] = Style;
        }

        // Core AOS animation
        attributes["data-aos"] = GetAosEffectName(Effect);

        // Duration - use custom duration or speed preset
        if (Duration.HasValue)
        {
            attributes["data-aos-duration"] = Duration.Value.ToString();
        }
        else
        {
            attributes["data-aos-duration"] = GetSpeedDuration(Speed).ToString();
        }

        // Easing
        attributes["data-aos-easing"] = GetAosEasingName(Easing);

        // Delay
        if (Delay > 0)
        {
            attributes["data-aos-delay"] = Delay.ToString();
        }

        // Once
        if (Once)
        {
            attributes["data-aos-once"] = "true";
        }

        // Mirror
        if (Mirror)
        {
            attributes["data-aos-mirror"] = "true";
        }

        // Anchor
        if (!string.IsNullOrWhiteSpace(Anchor))
        {
            attributes["data-aos-anchor"] = Anchor;
        }

        // Custom distance/offset
        if (Distance.HasValue)
        {
            attributes["data-aos-offset"] = Distance.Value.ToString();
        }
        else if (!string.IsNullOrWhiteSpace(Offset))
        {
            attributes["data-aos-offset"] = Offset;
        }
        else
        {
            // Set a default offset to ensure elements don't animate immediately when in viewport
            attributes["data-aos-offset"] = "50";
        }

        return attributes;
    }

    #endregion

    #region Static Helper Methods

    /// <summary>
    /// Gets the AOS animation name for the specified animation effect.
    /// </summary>
    /// <param name="effect">The animation effect.</param>
    /// <returns>The corresponding AOS animation name.</returns>
    private static string GetAosEffectName(AnimationEffect effect) => effect switch
    {
        AnimationEffect.Fade => "fade",
        AnimationEffect.FadeUp => "fade-up",
        AnimationEffect.FadeDown => "fade-down",
        AnimationEffect.FadeLeft => "fade-left",
        AnimationEffect.FadeRight => "fade-right",
        AnimationEffect.SlideUp => "slide-up",
        AnimationEffect.SlideDown => "slide-down",
        AnimationEffect.SlideLeft => "slide-left",
        AnimationEffect.SlideRight => "slide-right",
        AnimationEffect.ZoomIn => "zoom-in",
        AnimationEffect.ZoomOut => "zoom-out",
        _ => "fade-up"
    };

    /// <summary>
    /// Gets the duration in milliseconds for the specified animation speed.
    /// </summary>
    /// <param name="speed">The animation speed.</param>
    /// <returns>The corresponding duration in milliseconds.</returns>
    private static int GetSpeedDuration(AnimationSpeed speed) => speed switch
    {
        AnimationSpeed.Slow => 1000,
        AnimationSpeed.Normal => 600,
        AnimationSpeed.Fast => 400,
        AnimationSpeed.ExtraFast => 250,
        _ => 600
    };

    /// <summary>
    /// Gets the AOS easing name for the specified animation easing.
    /// </summary>
    /// <param name="easing">The animation easing.</param>
    /// <returns>The corresponding AOS easing name.</returns>
    private static string GetAosEasingName(AnimationEasing easing) => easing switch
    {
        AnimationEasing.Linear => "linear",
        AnimationEasing.EaseIn => "ease-in",
        AnimationEasing.EaseOut => "ease-out",
        AnimationEasing.EaseInOut => "ease-in-out",
        AnimationEasing.Bounce => "ease-in-back",
        AnimationEasing.Elastic => "ease-out-back",
        _ => "ease-out"
    };

    #endregion
}
