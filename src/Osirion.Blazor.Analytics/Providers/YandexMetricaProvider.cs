using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Internal;
using Osirion.Blazor.Analytics.Options;
using System.Text.Json;

namespace Osirion.Blazor.Analytics.Providers;

/// <summary>
/// Yandex Metrica analytics provider
/// </summary>
public class YandexMetricaProvider : IAnalyticsProvider
{
    private const string DefaultScriptUrl = "https://mc.yandex.ru/metrika/tag.js";

    private readonly YandexMetricaOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="YandexMetricaProvider"/> class.
    /// </summary>
    public YandexMetricaProvider(IOptions<YandexMetricaOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public string ProviderId => "yandex-metrica";

    /// <inheritdoc/>
    public bool IsEnabled => CanRender(_options);

    /// <inheritdoc/>
    public bool ShouldRender => IsEnabled;

    /// <inheritdoc/>
    public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        // Yandex Metrica tracks the first page view from the script; later enhanced navigations are
        // reported by the osirion-analytics.js module.
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        // Event tracking requires JavaScript interop
        return Task.CompletedTask;
    }

    /// <summary>Creates a copy of the configured options that a tracker can adjust for one render.</summary>
    public YandexMetricaOptions CreateOptionsCopy() => (YandexMetricaOptions)_options.ShallowCopy();

    /// <summary>
    /// Gets whether a script can be rendered for <paramref name="options"/>. The counter ID is emitted
    /// as a bare number, so anything that is not an integer disables rendering.
    /// </summary>
    public bool CanRender(YandexMetricaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.Enabled && AnalyticsFormats.IsYandexCounterId(options.CounterId);
    }

    /// <inheritdoc/>
    public string GetScript() => GetScript(_options, AnalyticsScriptContext.Default);

    /// <inheritdoc/>
    public string GetScript(AnalyticsScriptContext context) => GetScript(_options, context);

    /// <summary>Builds the script for <paramref name="options"/>.</summary>
    /// <remarks>
    /// Yandex's snippet also has a <c>noscript</c> pixel in a <c>div</c>. That is valid only in the body, and the tracker
    /// is placed in the head, so it is not written; a visitor without JavaScript is not counted.
    /// </remarks>
    public string GetScript(YandexMetricaOptions options, AnalyticsScriptContext context)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(context);

        var initOptions = new Dictionary<string, object>();

        if (options.ClickMap)
            initOptions["clickmap"] = true;

        if (options.TrackLinks)
            initOptions["trackLinks"] = true;

        if (options.AccurateTrackBounce)
            initOptions["accurateTrackBounce"] = true;

        if (options.WebVisor)
            initOptions["webvisor"] = true;

        // HashTracking is the older spelling of TrackHash, so only an explicit TrackHash overrides it.
        var trackHash = options.TrackHash ?? (options.HashTracking ? true : null);
        if (trackHash.HasValue)
            initOptions["trackHash"] = trackHash.Value;

        if (options.DeferLoad)
            initOptions["defer"] = true;

        if (options.EcommerceEnabled)
            initOptions["ecommerce"] = options.EcommerceContainerName;

        if (options.Params is not null && options.Params.Count > 0)
            initOptions["params"] = options.Params;

        if (options.UserParams is not null && options.UserParams.Count > 0)
            initOptions["userParams"] = options.UserParams;

        var initOptionsJson = JsonSerializer.Serialize(initOptions);
        var scriptUrl = AnalyticsFormats.IsHttpUrl(options.AlternativeCdn) ? options.AlternativeCdn! : DefaultScriptUrl;
        var pageViewModule = options.AutoTrackPageViews
            ? PageViewModuleTag.Build("yandex", options.CounterId)
            : string.Empty;

        // CounterId is bare JavaScript here; CanRender guarantees it is an integer.
        return $@"
            <script type=""text/javascript"">
                (function(m,e,t,r,i,k,a){{m[i]=m[i]||function(){{(m[i].a=m[i].a||[]).push(arguments)}};
                m[i].l=1*new Date();
                for (var j = 0; j < document.scripts.length; j++) {{if (document.scripts[j].src === r) {{ return; }}}}
                k=e.createElement(t),a=e.getElementsByTagName(t)[0],k.async=1,k.src=r,a.parentNode.insertBefore(k,a)}})
                (window, document, ""script"", {ScriptLiteral.Js(scriptUrl)}, ""ym"");

                ym({options.CounterId}, ""init"", {initOptionsJson});
            </script>
            {pageViewModule}
        ";
    }
}
