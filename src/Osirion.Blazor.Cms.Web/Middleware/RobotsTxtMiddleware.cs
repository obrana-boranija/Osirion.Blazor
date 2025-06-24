using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Osirion.Blazor.Cms.Domain.Services;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Web.Middleware;

/// <summary>
/// 
/// </summary>
public static class RobotsTxtMiddlewareExtensions
{
    /// <summary>
    /// Registers the middleware to handle requests for robots.txt files in the application pipeline.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseOsirionRobotsGenerator(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RobotsTxtMiddleware>();
    }
}

/// <summary>
/// 
/// </summary>
public class RobotsTxtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IContentProviderManager _contentProviderManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="RobotsTxtMiddleware"/> class.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="contentProviderManager"></param>
    public RobotsTxtMiddleware(RequestDelegate next, IContentProviderManager contentProviderManager)
    {
        _next = next;
        _contentProviderManager = contentProviderManager;
    }

    /// <summary>
    /// Invokes the middleware to handle requests for robots.txt.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/robots.txt"))
        {
            var result = await _contentProviderManager.GetContentByQueryAsync(new Cms.Domain.Repositories.ContentQuery { Url = "robots" });
            var output = result.FirstOrDefault()?.Content;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(output ?? "User-agent: *  \r\nDisallow: /search/ ");
        }
        else
        {
            await _next(context);
        }
    }
}