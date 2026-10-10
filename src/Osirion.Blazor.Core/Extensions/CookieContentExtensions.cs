using Microsoft.AspNetCore.Antiforgery;
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
    /// <remarks>
    /// Registers <see cref="IHttpContextAccessor"/> and the antiforgery
    /// services the consent form and endpoint use. Both registrations are idempotent.
    /// </remarks>
    public static IServiceCollection AddOsirionCookieConsent(this IServiceCollection services)
    {
        // Add IHttpContextAccessor for SSR support
        services.AddHttpContextAccessor();

        // The consent form carries an antiforgery token that the endpoint validates
        services.AddAntiforgery();

        return services;
    }

    /// <summary>
    /// Maps the cookie consent endpoint
    /// </summary>
    /// <remarks>
    /// The endpoint carries antiforgery metadata, so the application must call
    /// <c>app.UseAntiforgery()</c> after <c>app.UseRouting()</c> (when the app calls it) and before
    /// the endpoints, as the Blazor Web App template does. Without that middleware ASP.NET Core throws
    /// <see cref="InvalidOperationException"/> for every post to the endpoint. Requests without a
    /// valid token get 400 and record no consent. The endpoint only redirects to local paths.
    /// </remarks>
    public static IEndpointRouteBuilder MapOsirionCookieConsent(
        this IEndpointRouteBuilder endpoints,
        string pattern = "/api/cookie-consent")
    {
        // Cast the route handler to Delegate to ensure the return value is written to the response
        endpoints.MapPost(pattern, (Delegate)CookieConsentHandler.HandleConsentAsync)
            .WithName("CookieConsent")
            .WithMetadata(new RequireAntiforgeryTokenAttribute())
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
/// Middleware that exposes the visitor consent data through <c>HttpContext.Items["CookieConsent"]</c>.
/// </summary>
/// <remarks>
/// Earlier versions also wrote a <c>Permissions-Policy</c> header (obsolete Feature-Policy syntax, overwriting any
/// host header, geolocation tied to analytics consent). Consent has no bearing on browser feature permissions, so
/// the header is no longer written; hosts that want one should set it themselves.
/// </remarks>
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

        await _next(context);
    }
}
