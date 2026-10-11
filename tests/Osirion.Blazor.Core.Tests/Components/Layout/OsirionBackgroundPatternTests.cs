using Bunit;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Layout;

public class OsirionBackgroundPatternTests : OsirionComponentTestBase
{
    [Fact]
    public void BackgroundPattern_NoPattern_RendersNothing()
    {
        var cut = RenderStatic<OsirionBackgroundPattern>(parameters => parameters
            .Add(p => p.BackgroundPattern, null));

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void BackgroundPattern_PatternWithDefaults_RendersMaskedWrapperWithPatternClass()
    {
        var cut = RenderStatic<OsirionBackgroundPattern>(parameters => parameters
            .Add(p => p.BackgroundPattern, BackgroundPatternType.Grid));

        cut.Find("div").ClassList.ShouldBe(["osirion-bg-wrapper", "osirion-bg-image-mask", "osirion-bg-grid"]);
    }

    [Fact]
    public void BackgroundPattern_MaskDisabledAndCustomClass_LeavesOutMaskAndAppendsClass()
    {
        var cut = RenderStatic<OsirionBackgroundPattern>(parameters => parameters
            .Add(p => p.BackgroundPattern, BackgroundPatternType.Circuit)
            .Add(p => p.MaskImage, false)
            .Add(p => p.Class, "custom-bg"));

        cut.Find("div").ClassList.ShouldBe(["osirion-bg-wrapper", "osirion-bg-circuit", "custom-bg"]);
    }
}
