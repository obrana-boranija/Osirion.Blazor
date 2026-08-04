using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Components;

/// <summary>Renders the Microsoft Clarity tracking integration.</summary>
public partial class ClarityTracker
{
    /// <summary>Gets the configured Clarity analytics provider.</summary>
    protected override IAnalyticsProvider? Provider => _provider;

    [Inject]
    private ClarityProvider? _provider { get; set; }
}
