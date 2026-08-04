using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Admin.Features.Navigation;

/// <summary>Defines the NavigationServiceRegistration type.</summary>
public static class NavigationServiceRegistration
{
    /// <summary>Performs the AddNavigationFeature operation.</summary>
    public static IServiceCollection AddNavigationFeature(this IServiceCollection services)
    {
        // No specific services to register for navigation feature
        return services;
    }
}
