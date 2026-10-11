using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Options;
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
    /// Gets or sets the counter ID. When set it replaces the configured value for this render.
    /// </summary>
    [Parameter]
    public string? CounterId { get; set; }

    /// <summary>
    /// Gets or sets whether to enable WebVisor session replay. When set it replaces the configured value for this render.
    /// </summary>
    [Parameter]
    public bool? WebVisor { get; set; }

    /// <summary>
    /// Gets or sets whether to enable click map. When set it replaces the configured value for this render.
    /// </summary>
    [Parameter]
    public bool? ClickMap { get; set; }

    /// <summary>
    /// Gets or sets whether to track external links. When set it replaces the configured value for this render.
    /// </summary>
    [Parameter]
    public bool? TrackLinks { get; set; }

    /// <summary>
    /// Gets or sets whether to accurately track bounce rate. When set it replaces the configured value for this render.
    /// </summary>
    [Parameter]
    public bool? AccurateTrackBounce { get; set; }

    /// <inheritdoc/>
    protected override bool CanRenderProvider => _provider is not null && _provider.CanRender(CreateEffectiveOptions());

    /// <inheritdoc/>
    protected override string BuildScript(AnalyticsScriptContext context) =>
        _provider is null ? string.Empty : _provider.GetScript(CreateEffectiveOptions(), context);

    private YandexMetricaOptions CreateEffectiveOptions()
    {
        var options = _provider!.CreateOptionsCopy();
        options.Enabled = Enabled ?? options.Enabled;
        options.WebVisor = WebVisor ?? options.WebVisor;
        options.ClickMap = ClickMap ?? options.ClickMap;
        options.TrackLinks = TrackLinks ?? options.TrackLinks;
        options.AccurateTrackBounce = AccurateTrackBounce ?? options.AccurateTrackBounce;
        if (!string.IsNullOrWhiteSpace(CounterId))
        {
            options.CounterId = CounterId;
        }

        return options;
    }
}
