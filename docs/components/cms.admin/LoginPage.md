# LoginPage

Purpose
Admin page at `/osirion/login` where a user connects the admin to GitHub, with a personal access token or with GitHub OAuth. It is not a host sign-in page: like every admin page it requires the `Osirion.Cms.Admin` authorization policy, so the user must already be signed in to the host.

Behaviour
- It never signs anyone in with the server credential `Osirion:Cms:Admin:Authentication:PersonalAccessToken`. When one is configured, the page says so and offers a link to continue; admin services then use it on the server.
- After a successful login the user's token is kept in encrypted, user-bound browser storage, and the user returns to `returnUrl`.
- GitHub OAuth sends a single-use `state` and a PKCE S256 challenge. A callback is accepted only when its `state` matches the one stored for that browser tab, and the code is exchanged only after the page becomes interactive.
- The default OAuth redirect URI is this page (`{BaseUri}osirion/login`) unless `GitHubRedirectUri` is configured.

Parameters
- `ReturnUrl`: string - From the `returnUrl` query value. Only a local path such as `/osirion/content` is used; anything else is replaced by `/osirion`.

Example

```text
/osirion/login?returnUrl=%2Fosirion%2Fcontent
```
