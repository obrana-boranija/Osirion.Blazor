using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Admin.Features.Security;

public static class SecurityServiceRegistration
{
    public static IServiceCollection AddSecurityFeature(this IServiceCollection services)
    {
        // No specific services to register for security feature
        return services;
    }
}