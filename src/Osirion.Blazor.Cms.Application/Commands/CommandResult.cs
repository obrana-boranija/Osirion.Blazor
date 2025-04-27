namespace Osirion.Blazor.Cms.Application.Commands;

/// <summary>
/// Base implementation of ICommandResult
/// </summary>
public class CommandResult : ICommandResult
{
    /// <summary>
    /// Gets whether the command was successful
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the command failed
    /// </summary>
    public string? ErrorMessage { get; }

    private CommandResult(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static CommandResult Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with error message
    /// </summary>
    public static CommandResult Failure(string errorMessage) => new(false, errorMessage);
}