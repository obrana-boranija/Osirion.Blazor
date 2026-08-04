namespace Osirion.Blazor.Cms.Admin.Application.Core;

/// <summary>Represents the outcome of an operation.</summary>
public class Result
{
    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }
    /// <summary>Gets the error message when the operation fails.</summary>
    public string Error { get; }
    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets or sets the Result value.</summary>
    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => new(true, string.Empty);
    /// <summary>Creates a failed result.</summary>
    public static Result Failure(string error) => new(false, error);
}

/// <summary>Represents the outcome of an operation with a value.</summary>
public class Result<T> : Result
{
    /// <summary>Gets the operation result value.</summary>
    public T Value { get; }

    /// <summary>Gets or sets the Result value.</summary>
    protected Result(T value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>Creates a successful result containing a value.</summary>
    public static Result<T> Success(T value) => new(value, true, string.Empty);
    /// <summary>Creates a failed result for the specified error.</summary>
    public static new Result<T> Failure(string error) => new(default!, false, error);
}
