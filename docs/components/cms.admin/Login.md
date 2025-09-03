# Login

Purpose
Login form for the CMS admin interface.

Parameters
- `OnLogin`: EventCallback<LoginModel> - Login event handler
- `Title`: string - Form title (default: "Admin Login")
- `UsernamePlaceholder`: string - Username placeholder
- `PasswordPlaceholder`: string - Password placeholder
- `LoginButtonText`: string - Login button text (default: "Login")

Example

```razor
<Login OnLogin="@HandleLogin" />

@code {
    private async Task HandleLogin(LoginModel model)
    {
        // authenticate
    }
}
```
