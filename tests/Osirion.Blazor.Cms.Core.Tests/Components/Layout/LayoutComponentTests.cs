using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Core.Components;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Components.Layout;

public class LayoutComponentTests : BunitContext
{
    public LayoutComponentTests()
    {
        SetRendererInfo(new RendererInfo("Static", false));
    }

    [Theory]
    [InlineData(PageLayout.ContainerSize.Small, "osirion-container-sm")]
    [InlineData(PageLayout.ContainerSize.Medium, "osirion-container-md")]
    [InlineData(PageLayout.ContainerSize.Large, "osirion-container-lg")]
    [InlineData(PageLayout.ContainerSize.ExtraLarge, "osirion-container-xl")]
    [InlineData((PageLayout.ContainerSize)42, "osirion-container-md")]
    public void PageLayout_Size_SetsTheContainerClass(PageLayout.ContainerSize size, string expectedClass)
    {
        var cut = Render<PageLayout>(parameters => parameters.Add(p => p.Size, size));

        cut.Find("div").ClassList.ShouldBe(["osirion-container", expectedClass], ignoreOrder: true);
    }

    [Fact]
    public void PageLayout_TitleAndDescription_RenderTheDefaultHeader()
    {
        var cut = Render<PageLayout>(parameters => parameters
            .Add(p => p.Title, "Guides")
            .Add(p => p.Description, "How to")
            .Add(p => p.Class, "extra")
            .AddChildContent("<p class=\"body\">Body</p>"));

        cut.Find("div").ClassList.ShouldContain("extra");
        cut.Find("header h1.osirion-page-title").TextContent.ShouldBe("Guides");
        cut.Find("header p.osirion-page-description").TextContent.ShouldBe("How to");
        cut.Find("p.body").TextContent.ShouldBe("Body");
    }

    [Fact]
    public void PageLayout_BlankDescription_OmitsTheDescription()
    {
        var cut = Render<PageLayout>(parameters => parameters
            .Add(p => p.Title, "Guides")
            .Add(p => p.Description, " "));

        cut.FindAll(".osirion-page-description").ShouldBeEmpty();
    }

    [Fact]
    public void PageLayout_HeaderTemplate_ReplacesTheDefaultHeader()
    {
        var cut = Render<PageLayout>(parameters => parameters
            .Add(p => p.Title, "Ignored")
            .Add(p => p.HeaderTemplate, "<span class=\"custom\">Custom</span>"));

        cut.Find("header span.custom").TextContent.ShouldBe("Custom");
        cut.FindAll("h1").ShouldBeEmpty();
    }

    [Fact]
    public void PageLayout_HeaderHidden_RendersNoHeader()
    {
        var cut = Render<PageLayout>(parameters => parameters
            .Add(p => p.Title, "Guides")
            .Add(p => p.ShowHeader, false));

        cut.FindAll("header").ShouldBeEmpty();
    }

    [Theory]
    [InlineData(false, true, new[] { "osirion-layout-with-sidebar" })]
    [InlineData(true, true, new[] { "osirion-layout-with-sidebar", "osirion-sidebar-right" })]
    [InlineData(false, false, new[] { "osirion-layout-with-sidebar", "osirion-sidebar-static" })]
    [InlineData(true, false, new[] { "osirion-layout-with-sidebar", "osirion-sidebar-right", "osirion-sidebar-static" })]
    public void TwoColumnLayout_Options_SetTheLayoutClasses(bool sidebarOnRight, bool sticky, string[] expected)
    {
        var cut = Render<TwoColumnLayout>(parameters => parameters
            .Add(p => p.SidebarOnRight, sidebarOnRight)
            .Add(p => p.StickyNavigation, sticky));

        cut.Find("div").ClassList.ShouldBe(expected, ignoreOrder: true);
    }

    [Fact]
    public void TwoColumnLayout_Content_RendersSidebarAndMain()
    {
        var cut = Render<TwoColumnLayout>(parameters => parameters
            .Add(p => p.Class, "docs")
            .Add(p => p.SidebarContent, "<nav>Menu</nav>")
            .Add(p => p.MainContent, "<article>Text</article>"));

        cut.Find("div").ClassList.ShouldContain("docs");
        cut.Find(".osirion-sidebar nav").TextContent.ShouldBe("Menu");
        cut.Find(".osirion-main-content article").TextContent.ShouldBe("Text");
    }

    [Fact]
    public void BlogLayout_WithImageAndFooter_RendersArticleParts()
    {
        var cut = Render<BlogLayout>(parameters => parameters
            .Add(p => p.Title, "Post")
            .Add(p => p.ImageUrl, "/cover.png")
            .Add(p => p.ImageAlt, "Cover")
            .Add(p => p.ImageCaption, "A cover")
            .Add(p => p.FooterContent, "<em>End</em>")
            .AddChildContent("<p>Body</p>"));

        cut.Find("div").ClassList.ShouldContain("osirion-container-sm");
        cut.Find("h1").TextContent.ShouldBe("Post");
        cut.Find("img.osirion-blog-featured-image").GetAttribute("alt").ShouldBe("Cover");
        cut.Find("figcaption").TextContent.ShouldBe("A cover");
        cut.Find(".osirion-blog-content p").TextContent.ShouldBe("Body");
        cut.Find("footer.osirion-blog-footer em").TextContent.ShouldBe("End");
    }

    [Fact]
    public void BlogLayout_WithoutImageOrFooter_OmitsThem()
    {
        var cut = Render<BlogLayout>(parameters => parameters.Add(p => p.ImageUrl, "/cover.png"));

        cut.FindAll("figcaption").ShouldBeEmpty();
        cut.FindAll("footer").ShouldBeEmpty();
        Render<BlogLayout>().FindAll("figure").ShouldBeEmpty();
    }

    [Fact]
    public void DocumentationLayout_RendersTreeTitleSidebarAndMainContent()
    {
        var cut = Render<DocumentationLayout>(parameters => parameters
            .Add(p => p.Title, "Docs")
            .Add(p => p.SidebarOnRight, true)
            .Add(p => p.SidebarContent, "<ul><li>Intro</li></ul>")
            .Add(p => p.MainContent, "<p>Page</p>"));

        cut.Find("div").ClassList.ShouldContain("osirion-container-xl");
        cut.Find(".osirion-layout-with-sidebar").ClassList.ShouldContain("osirion-sidebar-right");
        cut.Find(".osirion-doc-tree-title").TextContent.ShouldBe("Documentation");
        cut.Find(".osirion-doc-tree-container li").TextContent.ShouldBe("Intro");
        cut.Find(".osirion-doc-content-container p").TextContent.ShouldBe("Page");
    }

    [Fact]
    public void DocumentationLayout_NullTreeTitle_OmitsTheTitle()
    {
        var cut = Render<DocumentationLayout>(parameters => parameters.Add(p => p.DocTreeTitle, null!));

        cut.FindAll(".osirion-doc-tree-title").ShouldBeEmpty();
    }
}
