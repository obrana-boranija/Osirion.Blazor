using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Core.Components;
using Osirion.Blazor.Cms.Core.Tests.TestFixtures;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Components.Navigation;

public class DocumentTreeTests : TestContext
{
    public DocumentTreeTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));
    }

    [Fact]
    public void DocumentTree_ShouldRenderChildItems_Correctly()
    {
        // Arrange
        var sections = TestData.GetSampleDocTree();

        // Third item in first section has children
        var itemWithChildren = sections[0].Items[2];
        itemWithChildren.Children.Count.ShouldBe(2);

        // Act
        var cut = RenderComponent<DocumentTree>(parameters => parameters
            .Add(p => p.Sections, sections)
        );

        // Assert
        var childLists = cut.FindAll(".osirion-doc-tree-children");
        childLists.Count.ShouldBeGreaterThan(0);

        // Use AngleSharp's DOM methods to find child items
        var childItems = childLists[0].QuerySelectorAll(".osirion-doc-tree-child");
        childItems.Length.ShouldBe(2);

        // Check child item text
        childItems[0].TextContent.ShouldContain("Basic Setup");
        childItems[1].TextContent.ShouldContain("Advanced Options");

        // Check URLs
        var childLinks = childItems.Select(item => item.QuerySelector("a")).ToList();
        childLinks[0]?.GetAttribute("href").ShouldBe("/docs/config/basic");
        childLinks[1]?.GetAttribute("href").ShouldBe("/docs/config/advanced");
    }
}
