using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Sections;

public class OsirionPageHeadingTests : OsirionComponentTestBase
{
    public OsirionPageHeadingTests() => SetRendererInfo(new RendererInfo("Static", false));

    [Fact]
    public void OsirionPageHeading_TitleAndDescription_RenderAnH1AndTheSentenceInAHeader()
    {
        var cut = Render<OsirionPageHeading>(p => p
            .Add(h => h.Title, "Privacy policy")
            .Add(h => h.Description, "What this site stores and why."));

        cut.Find("header.osirion-page-heading > h1.osirion-page-heading-title").TextContent.ShouldBe("Privacy policy");
        cut.Find(".osirion-page-heading-description").TextContent.ShouldBe("What this site stores and why.");
    }

    [Fact]
    public void OsirionPageHeading_NoDescription_RendersOnlyTheTitle()
    {
        var cut = Render<OsirionPageHeading>(p => p.Add(h => h.Title, "Contact"));

        cut.FindAll("p").ShouldBeEmpty();
    }

    [Fact]
    public void OsirionPageHeading_ClassAndChildContent_AreKept()
    {
        var cut = Render<OsirionPageHeading>(p => p
            .Add(h => h.Title, "Release notes")
            .Add(h => h.Class, "narrow")
            .AddChildContent("<span class=\"badge\">4.0</span>"));

        cut.Find("header").ClassList.ShouldBe(["osirion-page-heading", "narrow"], ignoreOrder: true);
        cut.Find("header .badge").TextContent.ShouldBe("4.0");
    }
}
