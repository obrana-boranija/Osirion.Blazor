using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Components;

public class OsirionStylesRenderingTests : BunitContext
{
    private const string AdapterFolder = "_content/Osirion.Blazor.Theming/css/adapters/";

    private readonly IThemeService _themeService = Substitute.For<IThemeService>();
    private readonly ThemingOptions _options = new();

    public OsirionStylesRenderingTests()
    {
        Services.AddSingleton(Options.Create(_options));
        Services.AddSingleton(_themeService);
        SetRendererInfo(new RendererInfo("Static", false));
        ComponentFactories.AddStub<BlazorJSComponents.JS>();
        _themeService.GenerateThemeVariables().Returns(string.Empty);
        _themeService.GetFrameworkClass().Returns(string.Empty);
        _themeService.CurrentMode.Returns(ThemeMode.Light);
    }

    [Theory]
    [InlineData(CssFramework.Bootstrap, "bootstrap-adapter.min.css")]
    [InlineData(CssFramework.FluentUI, "fluentui-adapter.min.css")]
    [InlineData(CssFramework.MudBlazor, "mudblazor-adapter.min.css")]
    [InlineData(CssFramework.Radzen, "radzen-adapter.min.css")]
    public void OsirionStyles_FrameworkParameter_LinksTheMatchingAdapter(CssFramework framework, string adapterFile)
    {
        var cut = Render<OsirionStyles>(p => p.Add(c => c.Framework, framework));

        Hrefs(cut).ShouldContain(AdapterFolder + adapterFile);
        Hrefs(cut).ShouldContain("_content/Osirion.Blazor.Theming/css/index.min.css");
    }

    [Fact]
    public void OsirionStyles_FrameworkFromOptions_LinksTheMatchingAdapter()
    {
        _options.Framework = CssFramework.Radzen;

        var cut = Render<OsirionStyles>();

        Hrefs(cut).ShouldContain(AdapterFolder + "radzen-adapter.min.css");
    }

    [Theory]
    [InlineData(CssFramework.None)]
    [InlineData((CssFramework)99)]
    // Rendered <link rel="stylesheet" href=""> before the fix, which requests the current page as CSS.
    public void OsirionStyles_NoKnownFramework_RendersNoAdapterLink(CssFramework framework)
    {
        var cut = Render<OsirionStyles>(p => p.Add(c => c.Framework, framework));

        Hrefs(cut).ShouldNotContain(string.Empty);
        Hrefs(cut).ShouldNotContain(h => h.StartsWith(AdapterFolder, StringComparison.Ordinal));
    }

    [Fact]
    public void OsirionStyles_UseStylesTrue_SkipsAdapterAndThemingStyles()
    {
        var cut = Render<OsirionStyles>(p => p
            .Add(c => c.UseStyles, true)
            .Add(c => c.Framework, CssFramework.Bootstrap));

        Hrefs(cut).ShouldBe(["_content/Osirion.Blazor.Core/css/index.min.css"]);
    }

    // Fails before the fix: the component linked the Core scoped bundle, which the host's {App}.styles.css already
    // imports, so every page downloaded it twice.
    [Fact]
    public void OsirionStyles_DoesNotLinkTheScopedBundleTheHostAlreadyImports()
    {
        var cut = Render<OsirionStyles>();

        Hrefs(cut).ShouldNotContain(h => h.EndsWith(".bundle.scp.css", StringComparison.Ordinal));
    }

    [Fact]
    public void OsirionStyles_UseDefaultStylesInOptions_SkipsAdapter()
    {
        _options.UseDefaultStyles = true;

        var cut = Render<OsirionStyles>();

        cut.FindAll("link").Count.ShouldBe(1);
    }

    [Fact]
    public void OsirionStyles_CustomVariables_AreWrittenIntoARootStyleBlock()
    {
        var cut = Render<OsirionStyles>(p => p.Add(c => c.CustomVariables, "--brand: #123456;"));

        var style = cut.Find("style");
        style.TextContent.ShouldContain(":root");
        style.TextContent.ShouldContain("--brand: #123456;");
    }

    private const string BreakoutValue = "--brand: red;</style><script>alert(1)</script><style>";

    // Fails before the fix: the value was written as markup, so "</style>" ended the element and the script ran.
    [Fact]
    public void OsirionStyles_CustomVariablesContainStyleEnd_CannotLeaveTheStyleElement()
    {
        var cut = Render<OsirionStyles>(p => p.Add(c => c.CustomVariables, BreakoutValue));

        AssertContained(cut);
    }

    // Fails before the fix: the configured value took the same unencoded path.
    [Fact]
    public void OsirionStyles_GeneratedVariablesContainStyleEnd_CannotLeaveTheStyleElement()
    {
        _themeService.GenerateThemeVariables().Returns(BreakoutValue);

        var cut = Render<OsirionStyles>();

        AssertContained(cut);
    }

    [Fact]
    public void OsirionStyles_CustomVariablesWithQuotesAndAmpersands_AreWrittenUnchanged()
    {
        const string value = "--hero: url(\"/a.png?x=1&y=2\"); --pad: calc(1px > 0 ? 1px : 2px);";

        var cut = Render<OsirionStyles>(p => p.Add(c => c.CustomVariables, value));

        cut.Find("style").TextContent.ShouldContain(value);
    }

    private static void AssertContained(IRenderedComponent<OsirionStyles> cut)
    {
        cut.Markup.ShouldNotContain("<script>alert");
        cut.FindAll("script").ShouldAllBe(script => !script.TextContent.Contains("alert(1)"));
        cut.FindAll("style").Count.ShouldBe(1);
        cut.Find("style").TextContent.ShouldContain("--brand: red;");
    }

    [Fact]
    public void OsirionStyles_NoVariables_WritesNoStyleBlock()
    {
        var cut = Render<OsirionStyles>();

        cut.FindAll("style").ShouldBeEmpty();
    }

    [Fact]
    public void OsirionStyles_GeneratedVariables_AreWrittenIntoARootStyleBlock()
    {
        _themeService.GenerateThemeVariables().Returns("--generated: 1px;");

        var cut = Render<OsirionStyles>();

        cut.Find("style").TextContent.ShouldContain("--generated: 1px;");
    }

    [Fact]
    public void OsirionStyles_Script_CarriesFrameworkAndThemeConfiguration()
    {
        _themeService.GetFrameworkClass().Returns("osirion-mudblazor-integration");
        _themeService.CurrentMode.Returns(ThemeMode.Dark);

        var cut = Render<OsirionStyles>(p => p
            .Add(c => c.Framework, CssFramework.MudBlazor)
            .Add(c => c.Mode, ThemeMode.System));

        var script = cut.Find("script").TextContent;
        script.ShouldContain("frameworkClass: 'osirion-mudblazor-integration'");
        script.ShouldContain("selectedFramework: 'MudBlazor'");
        script.ShouldContain("defaultTheme: 'system'");
        script.ShouldContain("currentTheme: 'dark'");
    }

    [Fact]
    public void OsirionStyles_LoadsTheStylesScript()
    {
        var cut = Render<OsirionStyles>();

        var stub = cut.FindComponent<Stub<BlazorJSComponents.JS>>();
        stub.Instance.Parameters.Get(x => x.Src).ShouldBe("./_content/Osirion.Blazor.Theming/js/osirion-styles.js");
    }

    [Fact]
    public void OsirionStyles_ThemeChanged_RendersAgain()
    {
        var cut = Render<OsirionStyles>();
        var rendersBefore = cut.RenderCount;
        _themeService.CurrentMode.Returns(ThemeMode.Dark);

        _themeService.ThemeChanged += Raise.EventWith(
            _themeService,
            new ThemeChangedEventArgs(ThemeMode.Dark, ThemeMode.Light));

        cut.WaitForAssertion(() => cut.RenderCount.ShouldBeGreaterThan(rendersBefore));
        cut.Find("script").TextContent.ShouldContain("currentTheme: 'dark'");
    }

    private static List<string> Hrefs(IRenderedComponent<OsirionStyles> cut) =>
        cut.FindAll("link").Select(l => l.GetAttribute("href") ?? "<none>").ToList();
}
