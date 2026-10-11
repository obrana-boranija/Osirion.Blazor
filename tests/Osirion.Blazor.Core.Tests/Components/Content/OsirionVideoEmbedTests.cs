using BlazorJSComponents;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Content;

public class OsirionVideoEmbedTests : OsirionComponentTestBase
{
    private const string YouTubeId = "dQw4w9WgXcQ";

    public OsirionVideoEmbedTests()
    {
        Services.AddJSComponents();
        SetRendererInfo(new RendererInfo("Static", false));
    }

    [Fact]
    public void OsirionVideoEmbed_YouTube_RendersALinkToTheVideoWithItsThumbnailAndNoPlayer()
    {
        var cut = Render<OsirionVideoEmbed>(p => p
            .Add(v => v.VideoId, YouTubeId)
            .Add(v => v.Title, "Launch talk"));

        var link = cut.Find("a.osirion-video-embed-link");
        link.GetAttribute("href").ShouldBe($"https://www.youtube.com/watch?v={YouTubeId}");
        link.GetAttribute("aria-label").ShouldBe("Play video: Launch talk");
        var thumbnail = cut.Find("img.osirion-video-embed-thumbnail");
        thumbnail.GetAttribute("src").ShouldBe($"https://i.ytimg.com/vi/{YouTubeId}/hqdefault.jpg");
        thumbnail.GetAttribute("alt").ShouldBe(string.Empty);
        thumbnail.GetAttribute("loading").ShouldBe("lazy");
        cut.FindAll("iframe").ShouldBeEmpty();
        cut.Find(".osirion-video-embed").ClassList.ShouldContain("osirion-video-embed-widescreen");
    }

    [Fact]
    public void OsirionVideoEmbed_YouTube_PassesThePrivacyEnhancedPlayerUrlToTheScript()
    {
        var cut = Render<OsirionVideoEmbed>(p => p
            .Add(v => v.VideoId, YouTubeId)
            .Add(v => v.Title, "Launch talk")
            .Add(v => v.StartSeconds, 90));

        cut.Find("a").GetAttribute("href").ShouldBe($"https://www.youtube.com/watch?v={YouTubeId}&t=90s");
        PlayerUrl(cut).ShouldBe($"https://www.youtube-nocookie.com/embed/{YouTubeId}?autoplay=1&start=90");
    }

    [Fact]
    public void OsirionVideoEmbed_YouTubeWithoutPrivacyEnhancement_UsesTheRegularPlayer()
    {
        var cut = Render<OsirionVideoEmbed>(p => p
            .Add(v => v.VideoId, YouTubeId)
            .Add(v => v.Title, "T")
            .Add(v => v.PrivacyEnhanced, false));

        PlayerUrl(cut).ShouldBe($"https://www.youtube.com/embed/{YouTubeId}?autoplay=1");
    }

    [Fact]
    public void OsirionVideoEmbed_Vimeo_UsesDoNotTrackAndAFragmentForTheStart()
    {
        var cut = Render<OsirionVideoEmbed>(p => p
            .Add(v => v.Provider, VideoProvider.Vimeo)
            .Add(v => v.VideoId, "76979871")
            .Add(v => v.Title, "T")
            .Add(v => v.StartSeconds, 30));

        cut.Find("a").GetAttribute("href").ShouldBe("https://vimeo.com/76979871#t=30s");
        PlayerUrl(cut).ShouldBe("https://player.vimeo.com/video/76979871?autoplay=1&dnt=1#t=30s");
        cut.FindAll("img").ShouldBeEmpty();
    }

    [Fact]
    public void OsirionVideoEmbed_ThumbnailUrlAndAspectRatio_AreApplied()
    {
        var cut = Render<OsirionVideoEmbed>(p => p
            .Add(v => v.VideoId, YouTubeId)
            .Add(v => v.Title, "T")
            .Add(v => v.ThumbnailUrl, "/img/poster.jpg")
            .Add(v => v.AspectRatio, VideoAspectRatio.Vertical)
            .Add(v => v.PlayLabelText, "Video abspielen"));

        cut.Find("img").GetAttribute("src").ShouldBe("/img/poster.jpg");
        cut.Find(".osirion-video-embed").ClassList.ShouldContain("osirion-video-embed-vertical");
        cut.Find("a").GetAttribute("aria-label").ShouldBe("Video abspielen");
    }

    [Theory]
    [InlineData(VideoProvider.YouTube, "short")]
    [InlineData(VideoProvider.YouTube, "dQw4w9WgXc\"")]
    [InlineData(VideoProvider.YouTube, "")]
    [InlineData(VideoProvider.Vimeo, "abc")]
    [InlineData(VideoProvider.Vimeo, "123?x=1")]
    public void OsirionVideoEmbed_InvalidVideoId_Throws(VideoProvider provider, string videoId)
    {
        Should.Throw<ArgumentException>(() => Render<OsirionVideoEmbed>(p => p
            .Add(v => v.Provider, provider)
            .Add(v => v.VideoId, videoId)
            .Add(v => v.Title, "T")));
    }

    [Fact]
    public void OsirionVideoEmbed_NegativeStart_Throws()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Render<OsirionVideoEmbed>(p => p
            .Add(v => v.VideoId, YouTubeId)
            .Add(v => v.Title, "T")
            .Add(v => v.StartSeconds, -1)));
    }

    private static string PlayerUrl(IRenderedComponent<OsirionVideoEmbed> cut) =>
        (string)cut.FindComponent<JS>().Instance.Args![1]!;
}
