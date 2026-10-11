using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Components;

/// <summary>Renders the Matomo tracking integration.</summary>
public partial class MatomoTracker
{
    /// <summary>Gets the configured Matomo analytics provider.</summary>
    protected override IAnalyticsProvider? Provider => _provider;

    // Resolved on demand, so a tracker left in a layout renders nothing instead of failing the page
    // when its provider is not registered.
    private MatomoProvider? _provider => field ??= GetOptionalService<MatomoProvider>();

    /// <inheritdoc/>
    protected override bool CanRenderProvider => _provider is not null && _provider.CanRender(CreateEffectiveOptions());

    /// <inheritdoc/>
    protected override string BuildScript(AnalyticsScriptContext context) =>
        _provider is null ? string.Empty : _provider.GetScript(CreateEffectiveOptions(), context);

    private MatomoOptions CreateEffectiveOptions()
    {
        var options = _provider!.CreateOptionsCopy();
        options.Enabled = Enabled ?? options.Enabled;
        if (!string.IsNullOrWhiteSpace(SiteId))
        {
            options.SiteId = SiteId;
        }

        return options;
    }
}
