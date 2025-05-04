using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;

namespace Osirion.Blazor.Analytics.Internal;

/// <summary>
/// Implementation of the analytics builder
/// </summary>
internal class AnalyticsBuilder : IAnalyticsBuilder
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

        Services.TryAddSingleton<ClarityProvider>();

        Services.TryAddSingleton<IAnalyticsProvider, ClarityProvider>();

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

        Services.TryAddSingleton<MatomoProvider>();
        Services.TryAddSingleton<IAnalyticsProvider, MatomoProvider>();

        return this;
    }

    /// <inheritdoc/>
    public IAnalyticsBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IAnalyticsProvider
    {
        if (configure != null)
        {
            Services.AddSingleton(sp => {
                var provider = ActivatorUtilities.CreateInstance<TProvider>(sp);
                configure(provider);
                return provider;
            });
        }
        else
        {
            Services.TryAddSingleton<TProvider>();
        }

        Services.TryAddSingleton<IAnalyticsProvider, TProvider>();

        return this;
    }
}