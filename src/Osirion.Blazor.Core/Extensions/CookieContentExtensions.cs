using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Handlers;

namespace Osirion.Blazor.Core.Extensions;

/// <summary>
/// Extension methods for configuring Osirion cookie consent
/// </summary>
public static class CookieContentExtensions
{
    /// <summary>
    /// Adds cookie consent services to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionCookieConsent(this IServiceCollection services)
    {
        // Add IHttpContextAccessor for SSR support
        services.AddHttpContextAccessor();

        return services;
    }

    /// <summary>
    /// Maps the cookie consent endpoint
    /// </summary>
    public static IEndpointRouteBuilder MapOsirionCookieConsent(
        this IEndpointRouteBuilder endpoints,
        string pattern = "/api/cookie-consent")
    {
        // Cast the route handler to Delegate to ensure the return value is written to the response
        endpoints.MapPost(pattern, (Delegate)CookieConsentHandler.HandleConsentAsync)
            .WithName("CookieConsent")
            .ExcludeFromDescription(); // Hide from API documentation

        return endpoints;
    }

    /// <summary>
    /// Adds cookie consent middleware for checking consent status
    /// </summary>
    public static IApplicationBuilder UseOsirionCookieConsent(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CookieConsentMiddleware>();
    }
}

/// <summary>
/// Middleware for handling cookie consent checks
/// </summary>
public class CookieConsentMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="CookieConsentMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next request delegate in the pipeline.</param>
    public CookieConsentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Processes the HTTP request to add cookie consent information.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Add consent status to HttpContext.Items for easy access
        var consentData = CookieConsentHandler.GetConsentData(context);
        context.Items["CookieConsent"] = consentData;

        // Set response headers based on consent
        if (consentData is not null)
        {
            // Example: Set feature policy based on consent
            var features = new List<string> { "camera 'none'", "microphone 'none'" };

            if (!CookieConsentHandler.IsCategoryConsented(context, "analytics"))
            {
                features.Add("geolocation 'none'");
            }

            context.Response.Headers["Permissions-Policy"] = string.Join("; ", features);
        }

        await _next(context);
    }
}
