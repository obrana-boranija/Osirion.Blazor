using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Analytics.Components;

public partial class ClarityTracker
{
    protected override IAnalyticsProvider? Provider => _provider;

    [Inject]
    private IAnalyticsProvider? _provider { get; set; }
}
