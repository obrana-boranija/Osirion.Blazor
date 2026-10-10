using Microsoft.AspNetCore.Http;
using Osirion.Blazor.Core.Extensions;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Extensions;

public class CookieConsentMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_DoesNotOverwritePermissionsPolicyHeaderSetByTheHost()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "osirion_cookie_consent=%7B%7D";
        context.Response.Headers["Permissions-Policy"] = "camera=(self)";
        var middleware = new CookieConsentMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        context.Response.Headers["Permissions-Policy"].ToString().ShouldBe("camera=(self)");
    }

    [Fact]
    public async Task InvokeAsync_WithoutConsentCookie_DoesNotAddPermissionsPolicyHeader()
    {
        var context = new DefaultHttpContext();
        var middleware = new CookieConsentMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        context.Response.Headers.ContainsKey("Permissions-Policy").ShouldBeFalse();
    }
}
