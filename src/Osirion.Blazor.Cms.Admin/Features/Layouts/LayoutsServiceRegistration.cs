using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Admin.Features.Layouts;

/// <summary>Defines the LayoutsServiceRegistration type.</summary>
public static class LayoutsServiceRegistration
{
    /// <summary>Performs the AddLayoutsFeature operation.</summary>
    public static IServiceCollection AddLayoutsFeature(this IServiceCollection services)
    {
        // No specific services to register for layouts feature
        return services;
    }
}
