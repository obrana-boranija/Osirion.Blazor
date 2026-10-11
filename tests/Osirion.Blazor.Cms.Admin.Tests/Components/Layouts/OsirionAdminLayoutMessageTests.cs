using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Components.Layouts;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Security;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Layouts;

public class OsirionAdminLayoutMessageTests : AdminComponentTestContext
{
    private readonly CmsState _state = new();

    public OsirionAdminLayoutMessageTests()
    {
        var authService = Substitute.For<IAuthenticationService>();
        authService.IsAuthenticated.Returns(true);
        authService.Username.Returns("octocat");
        Services.AddSingleton(authService);
        Services.AddSingleton(Substitute.For<IGitHubTokenStore>());
        Services.AddSingleton(Substitute.For<IGitHubSessionRestorer>());
        Services.AddSingleton(_state);
        var authorization = this.AddAuthorization();
        authorization.SetAuthorized("alice");
        authorization.SetPolicies(CmsAdminAuthorization.PolicyName);
        RenderInteractively();
    }

    private IRenderedComponent<OsirionAdminLayout> RenderLayout()
    {
        RenderFragment body = builder => builder.AddMarkupContent(0, "<p id=\"admin-body\">Admin content</p>");

        return Render<OsirionAdminLayout>(parameters => parameters.Add(layout => layout.Body, body));
    }

    [Fact]
    public void OsirionAdminLayout_StatusMessage_ShowsASuccessAlertThatDismisses()
    {
        _state.SetStatusMessage("Saved");
        var cut = RenderLayout();
        cut.Find(".alert-success").TextContent.ShouldContain("Saved");

        cut.Find(".alert-success .btn-close").Click();

        _state.StatusMessage.ShouldBeNull();
        cut.FindAll(".alert-success").ShouldBeEmpty();
    }

    [Fact]
    public void OsirionAdminLayout_ErrorMessage_ShowsAnErrorAlertThatDismisses()
    {
        _state.SetErrorMessage("Broken");
        var cut = RenderLayout();
        cut.Find(".alert-danger").TextContent.ShouldContain("Broken");

        cut.Find(".alert-danger .btn-close").Click();

        _state.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task OsirionAdminLayout_StateChange_RerendersTheLayout()
    {
        var cut = RenderLayout();

        await cut.InvokeAsync(() => _state.SetStatusMessage("Later"));

        cut.Find(".alert-success").TextContent.ShouldContain("Later");
    }
}
