using Osirion.Blazor.Cms.Domain.Exceptions;
namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Exception thrown when a provider API request fails
/// </summary>
public class ProviderApiException : ContentProviderException
{
    /// <summary>
    /// Gets the HTTP status code of the failed request
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Gets the URL of the failed request
    /// </summary>
    public string? RequestUrl { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderApiException"/> class.
    /// </summary>
    public ProviderApiException(string message, int? statusCode = null, string? requestUrl = null)
        : base(message)
    {
        StatusCode = statusCode;
        RequestUrl = requestUrl;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderApiException"/> class.
    /// </summary>
    public ProviderApiException(string message, Exception innerException, int? statusCode = null, string? requestUrl = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        RequestUrl = requestUrl;
    }
}