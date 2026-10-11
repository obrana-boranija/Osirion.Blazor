using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Osirion.Blazor.Core.Handlers;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Handlers;

public class AnnouncementHandlerTests
{
    [Fact]
    public async Task HandleDismissAsync_ValidPost_SetsTheCookieAndRedirectsBack()
    {
        var context = CreateContext(new() { ["id"] = "release-4-1", ["days"] = "7", ["returnUrl"] = "/blog?page=2" });

        var result = await AnnouncementHandler.HandleDismissAsync(context);

        result.ShouldBeOfType<RedirectHttpResult>().Url.ShouldBe("/blog?page=2");
        var cookie = context.Response.Headers.SetCookie.ToString();
        cookie.ShouldStartWith("osirion_announcement_release-4-1=dismissed");
        cookie.ShouldContain("httponly", Case.Insensitive);
        cookie.ShouldContain("secure", Case.Insensitive);
        cookie.ShouldContain("samesite=lax", Case.Insensitive);
        cookie.ShouldContain("path=/");
    }

    [Fact]
    public async Task HandleDismissAsync_PlainHttp_SetsANonSecureCookieSoTheBrowserKeepsIt()
    {
        var context = CreateContext(new() { ["id"] = "news", ["returnUrl"] = "/" });
        context.Request.Scheme = "http";

        await AnnouncementHandler.HandleDismissAsync(context);

        context.Response.Headers.SetCookie.ToString().ShouldNotContain("secure", Case.Insensitive);
    }

    [Theory]
    [InlineData("https://evil.example")]
    [InlineData("//evil.example")]
    [InlineData("javascript:alert(1)")]
    public async Task HandleDismissAsync_NonLocalReturnUrl_RedirectsToRoot(string returnUrl)
    {
        var context = CreateContext(new() { ["id"] = "news", ["returnUrl"] = returnUrl });

        var result = await AnnouncementHandler.HandleDismissAsync(context);

        result.ShouldBeOfType<RedirectHttpResult>().Url.ShouldBe("/");
    }

    [Theory]
    [InlineData("")]
    [InlineData("Upper")]
    [InlineData("a;b")]
    [InlineData("x=dismissed; Path=/")]
    [InlineData("an-id-that-is-much-too-long-to-be-a-cookie-name-part")]
    public async Task HandleDismissAsync_InvalidId_IsRejectedWithoutACookie(string id)
    {
        var context = CreateContext(new() { ["id"] = id, ["returnUrl"] = "/" });

        var result = await AnnouncementHandler.HandleDismissAsync(context);

        result.ShouldBeOfType<BadRequest>();
        context.Response.Headers.SetCookie.Count.ShouldBe(0);
    }

    [Fact]
    public async Task HandleDismissAsync_InvalidAntiforgeryToken_IsRejected()
    {
        var context = CreateContext(new() { ["id"] = "news" }, antiforgeryValid: false);

        var result = await AnnouncementHandler.HandleDismissAsync(context);

        result.ShouldBeOfType<BadRequest>();
        context.Response.Headers.SetCookie.Count.ShouldBe(0);
    }

    [Fact]
    public void IsDismissed_ReadsTheCookieForThatIdOnly()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "osirion_announcement_news=dismissed";

        AnnouncementHandler.IsDismissed(context, "news").ShouldBeTrue();
        AnnouncementHandler.IsDismissed(context, "other").ShouldBeFalse();
    }

    private static DefaultHttpContext CreateContext(Dictionary<string, StringValues> form, bool antiforgeryValid = true)
    {
        var context = new DefaultHttpContext { RequestServices = new ServiceCollection().BuildServiceProvider() };
        context.Request.Method = HttpMethods.Post;
        context.Request.Scheme = "https";
        context.Request.ContentType = "application/x-www-form-urlencoded";
        context.Request.Form = new FormCollection(form);
        context.Features.Set<IAntiforgeryValidationFeature>(new AntiforgeryValidationFeature(antiforgeryValid));
        return context;
    }

    private sealed class AntiforgeryValidationFeature(bool isValid) : IAntiforgeryValidationFeature
    {
        public bool IsValid => isValid;

        public Exception? Error => isValid ? null : new AntiforgeryValidationException("Invalid token.");
    }
}
