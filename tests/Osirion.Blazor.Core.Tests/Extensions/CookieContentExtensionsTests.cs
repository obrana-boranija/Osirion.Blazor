using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Extensions;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Extensions;

public class CookieContentExtensionsTests
{
    [Fact]
    public void AddOsirionCookieConsent_RegistersAntiforgeryServices()
    {
        var services = new ServiceCollection();

        services.AddOsirionCookieConsent();

        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IAntiforgery));
    }

    [Fact]
    public void AddOsirionCookieConsent_RegistersHttpContextAccessor()
    {
        var services = new ServiceCollection();

        services.AddOsirionCookieConsent();

        services.ShouldContain(descriptor => descriptor.ServiceType == typeof(IHttpContextAccessor));
    }

    [Theory]
    [InlineData(null, "/api/cookie-consent")]
    [InlineData("/consent", "/consent")]
    public async Task MapOsirionCookieConsent_Endpoint_RequiresAntiforgeryValidation(string? pattern, string expectedRoute)
    {
        await using var app = WebApplication.CreateSlimBuilder().Build();

        if (pattern is null)
        {
            app.MapOsirionCookieConsent();
        }
        else
        {
            app.MapOsirionCookieConsent(pattern);
        }

        var endpoint = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Single(candidate => candidate.RoutePattern.RawText == expectedRoute);

        endpoint.Metadata.GetMetadata<IAntiforgeryMetadata>().ShouldNotBeNull().RequiresValidation.ShouldBeTrue();
    }
}
