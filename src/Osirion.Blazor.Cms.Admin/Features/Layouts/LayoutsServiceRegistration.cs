using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Admin.Features.Layouts;

public static class LayoutsServiceRegistration
{
    public static IServiceCollection AddLayoutsFeature(this IServiceCollection services)
    {
        // No specific services to register for layouts feature
        return services;
    }
}