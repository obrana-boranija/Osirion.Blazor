using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Options;
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

    // Resolved on demand, so a tracker left in a layout renders nothing instead of failing the page
    // when its provider is not registered.
    private GA4Provider? _provider => field ??= GetOptionalService<GA4Provider>();

    /// <summary>
    /// Gets or sets the Measurement ID. When set it replaces the configured value for this render.
    /// </summary>
    [Parameter]
    public string? MeasurementId { get; set; }

    /// <summary>
    /// Gets or sets whether to enable debug mode. When set it replaces the configured value for this render.
    /// </summary>
    [Parameter]
    public bool? DebugMode { get; set; }

    /// <inheritdoc/>
    protected override bool CanRenderProvider => _provider is not null && _provider.CanRender(CreateEffectiveOptions());

    /// <inheritdoc/>
    protected override string BuildScript(AnalyticsScriptContext context) =>
        _provider is null ? string.Empty : _provider.GetScript(CreateEffectiveOptions(), context);

    private GA4Options CreateEffectiveOptions()
    {
        var options = _provider!.CreateOptionsCopy();
        options.Enabled = Enabled ?? options.Enabled;
        options.DebugMode = DebugMode ?? options.DebugMode;
        if (!string.IsNullOrWhiteSpace(MeasurementId))
        {
            options.MeasurementId = MeasurementId;
        }

        return options;
    }
}
