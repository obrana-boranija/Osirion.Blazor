using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features.Security;
using Osirion.Blazor.Cms.Admin.Features.Security.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Security.Components;

public class AuthGuardTests : AdminComponentTestContext
{
    private readonly BunitAuthorizationContext _authorization;

    public AuthGuardTests()
    {
        _authorization = this.AddAuthorization();
        RenderInteractively();
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("osirion/content?path=a.md");
    }

    [Theory]
    [InlineData(typeof(AuthGuard), "ShowLoginForm")]
    [InlineData(typeof(AuthGuard), "OnAuthResult")]
    [InlineData(typeof(Osirion.Blazor.Cms.Admin.Features.Layouts.Components.AdminPage), "ShowLoginForm")]
    public void AuthGuard_LoginFormParameterWithoutEffect_IsMarkedObsolete(Type componentType, string parameterName)
    {
        // Proves a page that still sets these parameters gets a compiler warning instead of a silent no-op.
        var property = componentType.GetProperty(parameterName);

        property.ShouldNotBeNull();
        Attribute.IsDefined(property, typeof(ObsoleteAttribute)).ShouldBeTrue();
    }

    [Fact]
    public void AuthGuard_AnonymousUser_HidesChildContentAndLinksToLogin()
    {
        // Proves the guard no longer renders protected content unconditionally.
        _authorization.SetNotAuthorized();

        var cut = RenderGuard();

        cut.FindAll("#protected").ShouldBeEmpty();
        cut.Find("a.btn").GetAttribute("href").ShouldBe("/osirion/login?returnUrl=%2Fosirion%2Fcontent%3Fpath%3Da.md");
    }

    [Fact]
    public void AuthGuard_UserWithoutAdminPolicy_HidesChildContent()
    {
        _authorization.SetAuthorized("alice");

        var cut = RenderGuard();

        cut.FindAll("#protected").ShouldBeEmpty();
    }

    [Fact]
    public void AuthGuard_UserWithAdminPolicy_RendersChildContent()
    {
        _authorization.SetAuthorized("alice");
        _authorization.SetPolicies(CmsAdminAuthorization.PolicyName);

        var cut = RenderGuard();

        cut.Find("#protected").TextContent.ShouldBe("Protected");
    }

    [Fact]
    public void AuthGuard_CustomPolicy_UsesSuppliedPolicyName()
    {
        _authorization.SetAuthorized("alice");
        _authorization.SetPolicies(CmsAdminAuthorization.PolicyName);

        var cut = RenderGuard(parameters => parameters.Add(guard => guard.Policy, "Editors"));

        cut.FindAll("#protected").ShouldBeEmpty();
    }

    [Fact]
    public void AuthGuard_OffSiteLoginPath_LinksToAdminLogin()
    {
        _authorization.SetNotAuthorized();

        var cut = RenderGuard(parameters => parameters.Add(guard => guard.LoginPath, "//evil.example/login"));

        cut.Find("a.btn").GetAttribute("href").ShouldStartWith("/osirion/login?returnUrl=");
    }

    private IRenderedComponent<AuthGuard> RenderGuard(Action<ComponentParameterCollectionBuilder<AuthGuard>>? configure = null)
    {
        RenderFragment content = builder => builder.AddMarkupContent(0, "<p id=\"protected\">Protected</p>");

        return Render<AuthGuard>(parameters =>
        {
            parameters.Add(guard => guard.ChildContent, content);
            configure?.Invoke(parameters);
        });
    }
}
