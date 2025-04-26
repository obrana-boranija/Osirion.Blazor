using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Analytics.Components;
public partial class MatomoTracker
{
    protected override IAnalyticsProvider? Provider => _provider;

    [Inject]
    private IAnalyticsProvider? _provider { get; set; }
}
