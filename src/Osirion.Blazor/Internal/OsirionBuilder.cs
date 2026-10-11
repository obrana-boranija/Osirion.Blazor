using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Analytics;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Web.DependencyInjection;
using Osirion.Blazor.Navigation;
using Osirion.Blazor.Theming;
using Osirion.Blazor.Theming.Extensions;
using Osirion.Blazor.Navigation.Extensions;
using Osirion.Blazor.Cms.Admin;
using Osirion.Blazor.Cms.Admin.DependencyInjection;
using Osirion.Blazor.Cms.Infrastructure.Builders;
using Osirion.Blazor.Core.Extensions;

namespace Osirion.Blazor.Internal;

internal class OsirionBuilder : IOsirionBuilder
{
    public OsirionBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public IServiceCollection Services { get; }

    public IOsirionBuilder UseContent(Action<IContentBuilder> configure)
    {
        Services.AddOsirionContent(configure);
        return this;
    }

    public IOsirionBuilder UseCmsAdmin(Action<ICmsAdminBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        Services.AddOsirionCmsAdminDI();

        // The host's configuration is registered as an instance by the web application builder; without it the
        // builder still applies everything the callback configures in code.
        var configuration = Services
            .LastOrDefault(descriptor => descriptor.ServiceType == typeof(IConfiguration))?
            .ImplementationInstance as IConfiguration ?? new ConfigurationBuilder().Build();

        configure(new CmsAdminBuilder(Services, configuration, NullLogger<CmsAdminBuilder>.Instance));
        return this;
    }

    //public IOsirionBuilder UseTheming(Action<IThemingBuilder> configure)
    //{
    //    Services.AddOsirionTheming(configure);
    //    return this;
    //}

    public IOsirionBuilder UseContent(IConfiguration configuration)
    {
        Services.AddOsirionContent(configuration);
        return this;
    }

    public IOsirionBuilder UseCmsAdmin(IConfiguration configuration)
    {
        Services.AddOsirionCmsAdmin(configuration);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseAnalytics(Action<IAnalyticsBuilder> configure)
    {
        Services.AddOsirionAnalytics(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseAnalytics(IConfiguration configuration)
    {
        Services.AddOsirionAnalytics(configuration);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseNavigation(Action<INavigationBuilder> configure)
    {
        Services.AddOsirionNavigation(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseNavigation(IConfiguration configuration)
    {
        Services.AddOsirionNavigation(configuration);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseTheming(Action<IThemingBuilder> configure)
    {
        Services.AddOsirionTheming(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseTheming(IConfiguration configuration)
    {
        Services.AddOsirionTheming(configuration);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseEmailServices(Action<Core.Configuration.EmailOptions> configure)
    {
        Services.AddOsirionEmailServices(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseEmailServices(IConfiguration configuration)
    {
        Services.AddOsirionEmailServices(configuration);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseSeoMetadata(Action<Cms.Web.Options.SeoMetadataOptions> configure)
    {
        Services.AddOsirionSeoMetadata(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseSeoMetadata(IConfiguration configuration)
    {
        Services.AddOsirionSeoMetadata(configuration);
        return this;
    }

    //public IOsirionBuilder UseTheming(IConfiguration configuration)
    //{
    //    Services.AddOsirionTheming(configuration);
    //    return this;
    //}
}
