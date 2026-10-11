using Bunit;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.States;

public class OsirionPageLoadingTests : OsirionComponentTestBase
{
    [Fact]
    public void PageLoading_Defaults_RendersSpinnerAndDefaultText()
    {
        var cut = RenderStatic<OsirionPageLoading>(_ => { });

        cut.Find(".osirion-loading .osirion-spinner").ShouldNotBeNull();
        cut.Find(".osirion-loading p").TextContent.ShouldBe("Loading content...");
    }

    [Fact]
    public void PageLoading_CustomText_RendersThatText()
    {
        var cut = RenderStatic<OsirionPageLoading>(parameters => parameters
            .Add(p => p.Text, "Fetching posts"));

        cut.Find(".osirion-loading p").TextContent.ShouldBe("Fetching posts");
    }

    [Theory]
    [InlineData(false, "Loading")]
    [InlineData(true, "   ")]
    public void PageLoading_TextHiddenOrBlank_RendersSpinnerOnly(bool showText, string text)
    {
        var cut = RenderStatic<OsirionPageLoading>(parameters => parameters
            .Add(p => p.ShowText, showText)
            .Add(p => p.Text, text));

        cut.FindAll(".osirion-loading p").ShouldBeEmpty();
        cut.FindAll(".osirion-spinner").Count.ShouldBe(1);
    }
}
