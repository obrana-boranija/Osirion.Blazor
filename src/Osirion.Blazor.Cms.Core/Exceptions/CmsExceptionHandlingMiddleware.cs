using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Core.Exceptions;
using Osirion.Blazor.Cms.Exceptions;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Middleware;

/// <summary>
/// Middleware to handle exceptions for CMS-related endpoints
/// </summary>
public class CmsExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CmsExceptionHandlingMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CmsExceptionHandlingMiddleware"/> class.
    /// </summary>
    public CmsExceptionHandlingMiddleware(RequestDelegate next, ILogger<CmsExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = GetStatusCode(exception);

        var response = new ExceptionResponse
        {
            StatusCode = context.Response.StatusCode,
            Message = GetErrorMessage(exception),
            TraceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }

    private int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ContentItemNotFoundException => StatusCodes.Status404NotFound,
            ContentValidationException => StatusCodes.Status422UnprocessableEntity,
            ContentAuthorizationException => StatusCodes.Status403Forbidden,
            Exceptions.DirectoryNotFoundException => StatusCodes.Status404NotFound,
            ContentConfigurationException => StatusCodes.Status500InternalServerError,
            ContentFileSystemException => StatusCodes.Status500InternalServerError,
            ProviderApiException apiEx => apiEx.StatusCode ?? StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            ContentItemNotFoundException notFound => $"Content not found: {notFound.ItemId}",
            ContentValidationException validation => $"Validation failed: {string.Join(", ", validation.Errors.Select(e => $"{e.Key}: {string.Join("; ", e.Value)}"))}",
            ContentAuthorizationException auth => $"Not authorized: {auth.Message}",
            Exceptions.DirectoryNotFoundException dirNotFound => $"Directory not found: {dirNotFound.DirectoryPath}",
            _ => exception.Message
        };
    }
}