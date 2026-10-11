using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Content;

public class OsirionAccordionTests : OsirionComponentTestBase
{
    [Fact]
    public void OsirionAccordion_Items_RenderAsNativeDetailsWithTheirTitles()
    {
        var cut = RenderAccordion(parameters => { });

        var items = cut.FindAll("details.osirion-accordion-item");
        items.Count.ShouldBe(2);
        items[0].QuerySelector("summary")!.TextContent.Trim().ShouldBe("First");
        items[1].QuerySelector(".osirion-accordion-content")!.TextContent.ShouldContain("Second content");
    }

    [Fact]
    public void OsirionAccordion_ItemOpen_RendersTheOpenAttribute()
    {
        var cut = RenderAccordion(parameters => { });

        cut.FindAll("details").Select(d => d.HasAttribute("open")).ShouldBe([true, false]);
    }

    [Fact]
    public void OsirionAccordion_AllowMultipleByDefault_ItemsHaveNoSharedName()
    {
        var cut = RenderAccordion(parameters => { });

        cut.FindAll("details").ShouldAllBe(d => !d.HasAttribute("name"));
    }

    [Fact]
    public void OsirionAccordion_OneAtATime_ItemsShareANameSoTheBrowserClosesTheOthers()
    {
        var cut = RenderAccordion(parameters => parameters.Add(p => p.AllowMultiple, false));

        var names = cut.FindAll("details").Select(d => d.GetAttribute("name")).ToList();
        names.ShouldAllBe(n => !string.IsNullOrEmpty(n));
        names.Distinct().Count().ShouldBe(1);
    }

    private IRenderedComponent<OsirionAccordion> RenderAccordion(Action<ComponentParameterCollectionBuilder<OsirionAccordion>> parameters) =>
        RenderStatic<OsirionAccordion>(builder =>
        {
            builder.Add(p => p.ChildContent, (RenderFragment)(tree =>
            {
                tree.AddContent(0, Item("First", open: true));
                tree.AddContent(1, Item("Second", open: false));
            }));
            parameters(builder);
        });

    private static RenderFragment Item(string title, bool open) => tree =>
    {
        tree.OpenComponent<OsirionAccordionItem>(0);
        tree.AddAttribute(1, nameof(OsirionAccordionItem.Title), title);
        tree.AddAttribute(2, nameof(OsirionAccordionItem.Open), open);
        tree.AddAttribute(3, nameof(OsirionAccordionItem.ChildContent), (RenderFragment)(b => b.AddContent(0, $"{title} content")));
        tree.CloseComponent();
    };
}
