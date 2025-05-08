using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Admin.Features.Navigation;

public static class NavigationServiceRegistration
{
    public static IServiceCollection AddNavigationFeature(this IServiceCollection services)
    {
        // No specific services to register for navigation feature
        return services;
    }
}