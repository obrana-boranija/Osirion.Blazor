using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Components;
using Osirion.Blazor.Cms.Admin.Features.Navigation.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Layouts.Components;

// Before the fix AdminLayout declared a second "Theme" parameter next to OsirionComponentBase.Theme, so Blazor threw
// on its first render and the sign-out below could never run.
public class AdminLayoutTests : AdminComponentTestContext
{
    private readonly IAuthenticationService _authService = Substitute.For<IAuthenticationService>();
    private readonly IGitHubTokenStore _tokenStore = Substitute.For<IGitHubTokenStore>();

    public AdminLayoutTests()
    {
        Services.AddSingleton(_authService);
        Services.AddSingleton(_tokenStore);
        ComponentFactories.AddStub<DefaultNavigation>();
        RenderInteractively();
    }

    [Fact]
    public void AdminLayout_Render_ShowsTitleAndChildContent()
    {
        var cut = RenderComponent<AdminLayout>(parameters => parameters
            .Add(p => p.Title, "Content admin")
            .AddChildContent("<p id=\"page-body\">Body</p>"));

        cut.Find(".sidebar-brand-text").TextContent.ShouldBe("Content admin");
        cut.Find("#page-body").TextContent.ShouldBe("Body");
    }

    [Fact]
    public void AdminLayout_SignOut_RemovesStoredGitHubToken()
    {
        _authService.IsAuthenticated.Returns(true);
        _authService.Username.Returns("alice");
        var cut = RenderComponent<AdminLayout>();

        cut.Find(".sidebar-footer button").Click();

        _authService.Received(1).SignOutAsync();
        _tokenStore.Received(1).RemoveTokenAsync();
    }
}
