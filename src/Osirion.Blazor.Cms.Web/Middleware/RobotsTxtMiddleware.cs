using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Osirion.Blazor.Cms.Domain.Services;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Web.Middleware;

public static class RobotsTxtMiddlewareExtensions
{
    public static IApplicationBuilder UseOsirionRobotsGenerator(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RobotsTxtMiddleware>();
    }
}

public class RobotsTxtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IContentProviderManager _contentProviderManager;

    public RobotsTxtMiddleware(RequestDelegate next, IContentProviderManager contentProviderManager)
    {
        _next = next;
        _contentProviderManager = contentProviderManager;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/robots.txt"))
        {
            var result = await _contentProviderManager.GetContentByQueryAsync(new Cms.Domain.Repositories.ContentQuery { Url = "robots" });
            var output = result.FirstOrDefault()?.Content;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(output);
        }
        else
        {
            await _next(context);
        }
    }
}