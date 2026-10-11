namespace Osirion.Blazor.Components;

/// <summary>The frame shapes <see cref="OsirionVideoEmbed"/> reserves before the player loads.</summary>
public enum VideoAspectRatio
{
    /// <summary>16:9, the shape of most online video.</summary>
    Widescreen,

    /// <summary>4:3.</summary>
    Standard,

    /// <summary>1:1.</summary>
    Square,

    /// <summary>9:16, for vertical video such as YouTube Shorts.</summary>
    Vertical,
}
