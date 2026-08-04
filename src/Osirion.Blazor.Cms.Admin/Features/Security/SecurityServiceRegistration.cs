using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Admin.Features.Security;

/// <summary>Defines the SecurityServiceRegistration type.</summary>
public static class SecurityServiceRegistration
{
    /// <summary>Performs the AddSecurityFeature operation.</summary>
    public static IServiceCollection AddSecurityFeature(this IServiceCollection services)
    {
        // No specific services to register for security feature
        return services;
    }
}
