using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features.Dashboard;
using Osirion.Blazor.Cms.Admin.Features.Layouts;
using Osirion.Blazor.Cms.Admin.Features.Navigation;
using Osirion.Blazor.Cms.Admin.Features.Security;

namespace Osirion.Blazor.Cms.Admin.Features;

public static class FeaturesServiceRegistration
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddLayoutsFeature();
        services.AddSecurityFeature();
        services.AddDashboardFeature();
        services.AddNavigationFeature();

        return services;
    }
}