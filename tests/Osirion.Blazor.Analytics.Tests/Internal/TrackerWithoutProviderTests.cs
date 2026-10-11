using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Components;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Internal;

public class TrackerWithoutProviderTests : BunitContext
{
    public TrackerWithoutProviderTests() => SetRendererInfo(new RendererInfo("Server", false));

    [Fact]
    public void Trackers_ProviderNotRegistered_RenderNothingInsteadOfThrowing()
    {
        Render<ClarityTracker>().Markup.ShouldBeEmpty();
        Render<MatomoTracker>().Markup.ShouldBeEmpty();
        Render<GA4Tracker>().Markup.ShouldBeEmpty();
        Render<YandexMetricaTracker>().Markup.ShouldBeEmpty();
    }
}
