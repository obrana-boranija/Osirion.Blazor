using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Admin.Features.Dashboard;

public static class DashboardServiceRegistration
{
    public static IServiceCollection AddDashboardFeature(this IServiceCollection services)
    {
        // No specific services to register for dashboard feature
        return services;
    }
}