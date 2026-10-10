namespace Osirion.Blazor.Cms.Admin.Features.Security;

/// <summary>
/// The authorization policy that protects the CMS admin pages.
/// </summary>
/// <remarks>
/// <para>
/// Every routed admin page requires the policy named <see cref="PolicyName"/>, and the admin layout shows its
/// content only to users who satisfy it. <c>AddOsirionCmsAdmin</c> registers a default policy under that name
/// that requires an authenticated user, unless the host has already defined a policy with the same name.
/// </para>
/// <para>
/// The library never adds an authentication scheme. The host adds one (cookies, OpenID Connect and so on), calls
/// <c>UseAuthentication</c> and <c>UseAuthorization</c>, and, when the whole app is interactive, routes with
/// <c>AuthorizeRouteView</c>. Without an authentication scheme, requests to admin pages fail instead of rendering.
/// </para>
/// <para>
/// On a site where anyone can register, override the policy so that only editors are let in:
/// <code>
/// builder.Services.AddAuthorization(options =&gt;
///     options.AddPolicy(CmsAdminAuthorization.PolicyName, policy =&gt; policy.RequireRole("Editor")));
/// </code>
/// </para>
/// </remarks>
public static class CmsAdminAuthorization
{
    /// <summary>
    /// The name of the authorization policy that protects the CMS admin pages.
    /// </summary>
    public const string PolicyName = "Osirion.Cms.Admin";
}
