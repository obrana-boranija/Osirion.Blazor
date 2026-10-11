using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Extensions;
using Shouldly;

namespace Osirion.Blazor.Tests.Integration;

/// <summary>
/// Builds a host the way a site with only static server rendering does, in the Development environment, where
/// <see cref="WebApplicationBuilder"/> validates every service registration on build.
/// </summary>
public class StaticHostStartupTests
{
    [Fact]
    public async Task AddOsirion_WithCmsAdminSectionInStaticDevelopmentHost_Builds()
    {
        // Proves a static SSR site with an Osirion:Cms:Admin section still starts: the protected browser storage
        // behind the admin's GitHub credential stores exists only with interactive server components.
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(StaticHostStartupTests).Assembly.GetName().Name,
            ContentRootPath = AppContext.BaseDirectory,
            EnvironmentName = Environments.Development
        });
        builder.Logging.ClearProviders();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Osirion:Cms:Web:GitHub:Blog:Owner"] = "o",
            ["Osirion:Cms:Web:GitHub:Blog:Repository"] = "r",
            ["Osirion:Cms:Web:GitHub:Blog:Branch"] = "main",
            ["Osirion:Cms:Web:GitHub:Blog:IsDefault"] = "true",
            ["Osirion:Cms:Admin:GitHub:Blog:Owner"] = "o"
        });
        builder.Services.AddRazorComponents();
        builder.Services.AddOsirion(builder.Configuration);

        WebApplication? app = null;
        try
        {
            Should.NotThrow(() => app = builder.Build());
        }
        finally
        {
            if (app is not null)
            {
                await app.DisposeAsync();
            }
        }
    }
}
