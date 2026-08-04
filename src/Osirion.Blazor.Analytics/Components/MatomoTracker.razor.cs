using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Components;
/// <summary>Renders the Matomo tracking integration.</summary>
public partial class MatomoTracker
{
    /// <summary>Gets the configured Matomo analytics provider.</summary>
    protected override IAnalyticsProvider? Provider => _provider;

    [Inject]
    private MatomoProvider? _provider { get; set; }
}
