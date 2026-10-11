using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Components;

/// <summary>Renders the Plausible Analytics tracking integration.</summary>
public partial class PlausibleTracker
{
    /// <summary>Gets the configured Plausible analytics provider.</summary>
    protected override IAnalyticsProvider? Provider => _provider;

    // Resolved on demand, so a tracker left in a layout renders nothing instead of failing the page
    // when its provider is not registered.
    private PlausibleProvider? _provider => field ??= GetOptionalService<PlausibleProvider>();

    /// <inheritdoc/>
    protected override bool CanRenderProvider => _provider is not null && _provider.CanRender(CreateEffectiveOptions());

    /// <inheritdoc/>
    protected override string BuildScript(AnalyticsScriptContext context) =>
        _provider is null ? string.Empty : _provider.GetScript(CreateEffectiveOptions(), context);

    private PlausibleOptions CreateEffectiveOptions()
    {
        var options = _provider!.CreateOptionsCopy();
        options.Enabled = Enabled ?? options.Enabled;
        return options;
    }
}
