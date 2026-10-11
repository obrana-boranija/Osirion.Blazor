using System.Text;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Utils;

namespace Osirion.Blazor.Core.Handlers;

/// <summary>
/// Checks shared by the endpoints that receive the library's no-script form posts (cookie consent, announcement
/// dismissal).
/// </summary>
internal static class FormPostSecurity
{
    /// <summary>Gets whether the request carries a valid antiforgery token.</summary>
    public static async Task<bool> IsAntiforgeryTokenValidAsync(HttpContext context)
    {
        // When UseAntiforgery has validated the request, its verdict is authoritative, and reading
        // the form after a failed validation would throw.
        var validationFeature = context.Features.Get<IAntiforgeryValidationFeature>();
        if (validationFeature is not null)
        {
            return validationFeature.IsValid;
        }

        // The middleware did not run for this request: validate here and fail closed when
        // antiforgery services are not registered.
        var antiforgery = context.RequestServices?.GetService<IAntiforgery>();
        return antiforgery is not null && await antiforgery.IsRequestValidAsync(context);
    }

    /// <summary>
    /// Returns the posted return URL when it is a local path that can go in a <c>Location</c> header, otherwise
    /// <see cref="LocalUrl.DefaultPath"/>.
    /// </summary>
    public static string GetReturnPath(string? returnUrl)
    {
        var localPath = LocalUrl.GetSafeLocalPath(returnUrl);

        // Non-ASCII characters cannot be sent in a Location header; the forms always post an
        // escaped path, so anything else is not worth repairing.
        return Ascii.IsValid(localPath) ? localPath : LocalUrl.DefaultPath;
    }
}
