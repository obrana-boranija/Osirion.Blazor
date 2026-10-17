using System.Text.RegularExpressions;
using Bunit;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

/// <summary>
/// The body of a content item is markup the component writes with <c>MarkupString</c> or through
/// <c>OsirionHtmlRenderer</c>, so its elements carry no scope attribute and a scoped rule for them (<c>.osirion-content-body p</c>
/// compiles to <c>p[b-xyz]</c>) never matches.
/// </summary>
public partial class ContentBodyStylesheetTests : CmsPageComponentTestBase
{
    private const string BodyMarkup = "<h2>Setup</h2><p>Text</p><img src=\"/a.png\" alt=\"a\">";

    // Fails before the fix: these stylesheets styled `.osirion-content-body h2`, `p`, `img` and more without ::deep, so
    // the compiled rules asked for an attribute the body's elements never have and not one of them matched.
    [Theory]
    [InlineData("ContentView.razor.css")]
    [InlineData("LocalizedContentView.razor.css")]
    [InlineData("ContentRenderer.razor.css")]
    public void ContentBodyRules_ForTheBodysElements_ReachThemThroughDeep(string file)
    {
        var css = Squash(ComponentCss(file));

        DescendantWithoutDeep().Matches(css).Select(match => match.Value).ShouldBeEmpty();
    }

    // The views write the body themselves, outside OsirionHtmlRenderer, so nothing else keeps a wide image or code block
    // inside the column. These two rules do, and reach the body through ::deep.
    [Theory]
    [InlineData("ContentView.razor.css")]
    [InlineData("LocalizedContentView.razor.css")]
    public void ContentViews_KeepImagesAndCodeBlocksInsideTheColumn(string file)
    {
        var css = Squash(ComponentCss(file));

        css.ShouldMatch(@"\.osirion-content-body ::deep img \{[^}]*max-width: 100%;[^}]*height: auto;");
        css.ShouldMatch(@"\.osirion-content-body ::deep pre \{[^}]*max-width: 100%;[^}]*overflow-x: auto;");
    }

    // ContentRenderer writes the body through OsirionHtmlRenderer, whose own stylesheet (shipped unscoped) styles it.
    [Fact]
    public void ContentRenderer_Body_IsWrittenInsideTheHtmlRendererAndItsStylesheet()
    {
        var cut = RenderStatic<ContentRenderer>(p => p.Add(c => c.Item, CreateItem(BodyMarkup)));

        cut.Find(".osirion-content-body article.osirion-html-renderer h2").TextContent.ShouldBe("Setup");
        cut.Find("link[href$='osirion-html-renderer.min.css']").ShouldNotBeNull();
    }

    [Theory]
    [InlineData("h2")]
    [InlineData("p")]
    [InlineData("img")]
    [InlineData("ul")]
    [InlineData("blockquote")]
    [InlineData("table")]
    [InlineData("code")]
    public void HtmlRendererStylesheet_StylesTheElementsOfABody(string element)
    {
        var css = Squash(RepositoryFile("src", "Osirion.Blazor.Core", "wwwroot", "css", "osirion-html-renderer.css"));

        css.ShouldMatch($@"\.osirion-html-renderer {element}\b[^{{]*\{{");
    }

    // The views do not use OsirionHtmlRenderer, which is why their two rules above are not redundant.
    [Fact]
    public void ContentView_Body_IsNotWrittenInsideTheHtmlRenderer()
    {
        var cut = RenderStatic<ContentView>(p => p.Add(c => c.Item, CreateItem(BodyMarkup)));

        cut.FindAll(".osirion-html-renderer").ShouldBeEmpty();
        cut.Find(".osirion-content-body h2").Attributes.Select(a => a.Name).ShouldNotContain(name => name.StartsWith("b-", StringComparison.Ordinal));
        cut.Find(".osirion-content-body").Attributes.Select(a => a.Name).ShouldContain(name => name.StartsWith("b-", StringComparison.Ordinal));
    }

    private static string Squash(string css) => WhitespaceRun().Replace(CssComment().Replace(css, string.Empty), " ");

    private static string ComponentCss(string file) =>
        RepositoryFile("src", "Osirion.Blazor.Cms.Web", "Components", file);

    private static string RepositoryFile(params string[] path)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return File.ReadAllText(Path.Combine([directory.FullName, .. path]));
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }

    // A selector that goes below the body (descendant or child) without ::deep.
    [GeneratedRegex(@"\.osirion-content-body\s+(?!::deep\b)[>+~a-zA-Z*.#\[:][^{,]*")]
    private static partial Regex DescendantWithoutDeep();

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex CssComment();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();
}
