using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Rendering;

/// <summary>
/// The language a code block declares is mapped to a Prism language name, and that name to the label shown in the
/// copy button header. Both mappings are a contract with Prism and with readers of the rendered page.
/// </summary>
public class OsirionHtmlRendererLanguageTests : OsirionComponentTestBase
{
    public OsirionHtmlRendererLanguageTests()
    {
        Services.AddJSComponents();
    }

    [Theory]
    [InlineData("cs", "csharp", "C#")]
    [InlineData("csharp", "csharp", "C#")]
    [InlineData("js", "javascript", "JavaScript")]
    [InlineData("javascript", "javascript", "JavaScript")]
    [InlineData("ts", "typescript", "TypeScript")]
    [InlineData("typescript", "typescript", "TypeScript")]
    [InlineData("py", "python", "Python")]
    [InlineData("python", "python", "Python")]
    [InlineData("rb", "ruby", "RUBY")]
    [InlineData("yml", "yaml", "YAML")]
    [InlineData("yaml", "yaml", "YAML")]
    [InlineData("sh", "bash", "Bash")]
    [InlineData("bash", "bash", "Bash")]
    [InlineData("ps1", "powershell", "PowerShell")]
    [InlineData("powershell", "powershell", "PowerShell")]
    [InlineData("razor", "cshtml", "Razor")]
    [InlineData("blazor", "cshtml", "Razor")]
    [InlineData("cshtml", "cshtml", "Razor")]
    [InlineData("html", "markup", "MARKUP")]
    [InlineData("xml", "markup", "MARKUP")]
    [InlineData("css", "css", "CSS")]
    [InlineData("sql", "sql", "SQL")]
    [InlineData("json", "json", "JSON")]
    [InlineData("go", "go", "GO")]
    [InlineData("CS", "CS", "CS")]
    public void OsirionHtmlRenderer_CodeBlockLanguage_MapsToPrismLanguageAndLabel(string declared, string expectedLanguage, string expectedLabel)
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, $"<pre><code class=\"language-{declared}\">x</code></pre>")
            .Add(p => p.EnableCopyButton, true));

        cut.Find(".osirion-code-wrapper").GetAttribute("data-language").ShouldBe(expectedLanguage);
        cut.Find(".osirion-language-label").TextContent.ShouldBe(expectedLabel);
        cut.Find("button.osirion-copy-button").GetAttribute("aria-label").ShouldBe($"Copy {expectedLabel} code to clipboard");
    }

    [Fact]
    public void OsirionHtmlRenderer_CodeBlockWithoutLanguage_IsLabelledPlainText()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, "<pre><code>x</code></pre>")
            .Add(p => p.EnableCopyButton, true));

        cut.Find(".osirion-code-wrapper").GetAttribute("data-language").ShouldBe("plaintext");
        cut.Find(".osirion-language-label").TextContent.ShouldBe("PLAINTEXT");
    }

    [Fact]
    public void OsirionHtmlRenderer_AliasedLanguage_LoadsTheMappedPrismComponent()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, "<pre><code class=\"language-cs\">x</code></pre>"));

        cut.Markup.ShouldContain("prism-csharp.min.js");
        cut.Markup.ShouldNotContain("prism-cs.min.js");
    }

    [Fact]
    public void OsirionHtmlRenderer_CoreLanguage_LoadsNoLanguageComponent()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, "<pre><code class=\"language-js\">x</code></pre>"));

        cut.Markup.ShouldContain("prism.min.js");
        cut.Markup.ShouldNotContain("prism-javascript.min.js");
    }

    [Fact]
    public void OsirionHtmlRenderer_SyntaxHighlightingDisabled_RendersCodeBlockUnwrapped()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, "<pre><code class=\"language-cs\">x</code></pre>")
            .Add(p => p.EnableSyntaxHighlighting, false)
            .Add(p => p.EnableCopyButton, true));

        cut.FindAll(".osirion-code-wrapper").ShouldBeEmpty();
        cut.Find("article code").GetAttribute("class").ShouldBe("language-cs");
    }

    [Fact]
    public void OsirionHtmlRenderer_WhitespaceContent_RendersNothing()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, "   "));

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void OsirionHtmlRenderer_ContentChangedToEmpty_ClearsTheRenderedContent()
    {
        var cut = RenderStatic<OsirionHtmlRenderer>(parameters => parameters
            .Add(p => p.HtmlContent, "<pre><code class=\"language-cs\">x</code></pre>"));

        cut.Render(parameters => parameters.Add(p => p.HtmlContent, string.Empty));

        cut.Markup.Trim().ShouldBeEmpty();
    }
}
