using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Content;

public class OsirionReadingProgressTests : OsirionComponentTestBase
{
    public OsirionReadingProgressTests() => SetRendererInfo(new RendererInfo("Static", false));

    [Fact]
    public void OsirionReadingProgress_RendersOneDecorativeElementWithoutScript()
    {
        var cut = Render<OsirionReadingProgress>(p => p.Add(r => r.Class, "article-progress"));

        var bar = cut.Find("div.osirion-reading-progress");
        bar.GetAttribute("aria-hidden").ShouldBe("true");
        bar.ClassList.ShouldContain("article-progress");
        cut.FindAll("script").ShouldBeEmpty();
    }
}
