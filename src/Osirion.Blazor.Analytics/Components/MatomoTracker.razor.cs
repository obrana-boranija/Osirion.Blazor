using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Components;
public partial class MatomoTracker
{
    protected override IAnalyticsProvider? Provider => _provider;

    [Inject]
    private MatomoProvider? _provider { get; set; }
}
