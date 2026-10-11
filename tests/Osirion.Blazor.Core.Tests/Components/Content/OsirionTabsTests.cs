using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Content;

public class OsirionTabsTests : OsirionComponentTestBase
{
    [Fact]
    public void OsirionTabs_ThreeTabs_RendersOneRadioGroupWithLinkedLabelsAndPanels()
    {
        var cut = RenderTabs(parameters => { });

        var inputs = cut.FindAll("input.osirion-tabs-input");
        inputs.Count.ShouldBe(3);
        inputs.Select(i => i.GetAttribute("type")).ShouldAllBe(t => t == "radio");
        inputs.Select(i => i.GetAttribute("name")).Distinct().Count().ShouldBe(1);

        for (var i = 0; i < 3; i++)
        {
            var input = inputs[i];
            var label = cut.Find($"label[for='{input.Id}']");
            var panel = cut.Find($"#{input.GetAttribute("aria-controls")}");
            panel.GetAttribute("aria-labelledby").ShouldBe(label.Id);
        }

        cut.FindAll("label.osirion-tabs-label").Select(l => l.TextContent).ShouldBe(["Bootstrap", "MudBlazor", "Fluent UI"]);
    }

    [Fact]
    public void OsirionTabs_Default_SelectsTheFirstTab()
    {
        var cut = RenderTabs(parameters => { });

        cut.FindAll("input.osirion-tabs-input").Select(i => i.HasAttribute("checked")).ShouldBe([true, false, false]);
    }

    [Fact]
    public void OsirionTabs_SelectedIndex_SelectsThatTab()
    {
        var cut = RenderTabs(parameters => parameters.Add(p => p.SelectedIndex, 2));

        cut.FindAll("input.osirion-tabs-input").Select(i => i.HasAttribute("checked")).ShouldBe([false, false, true]);
    }

    [Fact]
    public void OsirionTabs_TwoInstances_UseDifferentGroupNames()
    {
        var first = RenderTabs(parameters => { });
        var second = RenderTabs(parameters => { });

        first.Find("input").GetAttribute("name").ShouldNotBe(second.Find("input").GetAttribute("name"));
    }

    [Fact]
    public void OsirionTabs_ParentRendersAgain_KeepsIdsAndIndexes()
    {
        var cut = RenderTabs(parameters => { });
        var idsBefore = cut.FindAll("input").Select(i => i.Id).ToList();

        cut.Render(parameters => parameters.Add(p => p.AriaLabel, "Framework"));

        cut.FindAll("input").Select(i => i.Id).ShouldBe(idsBefore);
        cut.FindAll("input.osirion-tabs-input").Select(i => i.HasAttribute("checked")).ShouldBe([true, false, false]);
    }

    [Fact]
    public void OsirionTabs_IdGiven_PrefixesTheGeneratedIds()
    {
        var cut = RenderTabs(parameters => parameters.Add(p => p.Id, "install"));

        cut.Find("input").GetAttribute("name").ShouldBe("install");
        cut.Find("input").Id.ShouldBe("install-0");
        cut.Find(".osirion-tabs").GetAttribute("role").ShouldBe("group");
    }

    [Fact]
    public void OsirionTab_OutsideOsirionTabs_Throws()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        Should.Throw<InvalidOperationException>(() => Render<OsirionTab>(parameters => parameters.Add(p => p.Title, "Alone")));
    }

    private IRenderedComponent<OsirionTabs> RenderTabs(Action<ComponentParameterCollectionBuilder<OsirionTabs>> parameters) =>
        RenderStatic<OsirionTabs>(builder =>
        {
            builder.Add(p => p.ChildContent, (RenderFragment)(tree =>
            {
                tree.AddContent(0, Tab("Bootstrap"));
                tree.AddContent(1, Tab("MudBlazor"));
                tree.AddContent(2, Tab("Fluent UI"));
            }));
            parameters(builder);
        });

    private static RenderFragment Tab(string title) => tree =>
    {
        tree.OpenComponent<OsirionTab>(0);
        tree.AddAttribute(1, nameof(OsirionTab.Title), title);
        tree.AddAttribute(2, nameof(OsirionTab.ChildContent), (RenderFragment)(b => b.AddContent(0, $"{title} content")));
        tree.CloseComponent();
    };
}
