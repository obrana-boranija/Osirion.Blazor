using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Utils;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Core.Handlers;

/// <summary>
/// Handles cookie consent form submissions for SSR
/// </summary>
public static class CookieConsentHandler
{
    private const string ConsentCookieName = "osirion_cookie_consent";
    private const string ConsentCookieVersion = "1.0";
    private const string CustomizeQueryKey = "customize-cookies";

    /// <summary>
    /// Processes cookie consent form submission
    /// </summary>
    /// <remarks>
    /// The request must be a form post with a valid antiforgery token; otherwise the handler returns
    /// 400 without a body and records nothing. The posted <c>returnUrl</c> is followed only when it
    /// is a local path (see <see cref="LocalUrl.IsLocalPath(string?)"/>); any other value redirects
    /// to <c>/</c>. A <c>customize-cookies</c> query key left on the return URL is removed.
    /// </remarks>
    public static async Task<IResult> HandleConsentAsync(HttpContext context)
    {
        if (!await IsAntiforgeryTokenValidAsync(context) || !context.Request.HasFormContentType)
        {
            return Results.BadRequest();
        }

        var form = await context.Request.ReadFormAsync(context.RequestAborted);
        var consentValue = form["consent"].FirstOrDefault();
        var returnUrl = GetReturnPath(form["returnUrl"].FirstOrDefault());

        var consentData = new CookieConsentData
        {
            Version = ConsentCookieVersion,
            ConsentDate = DateTime.UtcNow,
            ConsentType = consentValue ?? "declined"
        };

        // Handle different consent types
        switch (consentValue)
        {
            case "accepted":
                consentData.Categories = new Dictionary<string, bool>
                {
                    ["necessary"] = true,
                    ["analytics"] = true,
                    ["marketing"] = true,
                    ["preferences"] = true
                };
                break;

            case "declined":
                consentData.Categories = new Dictionary<string, bool>
                {
                    ["necessary"] = true,
                    ["analytics"] = false,
                    ["marketing"] = false,
                    ["preferences"] = false
                };
                break;

            case "customize":
                // Redirect to the same URL with customize parameter
                return Results.Redirect(QueryHelpers.AddQueryString(returnUrl, CustomizeQueryKey, "true"));

            case "save-preferences":
                // Process custom preferences
                consentData.Categories = new Dictionary<string, bool>
                {
                    ["necessary"] = true // Always true
                };

                foreach (var key in form.Keys.Where(k => k.StartsWith("category_")))
                {
                    var categoryId = key.Replace("category_", "");
                    consentData.Categories[categoryId] = form[key] == "true";
                }
                break;

            default:
                return Results.BadRequest("Invalid consent value");
        }

        // Set the consent cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(365),
            Path = "/"
        };

        var consentJson = JsonSerializer.Serialize(consentData);
        context.Response.Cookies.Append(ConsentCookieName, consentJson, cookieOptions);

        // Redirect back to the original page
        return Results.Redirect(returnUrl);
    }

    private static async Task<bool> IsAntiforgeryTokenValidAsync(HttpContext context)
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

    private static string GetReturnPath(string? returnUrl)
    {
        var localPath = LocalUrl.GetSafeLocalPath(returnUrl);

        // Non-ASCII characters cannot be sent in a Location header; the banner always posts an
        // escaped path, so anything else is not worth repairing.
        return Ascii.IsValid(localPath) ? RemoveCustomizeFlag(localPath) : LocalUrl.DefaultPath;
    }

    private static string RemoveCustomizeFlag(string localPath)
    {
        var fragmentStart = localPath.IndexOf('#');
        var pathAndQuery = fragmentStart < 0 ? localPath : localPath[..fragmentStart];
        var fragment = fragmentStart < 0 ? string.Empty : localPath[fragmentStart..];

        var queryStart = pathAndQuery.IndexOf('?');
        if (queryStart < 0)
        {
            return localPath;
        }

        var parameters = QueryHelpers.ParseQuery(pathAndQuery[queryStart..]);
        if (!parameters.Remove(CustomizeQueryKey))
        {
            return localPath;
        }

        return QueryHelpers.AddQueryString(pathAndQuery[..queryStart], parameters) + fragment;
    }

    /// <summary>
    /// Gets the current cookie consent status
    /// </summary>
    public static CookieConsentData? GetConsentData(HttpContext context)
    {
        var consentCookie = context.Request.Cookies[ConsentCookieName];
        if (string.IsNullOrWhiteSpace(consentCookie))
            return null;

        try
        {
            return JsonSerializer.Deserialize<CookieConsentData>(consentCookie);
        }
        catch
        {
            // Invalid cookie data
            return null;
        }
    }

    /// <summary>
    /// Checks if a specific cookie category is consented to
    /// </summary>
    public static bool IsCategoryConsented(HttpContext context, string category)
    {
        var consentData = GetConsentData(context);
        if (consentData is null)
            return false;

        return consentData.Categories.TryGetValue(category, out var consented) && consented;
    }

    /// <summary>
    /// Clears the consent cookie
    /// </summary>
    public static void ClearConsent(HttpContext context)
    {
        context.Response.Cookies.Delete(ConsentCookieName);
    }
}

/// <summary>
/// Cookie consent data model
/// </summary>
public class CookieConsentData
{
    /// <summary>Gets or sets the consent data version.</summary>
    public string Version { get; set; } = string.Empty;
    /// <summary>Gets or sets when consent was recorded.</summary>
    public DateTime ConsentDate { get; set; }
    /// <summary>Gets or sets the consent type.</summary>
    public string ConsentType { get; set; } = string.Empty;
    /// <summary>Gets or sets the consent state for each category.</summary>
    public Dictionary<string, bool> Categories { get; set; } = new();
}
