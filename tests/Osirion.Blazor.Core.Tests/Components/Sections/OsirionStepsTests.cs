using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Models;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Sections;

public class OsirionStepsTests : OsirionComponentTestBase
{
    public OsirionStepsTests() => SetRendererInfo(new RendererInfo("Static", false));

    [Fact]
    public void OsirionSteps_Items_RenderAsAnOrderedListWithTitlesAndDescriptions()
    {
        var cut = Render<OsirionSteps>(p => p.Add(s => s.Items,
            [new StepItem("Install", "Add the package."), new StepItem("Configure"), new StepItem("Publish", "Deploy the site.")]));

        cut.Find("ol").ClassList.ShouldContain("osirion-steps");
        var steps = cut.FindAll("ol > li.osirion-step");
        steps.Select(s => s.QuerySelector(".osirion-step-title")!.TextContent).ShouldBe(["Install", "Configure", "Publish"]);
        steps[0].QuerySelector(".osirion-step-description")!.TextContent.ShouldBe("Add the package.");
        steps[1].QuerySelector(".osirion-step-description").ShouldBeNull();
        steps.ShouldAllBe(s => s.QuerySelector(".osirion-step-marker")!.GetAttribute("aria-hidden") == "true");
    }

    [Fact]
    public void OsirionSteps_Vertical_AddsTheVerticalClass()
    {
        var cut = Render<OsirionSteps>(p => p
            .Add(s => s.Items, [new StepItem("One")])
            .Add(s => s.Vertical, true)
            .Add(s => s.Class, "my-steps"));

        cut.Find("ol").ClassList.ShouldBe(["osirion-steps", "osirion-steps-vertical", "my-steps"], ignoreOrder: true);
    }

    [Fact]
    public void OsirionSteps_NoItems_RendersNothing()
    {
        Render<OsirionSteps>(p => p.Add(s => s.Items, [])).Markup.Trim().ShouldBeEmpty();
    }
}
