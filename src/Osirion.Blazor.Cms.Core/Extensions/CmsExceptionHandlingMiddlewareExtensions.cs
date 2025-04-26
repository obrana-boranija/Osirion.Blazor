using Microsoft.AspNetCore.Builder;
using Osirion.Blazor.Cms.Middleware;

namespace Osirion.Blazor.Cms.Core.Extensions;

/// <summary>
/// Extends IApplicationBuilder to add the CMS exception handling middleware
/// </summary>
public static class CmsExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adds CMS exception handling middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseCmsExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CmsExceptionHandlingMiddleware>();
    }
}