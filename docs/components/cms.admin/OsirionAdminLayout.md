# OsirionAdminLayout

Purpose
Root layout used by admin pages to compose header, navigation and footer.

The layout renders the page body only for users who satisfy the `Osirion.Cms.Admin` authorization policy (`CmsAdminAuthorization.PolicyName`); other users see a "not authorized" message. This is a backstop for hosts whose router does not check `[Authorize]` during navigation inside a circuit. The admin pages themselves require the policy at the endpoint. Signing out from the navigation menu also removes the user's stored GitHub token.

Parameters
- `Body`: RenderFragment - Main page content

Example

```razor
@layout OsirionAdminLayout
@attribute [Authorize(Policy = CmsAdminAuthorization.PolicyName)]

<h1>Admin page</h1>
```
