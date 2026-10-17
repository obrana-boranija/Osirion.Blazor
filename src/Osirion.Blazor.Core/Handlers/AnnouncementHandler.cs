using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Osirion.Blazor.Core.Handlers;

/// <summary>
/// Remembers that a visitor dismissed an announcement, for the no-script form <c>OsirionAnnouncementBar</c> posts.
/// </summary>
public static partial class AnnouncementHandler
{
    /// <summary>The prefix of the cookie that records a dismissed announcement; the announcement's ID follows it.</summary>
    public const string CookiePrefix = "osirion_announcement_";

    private const string DismissedValue = "dismissed";
    private const int DefaultDays = 30;
    private const int MaxDays = 365;

    /// <summary>
    /// Handles the dismissal form: checks the antiforgery token and the announcement ID, sets the cookie and redirects
    /// back to the posted local path.
    /// </summary>
    /// <param name="context">The request.</param>
    /// <returns>A redirect, or 400 Bad Request for an invalid post.</returns>
    public static async Task<IResult> HandleDismissAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!await FormPostSecurity.IsAntiforgeryTokenValidAsync(context) || !context.Request.HasFormContentType)
        {
            return Results.BadRequest();
        }

        var form = await context.Request.ReadFormAsync(context.RequestAborted);
        var id = form["id"].FirstOrDefault();
        if (!IsValidId(id))
        {
            return Results.BadRequest();
        }

        context.Response.Cookies.Append(CookiePrefix + id, DismissedValue, new CookieOptions
        {
            HttpOnly = true,
            // Always Secure: behind a proxy that ends TLS the request can reach the app as HTTP, and a flag taken from
            // the request would then be dropped on an HTTPS site. Browsers treat localhost as secure.
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(GetDays(form["days"].FirstOrDefault())),
            IsEssential = true,
        });

        return Results.Redirect(FormPostSecurity.GetReturnPath(form["returnUrl"].FirstOrDefault()));
    }

    /// <summary>Gets whether the visitor dismissed the announcement with ID <paramref name="id"/>.</summary>
    /// <param name="context">The request.</param>
    /// <param name="id">The announcement's ID.</param>
    public static bool IsDismissed(HttpContext context, string id)
    {
        ArgumentNullException.ThrowIfNull(context);
        return IsValidId(id) && context.Request.Cookies[CookiePrefix + id] == DismissedValue;
    }

    /// <summary>
    /// Gets whether <paramref name="id"/> can name an announcement: 1 to 40 lowercase letters, digits or hyphens, so
    /// it is safe in a cookie name.
    /// </summary>
    public static bool IsValidId(string? id) => id is not null && IdPattern().IsMatch(id);

    // The visitor can only change the lifetime of their own cookie, so a bounded value is accepted.
    private static int GetDays(string? posted) =>
        int.TryParse(posted, NumberStyles.None, CultureInfo.InvariantCulture, out var days) && days is >= 1 and <= MaxDays
            ? days
            : DefaultDays;

    [GeneratedRegex("^[a-z0-9-]{1,40}$", RegexOptions.CultureInvariant)]
    private static partial Regex IdPattern();
}
