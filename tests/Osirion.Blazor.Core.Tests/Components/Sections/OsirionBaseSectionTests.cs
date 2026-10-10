using AngleSharp.Css.Parser;
using Bunit;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Sections;

// Same defect as HeroSection.ImageUrl: BackgroundImageUrl was written into url('...') unescaped.
public class OsirionBaseSectionTests : OsirionComponentTestBase
{
    [Fact]
    public void BaseSection_BackgroundImageUrlThatClosesTheCssString_AddsNoDeclaration()
    {
        var declarations = RenderStyle(
            "x'); position:fixed; inset:0; z-index:2147483647; background:url('https://attacker.example/fake.png");

        declarations.GetPropertyValue("position").ShouldBeEmpty();
        declarations.GetPropertyValue("z-index").ShouldBeEmpty();
    }

    [Fact]
    public void BaseSection_BackgroundImageUrlWithLineBreak_WritesNoRawLineBreak()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.BackgroundImageUrl, "x\n); position:fixed;"));

        cut.Find("section").GetAttribute("style")!.ShouldNotContain("\n");
    }

    [Theory]
    [InlineData("assets/images/bg.webp")]
    [InlineData("https://cdn.example.com/images/bg.png?w=1200&h=630")]
    public void BaseSection_PlainBackgroundImageUrl_IsWrittenUnchanged(string imageUrl)
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.BackgroundImageUrl, imageUrl));

        cut.Find("section").GetAttribute("style")!.ShouldContain($"background-image: url('{imageUrl}')");
    }

    private AngleSharp.Css.Dom.ICssStyleDeclaration RenderStyle(string imageUrl)
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.BackgroundImageUrl, imageUrl));

        return new CssParser().ParseDeclaration(cut.Find("section").GetAttribute("style")!)
            ?? throw new InvalidOperationException("The style attribute could not be parsed.");
    }
}
