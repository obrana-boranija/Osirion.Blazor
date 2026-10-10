using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Galleries;

public class OsirionImageGalleryTests : OsirionComponentTestBase
{
    public OsirionImageGalleryTests()
    {
        Services.AddJSComponents();
    }

    private IRenderedComponent<OsirionImageGallery> RenderGallery(params OsirionImageGallery.GalleryItem[] items) =>
        RenderStatic<OsirionImageGallery>(parameters => parameters.Add(p => p.Items, items));

    // Fails before the fix: the caption was rendered inside the button, which is invalid (a button takes phrasing content only).
    [Fact]
    public void Gallery_Caption_IsOutsideTheTrigger()
    {
        var cut = RenderGallery(new OsirionImageGallery.GalleryItem("/a.jpg", "Alt A", "Caption A"));

        cut.FindAll(".osirion-image-gallery-trigger figcaption").ShouldBeEmpty();
        cut.Find("figure > figcaption").TextContent.ShouldBe("Caption A");
    }

    // Fails before the fix: an empty caption produced the label "View full size: ".
    [Theory]
    [InlineData("/a.jpg", "Alt A", "Caption A", "View full size: Caption A")]
    [InlineData("/a.jpg", "Alt A", "", "View full size: Alt A")]
    [InlineData("/a.jpg", "", "", "View full size image")]
    public void Gallery_TriggerLabel_NeverEndsWithADanglingColon(string src, string alt, string caption, string expected)
    {
        var cut = RenderGallery(new OsirionImageGallery.GalleryItem(src, alt, caption));

        cut.Find(".osirion-image-gallery-trigger").GetAttribute("aria-label").ShouldBe(expected);
    }

    // Fails before the fix: the trigger was a button that did nothing without script.
    [Fact]
    public void Gallery_WithoutScript_TriggerIsALinkToTheImage()
    {
        var cut = RenderGallery(new OsirionImageGallery.GalleryItem("/images/a.jpg", "Alt A", "Caption A"));

        var trigger = cut.Find(".osirion-image-gallery-trigger");
        trigger.TagName.ShouldBe("A");
        trigger.GetAttribute("href").ShouldBe("/images/a.jpg");
    }

    [Fact]
    public void GalleryScript_TrapsFocusInsideTheOpenViewer()
    {
        var script = File.ReadAllText(LocateSource("Galleries", "OsirionImageGallery.razor.js"));

        script.ShouldContain("trapFocus");
        script.ShouldContain("event.key === 'Tab'");
    }

    private static string LocateSource(string folder, string file)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "src", "Osirion.Blazor.Core", "Components", folder, file);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(file);
    }
}
