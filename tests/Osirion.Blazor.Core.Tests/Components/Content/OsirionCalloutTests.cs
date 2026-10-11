using Bunit;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Content;

public class OsirionCalloutTests : OsirionComponentTestBase
{
    [Fact]
    public void OsirionCallout_Default_RendersANoteWithItsTitleAndContent()
    {
        var cut = RenderStatic<OsirionCallout>(parameters => parameters.AddChildContent("<p>Body text</p>"));

        var callout = cut.Find("div.osirion-callout");
        callout.ClassList.ShouldContain("osirion-callout-note");
        callout.GetAttribute("role").ShouldBe("note");
        cut.Find(".osirion-callout-title").TextContent.ShouldBe("Note");
        cut.Find(".osirion-callout-body p").TextContent.ShouldBe("Body text");
    }

    [Theory]
    [InlineData(CalloutType.Note, "osirion-callout-note", "Note")]
    [InlineData(CalloutType.Tip, "osirion-callout-tip", "Tip")]
    [InlineData(CalloutType.Important, "osirion-callout-important", "Important")]
    [InlineData(CalloutType.Warning, "osirion-callout-warning", "Warning")]
    [InlineData(CalloutType.Caution, "osirion-callout-caution", "Caution")]
    public void OsirionCallout_Type_SetsTheClassAndDefaultTitle(CalloutType type, string expectedClass, string expectedTitle)
    {
        var cut = RenderStatic<OsirionCallout>(parameters => parameters.Add(p => p.Type, type));

        cut.Find("div.osirion-callout").ClassList.ShouldContain(expectedClass);
        cut.Find(".osirion-callout-title").TextContent.ShouldBe(expectedTitle);
    }

    [Fact]
    public void OsirionCallout_CustomTitleClassAndAttributes_AreApplied()
    {
        var cut = RenderStatic<OsirionCallout>(parameters => parameters
            .Add(p => p.Type, CalloutType.Warning)
            .Add(p => p.Title, "Back up first")
            .Add(p => p.Class, "my-callout")
            .AddUnmatched("id", "backup-warning"));

        var callout = cut.Find("div.osirion-callout");
        callout.ClassList.ShouldContain("my-callout");
        callout.Id.ShouldBe("backup-warning");
        cut.FindAll("#backup-warning").Count.ShouldBe(1);
        cut.Find(".osirion-callout-title").TextContent.ShouldBe("Back up first");
    }
}
