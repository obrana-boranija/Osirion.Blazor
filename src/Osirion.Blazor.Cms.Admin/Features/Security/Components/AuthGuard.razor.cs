using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Core.Utils;

namespace Osirion.Blazor.Cms.Admin.Features.Security.Components;

/// <summary>
/// Shows its content only to users who satisfy an authorization policy, the CMS admin policy by default.
/// </summary>
/// <remarks>
/// AuthGuard hides content while rendering; it does not protect an endpoint. Routed admin pages are protected by
/// <c>[Authorize(Policy = CmsAdminAuthorization.PolicyName)]</c>, so put embedded admin components on pages that
/// carry the same attribute. AuthGuard needs the cascading authentication state that <c>AddOsirionCmsAdmin</c>
/// registers. Users who do not satisfy the policy see a link to <see cref="LoginPath"/>.
/// </remarks>
public partial class AuthGuard
{
    /// <summary>Gets or sets the content shown to users who satisfy <see cref="Policy"/>.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the name of the authorization policy the user must satisfy.
    /// Defaults to <see cref="CmsAdminAuthorization.PolicyName"/>.
    /// </summary>
    [Parameter]
    public string Policy { get; set; } = CmsAdminAuthorization.PolicyName;

    /// <summary>
    /// Gets or sets the local path of the sign-in page linked for users who do not satisfy <see cref="Policy"/>.
    /// A value that is not a local path is replaced by the admin login page.
    /// </summary>
    [Parameter]
    public string LoginPath { get; set; } = "/osirion/login";

    /// <summary>
    /// Gets or sets the ShowLoginForm value. It has no effect: the GitHub login form cannot make a user satisfy the
    /// authorization policy, so it is no longer shown in place of the protected content.
    /// </summary>
    [Parameter]
    public bool ShowLoginForm { get; set; } = false;

    /// <summary>
    /// Gets or sets the OnAuthResult value. It is never invoked, because the GitHub login form is no longer shown.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OnAuthResult { get; set; }

    private string LoginUrl
    {
        get
        {
            var loginPath = LocalUrl.GetSafeLocalPath(LoginPath, CmsConstants.Routes.Login);
            var returnUrl = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri);

            return $"{loginPath}?returnUrl={Uri.EscapeDataString(returnUrl)}";
        }
    }
}
