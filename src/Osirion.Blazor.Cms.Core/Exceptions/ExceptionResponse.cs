namespace Osirion.Blazor.Cms.Core.Exceptions;

/// <summary>
/// Response model for exceptions
/// </summary>
public class ExceptionResponse
{
    /// <summary>
    /// Gets or sets the HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the request trace ID for diagnostics
    /// </summary>
    public string TraceId { get; set; } = string.Empty;
}