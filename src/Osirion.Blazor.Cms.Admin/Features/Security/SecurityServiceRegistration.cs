using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Admin.Features.Security;

/// <summary>Registers the services that protect the CMS admin.</summary>
public static class SecurityServiceRegistration
{
    /// <summary>
    /// Adds authorization, the cascading authentication state and the default
    /// <see cref="CmsAdminAuthorization.PolicyName"/> policy.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    /// <remarks>
    /// The default policy requires an authenticated user. It is added after all other configuration has run, and
    /// only when no policy with that name exists, so a host policy wins whether it is registered before or after
    /// this call. No authentication scheme is added; that belongs to the host.
    /// </remarks>
    public static IServiceCollection AddSecurityFeature(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddCascadingAuthenticationState();

        services.PostConfigure<AuthorizationOptions>(options =>
        {
            if (options.GetPolicy(CmsAdminAuthorization.PolicyName) is null)
            {
                options.AddPolicy(CmsAdminAuthorization.PolicyName, policy => policy.RequireAuthenticatedUser());
            }
        });

        return services;
    }
}
