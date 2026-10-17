using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// A YouTube or Vimeo video that shows a thumbnail and loads the player only when the visitor clicks it. The page
/// stays fast, and nothing from the video host but the thumbnail loads before that click. Without script the
/// thumbnail is a link to the video on its own site.
/// </summary>
public partial class OsirionVideoEmbed
{
    /// <summary>
    /// Gets or sets the video host: <see cref="VideoProvider.YouTube"/> or <see cref="VideoProvider.Vimeo"/>.
    /// </summary>
    [Parameter]
    public VideoProvider Provider { get; set; } = VideoProvider.YouTube;

    /// <summary>
    /// Gets or sets the video ID: the 11 characters after <c>v=</c> in a YouTube watch address (letters, digits,
    /// <c>-</c> or <c>_</c>), or the number in a Vimeo address (digits). Any other text throws
    /// <see cref="ArgumentException"/>. Required.
    /// </summary>
    [Parameter, EditorRequired]
    public string VideoId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the video's title. It is shown over the top of the thumbnail and names the play link and the
    /// player for screen readers. Required.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the thumbnail image. Defaults to YouTube's own thumbnail (from <c>i.ytimg.com</c>); Vimeo has
    /// no public thumbnail address, so without one a Vimeo embed shows a plain black frame. Host the image yourself
    /// to send no request to the video host before the click.
    /// </summary>
    [Parameter]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Gets or sets where playback starts, in seconds, in the player address and in the link that works without
    /// script. Not set by default; a negative value throws <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Parameter]
    public int? StartSeconds { get; set; }

    /// <summary>
    /// Gets or sets whether the player is asked not to track the visitor: YouTube's <c>youtube-nocookie.com</c>
    /// domain, or Vimeo's <c>dnt=1</c>.
    /// </summary>
    [Parameter]
    public bool PrivacyEnhanced { get; set; } = true;

    /// <summary>
    /// Gets or sets the frame's shape: <c>Widescreen</c> (16:9), <c>Standard</c> (4:3), <c>Square</c> (1:1) or
    /// <c>Vertical</c> (9:16, at most 22.5rem wide).
    /// </summary>
    [Parameter]
    public VideoAspectRatio AspectRatio { get; set; } = VideoAspectRatio.Widescreen;

    /// <summary>
    /// Gets or sets the accessible name of the play link. Defaults to "Play video: " followed by the
    /// <see cref="Title"/>; set it for other languages.
    /// </summary>
    [Parameter]
    public string? PlayLabelText { get; set; }

    private string PlayLabel => string.IsNullOrWhiteSpace(PlayLabelText) ? $"Play video: {Title}" : PlayLabelText;

    private string WatchUrl { get; set; } = string.Empty;

    private string EmbedUrl { get; set; } = string.Empty;

    private string? EffectiveThumbnailUrl { get; set; }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if (!IsValidVideoId(Provider, VideoId))
        {
            throw new ArgumentException(
                Provider == VideoProvider.YouTube
                    ? $"'{VideoId}' is not a YouTube video ID (11 letters, digits, '-' or '_')."
                    : $"'{VideoId}' is not a Vimeo video ID (digits).",
                nameof(VideoId));
        }

        if (StartSeconds is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(StartSeconds), StartSeconds, "StartSeconds cannot be negative.");
        }

        var start = StartSeconds is > 0 ? StartSeconds.Value.ToString(CultureInfo.InvariantCulture) : null;
        (WatchUrl, EmbedUrl) = Provider == VideoProvider.YouTube ? YouTubeUrls(start) : VimeoUrls(start);
        EffectiveThumbnailUrl = !string.IsNullOrWhiteSpace(ThumbnailUrl)
            ? ThumbnailUrl
            : Provider == VideoProvider.YouTube ? $"https://i.ytimg.com/vi/{VideoId}/hqdefault.jpg" : null;
    }

    private (string Watch, string Embed) YouTubeUrls(string? start)
    {
        var host = PrivacyEnhanced ? "https://www.youtube-nocookie.com" : "https://www.youtube.com";
        var watch = $"https://www.youtube.com/watch?v={VideoId}" + (start is null ? string.Empty : $"&t={start}s");
        var embed = $"{host}/embed/{VideoId}?autoplay=1" + (start is null ? string.Empty : $"&start={start}");
        return (watch, embed);
    }

    private (string Watch, string Embed) VimeoUrls(string? start)
    {
        var fragment = start is null ? string.Empty : $"#t={start}s";
        var watch = $"https://vimeo.com/{VideoId}{fragment}";
        var embed = $"https://player.vimeo.com/video/{VideoId}?autoplay=1" + (PrivacyEnhanced ? "&dnt=1" : string.Empty) + fragment;
        return (watch, embed);
    }

    private static bool IsValidVideoId(VideoProvider provider, string? videoId) =>
        !string.IsNullOrEmpty(videoId) && (provider == VideoProvider.YouTube
            ? YouTubeIdPattern().IsMatch(videoId)
            : VimeoIdPattern().IsMatch(videoId));

    private string GetContainerClass()
    {
        var classes = $"osirion-video-embed osirion-video-embed-{AspectRatio.ToString().ToLowerInvariant()}";
        return string.IsNullOrWhiteSpace(Class) ? classes : $"{classes} {Class}";
    }

    [GeneratedRegex("^[A-Za-z0-9_-]{11}$", RegexOptions.CultureInvariant)]
    private static partial Regex YouTubeIdPattern();

    [GeneratedRegex("^[0-9]{1,20}$", RegexOptions.CultureInvariant)]
    private static partial Regex VimeoIdPattern();
}
