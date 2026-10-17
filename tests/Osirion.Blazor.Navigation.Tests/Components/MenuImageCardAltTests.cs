using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuImageCardAltTests : BunitContext
{
    public MenuImageCardAltTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));
    }

    // Fails before the fix: without ImageAlt the image rendered with no alt attribute at all, so a screen reader
    // announced the file name of the image instead of skipping it.
    [Fact]
    public void MenuImageCard_WithoutImageAlt_RendersAnEmptyAltSoTheImageIsDecorative()
    {
        var cut = Render<MenuImageCard>(parameters => parameters
            .Add(card => card.ImageUrl, "/story.webp")
            .Add(card => card.Title, "Customer Stories"));

        var image = cut.Find(".osirion-menu-image-card-image");
        image.HasAttribute("alt").ShouldBeTrue();
        image.GetAttribute("alt").ShouldBe(string.Empty);
    }
}
