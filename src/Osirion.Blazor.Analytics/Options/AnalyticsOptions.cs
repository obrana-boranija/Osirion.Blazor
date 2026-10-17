namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Base options for analytics providers
/// </summary>
public abstract class AnalyticsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "Osirion:Analytics";

    /// <summary>
    /// Gets or sets the site ID. Clarity (the project ID), Matomo (the site ID) and Umami (the website ID, a GUID) read it.
    /// <c>GA4Options.MeasurementId</c> and <c>YandexMetricaOptions.CounterId</c> are those providers' IDs, and Plausible
    /// takes none: for these three providers this value is ignored.
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// Gets or sets whether the provider is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether page views are tracked automatically. Matomo and Yandex Metrica still count the page that was
    /// loaded; <c>false</c> only switches off the script that reports Blazor enhanced navigations. Umami (<c>data-auto-track</c>)
    /// and Plausible (<c>autoCapturePageviews</c>) hand it to the provider script. It has no effect for Clarity, which records
    /// every page, or for GA4, where <c>GA4Options.SendPageView</c> decides.
    /// </summary>
    public bool AutoTrackPageViews { get; set; } = true;

    /// <summary>Creates a shallow copy so a tracker can apply per-render overrides without touching the shared options.</summary>
    internal AnalyticsOptions ShallowCopy() => (AnalyticsOptions)MemberwiseClone();
}
