using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Components;

/// <summary>Renders the Microsoft Clarity tracking integration.</summary>
public partial class ClarityTracker
{
    /// <summary>Gets the configured Clarity analytics provider.</summary>
    protected override IAnalyticsProvider? Provider => _provider;

    [Inject]
    private ClarityProvider? _provider { get; set; }

    /// <inheritdoc/>
    protected override bool CanRenderProvider => _provider is not null && _provider.CanRender(CreateEffectiveOptions());

    /// <inheritdoc/>
    protected override string BuildScript(AnalyticsScriptContext context) =>
        _provider is null ? string.Empty : _provider.GetScript(CreateEffectiveOptions(), context);

    private ClarityOptions CreateEffectiveOptions()
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
