using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Osirion.Blazor.Core.Handlers;

namespace Osirion.Blazor.Core.Extensions;

/// <summary>
/// Extension methods for the announcement bar.
/// </summary>
public static class AnnouncementExtensions
{
    /// <summary>The default path of the dismissal endpoint, which <c>OsirionAnnouncementBar</c> posts to.</summary>
    public const string DefaultDismissPattern = "/api/announcement/dismiss";

    /// <summary>
    /// Maps the endpoint that remembers a dismissed announcement in a cookie, so the bar can be closed without script.
    /// </summary>
    /// <remarks>
    /// Like the cookie consent endpoint it requires an antiforgery token, so call <c>app.UseAntiforgery()</c> before
    /// the endpoints, as the Blazor Web App template does. It only redirects to local paths.
    /// </remarks>
    /// <param name="endpoints">The application's endpoint route builder.</param>
    /// <param name="pattern">The endpoint's path; pass the same value to the bar's <c>DismissEndpoint</c>.</param>
    /// <returns>The same <paramref name="endpoints"/>, for chaining.</returns>
    public static IEndpointRouteBuilder MapOsirionAnnouncements(
        this IEndpointRouteBuilder endpoints,
        string pattern = DefaultDismissPattern)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapPost(pattern, (Delegate)AnnouncementHandler.HandleDismissAsync)
            .WithName("OsirionAnnouncementDismiss")
            .WithMetadata(new RequireAntiforgeryTokenAttribute())
            .ExcludeFromDescription();

        return endpoints;
    }
}
