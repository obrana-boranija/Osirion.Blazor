using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Rendering;

public class ContentDemoTests : OsirionComponentTestBase
{
    private const string DemoBlock = "<pre><code class=\"language-demo\">greeting\n</code></pre>";

    public ContentDemoTests()
    {
        Services.AddJSComponents();
        ComponentFactories.AddStub<BlazorJSComponents.JS>();
    }

    [Fact]
    public void Renderer_TopLevelDemoBlock_RendersTheComponentInPlace()
    {
        Services.AddOsirionContentDemos(demos => demos.Add<GreetingDemo>("greeting"));

        var cut = RenderStatic<OsirionHtmlRenderer>(p => p.Add(r => r.HtmlContent, $"<p>Before</p>{DemoBlock}<p>After</p>"));

        var article = cut.Find("article");
        article.Children.Select(c => c.TagName.ToLowerInvariant() + "." + c.ClassName).ShouldBe(["p.", "div.osirion-content-demo", "p."]);
        cut.Find(".osirion-content-demo-preview .greeting").TextContent.ShouldBe("Hello from a demo");
        cut.FindAll("code.language-demo").ShouldBeEmpty();
        cut.FindAll(".osirion-tabs").ShouldBeEmpty();
    }

    [Fact]
    public void Renderer_DemoWithSource_ShowsPreviewAndEncodedCodeTabs()
    {
        Services.AddOsirionContentDemos(demos => demos.Add<GreetingDemo>("greeting", source: "<GreetingDemo />\n<script>alert(1)</script>", title: "Greeting"));

        var cut = RenderStatic<OsirionHtmlRenderer>(p => p.Add(r => r.HtmlContent, DemoBlock));

        cut.FindAll(".osirion-tabs-label").Select(l => l.TextContent).ShouldBe(["Preview", "Code"]);
        cut.Find(".osirion-tabs").GetAttribute("aria-label").ShouldBe("Greeting example");
        var code = cut.Find(".osirion-content-demo code.language-razor");
        code.TextContent.ShouldContain("<script>alert(1)</script>");
        cut.FindAll(".osirion-content-demo script").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("<ul><li><pre><code class=\"language-demo\">greeting</code></pre></li></ul>")]
    [InlineData("<pre><code class=\"language-demo\">unknown-key</code></pre>")]
    public void Renderer_NestedOrUnknownDemo_StaysACodeBlock(string html)
    {
        Services.AddOsirionContentDemos(demos => demos.Add<GreetingDemo>("greeting"));

        var cut = RenderStatic<OsirionHtmlRenderer>(p => p.Add(r => r.HtmlContent, html));

        cut.FindAll(".osirion-content-demo").ShouldBeEmpty();
        cut.FindAll("code.language-demo").Count.ShouldBe(1);
    }

    [Fact]
    public void Renderer_NoRegistry_StaysACodeBlock()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(p => p.Add(r => r.HtmlContent, DemoBlock));

        cut.FindAll("code.language-demo").Count.ShouldBe(1);
    }

    [Fact]
    public void Renderer_DemosDisabled_StaysACodeBlock()
    {
        Services.AddOsirionContentDemos(demos => demos.Add<GreetingDemo>("greeting"));

        var cut = RenderStatic<OsirionHtmlRenderer>(p => p
            .Add(r => r.HtmlContent, DemoBlock)
            .Add(r => r.EnableDemos, false));

        cut.FindAll(".osirion-content-demo").ShouldBeEmpty();
        cut.FindAll("code.language-demo").Count.ShouldBe(1);
    }

    [Fact]
    public void Renderer_MarkerLookalikeInContent_IsRemovedBySanitizingNotRenderedAsADemo()
    {
        Services.AddOsirionContentDemos(demos => demos.Add<GreetingDemo>("greeting"));

        var cut = RenderStatic<OsirionHtmlRenderer>(p => p.Add(r => r.HtmlContent, "<p>Text</p><!--osirion-demo-0000-0-->"));

        cut.FindAll(".osirion-content-demo").ShouldBeEmpty();
    }

    [Fact]
    public void ContentDemo_KeyParameter_ResolvesFromTheRegistryWithParameters()
    {
        Services.AddOsirionContentDemos(demos => demos.Add<GreetingDemo>(
            "greeting", parameters: new Dictionary<string, object?> { ["Name"] = "docs" }));

        var cut = RenderStatic<OsirionContentDemo>(p => p.Add(d => d.Key, "greeting"));

        cut.Find(".greeting").TextContent.ShouldBe("Hello from docs");
    }

    [Theory]
    [InlineData("Upper")]
    [InlineData("has space")]
    [InlineData("")]
    public void Registry_InvalidKey_Throws(string key)
    {
        Should.Throw<ArgumentException>(() => new ContentDemoRegistry().Add<GreetingDemo>(key));
    }

    [Fact]
    public void Registry_DuplicateKey_Throws()
    {
        var registry = new ContentDemoRegistry().Add<GreetingDemo>("greeting");

        Should.Throw<ArgumentException>(() => registry.Add<GreetingDemo>("greeting"));
    }

    // Fails before the fix: used directly on a page, the demo rendered a Copy button with an inline handler and loaded
    // no script, so the button did nothing.
    [Fact]
    public void Demo_UsedDirectlyWithSource_LoadsTheCopyScript()
    {
        Services.AddOsirionContentDemos(demos => demos.Add<GreetingDemo>("greeting", source: "<GreetingDemo />"));

        var cut = RenderStatic<OsirionContentDemo>(p => p.Add(d => d.Key, "greeting"));

        cut.Find("button.osirion-copy-button").ShouldNotBeNull();
        cut.FindComponents<Stub<BlazorJSComponents.JS>>()
            .Select(stub => stub.Instance.Parameters.Get(js => js.Src))
            .ShouldContain("./_content/Osirion.Blazor.Core/js/osirion-copy-button.es5.min.js");
    }

    [Fact]
    public void Demo_CopyButtonOff_LoadsNoScript()
    {
        Services.AddOsirionContentDemos(demos => demos.Add<GreetingDemo>("greeting", source: "<GreetingDemo />"));

        var cut = RenderStatic<OsirionContentDemo>(p => p.Add(d => d.Key, "greeting").Add(d => d.EnableCopyButton, false));

        cut.FindComponents<Stub<BlazorJSComponents.JS>>().ShouldBeEmpty();
        cut.FindAll("button.osirion-copy-button").ShouldBeEmpty();
    }

    private sealed class GreetingDemo : ComponentBase
    {
        [Parameter]
        public string Name { get; set; } = "a demo";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "p");
            builder.AddAttribute(1, "class", "greeting");
            builder.AddContent(2, $"Hello from {Name}");
            builder.CloseElement();
        }
    }
}
