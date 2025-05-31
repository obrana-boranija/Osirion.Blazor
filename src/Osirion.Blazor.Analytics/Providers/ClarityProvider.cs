using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Options;

namespace Osirion.Blazor.Analytics.Providers;

/// <summary>
/// Microsoft Clarity analytics provider
/// </summary>
public class ClarityProvider : IAnalyticsProvider
{
    private readonly ClarityOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClarityProvider"/> class.
    /// </summary>
    public ClarityProvider(IOptions<ClarityOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public string ProviderId => "clarity";

    /// <inheritdoc/>
    public bool IsEnabled => _options.Enabled && !string.IsNullOrWhiteSpace(_options.SiteId);

    /// <inheritdoc/>
    public bool ShouldRender => IsEnabled;

    /// <inheritdoc/>
    public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        // Clarity tracks page views automatically
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        // Clarity doesn't support custom event tracking
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public string GetScript()
    {
        return $@"
            <script type=""text/javascript"">
                (function (c, l, a, r, i, t, y) {{
                    c[a] = c[a] || function () {{ (c[a].q = c[a].q || []).push(arguments) }};
                    t = l.createElement(r); t.async = 1; t.src = ""{_options.TrackerUrl}"" + i;
                    y = l.getElementsByTagName(r)[0]; y.parentNode.insertBefore(t, y);
                }})(window, document, ""clarity"", ""script"", ""{_options.SiteId}"");
            </script>
        ";
    }
}