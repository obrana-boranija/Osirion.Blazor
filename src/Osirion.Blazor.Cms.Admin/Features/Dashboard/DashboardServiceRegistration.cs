using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Admin.Features.Dashboard;

/// <summary>Defines the DashboardServiceRegistration type.</summary>
public static class DashboardServiceRegistration
{
    /// <summary>Performs the AddDashboardFeature operation.</summary>
    public static IServiceCollection AddDashboardFeature(this IServiceCollection services)
    {
        // No specific services to register for dashboard feature
        return services;
    }
}
