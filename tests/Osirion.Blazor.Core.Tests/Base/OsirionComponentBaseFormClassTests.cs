using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Base;

public class OsirionComponentBaseFormClassTests : OsirionComponentTestBase
{
    // Fails before the fix: MudBlazor got "mud-input-root mud-input-underline" and "mud-select", classes MudBlazor
    // styles only inside its own input markup, so Osirion's plain inputs rendered as white, borderless 20px boxes.
    [Fact]
    public void FormClasses_MudBlazor_UseTheNeutralOsirionClassesTheAdapterStyles()
    {
        Services.AddSingleton(Options.Create(new ThemingOptions { Framework = CssFramework.MudBlazor }));
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<FormClassProbe>();

        cut.Find("input").ClassName.ShouldBe("osirion-input");
        cut.Find("select").ClassName.ShouldBe("osirion-select");
        cut.Find("textarea").ClassName.ShouldBe("osirion-textarea osirion-contact-textarea");
    }

    [Fact]
    public void FormClasses_Bootstrap_KeepBootstrapsFormClasses()
    {
        Services.AddSingleton(Options.Create(new ThemingOptions { Framework = CssFramework.Bootstrap }));
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<FormClassProbe>();

        cut.Find("input").ClassName.ShouldBe("form-control");
        cut.Find("select").ClassName.ShouldBe("form-select");
        cut.Find("textarea").ClassName.ShouldBe("form-control osirion-contact-textarea");
    }

    private sealed class FormClassProbe : OsirionComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "class", GetInputClass());
            builder.CloseElement();
            builder.OpenElement(2, "select");
            builder.AddAttribute(3, "class", GetSelectClass());
            builder.CloseElement();
            builder.OpenElement(4, "textarea");
            builder.AddAttribute(5, "class", GetTextAreaClass());
            builder.CloseElement();
        }
    }
}
