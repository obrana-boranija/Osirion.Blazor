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

    // Resolved on demand, so a tracker left in a layout renders nothing instead of failing the page
    // when its provider is not registered.
    private YandexMetricaProvider? _provider => field ??= GetOptionalService<YandexMetricaProvider>();

    /// <summary>
    /// Gets or sets the counter ID for this render, replacing the configured <c>CounterId</c>. It must be a number of up to 18
    /// digits; anything else renders nothing.
    /// </summary>
    [Parameter]
    public string? CounterId { get; set; }

    /// <summary>
    /// Gets or sets whether Webvisor session replay is on for this render, replacing the configured <c>WebVisor</c>, which is
    /// <c>false</c> unless configured.
    /// </summary>
    [Parameter]
    public bool? WebVisor { get; set; }

    /// <summary>
    /// Gets or sets whether the click map is on for this render, replacing the configured <c>ClickMap</c>, which is <c>true</c>
    /// unless configured.
    /// </summary>
    [Parameter]
    public bool? ClickMap { get; set; }

    /// <summary>
    /// Gets or sets whether clicks on outbound links are tracked for this render, replacing the configured <c>TrackLinks</c>,
    /// which is <c>true</c> unless configured.
    /// </summary>
    [Parameter]
    public bool? TrackLinks { get; set; }

    /// <summary>
    /// Gets or sets whether accurate bounce rate tracking is on for this render, replacing the configured
    /// <c>AccurateTrackBounce</c>, which is <c>true</c> unless configured.
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
