using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Osirion.Blazor.Core.Handlers;

/// <summary>
/// Handles cookie consent form submissions for SSR
/// </summary>
public static class CookieConsentHandler
{
    private const string ConsentCookieName = "osirion_cookie_consent";
    private const string ConsentCookieVersion = "1.0";

    /// <summary>
    /// Processes cookie consent form submission
    /// </summary>
    public static async Task<IResult> HandleConsentAsync(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();
        var consentValue = form["consent"].FirstOrDefault();
        var returnUrl = form["returnUrl"].FirstOrDefault() ?? "/";

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
                return Results.Redirect($"{returnUrl}?customize-cookies=true");

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
    public string Version { get; set; } = string.Empty;
    public DateTime ConsentDate { get; set; }
    public string ConsentType { get; set; } = string.Empty;
    public Dictionary<string, bool> Categories { get; set; } = new();
}