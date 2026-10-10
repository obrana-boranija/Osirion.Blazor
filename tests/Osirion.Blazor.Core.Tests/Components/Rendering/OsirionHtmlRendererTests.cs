using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Rendering;

public class OsirionHtmlRendererTests : OsirionComponentTestBase
{
    private const string HostileContent = "<p>Hello</p><img src=\"/a.png\" alt=\"a\" onerror=\"alert(1)\"><script>alert(2)</script>";

    /// <summary>A highlighted code block whose title attribute holds markup; a browser keeps it as attribute text.</summary>
    private const string CodeBlockWithMarkupInAttribute =
        "<pre><code class=\"language-js\" title=\"x><img src=x onerror=alert(1)>\">y</code></pre>";

    // Fails before the fix: SanitizeHtml defaulted to false and the content was rendered as given.
    [Fact]
    public void OsirionHtmlRenderer_DefaultParameters_RendersContentWithoutScriptOrHandlers()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, HostileContent));

        cut.Find("article").InnerHtml.MarkupMatches("<p>Hello</p><img src=\"/a.png\" alt=\"a\">");
    }

    [Fact]
    public void OsirionHtmlRenderer_SanitizeHtmlFalse_RendersMarkupAsGiven()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, "<p onclick=\"track()\">Trusted</p>")
            .Add(p => p.SanitizeHtml, false));

        cut.Find("article p").GetAttribute("onclick").ShouldBe("track()");
    }

    [Fact]
    public void OsirionHtmlRenderer_CustomSanitizerFunc_RendersItsOutput()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, HostileContent)
            .Add(p => p.HtmlSanitizer, _ => "<p>custom</p>"));

        cut.Find("article").InnerHtml.MarkupMatches("<p>custom</p>");
    }

    [Fact]
    public void OsirionHtmlRenderer_RegisteredSanitizer_RendersItsOutput()
    {
        var sanitizer = Substitute.For<IHtmlContentSanitizer>();
        sanitizer.Sanitize(HostileContent).Returns("<p>registered</p>");
        Services.AddSingleton(sanitizer);

        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, HostileContent));

        cut.Find("article").InnerHtml.MarkupMatches("<p>registered</p>");
    }

    [Fact]
    public void OsirionHtmlRenderer_HostAllowList_KeepsCustomElement()
    {
        Services.AddOsirionHtmlSanitizer(options => options.AllowElement("chart", "type", "data"));

        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, "<chart type=\"line\" data=\"[[1, 2]]\" onclick=\"alert(1)\"></chart>"));

        cut.Find("article").InnerHtml.MarkupMatches("<chart type=\"line\" data=\"[[1, 2]]\"></chart>");
    }

    // The copy button handler is added by the component after sanitization; content handlers are not.
    [Fact]
    public void OsirionHtmlRenderer_CopyButtonEnabled_KeepsLibraryHandlerAndRemovesContentHandler()
    {
        Services.AddJSComponents();

        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, "<pre><code class=\"language-csharp\" onclick=\"alert(1)\">var x = 1;</code></pre>")
            .Add(p => p.EnableCopyButton, true));

        cut.Find("button.osirion-copy-button").GetAttribute("onclick").ShouldBe("osirionCopyCode(this)");
        cut.Find("article code").HasAttribute("onclick").ShouldBeFalse();
    }

    [Fact]
    public void OsirionHtmlRenderer_CodeBlockWithMarkupInAttribute_RendersNoElementFromTheAttribute()
    {
        Services.AddJSComponents();

        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, CodeBlockWithMarkupInAttribute));

        cut.FindAll("article img").ShouldBeEmpty();
    }

    // Fails before the fix: after sanitization the code tag was rewritten with a pattern that stopped at the
    // first '>', even inside a quoted attribute value, so a sanitizer that leaves '>' unescaped in attribute
    // values (valid HTML) saw the markup in the title turn into a live element.
    [Fact]
    public void OsirionHtmlRenderer_CodeBlockWithMarkupInAttributeAndCustomSanitizer_RendersNoElementFromTheAttribute()
    {
        Services.AddJSComponents();

        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, CodeBlockWithMarkupInAttribute)
            .Add(p => p.HtmlSanitizer, html => html));

        cut.FindAll("article img").ShouldBeEmpty();
        cut.Find("article code").GetAttribute("title").ShouldBe("x><img src=x onerror=alert(1)>");
    }
}
