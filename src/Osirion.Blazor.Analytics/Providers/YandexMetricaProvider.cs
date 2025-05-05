using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Options;
using System.Text.Json;

namespace Osirion.Blazor.Analytics.Providers;

/// <summary>
/// Yandex Metrica analytics provider
/// </summary>
public class YandexMetricaProvider : IAnalyticsProvider
{
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
    public bool IsEnabled => _options.Enabled && !string.IsNullOrEmpty(_options.CounterId);

    /// <inheritdoc/>
    public bool ShouldRender => IsEnabled;

    /// <inheritdoc/>
    public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        // Yandex Metrica tracks page views automatically or via JavaScript interop
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        // Event tracking requires JavaScript interop
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public string GetScript()
    {
        var initOptions = new Dictionary<string, object>();

        if (_options.ClickMap)
            initOptions["clickmap"] = true;

        if (_options.TrackLinks)
            initOptions["trackLinks"] = true;

        if (_options.AccurateTrackBounce)
            initOptions["accurateTrackBounce"] = true;

        if (_options.WebVisor)
            initOptions["webvisor"] = true;

        if (_options.TrackHash.HasValue)
            initOptions["trackHash"] = _options.TrackHash.Value;

        if (_options.DeferLoad)
            initOptions["defer"] = true;

        if (_options.EcommerceEnabled)
            initOptions["ecommerce"] = _options.EcommerceContainerName;

        if (_options.Params != null && _options.Params.Count > 0)
            initOptions["params"] = _options.Params;

        if (_options.UserParams != null && _options.UserParams.Count > 0)
            initOptions["userParams"] = _options.UserParams;

        var initOptionsJson = JsonSerializer.Serialize(initOptions);
        var scriptUrl = _options.AlternativeCdn ?? "https://mc.yandex.ru/metrika/tag.js";

        return $@"
            <script type=""text/javascript"">
                (function(m,e,t,r,i,k,a){{m[i]=m[i]||function(){{(m[i].a=m[i].a||[]).push(arguments)}};
                m[i].l=1*new Date();
                for (var j = 0; j < document.scripts.length; j++) {{if (document.scripts[j].src === r) {{ return; }}}}
                k=e.createElement(t),a=e.getElementsByTagName(t)[0],k.async=1,k.src=r,a.parentNode.insertBefore(k,a)}})
                (window, document, ""script"", ""{scriptUrl}"", ""ym"");

                ym({_options.CounterId}, ""init"", {initOptionsJson});
            </script>
            <noscript><div><img src=""https://mc.yandex.ru/watch/{_options.CounterId}"" style=""position:absolute; left:-9999px;"" alt="""" /></div></noscript>
        ";
    }
}