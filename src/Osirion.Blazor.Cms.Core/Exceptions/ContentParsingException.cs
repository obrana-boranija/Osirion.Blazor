namespace Osirion.Blazor.Cms.Exceptions;

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