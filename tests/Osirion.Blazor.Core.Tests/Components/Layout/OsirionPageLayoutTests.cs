using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Layout;

public class OsirionPageLayoutTests : OsirionComponentTestBase
{
    public OsirionPageLayoutTests()
    {
        Services.AddJSComponents();
        ComponentFactories.AddStub<BlazorJSComponents.JS>();
    }

    [Fact]
    public void OsirionPageLayout_Default_RendersSkipLinkFirstPointingAtMain()
    {
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("http://localhost/blog/post?page=2");

        var cut = RenderLayout(_ => { });

        var link = cut.Find(".osirion-page-layout > a.osirion-skip-link");
        cut.Find(".osirion-page-layout").FirstElementChild.ShouldBe(link);
        link.TextContent.ShouldBe("Skip to main content");
        var main = cut.Find("main");
        main.Id.ShouldBe("osirion-main-content");
        main.GetAttribute("tabindex").ShouldBe("-1");
        // The site sets <base href="/">, so a bare "#id" would resolve to the home page: keep the current address.
        link.GetAttribute("href").ShouldBe("http://localhost/blog/post?page=2#osirion-main-content");
    }

    [Fact]
    public void OsirionPageLayout_CurrentAddressHasFragment_ReplacesIt()
    {
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("http://localhost/docs#install");

        var cut = RenderLayout(_ => { });

        cut.Find("a.osirion-skip-link").GetAttribute("href").ShouldBe("http://localhost/docs#osirion-main-content");
    }

    [Fact]
    public void OsirionPageLayout_CustomTextAndTarget_AreUsed()
    {
        var cut = RenderLayout(parameters => parameters
            .Add(p => p.SkipLinkText, "Zum Inhalt springen")
            .Add(p => p.MainContentId, "content"));

        var link = cut.Find("a.osirion-skip-link");
        link.TextContent.ShouldBe("Zum Inhalt springen");
        link.GetAttribute("href").ShouldEndWith("#content");
        cut.Find("main").Id.ShouldBe("content");
    }

    [Fact]
    public void OsirionPageLayout_SkipLinkDisabled_RendersNoLinkButKeepsTheMainId()
    {
        var cut = RenderLayout(parameters => parameters.Add(p => p.ShowSkipLink, false));

        cut.FindAll("a.osirion-skip-link").ShouldBeEmpty();
        cut.Find("main").Id.ShouldBe("osirion-main-content");
    }

    // axe landmark-no-duplicate-contentinfo: OsirionFooter in the footer slot rendered a second <footer>.
    [Fact]
    public void OsirionPageLayout_OsirionFooterInTheFooterSlot_LeavesOneFooterLandmark()
    {
        var cut = RenderLayout(parameters => parameters.Add(p => p.Footer, (RenderFragment)(b =>
        {
            b.OpenComponent<OsirionFooter>(0);
            b.AddAttribute(1, nameof(OsirionFooter.CompanyName), "Osirion");
            b.CloseComponent();
        })));

        cut.FindAll("footer").Count.ShouldBe(1);
        cut.Find("footer.osirion-page-footer > div.osirion-footer").ShouldNotBeNull();
    }

    [Fact]
    public void OsirionFooter_OutsideThePageLayout_IsItsOwnFooterLandmark()
    {
        var cut = RenderStatic<OsirionFooter>(parameters => parameters.Add(p => p.CompanyName, "Osirion"));

        cut.Find("footer.osirion-footer").ShouldNotBeNull();
    }

    private IRenderedComponent<OsirionPageLayout> RenderLayout(
        Action<ComponentParameterCollectionBuilder<OsirionPageLayout>> parameters) =>
        RenderStatic<OsirionPageLayout>(builder =>
        {
            builder.Add(p => p.Body, (RenderFragment)(b => b.AddContent(0, "Body")));
            parameters(builder);
        });
}
