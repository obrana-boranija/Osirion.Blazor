using Bunit;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Components;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Layouts.Components;

public class CmsLayoutEditorBehaviorTests : AdminComponentTestContext
{
    public CmsLayoutEditorBehaviorTests()
    {
        RenderInteractively();
    }

    /// <summary>Exposes the inherited error message, which only a derived layout can set.</summary>
    public sealed class ErrorEditor : CmsLayoutEditor
    {
        public void ShowError(string message)
        {
            ErrorMessage = message;
        }
    }

    [Fact]
    public void CmsLayoutEditor_BreadcrumbItems_SeparateItemsAndLinkAllButTheLast()
    {
        var cut = Render<CmsLayoutEditor>(parameters => parameters
            .Add(p => p.BreadcrumbItems, [new BreadcrumbItem("Content", "/admin/content"), new BreadcrumbItem("Drafts"), new BreadcrumbItem("Post", "/admin/post")]));

        cut.FindAll(".osirion-editor-breadcrumb-separator").Count.ShouldBe(2);
        cut.FindAll("a.osirion-editor-breadcrumb-item").Select(link => link.GetAttribute("href")).ShouldBe(["/admin/content"]);
        cut.FindAll(".osirion-editor-breadcrumb-current").Select(item => item.TextContent).ShouldBe(["Drafts", "Post"]);
    }

    [Fact]
    public void CmsLayoutEditor_EmptyBreadcrumbItems_ShowTheTitle()
    {
        var cut = Render<CmsLayoutEditor>(parameters => parameters
            .Add(p => p.Title, "Editor")
            .Add(p => p.BreadcrumbItems, new List<BreadcrumbItem>()));

        cut.Find(".osirion-editor-breadcrumb-current").TextContent.ShouldBe("Editor");
    }

    [Fact]
    public void CmsLayoutEditor_ActionsChildContentAndClass_AreRendered()
    {
        var cut = Render<CmsLayoutEditor>(parameters => parameters
            .Add(p => p.Class, "wide")
            .Add(p => p.ActionsTemplate, "<button id=\"save\">Save</button>")
            .AddChildContent("<p id=\"body\">Body</p>"));

        cut.Find(".osirion-editor-header-right #save").ShouldNotBeNull();
        cut.Find(".osirion-editor-main #body").ShouldNotBeNull();
        cut.Find(".osirion-editor-layout").ClassList.ShouldContain("wide");
        cut.FindAll(".osirion-admin-alert").ShouldBeEmpty();
    }

    [Fact]
    public void CmsLayoutEditor_CloseStatusMessage_HidesIt()
    {
        var cut = Render<CmsLayoutEditor>(parameters => parameters.Add(p => p.StatusMessage, "Saved"));
        cut.Find(".osirion-admin-alert-success").TextContent.ShouldContain("Saved");

        cut.Find(".osirion-admin-alert-success .osirion-admin-alert-close").Click();

        cut.FindAll(".osirion-admin-alert-success").ShouldBeEmpty();
    }

    [Fact]
    public async Task CmsLayoutEditor_CloseErrorMessage_HidesIt()
    {
        var cut = Render<ErrorEditor>();
        await cut.InvokeAsync(() => cut.Instance.ShowError("Broken"));
        cut.Render();
        cut.Find(".osirion-admin-alert-error").TextContent.ShouldContain("Broken");

        cut.Find(".osirion-admin-alert-error .osirion-admin-alert-close").Click();

        cut.FindAll(".osirion-admin-alert-error").ShouldBeEmpty();
    }
}
