using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Bunit;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Services.Implementations;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Services;

public class HtmlContentSanitizerTests
{
    private static readonly string[] ActiveElements =
    [
        "script", "iframe", "frame", "object", "embed", "applet", "svg", "math", "template",
        "style", "form", "input", "button", "textarea", "select", "base", "meta", "link", "noscript"
    ];

    private static readonly string[] UrlAttributes = ["href", "src", "action", "formaction", "xlink:href", "data", "poster", "background"];

    private static readonly string[] UnsafeSchemes = ["javascript:", "vbscript:", "data:"];

    private readonly IHtmlContentSanitizer _sanitizer = HtmlContentSanitizer.Default;

    // Each row is an XSS or HTML injection vector. The assertion parses the output the way a browser
    // does and proves no executable element, event handler or script URL survives.
    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<SCRIPT>alert(1)</SCRIPT>")]
    [InlineData("<scr<script></script>ipt>alert(1)</scr<script></script>ipt>")]
    [InlineData("<script src=//evil.example/x.js>")]
    [InlineData("<img src=x onerror=alert(1)>")]
    [InlineData("<img src=\"x\" ONERROR='alert(1)'>")]
    [InlineData("<a href=\"javascript:alert(1)\">x</a>")]
    [InlineData("<a href=\"jav&#x61;script:alert(1)\">x</a>")]
    [InlineData("<a href=\"java&#09;script:alert(1)\">x</a>")]
    [InlineData("<a href=\"java&#x0A;script:alert(1)\">x</a>")]
    [InlineData("<a href=\"&#x01;javascript:alert(1)\">x</a>")]
    [InlineData("<a href=\"JaVaScRiPt:alert(1)?t=10:30\">x</a>")]
    [InlineData("<a href=\" javascript:alert(1)\">x</a>")]
    [InlineData("<a href=\"vbscript:msgbox(1)\">x</a>")]
    [InlineData("<a href=\"data:text/html,<script>alert(1)</script>\">x</a>")]
    [InlineData("<img src=\"data:image/svg+xml;base64,PHN2Zz48L3N2Zz4=\">")]
    [InlineData("<iframe src=\"https://evil.example\"></iframe>")]
    [InlineData("<iframe srcdoc=\"<script>alert(1)</script>\"></iframe>")]
    [InlineData("<object data=\"javascript:alert(1)\"></object>")]
    [InlineData("<embed src=\"https://evil.example/x.swf\">")]
    [InlineData("<form action=\"https://evil.example\"><input name=\"password\"><button formaction=\"javascript:alert(1)\">Go</button></form>")]
    [InlineData("<meta http-equiv=\"refresh\" content=\"0;url=https://evil.example\">")]
    [InlineData("<base href=\"https://evil.example/\">")]
    [InlineData("<link rel=\"stylesheet\" href=\"https://evil.example/x.css\">")]
    [InlineData("<style>body{background:url(https://evil.example)}</style>")]
    [InlineData("<svg onload=alert(1)><circle /></svg>")]
    [InlineData("<svg><a xlink:href=\"javascript:alert(1)\"><text>x</text></a></svg>")]
    [InlineData("<math><mtext><table><mglyph><style><img src=x onerror=alert(1)>")]
    [InlineData("<template><script>alert(1)</script></template>")]
    [InlineData("<noscript><p title=\"</noscript><img src=x onerror=alert(1)>\"></noscript>")]
    [InlineData("<details open ontoggle=alert(1)><summary>x</summary></details>")]
    [InlineData("<div style=\"background:url(javascript:alert(1))\">x</div>")]
    [InlineData("<p>a</p><!--<img src=x onerror=alert(1)>-->")]
    public void Sanitize_HostileMarkup_LeavesNothingExecutable(string html)
    {
        var document = Parse(_sanitizer.Sanitize(html));

        var elements = document.Body!.QuerySelectorAll("*");
        elements.Select(element => element.LocalName).Intersect(ActiveElements).ShouldBeEmpty();
        elements.SelectMany(element => element.Attributes)
            .Where(attribute => attribute.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase))
            .ShouldBeEmpty();
        elements.SelectMany(element => element.Attributes)
            .Where(attribute => UrlAttributes.Contains(attribute.Name) && HasUnsafeScheme(attribute.Value))
            .ShouldBeEmpty();
        elements.Where(element => element.HasAttribute("style")).ShouldBeEmpty();
        document.Body!.Descendants<IComment>().ShouldBeEmpty();
    }

    // Fails before the fix: the library reads everything before the first colon as a scheme, so these
    // relative URLs were dropped as unknown schemes although a browser resolves them against the page.
    [Theory]
    [InlineData("<a href=\"page.html?t=10:30\">Session</a>")]
    [InlineData("<a href=\"?tag=topic:blazor\">Filter</a>")]
    [InlineData("<a href=\"index.html?from=12:00&amp;to=13:00\">Slot</a>")]
    [InlineData("<img src=\"thumb.png?size=10:20\" alt=\"t\">")]
    public void Sanitize_RelativeUrlWithColonInQuery_KeepsTheUrl(string html)
    {
        _sanitizer.Sanitize(html).MarkupMatches(html);
    }

    // Fails before the fix: "name" on an image adds a named property to document that hides built-ins
    // such as document.querySelectorAll, which breaks the page's scripts.
    [Fact]
    public void Sanitize_NameAttribute_IsRemoved()
    {
        var result = _sanitizer.Sanitize("<img name=\"querySelectorAll\" src=\"/a.png\" alt=\"a\">");

        result.MarkupMatches("<img src=\"/a.png\" alt=\"a\">");
    }

    [Fact]
    public void Sanitize_EventHandlerAttribute_RemovesOnlyTheHandler()
    {
        var result = _sanitizer.Sanitize("<img src=\"/images/a.png\" alt=\"A\" onerror=\"alert(1)\">");

        result.MarkupMatches("<img src=\"/images/a.png\" alt=\"A\">");
    }

    [Fact]
    public void Sanitize_JavaScriptLink_KeepsLinkTextWithoutHref()
    {
        var result = _sanitizer.Sanitize("<a href=\"javascript:alert(1)\" title=\"t\">Read</a>");

        result.MarkupMatches("<a title=\"t\">Read</a>");
    }

    [Fact]
    public void Sanitize_ScriptElement_DropsItsCodeInsteadOfShowingIt()
    {
        var result = _sanitizer.Sanitize("<p>a<script>alert(1)</script>b</p>");

        result.MarkupMatches("<p>ab</p>");
    }

    [Fact]
    public void Sanitize_FormWithContent_RemovesControlsAndKeepsSanitizedText()
    {
        var result = _sanitizer.Sanitize(
            "<form action=\"/subscribe\" method=\"post\"><p>Hello <b onclick=\"alert(1)\">world</b></p><input name=\"email\"></form>");

        result.MarkupMatches("<p>Hello <b>world</b></p>");
    }

    // Regression from real content: "content/<locale>/" in a markdown list item. An unknown element is
    // unwrapped, so the text after it is not lost.
    [Fact]
    public void Sanitize_UnknownElementInProse_KeepsTheTextAfterIt()
    {
        var result = _sanitizer.Sanitize("<ul><li>content/<locale>/ structure (en, fr, es)</li></ul>");

        result.MarkupMatches("<ul><li>content// structure (en, fr, es)</li></ul>");
    }

    [Fact]
    public void Sanitize_IframeByDefault_RemovesItWithItsFallbackContent()
    {
        var result = _sanitizer.Sanitize("<p>before</p><iframe src=\"https://www.youtube.com/embed/x\">fallback</iframe><p>after</p>");

        result.MarkupMatches("<p>before</p><p>after</p>");
    }

    [Fact]
    public void Sanitize_StyleAttributeByDefault_RemovesIt()
    {
        var result = _sanitizer.Sanitize("<p style=\"color: red\">x</p>");

        result.MarkupMatches("<p>x</p>");
    }

    [Theory]
    [InlineData("<div class=\"hero-section\"><p>Text</p></div>")]
    [InlineData("<h2 id=\"getting-started\">Getting started</h2>")]
    [InlineData("<pre><code class=\"language-csharp\">var ok = a &lt; b &amp;&amp; c &gt; d;</code></pre>")]
    [InlineData("<table><thead><tr><th>Name</th></tr></thead><tbody><tr><td>Value</td></tr></tbody></table>")]
    [InlineData("<a href=\"/docs/getting-started\">Docs</a>")]
    [InlineData("<a href=\"#install\">Install</a>")]
    [InlineData("<a href=\"https://example.com/page?a=1&amp;b=2\" target=\"_blank\" rel=\"noopener noreferrer\">Site</a>")]
    [InlineData("<a href=\"mailto:hello@example.com\">Mail</a>")]
    [InlineData("<a href=\"tel:+381111222\">Call</a>")]
    [InlineData("<img src=\"/images/blog/a.jpg\" alt=\"Alt text\" title=\"Title\">")]
    [InlineData("<details><summary>More</summary><p>Body</p></details>")]
    [InlineData("<p>line<br>next</p><hr><blockquote><p>quote</p></blockquote>")]
    [InlineData("<ul><li><strong>bold</strong> <em>em</em> <code>code</code></li></ul><ol><li>one</li></ol>")]
    public void Sanitize_ContentMarkup_IsKeptUnchanged(string html)
    {
        _sanitizer.Sanitize(html).MarkupMatches(html);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Sanitize_NullOrWhiteSpace_ReturnsEmpty(string? html)
    {
        _sanitizer.Sanitize(html).ShouldBe(string.Empty);
    }

    // Shape of the consumer's chart elements. "data" is a URL attribute in the underlying library, and a
    // value with a colon in it would be dropped as an unknown URL scheme; this proves it stays unmangled.
    [Fact]
    public void AllowElement_CustomElement_KeepsScopedAttributesWithValuesUnchanged()
    {
        var sanitizer = CreateSanitizer(options => options.AllowElement("chart", "type", "title", "data", "colors", "x-labels"));
        const string chart =
            "<chart type=\"line\" title=\"Trend\" data=\"[{&quot;label&quot;: &quot;a:b&quot;, &quot;value&quot;: 1}]\" " +
            "colors=\"[&quot;#007b31&quot;, &quot;#d32f2f&quot;]\" x-labels=\"[&quot;Mon&quot;, &quot;Tue&quot;]\"></chart>";

        sanitizer.Sanitize(chart).MarkupMatches(chart);
    }

    [Fact]
    public void AllowElement_CustomElement_StillRemovesEventHandlers()
    {
        var sanitizer = CreateSanitizer(options => options.AllowElement("chart", "type"));

        var result = sanitizer.Sanitize("<chart type=\"line\" onclick=\"alert(1)\" onmouseover=\"alert(2)\"></chart>");

        result.MarkupMatches("<chart type=\"line\"></chart>");
    }

    [Fact]
    public void AllowElement_ScopedAttribute_IsNotAllowedOnOtherElements()
    {
        var sanitizer = CreateSanitizer(options => options.AllowElement("chart", "data", "colors"));

        var result = sanitizer.Sanitize("<div data=\"x\" colors=\"y\">text</div>");

        result.MarkupMatches("<div>text</div>");
    }

    // Opting in to iframes through AllowElement must not switch off URL screening for their src.
    [Fact]
    public void AllowElement_IframeOptIn_KeepsEmbedButStillScreensUrls()
    {
        var sanitizer = CreateSanitizer(options => options.AllowElement("iframe", "src", "width", "height"));

        var result = sanitizer.Sanitize(
            "<iframe src=\"https://www.youtube.com/embed/x\" width=\"560\" height=\"315\" onload=\"alert(1)\"></iframe>" +
            "<iframe src=\"javascript:alert(1)\"></iframe>");

        result.MarkupMatches("<iframe src=\"https://www.youtube.com/embed/x\" width=\"560\" height=\"315\"></iframe><iframe></iframe>");
    }

    [Theory]
    [InlineData("onclick")]
    [InlineData("OnLoad")]
    public void AllowElement_EventHandlerAttribute_Throws(string attributeName)
    {
        var options = new HtmlContentSanitizerOptions();

        Should.Throw<ArgumentException>(() => options.AllowElement("chart", attributeName));
    }

    [Theory]
    [InlineData("script")]
    [InlineData("svg")]
    [InlineData("math")]
    [InlineData("template")]
    [InlineData("object")]
    [InlineData("embed")]
    [InlineData("meta")]
    [InlineData("base")]
    [InlineData("link")]
    public void AllowElement_NeverAllowedElement_Throws(string tagName)
    {
        var options = new HtmlContentSanitizerOptions();

        Should.Throw<ArgumentException>(() => options.AllowElement(tagName, "class"));
    }

    [Fact]
    public void AllowedTags_NeverAllowedElementAdded_IsStillRemoved()
    {
        var sanitizer = CreateSanitizer(options =>
        {
            options.AllowedTags.Add("script");
            options.AllowedTags.Add("svg");
        });

        var result = sanitizer.Sanitize("<p>x</p><script>alert(1)</script><svg onload=\"alert(1)\"></svg>");

        result.MarkupMatches("<p>x</p>");
    }

    [Fact]
    public void AllowedAttributes_EventHandlerAdded_IsStillRemoved()
    {
        var sanitizer = CreateSanitizer(options => options.AllowedAttributes.Add("onclick"));

        var result = sanitizer.Sanitize("<p onclick=\"alert(1)\">x</p>");

        result.MarkupMatches("<p>x</p>");
    }

    [Fact]
    public void AllowedSchemes_JavaScriptAdded_IsStillRemoved()
    {
        var sanitizer = CreateSanitizer(options => options.AllowedSchemes.Add("javascript"));

        var result = sanitizer.Sanitize("<a href=\"javascript:alert(1)\">x</a>");

        result.MarkupMatches("<a>x</a>");
    }

    [Fact]
    public void AllowedAttributes_StyleOptIn_KeepsFilteredInlineStyle()
    {
        var sanitizer = CreateSanitizer(options => options.AllowedAttributes.Add("style"));

        var paragraph = Parse(sanitizer.Sanitize("<p style=\"color: red; behavior: url(x.htc)\">x</p>")).QuerySelector("p")!;

        paragraph.GetAttribute("style").ShouldNotBeNull().ShouldContain("color");
        paragraph.GetAttribute("style").ShouldNotBeNull().ShouldNotContain("behavior");
    }

    [Fact]
    public void Sanitize_ConcurrentCallsOnSharedInstance_ProduceIdenticalOutput()
    {
        const string html = "<h2 id=\"a\">A</h2><p onclick=\"x\">t<script>y</script></p><a href=\"javascript:z\">l</a>";
        var expected = _sanitizer.Sanitize(html);
        var results = new string[500];

        Parallel.For(0, results.Length, index => results[index] = _sanitizer.Sanitize(html));

        results.ShouldAllBe(result => result == expected);
    }

    private static IHtmlContentSanitizer CreateSanitizer(Action<HtmlContentSanitizerOptions> configure)
    {
        var options = new HtmlContentSanitizerOptions();
        configure(options);
        return new HtmlContentSanitizer(Options.Create(options));
    }

    private static IDocument Parse(string html) =>
        new HtmlParser().ParseDocument("<!doctype html><html><body>" + html);

    private static bool HasUnsafeScheme(string value)
    {
        var compact = new string(value.Where(character => !char.IsWhiteSpace(character) && !char.IsControl(character)).ToArray());
        return UnsafeSchemes.Any(scheme => compact.StartsWith(scheme, StringComparison.OrdinalIgnoreCase));
    }
}
