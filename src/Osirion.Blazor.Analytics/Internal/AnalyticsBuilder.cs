using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Internal;

/// <summary>
/// Implementation of the analytics builder
/// </summary>
public class AnalyticsBuilder : IAnalyticsBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsBuilder"/> class.
    /// </summary>
    public AnalyticsBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public IAnalyticsBuilder AddClarity(Action<ClarityOptions>? configure = null)
    {
        var options = new ClarityOptions();
        configure?.Invoke(options);

        Services.Configure<ClarityOptions>(opt =>
        {
            opt.SiteId = options.SiteId;
            opt.Enabled = options.Enabled;
            opt.AutoTrackPageViews = options.AutoTrackPageViews;
            opt.TrackerUrl = options.TrackerUrl;
            opt.TrackUserAttributes = options.TrackUserAttributes;
        });

        RegisterProvider<ClarityProvider, ClarityOptions, ClarityOptionsValidator>();

        return this;
    }

    /// <inheritdoc/>
    public IAnalyticsBuilder AddMatomo(Action<MatomoOptions>? configure = null)
    {
        var options = new MatomoOptions();
        configure?.Invoke(options);

        Services.Configure<MatomoOptions>(opt =>
        {
            opt.SiteId = options.SiteId;
            opt.TrackerUrl = options.TrackerUrl;
            opt.Enabled = options.Enabled;
            opt.AutoTrackPageViews = options.AutoTrackPageViews;
            opt.TrackLinks = options.TrackLinks;
            opt.TrackDownloads = options.TrackDownloads;
            opt.RequireConsent = options.RequireConsent;
        });

        RegisterProvider<MatomoProvider, MatomoOptions, MatomoOptionsValidator>();

        return this;
    }

    /// <inheritdoc/>
    public IAnalyticsBuilder AddGA4(Action<GA4Options>? configure = null)
    {
        var options = new GA4Options();
        configure?.Invoke(options);

        Services.Configure<GA4Options>(opt =>
        {
            opt.MeasurementId = options.MeasurementId;
            opt.Enabled = options.Enabled;
            opt.AutoTrackPageViews = options.AutoTrackPageViews;
            opt.AnonymizeIp = options.AnonymizeIp;
            opt.LinkAttribution = options.LinkAttribution;
            opt.DebugMode = options.DebugMode;
            opt.CookieFlags = options.CookieFlags;
            opt.ConfigParameters = options.ConfigParameters;
            opt.SendPageView = options.SendPageView;
            opt.TransportType = options.TransportType;
            opt.TrackOutboundLinks = options.TrackOutboundLinks;
            opt.CookieDomain = options.CookieDomain;
            opt.CookieExpires = options.CookieExpires;
            opt.RestrictDataProcessing = options.RestrictDataProcessing;
        });

        RegisterProvider<GA4Provider, GA4Options, GA4OptionsValidator>();

        return this;
    }

    /// <inheritdoc/>
    public IAnalyticsBuilder AddYandexMetrica(Action<YandexMetricaOptions>? configure = null)
    {
        var options = new YandexMetricaOptions();
        configure?.Invoke(options);

        Services.Configure<YandexMetricaOptions>(opt =>
        {
            opt.CounterId = options.CounterId;
            opt.Enabled = options.Enabled;
            opt.AutoTrackPageViews = options.AutoTrackPageViews;
            opt.TrackLinks = options.TrackLinks;
            opt.AccurateTrackBounce = options.AccurateTrackBounce;
            opt.WebVisor = options.WebVisor;
            opt.ClickMap = options.ClickMap;
            opt.TrackHash = options.TrackHash;
            opt.HashTracking = options.HashTracking;
            opt.DeferLoad = options.DeferLoad;
            opt.AlternativeCdn = options.AlternativeCdn;
            opt.Params = options.Params;
            opt.UserParams = options.UserParams;
            opt.EcommerceEnabled = options.EcommerceEnabled;
            opt.EcommerceContainerName = options.EcommerceContainerName;
        });

        RegisterProvider<YandexMetricaProvider, YandexMetricaOptions, YandexMetricaOptionsValidator>();

        return this;
    }

    /// <inheritdoc/>
    public IAnalyticsBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IAnalyticsProvider
    {
        if (configure is not null)
        {
            Services.AddSingleton(sp => {
                var provider = ActivatorUtilities.CreateInstance<TProvider>(sp);
                configure(provider);
                return provider;
            });

            Services.AddSingleton<IAnalyticsProvider>(sp => sp.GetRequiredService<TProvider>());
        }
        else
        {
            RegisterProviderOnce<TProvider>();
        }

        return this;
    }

    private void RegisterProvider<TProvider, TOptions, TValidator>()
        where TProvider : class, IAnalyticsProvider
        where TOptions : class
        where TValidator : class, IValidateOptions<TOptions>
    {
        Services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<TOptions>, TValidator>());
        Services.AddOptions<TOptions>().ValidateOnStart();
        RegisterProviderOnce<TProvider>();
    }

    // One concrete singleton, exposed through IAnalyticsProvider as the same instance, so the tracker
    // component (which injects the concrete type) and IAnalyticsService share state, and repeated
    // Add* calls do not register duplicates.
    private void RegisterProviderOnce<TProvider>()
        where TProvider : class, IAnalyticsProvider
    {
        if (Services.Any(descriptor => descriptor.ServiceType == typeof(TProvider)))
        {
            return;
        }

        Services.AddSingleton<TProvider>();
        Services.AddSingleton<IAnalyticsProvider>(serviceProvider => serviceProvider.GetRequiredService<TProvider>());
    }
}
