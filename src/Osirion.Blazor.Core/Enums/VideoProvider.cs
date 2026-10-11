namespace Osirion.Blazor.Components;

/// <summary>The video hosts <see cref="OsirionVideoEmbed"/> can embed.</summary>
public enum VideoProvider
{
    /// <summary>YouTube; the video ID is the 11 characters after <c>v=</c> in a watch URL.</summary>
    YouTube,

    /// <summary>Vimeo; the video ID is the number in the video's URL.</summary>
    Vimeo,
}
