# AuthGuard

Purpose
Shows its content only to users who satisfy an authorization policy, the CMS admin policy (`CmsAdminAuthorization.PolicyName`) by default. Other users see an "Authentication Required" message with a link to the sign-in page.

AuthGuard hides content while rendering; it does not protect an endpoint. Routed admin pages are protected by `[Authorize(Policy = CmsAdminAuthorization.PolicyName)]`. Put embedded admin components on pages that carry the same attribute. AuthGuard needs the cascading authentication state that `AddOsirionCmsAdmin` registers.

Parameters
- `ChildContent`: RenderFragment - Content shown to users who satisfy the policy
- `Policy`: string - Name of the authorization policy (default: `CmsAdminAuthorization.PolicyName`, `"Osirion.Cms.Admin"`)
- `LoginPath`: string - Local path of the sign-in page linked from the message (default: `/osirion/login`). The current page is passed as `returnUrl`. A value that is not a local path is replaced by `/osirion/login`.
- `ShowLoginForm`: bool - Obsolete, no effect. The GitHub login form cannot make a user satisfy the policy, so it is no longer shown. Setting it gives a compiler warning.
- `OnAuthResult`: EventCallback<bool> - Obsolete, never invoked, for the same reason.

Example

```razor
@using Osirion.Blazor.Cms.Admin.Features.Security
@using Osirion.Blazor.Cms.Admin.Features.Security.Components

<AuthGuard>
    <ContentEditor />
</AuthGuard>

<AuthGuard Policy="Editors" LoginPath="/account/login">
    <p>Visible to users who satisfy the Editors policy.</p>
</AuthGuard>
```
