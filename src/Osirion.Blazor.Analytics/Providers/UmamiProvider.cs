using System.Text;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Internal;
using Osirion.Blazor.Analytics.Options;

namespace Osirion.Blazor.Analytics.Providers;

/// <summary>
/// Umami analytics provider
/// </summary>
public class UmamiProvider : IAnalyticsProvider
{
    private readonly UmamiOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="UmamiProvider"/> class.
    /// </summary>
    public UmamiProvider(IOptions<UmamiOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public string ProviderId => "umami";

    /// <inheritdoc/>
    public bool IsEnabled => CanRender(_options);

    /// <inheritdoc/>
    public bool ShouldRender => IsEnabled;

    /// <inheritdoc/>
    public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        // The script reports page views itself, including enhanced navigations, through the History API.
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        // Custom events are sent from the browser with umami.track() or data-umami-event attributes.
        return Task.CompletedTask;
    }

    /// <summary>Creates a copy of the configured options that a tracker can adjust for one render.</summary>
    public UmamiOptions CreateOptionsCopy() => (UmamiOptions)_options.ShallowCopy();

    /// <summary>Gets whether a script can be rendered for <paramref name="options"/>.</summary>
    public bool CanRender(UmamiOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.Enabled && Guid.TryParse(options.SiteId, out _)
            && AnalyticsFormats.IsHttpOrSiteRelativeUrl(options.ScriptUrl)
            && (string.IsNullOrWhiteSpace(options.HostUrl) || AnalyticsFormats.IsHttpOrProtocolRelativeUrl(options.HostUrl));
    }

    /// <inheritdoc/>
    public string GetScript() => GetScript(_options, AnalyticsScriptContext.Default);

    /// <inheritdoc/>
    public string GetScript(AnalyticsScriptContext context) => GetScript(_options, context);

    /// <summary>Builds the script for <paramref name="options"/>.</summary>
    /// <remarks>The tracker is configured through attributes only, so no inline script is emitted.</remarks>
    public string GetScript(UmamiOptions options, AnalyticsScriptContext context)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(context);

        var tag = new StringBuilder("<script defer");
        AppendAttribute(tag, "src", options.ScriptUrl);
        AppendAttribute(tag, "data-website-id", options.SiteId);
        AppendAttribute(tag, "data-host-url", options.HostUrl);
        AppendAttribute(tag, "data-domains", options.Domains);
        AppendAttribute(tag, "data-tag", options.Tag);
        AppendAttribute(tag, "data-auto-track", options.AutoTrackPageViews ? null : "false");
        AppendAttribute(tag, "data-exclude-search", options.ExcludeSearch ? "true" : null);
        AppendAttribute(tag, "data-exclude-hash", options.ExcludeHash ? "true" : null);
        AppendAttribute(tag, "data-do-not-track", options.DoNotTrack ? "true" : null);
        return tag.Append("></script>").ToString();
    }

    private static void AppendAttribute(StringBuilder tag, string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            tag.Append(' ').Append(name).Append("=\"").Append(ScriptLiteral.Attribute(value)).Append('"');
        }
    }
}
