using Osirion.Blazor.Components.Analytics.Options;

namespace Osirion.Blazor.Components.Analytics;

/// <summary>
/// Component for Microsoft Clarity analytics tracking
/// </summary>
public partial class ClarityTracker : TrackerComponentBase<ClarityOptions>
{
    /// <summary>
    /// Gets the Clarity tracking script
    /// </summary>
    protected override string GetScript()
    {
        return $@"
            <script type=""text/javascript"">
                (function (c, l, a, r, i, t, y) {{
                    c[a] = c[a] || function () {{ (c[a].q = c[a].q || []).push(arguments) }};
                    t = l.createElement(r); t.async = 1; t.src = ""{TrackerOptions?.TrackerUrl}"" + i;
                    y = l.getElementsByTagName(r)[0]; y.parentNode.insertBefore(t, y);
                }})(window, document, ""clarity"", ""script"", ""{TrackerOptions?.SiteId}"");
            </script>
        ";
    }
}