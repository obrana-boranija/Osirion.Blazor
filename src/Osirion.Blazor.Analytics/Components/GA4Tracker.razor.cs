using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Components;

/// <summary>
/// Component for Google Analytics 4 tracking.
/// </summary>
public partial class GA4Tracker
{
    /// <summary>
    /// Gets the analytics provider.
    /// </summary>
    protected override IAnalyticsProvider? Provider => _provider;

    /// <summary>
    /// The GA4 provider instance.
    /// </summary>
    [Inject]
    private GA4Provider? _provider { get; set; }

    /// <summary>
    /// Gets or sets the Measurement ID (overrides configured value).
    /// </summary>
    [Parameter]
    public string? MeasurementId { get; set; }

    /// <summary>
    /// Gets or sets whether to enable debug mode.
    /// </summary>
    [Parameter]
    public bool? DebugMode { get; set; }

    /// <summary>
    /// Initializes component parameters.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Provider is GA4Provider ga4Provider)
        {
            // Apply any parameter overrides if needed
            // This is where we would override options from Parameters
        }
    }
}