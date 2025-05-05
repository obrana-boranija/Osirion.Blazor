using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Components;

public partial class ClarityTracker
{
    protected override IAnalyticsProvider? Provider => _provider;

    [Inject]
    private ClarityProvider? _provider { get; set; }
}
