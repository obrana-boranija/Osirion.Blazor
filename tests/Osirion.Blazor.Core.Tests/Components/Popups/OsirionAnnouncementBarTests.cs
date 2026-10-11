using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Popups;

public class OsirionAnnouncementBarTests : OsirionComponentTestBase
{
    public OsirionAnnouncementBarTests() => SetRendererInfo(new RendererInfo("Static", false));

    [Fact]
    public void OsirionAnnouncementBar_NotDismissed_RendersTheMessageAndANoScriptCloseForm()
    {
        var cut = Render(new DefaultHttpContext());

        var bar = cut.Find("div.osirion-announcement");
        bar.GetAttribute("role").ShouldBe("region");
        bar.GetAttribute("aria-label").ShouldBe("Announcement");
        cut.Find(".osirion-announcement-content").TextContent.ShouldBe("Version 4.0 is out");
        var form = cut.Find("form");
        form.GetAttribute("method").ShouldBe("post");
        form.GetAttribute("action").ShouldBe("/api/announcement/dismiss");
        cut.Find("input[name='id']").GetAttribute("value").ShouldBe("release-4-0");
        cut.Find("input[name='days']").GetAttribute("value").ShouldBe("30");
        cut.Find("input[name='returnUrl']").GetAttribute("value").ShouldBe("/");
        cut.Find("button[type='submit']").GetAttribute("aria-label").ShouldBe("Dismiss announcement");
    }

    [Fact]
    public void OsirionAnnouncementBar_Dismissed_RendersNothing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "osirion_announcement_release-4-0=dismissed";

        Render(context).Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void OsirionAnnouncementBar_NoHttpContext_RendersNothing()
    {
        var cut = Render<OsirionAnnouncementBar>(p => p
            .Add(b => b.Id, "release-4-0")
            .AddChildContent("Version 4.0 is out"));

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void OsirionAnnouncementBar_NotDismissible_HasNoForm()
    {
        var cut = Render(new DefaultHttpContext(), dismissible: false);

        cut.FindAll("form").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("Release")]
    [InlineData("")]
    [InlineData("a b")]
    public void OsirionAnnouncementBar_InvalidId_Throws(string id)
    {
        Should.Throw<ArgumentException>(() => Render<OsirionAnnouncementBar>(p => p
            .Add(b => b.Id, id)
            .AddCascadingValue((HttpContext)new DefaultHttpContext())));
    }

    private IRenderedComponent<OsirionAnnouncementBar> Render(HttpContext context, bool dismissible = true) =>
        Render<OsirionAnnouncementBar>(p => p
            .Add(b => b.Id, "release-4-0")
            .Add(b => b.Dismissible, dismissible)
            .AddChildContent("Version 4.0 is out")
            .AddCascadingValue(context));
}
