using Osirion.Blazor.Components.Analytics.Options;

namespace Osirion.Blazor.Components.Analytics;

/// <summary>
/// Component for Matomo analytics tracking
/// </summary>
public partial class MatomoTracker : TrackerComponentBase<MatomoOptions>
{
    /// <summary>
    /// Gets the Matomo tracking script
    /// </summary>
    protected override string GetScript()
    {
        return $@"
            <script>
                var _paq = window._paq = window._paq || [];
                _paq.push(['trackPageView']);
                _paq.push(['enableLinkTracking']);
                (function () {{
                    var u = '{TrackerOptions?.TrackerUrl}';
                    _paq.push(['setTrackerUrl', u + 'matomo.php']);
                    _paq.push(['setSiteId', '{TrackerOptions?.SiteId}']);
                    var d = document, g = d.createElement('script'), s = d.getElementsByTagName('script')[0];
                    g.async = true; g.src = u + 'matomo.js'; s.parentNode.insertBefore(g, s);
                }})();
            </script>
        ";
    }
}