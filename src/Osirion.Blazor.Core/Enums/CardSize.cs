namespace Osirion.Blazor.Components;

/// <summary>
/// Defines the size of the feature card, which affects the maximum dimensions based on image position.
/// </summary>
public enum CardSize
{
    /// <summary>
    /// Extra extra large card size. 
    /// For Top/Bottom image positions: max-width 1200px.
    /// For Left/Right image positions: max-height 1200px.
    /// </summary>
    ExtraExtraLarge,

    /// <summary>
    /// Extra large card size.
    /// For Top/Bottom image positions: max-width 900px.
    /// For Left/Right image positions: max-height 900px.
    /// </summary>
    ExtraLarge,

    /// <summary>
    /// Large card size.
    /// For Top/Bottom image positions: max-width 600px.
    /// For Left/Right image positions: max-height 600px.
    /// </summary>
    Large,

    /// <summary>
    /// Normal (default) card size.
    /// For Top/Bottom image positions: max-width 450px.
    /// For Left/Right image positions: max-height 450px.
    /// </summary>
    Normal,

    /// <summary>
    /// Small card size.
    /// For Top/Bottom image positions: max-width 300px.
    /// For Left/Right image positions: max-height 300px.
    /// </summary>
    Small,

    /// <summary>
    /// Extra small card size.
    /// For Top/Bottom image positions: max-width 150px.
    /// For Left/Right image positions: max-height 150px.
    /// </summary>
    ExtraSmall,

    /// <summary>
    /// Extra extra small card size.
    /// For Top/Bottom image positions: max-width 75px.
    /// For Left/Right image positions: max-height 75px.
    /// </summary>
    ExtraExtraSmall
}