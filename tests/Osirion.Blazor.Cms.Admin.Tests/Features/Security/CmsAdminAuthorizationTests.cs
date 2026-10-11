using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Components.Layouts;
using Osirion.Blazor.Cms.Admin.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features.Security;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;
using System.Reflection;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Security;

public class CmsAdminAuthorizationTests
{
    public static TheoryData<Type> RoutableAdminComponents()
    {
        var data = new TheoryData<Type>();
        foreach (var type in typeof(_Imports).Assembly.GetTypes().Where(type => type.GetCustomAttributes<RouteAttribute>().Any()))
        {
            data.Add(type);
        }

        return data;
    }

    [Fact]
    public void AdminAssembly_HasRoutableComponents()
    {
        RoutableAdminComponents().Count.ShouldBeGreaterThanOrEqualTo(4);
    }

    [Theory]
    [MemberData(nameof(RoutableAdminComponents))]
    public void AdminPage_RequiresAdminPolicy(Type pageType)
    {
        // Proves every routed admin page, including any page added later, is deny by default.
        var authorize = pageType.GetCustomAttributes<AuthorizeAttribute>(inherit: true).ShouldHaveSingleItem();

        authorize.Policy.ShouldBe(CmsAdminAuthorization.PolicyName);
        pageType.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(RoutableAdminComponents))]
    public void AdminPage_UsesAdminLayout(Type pageType)
    {
        // The layout's authorization backstop covers only pages that use it.
        pageType.GetCustomAttribute<LayoutAttribute>(inherit: true)
            .ShouldNotBeNull()
            .LayoutType.ShouldBe(typeof(OsirionAdminLayout));
    }

    [Fact]
    public async Task AddOsirionCmsAdmin_RegistersPolicyThatRequiresAuthenticatedUser()
    {
        using var provider = BuildProvider();

        var policy = await provider.GetRequiredService<IAuthorizationPolicyProvider>()
            .GetPolicyAsync(CmsAdminAuthorization.PolicyName);

        policy.ShouldNotBeNull();
        policy.Requirements.ShouldHaveSingleItem().ShouldBeOfType<DenyAnonymousAuthorizationRequirement>();
    }

    [Fact]
    public async Task AddOsirionCmsAdmin_HostPolicyRegisteredBefore_HostPolicyWins()
    {
        using var provider = BuildProvider(beforeLibrary: services =>
            services.AddAuthorization(options => options.AddPolicy(CmsAdminAuthorization.PolicyName, policy => policy.RequireRole("Editor"))));

        var policy = await provider.GetRequiredService<IAuthorizationPolicyProvider>()
            .GetPolicyAsync(CmsAdminAuthorization.PolicyName);

        policy.ShouldNotBeNull().Requirements.ShouldHaveSingleItem().ShouldBeOfType<RolesAuthorizationRequirement>();
    }

    [Fact]
    public async Task AddOsirionCmsAdmin_HostPolicyRegisteredAfter_HostPolicyWins()
    {
        using var provider = BuildProvider(afterLibrary: services =>
            services.AddAuthorizationBuilder().AddPolicy(CmsAdminAuthorization.PolicyName, policy => policy.RequireRole("Editor")));

        var policy = await provider.GetRequiredService<IAuthorizationPolicyProvider>()
            .GetPolicyAsync(CmsAdminAuthorization.PolicyName);

        policy.ShouldNotBeNull().Requirements.ShouldHaveSingleItem().ShouldBeOfType<RolesAuthorizationRequirement>();
    }

    [Fact]
    public async Task AdminPolicy_AnonymousUser_IsDenied()
    {
        using var provider = BuildProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();

        var result = await authorizationService.AuthorizeAsync(
            TestAuthenticationStateProvider.CreateAnonymousUser(), CmsAdminAuthorization.PolicyName);

        result.Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task AdminPolicy_AuthenticatedUser_IsAllowed()
    {
        using var provider = BuildProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();

        var result = await authorizationService.AuthorizeAsync(
            TestAuthenticationStateProvider.CreateUser("alice-id", "alice"), CmsAdminAuthorization.PolicyName);

        result.Succeeded.ShouldBeTrue();
    }

    private static ServiceProvider BuildProvider(
        Action<IServiceCollection>? beforeLibrary = null,
        Action<IServiceCollection>? afterLibrary = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Osirion:Cms:Admin:DefaultContentProvider"] = "GitHub" })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();

        beforeLibrary?.Invoke(services);
        services.AddOsirionCmsAdmin(configuration);
        afterLibrary?.Invoke(services);

        return services.BuildServiceProvider();
    }
}
