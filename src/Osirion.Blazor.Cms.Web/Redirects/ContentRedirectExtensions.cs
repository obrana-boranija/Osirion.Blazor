using Microsoft.AspNetCore.Builder;

namespace Osirion.Blazor.Cms.Web.Redirects;

/// <summary>
/// Adds permanent redirects from the earlier URLs of content to its current URL.
/// </summary>
public static class ContentRedirectExtensions
{
    /// <summary>
    /// Answers GET and HEAD requests for an old URL with a redirect (301 by default) to the page's current URL. Old URLs
    /// come from the <c>redirect_from</c> front matter of published content and from
    /// <see cref="OsirionRedirectOptions.Redirects"/>. Place it before the endpoints that render pages.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configure">Adds fixed redirects and changes how content URLs are built.</param>
    /// <returns>The same <paramref name="app"/>, for chaining.</returns>
    /// <exception cref="ArgumentException">A fixed redirect is not valid; see <see cref="OsirionRedirectOptions.Validate"/>.</exception>
    /// <example>
    /// <code>
    /// app.UseOsirionRedirects(redirects =&gt;
    /// {
    ///     redirects.Redirects["/docs/components/core"] = "/components";
    /// });
    /// </code>
    /// </example>
    public static IApplicationBuilder UseOsirionRedirects(this IApplicationBuilder app, Action<OsirionRedirectOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = new OsirionRedirectOptions();
        configure?.Invoke(options);
        options.Validate();

        return app.UseMiddleware<ContentRedirectMiddleware>(options);
    }
}
