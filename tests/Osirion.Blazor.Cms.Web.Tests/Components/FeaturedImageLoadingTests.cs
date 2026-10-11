using Bunit;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

// The article's featured image sits in its header, in the first view: lazy loading it delayed Largest Contentful
// Paint. Lists further down the page keep lazy loading.
public class FeaturedImageLoadingTests : CmsWebComponentTestBase
{
    [Fact]
    public void ContentView_FeaturedImage_LoadsEagerlyWithHighPriority()
    {
        var item = CreateItem("<p>Post</p>");
        item.SetFeaturedImage("/img/cover.jpg");

        var img = RenderStatic<ContentView>(parameters => parameters.Add(p => p.Item, item)).Find("img.osirion-content-featured-image");

        img.GetAttribute("loading").ShouldBe("eager");
        img.GetAttribute("fetchpriority").ShouldBe("high");
        img.GetAttribute("decoding").ShouldBe("async");
    }

    [Fact]
    public void LocalizedContentView_FeaturedImage_LoadsEagerlyWithHighPriority()
    {
        var item = CreateItem("<p>Post</p>");
        item.SetFeaturedImage("/img/cover.jpg");

        var img = RenderStatic<LocalizedContentView>(parameters => parameters.Add(p => p.Item, item)).Find("img.osirion-content-featured-image");

        img.GetAttribute("loading").ShouldBe("eager");
        img.GetAttribute("fetchpriority").ShouldBe("high");
    }
}
