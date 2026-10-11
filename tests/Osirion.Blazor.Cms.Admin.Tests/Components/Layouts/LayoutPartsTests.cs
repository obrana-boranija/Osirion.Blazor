using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Components.Layouts;
using Osirion.Blazor.Cms.Admin.Components.Layouts.Alerts;
using Osirion.Blazor.Cms.Admin.Components.Layouts.Navigation;
using Osirion.Blazor.Cms.Admin.Components.UI;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Layouts;

public class LayoutPartsTests : BunitContext
{
    public LayoutPartsTests()
    {
        SetRendererInfo(new RendererInfo("Static", false));
    }

    [Fact]
    public void BreadcrumbItem_DefaultsToNoUrl()
    {
        var item = new BreadcrumbItem("Posts");

        item.Text.ShouldBe("Posts");
        item.Url.ShouldBeEmpty();
    }

    [Fact]
    public void Breadcrumb_NoItemsOrPage_ShowsOnlyTheDashboardLink()
    {
        var cut = Render<Breadcrumb>();

        cut.FindAll(".breadcrumb-item").Count.ShouldBe(1);
        cut.Find(".breadcrumb-item a").GetAttribute("href").ShouldBe("/osirion");
    }

    [Fact]
    public void Breadcrumb_CurrentPageOnly_MarksItActive()
    {
        var cut = Render<Breadcrumb>(parameters => parameters.Add(p => p.CurrentPage, "Settings"));

        cut.Find(".breadcrumb-item.active").TextContent.ShouldBe("Settings");
    }

    [Fact]
    public void Breadcrumb_Items_LinkEveryItemWithAUrlExceptTheLast()
    {
        var cut = Render<Breadcrumb>(parameters => parameters
            .Add(p => p.CurrentPage, "Ignored")
            .Add(p => p.Items, [new BreadcrumbItem("Content", "/osirion/content"), new BreadcrumbItem("Drafts"), new BreadcrumbItem("Post", "/osirion/post")]));

        cut.FindAll(".breadcrumb-item a").Select(link => link.GetAttribute("href")).ShouldBe(["/osirion", "/osirion/content"]);
        cut.FindAll(".breadcrumb-item.active").Select(item => item.TextContent).ShouldBe(["Drafts", "Post"]);
        cut.Markup.ShouldNotContain("Ignored");
    }

    [Fact]
    public void Breadcrumb_EmptyItems_FallBackToTheCurrentPage()
    {
        var cut = Render<Breadcrumb>(parameters => parameters
            .Add(p => p.CurrentPage, "Media")
            .Add(p => p.Items, new List<BreadcrumbItem>()));

        cut.Find(".breadcrumb-item.active").TextContent.ShouldBe("Media");
    }

    [Fact]
    public void Header_Defaults_ShowTitleAndBreadcrumbWithoutSubtitleOrActions()
    {
        var cut = Render<Header>(parameters => parameters.Add(p => p.Title, "Posts"));

        cut.Find("h1").TextContent.ShouldBe("Posts");
        cut.Find(".breadcrumb-item.active").TextContent.ShouldBe("Posts");
        cut.FindAll("p.text-muted").ShouldBeEmpty();
        cut.FindAll(".gap-2").ShouldBeEmpty();
    }

    [Fact]
    public void Header_SubtitleActionsAndHiddenBreadcrumb_AreHonoured()
    {
        var cut = Render<Header>(parameters => parameters
            .Add(p => p.Subtitle, "All posts")
            .Add(p => p.ShowBreadcrumb, false)
            .Add(p => p.ActionContent, "<button id=\"new\">New</button>"));

        cut.Find("p.text-muted").TextContent.ShouldBe("All posts");
        cut.FindAll(".breadcrumb").ShouldBeEmpty();
        cut.Find(".gap-2 #new").ShouldNotBeNull();
    }

    [Fact]
    public void Header_BreadcrumbItems_ArePassedToTheBreadcrumb()
    {
        var cut = Render<Header>(parameters => parameters
            .Add(p => p.BreadcrumbItems, [new BreadcrumbItem("Content", "/osirion/content"), new BreadcrumbItem("Post")]));

        cut.FindAll(".breadcrumb-item").Select(item => item.TextContent.Trim()).ShouldBe(["Dashboard", "Content", "Post"]);
    }

    [Fact]
    public void StatusAlert_NoMessage_RendersNothing()
    {
        var cut = Render<StatusAlert>(parameters => parameters.Add(p => p.Message, " "));

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Theory]
    [InlineData(StatusAlert.AlertType.Success, "alert-success", "bi-check-circle-fill")]
    [InlineData(StatusAlert.AlertType.Error, "alert-danger", "bi-exclamation-circle-fill")]
    [InlineData(StatusAlert.AlertType.Warning, "alert-warning", "bi-exclamation-triangle-fill")]
    [InlineData(StatusAlert.AlertType.Info, "alert-info", "bi-info-circle-fill")]
    public void StatusAlert_Type_SelectsTheClassAndIcon(StatusAlert.AlertType type, string alertClass, string iconClass)
    {
        var cut = Render<StatusAlert>(parameters => parameters
            .Add(p => p.Message, "Hello")
            .Add(p => p.Type, type));

        cut.Find(".alert").ClassList.ShouldContain(alertClass);
        cut.Find("i").ClassList.ShouldContain(iconClass);
        cut.Find(".alert").TextContent.ShouldContain("Hello");
        cut.FindAll(".btn-close").ShouldBeEmpty();
    }

    [Fact]
    public void StatusAlert_UnknownType_FallsBackToInfoWithoutIcon()
    {
        var cut = Render<StatusAlert>(parameters => parameters
            .Add(p => p.Message, "Hello")
            .Add(p => p.Type, (StatusAlert.AlertType)99));

        cut.Find(".alert").ClassList.ShouldContain("alert-info");
        cut.FindAll("i").ShouldBeEmpty();
    }

    [Fact]
    public void StatusAlert_WithDismissHandler_ShowsACloseButtonThatInvokesIt()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        var dismissed = false;
        var cut = Render<StatusAlert>(parameters => parameters
            .Add(p => p.Message, "Hello")
            .Add(p => p.OnDismiss, () => dismissed = true));

        cut.Find(".btn-close").Click();

        dismissed.ShouldBeTrue();
    }

    [Fact]
    public void Card_BodyOnly_HasNoHeaderOrFooter()
    {
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.CssClass, "outer")
            .Add(p => p.BodyCssClass, "inner")
            .AddChildContent("<p id=\"body\">Body</p>"));

        cut.Find(".card").ClassList.ShouldContain("outer");
        cut.Find(".card-body").ClassList.ShouldContain("inner");
        cut.Find(".card-body #body").ShouldNotBeNull();
        cut.FindAll(".card-header").ShouldBeEmpty();
        cut.FindAll(".card-footer").ShouldBeEmpty();
    }

    [Fact]
    public void Card_TitleAndFooter_RenderHeaderAndFooter()
    {
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.Title, "Stats")
            .Add(p => p.FooterContent, "<span id=\"foot\">Foot</span>"));

        cut.Find(".card-header .card-title").TextContent.ShouldBe("Stats");
        cut.Find(".card-footer #foot").ShouldNotBeNull();
    }

    [Fact]
    public void Card_HeaderContentWithoutTitle_RendersHeaderWithoutTitle()
    {
        var cut = Render<Card>(parameters => parameters.Add(p => p.HeaderContent, "<button id=\"act\">Act</button>"));

        cut.Find(".card-header #act").ShouldNotBeNull();
        cut.FindAll(".card-title").ShouldBeEmpty();
    }
}
