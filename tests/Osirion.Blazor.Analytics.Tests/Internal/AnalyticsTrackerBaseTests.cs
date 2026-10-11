using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Analytics.Components;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Internal;

public class AnalyticsTrackerBaseTests : BunitContext
{
    private const string ProviderScript = "<script>provider()</script>";

    private readonly IAnalyticsProvider _provider = Substitute.For<IAnalyticsProvider>();

    public AnalyticsTrackerBaseTests()
    {
        _provider.GetScript(Arg.Any<AnalyticsScriptContext>()).Returns(ProviderScript);
    }

    [Fact]
    public void AnalyticsTracker_ProviderCanRender_EmitsProviderScript()
    {
        SetRendererInfo(new RendererInfo("Static", false));
        _provider.ShouldRender.Returns(true);

        var cut = Render<ProviderOnlyTracker>(p => p.Add(c => c.TestProvider, _provider));

        cut.Markup.ShouldBe(ProviderScript);
        _provider.Received().GetScript(Arg.Is<AnalyticsScriptContext>(c => !c.ConsentGateActive));
    }

    [Fact]
    public void AnalyticsTracker_ProviderCannotRender_EmitsNothing()
    {
        SetRendererInfo(new RendererInfo("Static", false));
        _provider.ShouldRender.Returns(false);

        var cut = Render<ProviderOnlyTracker>(p => p.Add(c => c.TestProvider, _provider));

        cut.Markup.ShouldBeEmpty();
    }

    [Fact]
    public void AnalyticsTracker_EnabledFalse_EmitsNothingEvenWhenProviderCanRender()
    {
        SetRendererInfo(new RendererInfo("Static", false));
        _provider.ShouldRender.Returns(true);

        var cut = Render<ProviderOnlyTracker>(p => p
            .Add(c => c.TestProvider, _provider)
            .Add(c => c.Enabled, false));

        cut.Markup.ShouldBeEmpty();
    }

    [Fact]
    public void AnalyticsTracker_NoProvider_EmitsNothing()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<ProviderOnlyTracker>();

        cut.Markup.ShouldBeEmpty();
    }

    [Fact]
    public void AnalyticsTracker_NoProviderButScriptRequested_ReturnsEmptyScript()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<ProviderOnlyTracker>(p => p.Add(c => c.ForceMarkup, true));

        cut.Markup.ShouldBeEmpty();
    }

    [Fact]
    public void AnalyticsTracker_EnabledLater_RendersAgainWithTheScript()
    {
        Services.AddOsirionAnalytics(a => a.AddClarity(o => o.SiteId = "clarity-later"));
        SetRendererInfo(new RendererInfo("Server", true));
        var cut = Render<ClarityTracker>(p => p.Add(c => c.Enabled, false));
        cut.Markup.ShouldNotContain("<script");

        cut.Render(p => p.Add(c => c.Enabled, true));

        cut.Markup.ShouldContain("\"clarity-later\"");
    }

    [Fact]
    public void AnalyticsTracker_DisabledLater_SkipsTheRender()
    {
        Services.AddOsirionAnalytics(a => a.AddClarity(o => o.SiteId = "clarity-kept"));
        SetRendererInfo(new RendererInfo("Server", true));
        var cut = Render<ClarityTracker>();
        var renders = cut.RenderCount;

        cut.Render(p => p.Add(c => c.Enabled, false));

        cut.RenderCount.ShouldBe(renders);
        cut.Markup.ShouldContain("\"clarity-kept\"");
    }

    [Fact]
    public void Trackers_Provider_IsTheInjectedProvider()
    {
        Services.AddOsirionAnalytics(a =>
        {
            a.AddClarity(o => o.SiteId = "c");
            a.AddGA4(o => o.MeasurementId = "G-TEST1234");
            a.AddMatomo(o => { o.SiteId = "1"; o.TrackerUrl = "https://m.example.com/"; });
            a.AddYandexMetrica(o => o.CounterId = "1");
        });
        SetRendererInfo(new RendererInfo("Static", false));

        var clarity = Render<ExposedClarityTracker>().Instance.ExposedProvider;
        var ga4 = Render<ExposedGA4Tracker>().Instance.ExposedProvider;
        var matomo = Render<ExposedMatomoTracker>().Instance.ExposedProvider;
        var yandex = Render<ExposedYandexTracker>().Instance.ExposedProvider;

        clarity.ShouldBeSameAs(Services.GetRequiredService<ClarityProvider>());
        ga4.ShouldBeSameAs(Services.GetRequiredService<GA4Provider>());
        matomo.ShouldBeSameAs(Services.GetRequiredService<MatomoProvider>());
        yandex.ShouldBeSameAs(Services.GetRequiredService<YandexMetricaProvider>());
    }

    public sealed class ProviderOnlyTracker : AnalyticsTracker
    {
        [Parameter]
        public IAnalyticsProvider? TestProvider { get; set; }

        [Parameter]
        public bool ForceMarkup { get; set; }

        protected override IAnalyticsProvider? Provider => TestProvider;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (ShouldRenderTracker || ForceMarkup)
            {
                builder.AddContent(0, TrackerMarkup);
            }
        }
    }

    public sealed class ExposedClarityTracker : ClarityTracker
    {
        public IAnalyticsProvider? ExposedProvider => Provider;
    }

    public sealed class ExposedGA4Tracker : GA4Tracker
    {
        public IAnalyticsProvider? ExposedProvider => Provider;
    }

    public sealed class ExposedMatomoTracker : MatomoTracker
    {
        public IAnalyticsProvider? ExposedProvider => Provider;
    }

    public sealed class ExposedYandexTracker : YandexMetricaTracker
    {
        public IAnalyticsProvider? ExposedProvider => Provider;
    }
}
