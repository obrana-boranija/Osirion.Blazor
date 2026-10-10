namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Controls whether tracker scripts wait for the visitor's analytics consent.
/// </summary>
/// <remarks>
/// Consent is read on the server from the cookie written by <c>OsirionCookieConsent</c>, so the gate
/// works under Static SSR and in the prerender pass of interactive modes. When the gate is on and no
/// <c>HttpContext</c> is available (for example inside an already interactive component), the tracker
/// renders nothing. Turning the gate on is recommended for sites with visitors in the EU.
/// </remarks>
public class AnalyticsConsentOptions
{
    /// <summary>Configuration section name.</summary>
    public const string Section = "Osirion:Analytics:Consent";

    /// <summary>
    /// Gets or sets whether trackers render only after the visitor consented to analytics.
    /// The default is <see langword="false"/> so existing hosts keep tracking as before.
    /// </summary>
    public bool RequireConsent { get; set; }

    /// <summary>
    /// Gets or sets the consent category that must be accepted. Defaults to <c>analytics</c>, the
    /// category written by <c>OsirionCookieConsent</c>.
    /// </summary>
    public string Category { get; set; } = "analytics";
}
