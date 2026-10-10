# Login

Purpose
Embeddable GitHub login form for the CMS admin: a personal access token field and, when `GitHubClientId` and `GitHubClientSecret` are configured, a GitHub OAuth button. It behaves like `LoginPage`: it never signs anyone in with the configured server personal access token, keeps a user's token in encrypted, user-bound browser storage, verifies the OAuth `state` and sends a PKCE challenge, and navigates only to a local `ReturnUrl`. Place it only on pages that require the `Osirion.Cms.Admin` authorization policy.

Parameters
- `Title`: string - Form title (default: "Osirion CMS Admin")
- `Description`: string - Text under the title (default: "Sign in to manage your content.")
- `ReturnUrl`: string - Local path to open after login (default: "/admin"). Anything that is not a local path is replaced by `/osirion`.
- `EnableGitHubAuth`: bool - Replaced by whether GitHub OAuth is configured
- `OnLoginResult`: EventCallback<bool> - Raised after a login attempt with `true` when it succeeded

Example

```razor
@attribute [Authorize(Policy = CmsAdminAuthorization.PolicyName)]

<Login ReturnUrl="/osirion/content" OnLoginResult="@HandleLoginResult" />

@code {
    private Task HandleLoginResult(bool succeeded)
    {
        return Task.CompletedTask;
    }
}
```
