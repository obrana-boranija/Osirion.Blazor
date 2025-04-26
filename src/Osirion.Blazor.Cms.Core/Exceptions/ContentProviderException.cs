namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Base exception for content providers
/// </summary>
public class ContentProviderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderException"/> class.
    /// </summary>
    public ContentProviderException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderException"/> class.
    /// </summary>
    public ContentProviderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a content item is not found
/// </summary>
public class ContentItemNotFoundException : ContentProviderException
{
    /// <summary>
    /// Gets the item ID that was not found
    /// </summary>
    public string ItemId { get; }

    /// <summary>
    /// Gets the provider ID where the item was not found
    /// </summary>
    public string? ProviderId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentItemNotFoundException"/> class.
    /// </summary>
    public ContentItemNotFoundException(string itemId, string? providerId = null)
        : base($"Content item with ID '{itemId}' not found{(providerId != null ? $" in provider '{providerId}'" : "")}.")
    {
        ItemId = itemId;
        ProviderId = providerId;
    }
}

/// <summary>
/// Exception thrown when content validation fails
/// </summary>
public class ContentValidationException : ContentProviderException
{
    /// <summary>
    /// Gets the validation errors
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentValidationException"/> class.
    /// </summary>
    public ContentValidationException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentValidationException"/> class.
    /// </summary>
    public ContentValidationException(string message, string propertyName, string error)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { error } }
        };
    }
}

/// <summary>
/// Exception thrown when the user is not authorized to perform an operation
/// </summary>
public class ContentAuthorizationException : ContentProviderException
{
    /// <summary>
    /// Gets the required permission that was missing
    /// </summary>
    public string RequiredPermission { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentAuthorizationException"/> class.
    /// </summary>
    public ContentAuthorizationException(string message, string requiredPermission)
        : base(message)
    {
        RequiredPermission = requiredPermission;
    }
}

/// <summary>
/// Exception thrown when a directory is not found
/// </summary>
public class DirectoryNotFoundException : ContentProviderException
{
    /// <summary>
    /// Gets the directory path that was not found
    /// </summary>
    public string DirectoryPath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryNotFoundException"/> class.
    /// </summary>
    public DirectoryNotFoundException(string directoryPath)
        : base($"Directory with path '{directoryPath}' not found.")
    {
        DirectoryPath = directoryPath;
    }
}

/// <summary>
/// Exception thrown when there is a problem with the configuration
/// </summary>
public class ContentConfigurationException : ContentProviderException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentConfigurationException"/> class.
    /// </summary>
    public ContentConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentConfigurationException"/> class.
    /// </summary>
    public ContentConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when there is a problem with the file system
/// </summary>
public class ContentFileSystemException : ContentProviderException
{
    /// <summary>
    /// Gets the path that caused the exception
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentFileSystemException"/> class.
    /// </summary>
    public ContentFileSystemException(string message, string filePath)
        : base(message)
    {
        FilePath = filePath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentFileSystemException"/> class.
    /// </summary>
    public ContentFileSystemException(string message, string filePath, Exception innerException)
        : base(message, innerException)
    {
        FilePath = filePath;
    }
}

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

/// <summary>
/// Exception thrown when there is a problem with content parsing
/// </summary>
public class ContentParsingException : ContentProviderException
{
    /// <summary>
    /// Gets the line number where the parsing error occurred
    /// </summary>
    public int? LineNumber { get; }

    /// <summary>
    /// Gets the column number where the parsing error occurred
    /// </summary>
    public int? ColumnNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentParsingException"/> class.
    /// </summary>
    public ContentParsingException(string message, int? lineNumber = null, int? columnNumber = null)
        : base(message)
    {
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentParsingException"/> class.
    /// </summary>
    public ContentParsingException(string message, Exception innerException, int? lineNumber = null, int? columnNumber = null)
        : base(message, innerException)
    {
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }
}