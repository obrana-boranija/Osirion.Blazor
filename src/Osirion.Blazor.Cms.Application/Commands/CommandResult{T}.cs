namespace Osirion.Blazor.Cms.Application.Commands;

/// <summary>
/// Generic implementation of ICommandResult that includes a result value
/// </summary>
/// <typeparam name="T">Type of the result</typeparam>
public class CommandResult<T> : ICommandResult
{
    /// <summary>
    /// Gets whether the command was successful
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the command failed
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the result value
    /// </summary>
    public T? Result { get; }

    private CommandResult(bool isSuccess, T? result, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Result = result;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result with data
    /// </summary>
    public static CommandResult<T> Success(T result) => new(true, result, null);

    /// <summary>
    /// Creates a failed result with error message
    /// </summary>
    public static CommandResult<T> Failure(string errorMessage) => new(false, default, errorMessage);
}