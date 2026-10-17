using System.Text.Json;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Components;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Osirion.Blazor.Core.Handlers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

public class PlausibleAndUmamiTests : BunitContext
{
    private const string WebsiteId = "94db1cb1-74f4-4a40-ad6c-962362670409";
    private const string PlausibleScript = "https://plausible.io/js/pa-AbC123.js";

    private static IOptions<T> Wrap<T>(T value) where T : class => Microsoft.Extensions.Options.Options.Create(value);

    [Fact]
    public void Plausible_GetScript_LoadsTheSiteScriptAndInitialisesWithNoOverrides()
    {
        var provider = new PlausibleProvider(Wrap(new PlausibleOptions { ScriptUrl = PlausibleScript }));

        var script = provider.GetScript();

        script.ShouldContain($"<script async src=\"{PlausibleScript}\"></script>");
        script.ShouldContain("plausible.init=plausible.init||function(i){plausible.o=i||{}};");
        script.ShouldContain("plausible.init({});");
    }

    [Fact]
    public void Plausible_GetScript_WritesOnlyTheOptionsThatWereSet()
    {
        var provider = new PlausibleProvider(Wrap(new PlausibleOptions
        {
            ScriptUrl = "/js/script.js",
            Endpoint = "/api/event",
            AutoTrackPageViews = false,
            OutboundLinks = true,
            FileDownloads = false,
        }));

        var script = provider.GetScript();

        script.ShouldContain("plausible.init({\"endpoint\":\"/api/event\",\"autoCapturePageviews\":false,\"outboundLinks\":true,\"fileDownloads\":false});");
        script.ShouldNotContain("hashBasedRouting");
        script.ShouldNotContain("formSubmissions");
    }

    [Fact]
    public void Plausible_GetScript_EncodesHostileValues()
    {
        var options = new PlausibleOptions { ScriptUrl = "/js/x.js\"><script>alert(1)</script>", Endpoint = "/api</script><script>alert(2)</script>" };
        var provider = new PlausibleProvider(Wrap(options));

        var script = provider.GetScript(options, AnalyticsScriptContext.Default);

        script.ShouldNotContain("alert(1)</script>");
        script.ShouldNotContain("alert(2)</script>");
        script.ShouldContain("\\u003C/script\\u003E");
    }

    [Theory]
    [InlineData("")]
    [InlineData("javascript:alert(1)")]
    [InlineData("//")]
    [InlineData("js/script.js")]
    public void Plausible_ScriptUrlNotAnHttpUrlOrSitePath_DoesNotRender(string scriptUrl)
    {
        new PlausibleProvider(Wrap(new PlausibleOptions { ScriptUrl = scriptUrl })).ShouldRender.ShouldBeFalse();
    }

    [Fact]
    public void Umami_GetScript_IsOneDeferredScriptConfiguredByAttributes()
    {
        var provider = new UmamiProvider(Wrap(new UmamiOptions { SiteId = WebsiteId }));

        provider.GetScript().ShouldBe($"<script defer src=\"https://cloud.umami.is/script.js\" data-website-id=\"{WebsiteId}\"></script>");
    }

    [Fact]
    public void Umami_GetScript_WritesTheConfiguredAttributes()
    {
        var provider = new UmamiProvider(Wrap(new UmamiOptions
        {
            SiteId = WebsiteId,
            ScriptUrl = "https://umami.example.com/script.js",
            HostUrl = "https://collect.example.com",
            Domains = "example.com,www.example.com",
            Tag = "docs",
            AutoTrackPageViews = false,
            ExcludeSearch = true,
            ExcludeHash = true,
            DoNotTrack = true,
        }));

        var script = provider.GetScript();

        script.ShouldContain("src=\"https://umami.example.com/script.js\"");
        script.ShouldContain("data-host-url=\"https://collect.example.com\"");
        script.ShouldContain("data-domains=\"example.com,www.example.com\"");
        script.ShouldContain("data-tag=\"docs\"");
        script.ShouldContain("data-auto-track=\"false\"");
        script.ShouldContain("data-exclude-search=\"true\"");
        script.ShouldContain("data-exclude-hash=\"true\"");
        script.ShouldContain("data-do-not-track=\"true\"");
    }

    [Fact]
    public void Umami_GetScript_EncodesAttributeValues()
    {
        var options = new UmamiOptions { SiteId = WebsiteId, Tag = "a\"><script>alert(1)</script>" };

        var script = new UmamiProvider(Wrap(options)).GetScript(options, AnalyticsScriptContext.Default);

        script.ShouldNotContain("\"><script>");
        script.ShouldContain("data-tag=\"a&quot;&gt;&lt;script&gt;");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("not-a-guid")]
    public void Umami_WebsiteIdMissingOrNotAGuid_DoesNotRender(string? siteId)
    {
        new UmamiProvider(Wrap(new UmamiOptions { SiteId = siteId })).ShouldRender.ShouldBeFalse();
    }

    [Fact]
    public void Configuration_BindsBothProvidersAndTheTrackersRender()
    {
        var configuration = new ConfigurationManager();
        configuration["Osirion:Analytics:Plausible:ScriptUrl"] = PlausibleScript;
        configuration["Osirion:Analytics:Plausible:HashBasedRouting"] = "true";
        configuration["Osirion:Analytics:Umami:SiteId"] = WebsiteId;
        Services.AddOsirionAnalytics(configuration);
        SetRendererInfo(new RendererInfo("Server", false));

        Render<PlausibleTracker>().Markup.ShouldContain("\"hashBasedRouting\":true");
        Render<UmamiTracker>().Markup.ShouldContain(WebsiteId);
    }

    [Fact]
    public void UmamiTracker_SiteIdParameter_OverridesTheConfiguredWebsiteId()
    {
        const string other = "0b5a9a3c-0000-4000-8000-000000000001";
        Services.AddOsirionAnalytics(a => a.AddUmami(o => o.SiteId = WebsiteId));
        SetRendererInfo(new RendererInfo("Server", false));

        Render<UmamiTracker>(p => p.Add(t => t.SiteId, other)).Markup.ShouldContain(other);
    }

    // The Bootstrap example renders both trackers while its settings configure other providers only, and its privacy
    // policy says nothing else loads. Passes before the change: it pins that behavior.
    [Fact]
    public void Trackers_WithoutTheirSettings_RenderNothing()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Osirion:Analytics:Matomo:TrackerUrl"] = "https://analytics.example.com/",
                ["Osirion:Analytics:Matomo:SiteId"] = "1",
            })
            .Build();
        Services.AddOsirionAnalytics(configuration);
        SetRendererInfo(new RendererInfo("Server", false));

        Render<PlausibleTracker>().Markup.Trim().ShouldBeEmpty();
        Render<UmamiTracker>().Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Trackers_RegisteredWithDefaultOptions_RenderNothing()
    {
        // Plausible has no script address and Umami no website ID until one is set, so neither can load a script.
        Services.AddOsirionAnalytics(a =>
        {
            a.AddPlausible();
            a.AddUmami();
        });
        SetRendererInfo(new RendererInfo("Server", false));

        Render<PlausibleTracker>().Markup.Trim().ShouldBeEmpty();
        Render<UmamiTracker>().Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Trackers_WithoutAnyAnalyticsRegistration_RenderNothing()
    {
        SetRendererInfo(new RendererInfo("Server", false));

        Render<PlausibleTracker>().Markup.Trim().ShouldBeEmpty();
        Render<UmamiTracker>().Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void PlausibleTracker_EnabledFalse_RendersNothing()
    {
        Services.AddOsirionAnalytics(a => a.AddPlausible(o => o.ScriptUrl = PlausibleScript));
        SetRendererInfo(new RendererInfo("Server", false));

        Render<PlausibleTracker>(p => p.Add(t => t.Enabled, false)).Markup.ShouldNotContain("<script");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ConsentGateOn_RendersOnlyAfterAnalyticsConsent(bool consented)
    {
        Services.AddOsirionAnalytics(a =>
        {
            a.AddPlausible(o => o.ScriptUrl = PlausibleScript);
            a.AddUmami(o => o.SiteId = WebsiteId);
            a.RequireConsent();
        });
        SetRendererInfo(new RendererInfo("Server", false));
        var context = HttpContextWithAnalyticsConsent(consented);

        Render<PlausibleTracker>(p => p.AddCascadingValue((HttpContext)context)).Markup.Contains("<script").ShouldBe(consented);
        Render<UmamiTracker>(p => p.AddCascadingValue((HttpContext)context)).Markup.Contains("<script").ShouldBe(consented);
    }

    [Fact]
    public void Validation_InvalidValues_FailAtStartup()
    {
        var services = new ServiceCollection();
        services.AddOsirionAnalytics(a =>
        {
            a.AddPlausible(o => { o.ScriptUrl = "javascript:alert(1)"; o.Endpoint = "ftp://x"; });
            a.AddUmami(o => { o.SiteId = "nope"; o.HostUrl = "/relative"; });
        });
        using var provider = services.BuildServiceProvider();

        var plausible = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IOptions<PlausibleOptions>>().Value);
        plausible.Failures.Count().ShouldBe(2);
        var umami = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IOptions<UmamiOptions>>().Value);
        umami.Failures.Count().ShouldBe(2);
    }

    [Fact]
    public void Registration_AddingTwice_RegistersOneProviderEach()
    {
        var services = new ServiceCollection();
        services.AddOsirionAnalytics(a => a.AddPlausible().AddPlausible().AddUmami().AddUmami());
        using var provider = services.BuildServiceProvider();

        var providers = provider.GetServices<IAnalyticsProvider>().ToList();
        providers.Count.ShouldBe(2);
        providers.ShouldContain(p => p is PlausibleProvider);
        providers.ShouldContain(p => p is UmamiProvider);
    }

    private static DefaultHttpContext HttpContextWithAnalyticsConsent(bool analytics)
    {
        var context = new DefaultHttpContext();
        var data = new CookieConsentData
        {
            Version = "1.0",
            ConsentType = "save-preferences",
            Categories = new Dictionary<string, bool> { ["necessary"] = true, ["analytics"] = analytics }
        };
        context.Request.Headers.Cookie = "osirion_cookie_consent=" + Uri.EscapeDataString(JsonSerializer.Serialize(data));
        return context;
    }
}
