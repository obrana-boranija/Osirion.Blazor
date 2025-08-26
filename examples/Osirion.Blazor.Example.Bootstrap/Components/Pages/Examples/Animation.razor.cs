using Osirion.Blazor.Components;

namespace Osirion.Blazor.Example.Bootstrap.Components.Pages.Examples;
public partial class Animation
{
    #region Properties

    /// <summary>
    /// Gets a list of animation effects with their corresponding delays for demonstration.
    /// </summary>
    protected List<(AnimationEffect effect, int delay)> AnimationEffects { get; private set; } = new();

    /// <summary>
    /// Gets a list of sample features for demonstration purposes.
    /// </summary>
    protected List<FeatureItem> Features { get; private set; } = new();

    #endregion

    #region Lifecycle Methods

    /// <summary>
    /// Initializes the component with sample data for demonstrations.
    /// </summary>
    protected override void OnInitialized()
    {
        InitializeAnimationEffects();
        InitializeFeatures();
        base.OnInitialized();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets a human-readable description for the specified animation effect.
    /// </summary>
    /// <param name="effect">The animation effect to describe.</param>
    /// <returns>A descriptive string explaining what the animation does.</returns>
    public string GetEffectDescription(AnimationEffect effect) => effect switch
    {
        AnimationEffect.Fade => "Simple opacity fade without movement",
        AnimationEffect.FadeUp => "Fade with upward slide movement",
        AnimationEffect.FadeDown => "Fade with downward slide movement",
        AnimationEffect.FadeLeft => "Fade with leftward slide movement",
        AnimationEffect.FadeRight => "Fade with rightward slide movement",
        AnimationEffect.SlideUp => "Slide up without opacity change",
        AnimationEffect.SlideDown => "Slide down without opacity change",
        AnimationEffect.SlideLeft => "Slide left without opacity change",
        AnimationEffect.SlideRight => "Slide right without opacity change",
        AnimationEffect.ZoomIn => "Scale up with fade effect",
        AnimationEffect.ZoomOut => "Scale down with fade effect",
        _ => "Animation effect demonstration"
    };

    /// <summary>
    /// Gets an alternating animation effect based on the index position.
    /// Creates a pattern of Left -> Up -> Right that repeats.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <returns>The animation effect for the given index.</returns>
    public AnimationEffect GetAlternatingEffect(int index) => (index % 3) switch
    {
        0 => AnimationEffect.FadeLeft,
        1 => AnimationEffect.FadeUp,
        _ => AnimationEffect.FadeRight
    };

    /// <summary>
    /// Gets a feature-specific animation effect that cycles through different patterns.
    /// Creates visual variety for feature lists.
    /// </summary>
    /// <param name="index">The zero-based index of the feature.</param>
    /// <returns>The animation effect for the feature at the given index.</returns>
    public AnimationEffect GetFeatureEffect(int index) => (index % 4) switch
    {
        0 => AnimationEffect.FadeUp,
        1 => AnimationEffect.FadeRight,
        2 => AnimationEffect.FadeLeft,
        _ => AnimationEffect.ZoomIn
    };

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes the list of animation effects with staggered delays for demonstration.
    /// </summary>
    private void InitializeAnimationEffects()
    {
        AnimationEffects = new List<(AnimationEffect effect, int delay)>
        {
            (AnimationEffect.Fade, 1),
            (AnimationEffect.FadeUp, 2),
            (AnimationEffect.FadeDown, 3),
            (AnimationEffect.FadeLeft, 4),
            (AnimationEffect.FadeRight, 5),
            (AnimationEffect.SlideUp, 6),
            (AnimationEffect.SlideLeft, 7),
            (AnimationEffect.SlideRight, 8),
            (AnimationEffect.ZoomIn, 9),
            (AnimationEffect.ZoomOut, 10)
        };
    }

    /// <summary>
    /// Initializes the sample features list for demonstration purposes.
    /// </summary>
    private void InitializeFeatures()
    {
        Features = new List<FeatureItem>
        {
            new()
            {
                Icon = "🚀",
                Title = "High Performance",
                Description = "Optimized animations with GPU acceleration and minimal JavaScript footprint for smooth 60fps performance."
            },
            new()
            {
                Icon = "🔒",
                Title = "SSR Compatible",
                Description = "Works seamlessly across all Blazor hosting models including Static SSR, Server Interactive, and WebAssembly."
            },
            new()
            {
                Icon = "📱",
                Title = "Mobile Friendly",
                Description = "Responsive animations that adapt to different screen sizes and respect user motion preferences."
            },
            new()
            {
                Icon = "🎨",
                Title = "Highly Customizable",
                Description = "Rich set of parameters and CSS variables for fine-tuning animations to match your design system."
            },
            new()
            {
                Icon = "♿",
                Title = "Accessible by Default",
                Description = "Automatically respects prefers-reduced-motion and provides fallbacks for better accessibility."
            },
            new()
            {
                Icon = "🧪",
                Title = "Well Tested",
                Description = "Comprehensive unit tests ensure reliability and compatibility across different scenarios and environments."
            }
        };
    }

    #endregion
}

/// <summary>
/// Represents a feature item for demonstration purposes.
/// </summary>
public class FeatureItem
{
    /// <summary>
    /// Gets or sets the icon representation for the feature (emoji, SVG, or icon class).
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the feature.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of the feature.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}