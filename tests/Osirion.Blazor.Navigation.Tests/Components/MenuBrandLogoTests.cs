using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuBrandLogoTests : BunitContext
{
    public MenuBrandLogoTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));
    }

    // Fails before the fix: the alt text was "Osirion brand logo", so a screen reader said the name twice, once for
    // the image and once for the visible text next to it.
    [Fact]
    public void Menu_LogoBesideBrandText_HasAnEmptyAltSoTheNameIsSaidOnce()
    {
        var cut = Render<Menu>(p => p
            .Add(c => c.BrandLogo, "/logo.svg")
            .Add(c => c.BrandText, "Osirion"));

        var logo = cut.Find(".osirion-brand-link img.osirion-brand-logo");
        logo.HasAttribute("alt").ShouldBeTrue();
        logo.GetAttribute("alt").ShouldBe(string.Empty);
    }

    // Fails before the fix: the alt text was " brand logo" with a leading space, and the link had no usable name.
    [Fact]
    public void Menu_LogoWithoutBrandText_NamesTheLinkWithoutAStrayWord()
    {
        var cut = Render<Menu>(p => p.Add(c => c.BrandLogo, "/logo.svg"));

        cut.Find(".osirion-brand-link img.osirion-brand-logo").GetAttribute("alt").ShouldBe("Home");
    }

    // Fails before the fix: the markup said 60 by 60 while the stylesheet draws the logo at 2.25rem (36px), so
    // the reserved box and the drawn box disagreed.
    [Fact]
    public void Menu_Logo_DeclaresTheSizeTheStylesheetDraws()
    {
        var cut = Render<Menu>(p => p.Add(c => c.BrandLogo, "/logo.svg"));
        var logo = cut.Find("img.osirion-brand-logo");
        var css = File.ReadAllText(StylesheetPath("Menu.razor.css"));

        css.ShouldContain("--osirion-menu-logo-size, 2.25rem");
        logo.GetAttribute("width").ShouldBe("36");
        logo.GetAttribute("height").ShouldBe("36");
    }

    internal static string StylesheetPath(string fileName)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return Path.Combine(directory.FullName, "src", "Osirion.Blazor.Navigation", "Components", fileName);
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }
}
