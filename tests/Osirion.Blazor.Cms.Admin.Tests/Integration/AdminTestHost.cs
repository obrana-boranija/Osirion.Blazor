using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Security.Claims;
using System.Text.Encodings.Web;
using IGitHubAuthenticationService = Osirion.Blazor.Cms.Domain.Interfaces.IAuthenticationService;

namespace Osirion.Blazor.Cms.Admin.Tests.Integration;

/// <summary>
/// Starts an in-memory Blazor host that maps the admin pages the way a real host does, with a header-based test
/// authentication scheme standing in for the host's own sign-in.
/// </summary>
internal static class AdminTestHost
{
    public const string UserHeader = "X-Test-User";
    public const string RoleHeader = "X-Test-Role";
    public const string PublicRoute = "/public";
    public const string PublicResponse = "public page";

    /// <summary>
    /// Starts a host with an authentication scheme and explicit <c>UseAuthentication</c> and <c>UseAuthorization</c>.
    /// </summary>
    public static async Task<WebApplication> StartAsync(Action<IServiceCollection>? configureServices = null)
    {
        var builder = CreateBuilder();
        builder.Services
            .AddAuthentication(TestAuthenticationHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.SchemeName, configureOptions: null);
        configureServices?.Invoke(builder.Services);

        var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        MapEndpoints(app);

        await app.StartAsync();
        return app;
    }

    /// <summary>
    /// Starts a host shaped like a public site that never configured authentication: no scheme, an explicit
    /// <c>UseRouting</c> and no <c>UseAuthorization</c> call.
    /// </summary>
    public static async Task<WebApplication> StartWithoutAuthenticationAsync()
    {
        var app = CreateBuilder().Build();
        app.UseRouting();
        MapEndpoints(app);

        await app.StartAsync();
        return app;
    }

    private static WebApplicationBuilder CreateBuilder()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(AdminTestHost).Assembly.GetName().Name,
            ContentRootPath = AppContext.BaseDirectory,
            EnvironmentName = Environments.Production
        });

        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddOsirionCmsAdmin(builder.Configuration);

        // Doubles for the GitHub services the login page needs, which a real host registers with a content provider
        builder.Services.AddScoped(_ => Substitute.For<IGitHubAuthenticationService>());
        builder.Services.AddScoped(_ => Substitute.For<IGitHubTokenProvider>());
        builder.Services.AddScoped(_ => Substitute.For<IStateStorageService>());
        builder.Services.AddScoped(_ => Substitute.For<IContentRepositoryAdapter>());

        return builder;
    }

    private static void MapEndpoints(WebApplication app)
    {
        app.UseAntiforgery();
        app.MapGet(PublicRoute, () => PublicResponse);
        app.MapRazorComponents<TestApp>()
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(_Imports).Assembly);
    }

    /// <summary>
    /// Signs in the user named in the <see cref="UserHeader"/> request header, with the role in <see cref="RoleHeader"/>.
    /// Without the header the request stays anonymous and a challenge answers 401.
    /// </summary>
    private sealed class TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string SchemeName = "Test";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userName = Request.Headers[UserHeader].ToString();
            if (string.IsNullOrWhiteSpace(userName))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userName),
                new(ClaimTypes.Name, userName)
            };

            var role = Request.Headers[RoleHeader].ToString();
            if (!string.IsNullOrWhiteSpace(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
        }
    }

    /// <summary>
    /// The host's root component: a router over the admin assembly, as in the example app.
    /// </summary>
    private sealed class TestApp : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            RenderFragment<RouteData> found = routeData => routeBuilder =>
            {
                routeBuilder.OpenComponent<RouteView>(0);
                routeBuilder.AddComponentParameter(1, nameof(RouteView.RouteData), routeData);
                routeBuilder.CloseComponent();
            };

            builder.OpenComponent<Router>(0);
            builder.AddComponentParameter(1, nameof(Router.AppAssembly), typeof(TestApp).Assembly);
            builder.AddComponentParameter(2, nameof(Router.AdditionalAssemblies), new[] { typeof(_Imports).Assembly });
            builder.AddComponentParameter(3, nameof(Router.Found), found);
            builder.CloseComponent();
        }
    }
}
