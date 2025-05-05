using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Components;

/// <summary>
/// Component for Yandex Metrica analytics tracking.
/// </summary>
public partial class YandexMetricaTracker
{
    /// <summary>
    /// Gets the analytics provider.
    /// </summary>
    protected override IAnalyticsProvider? Provider => _provider;

    /// <summary>
    /// The Yandex Metrica provider instance.
    /// </summary>
    [Inject]
    private YandexMetricaProvider? _provider { get; set; }

    /// <summary>
    /// Gets or sets the counter ID (overrides configured value).
    /// </summary>
    [Parameter]
    public string? CounterId { get; set; }

    /// <summary>
    /// Gets or sets whether to enable WebVisor session replay.
    /// </summary>
    [Parameter]
    public bool? WebVisor { get; set; }

    /// <summary>
    /// Gets or sets whether to enable click map.
    /// </summary>
    [Parameter]
    public bool? ClickMap { get; set; }

    /// <summary>
    /// Gets or sets whether to track external links.
    /// </summary>
    [Parameter]
    public bool? TrackLinks { get; set; }

    /// <summary>
    /// Gets or sets whether to accurately track bounce rate.
    /// </summary>
    [Parameter]
    public bool? AccurateTrackBounce { get; set; }

    /// <summary>
    /// Initializes component parameters.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Provider is YandexMetricaProvider yandexProvider)
        {
            // This might require refactoring the provider to accept runtime configuration
        }
    }
}