using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Internal;
using Osirion.Blazor.Analytics.Options;

namespace Osirion.Blazor.Analytics.Providers;

/// <summary>
/// Plausible Analytics provider
/// </summary>
public class PlausibleProvider : IAnalyticsProvider
{
    // Plausible's own snippet: queues calls made before the script loads and defines plausible.init.
    private const string QueueStub =
        "window.plausible=window.plausible||function(){(plausible.q=plausible.q||[]).push(arguments)},plausible.init=plausible.init||function(i){plausible.o=i||{}};";

    private readonly PlausibleOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlausibleProvider"/> class.
    /// </summary>
    public PlausibleProvider(IOptions<PlausibleOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public string ProviderId => "plausible";

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
        // Custom events are sent from the browser with plausible('Event name').
        return Task.CompletedTask;
    }

    /// <summary>Creates a copy of the configured options that a tracker can adjust for one render.</summary>
    public PlausibleOptions CreateOptionsCopy() => (PlausibleOptions)_options.ShallowCopy();

    /// <summary>Gets whether a script can be rendered for <paramref name="options"/>.</summary>
    public bool CanRender(PlausibleOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.Enabled && AnalyticsFormats.IsHttpOrSiteRelativeUrl(options.ScriptUrl)
            && (string.IsNullOrWhiteSpace(options.Endpoint) || AnalyticsFormats.IsHttpOrSiteRelativeUrl(options.Endpoint));
    }

    /// <inheritdoc/>
    public string GetScript() => GetScript(_options, AnalyticsScriptContext.Default);

    /// <inheritdoc/>
    public string GetScript(AnalyticsScriptContext context) => GetScript(_options, context);

    /// <summary>Builds the script for <paramref name="options"/>.</summary>
    public string GetScript(PlausibleOptions options, AnalyticsScriptContext context)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(context);

        // System.Text.Json escapes <, > and &, so no value can close the script element.
        return $@"
            <script async src=""{ScriptLiteral.Attribute(options.ScriptUrl)}""></script>
            <script>
                {QueueStub}
                plausible.init({CreateInitOptions(options).ToJsonString()});
            </script>
        ";
    }

    // Only the options that were set: the script reads the rest from the site's settings in Plausible.
    private static JsonObject CreateInitOptions(PlausibleOptions options)
    {
        var init = new JsonObject();
        if (!string.IsNullOrWhiteSpace(options.Endpoint))
        {
            init["endpoint"] = options.Endpoint;
        }

        if (!options.AutoTrackPageViews)
        {
            init["autoCapturePageviews"] = false;
        }

        AddIfSet(init, "hashBasedRouting", options.HashBasedRouting);
        AddIfSet(init, "outboundLinks", options.OutboundLinks);
        AddIfSet(init, "fileDownloads", options.FileDownloads);
        AddIfSet(init, "formSubmissions", options.FormSubmissions);
        AddIfSet(init, "captureOnLocalhost", options.CaptureOnLocalhost);
        return init;
    }

    private static void AddIfSet(JsonObject init, string name, bool? value)
    {
        if (value is { } set)
        {
            init[name] = set;
        }
    }
}
