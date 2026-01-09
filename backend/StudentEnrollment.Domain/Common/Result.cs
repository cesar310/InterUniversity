namespace StudentEnrollment.Domain.Common;

/// <summary>
/// Represents the result of an operation that can either succeed or fail
/// </summary>
/// <typeparam name="T">The type of the value returned on success</typeparam>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, T? value, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a successful result with a value
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, null, null);

    /// <summary>
    /// Creates a failed result with an error message and optional error code
    /// </summary>
    public static Result<T> Failure(string error, string? errorCode = null) 
        => new(false, default, error, errorCode);

    /// <summary>
    /// Maps the value if the result is successful
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (IsFailure)
            return Result<TNew>.Failure(Error!, ErrorCode);

        return Result<TNew>.Success(mapper(Value!));
    }

    /// <summary>
    /// Binds to another result-returning operation if successful
    /// </summary>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
    {
        if (IsFailure)
            return Result<TNew>.Failure(Error!, ErrorCode);

        return binder(Value!);
    }

    /// <summary>
    /// Matches on success or failure and returns a value
    /// </summary>
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, string?, TResult> onFailure)
    {
        return IsSuccess
            ? onSuccess(Value!)
            : onFailure(Error!, ErrorCode);
    }
}

/// <summary>
/// Represents the result of an operation that doesn't return a value
/// </summary>
public sealed class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result Success() => new(true, null, null);

    /// <summary>
    /// Creates a failed result with an error message and optional error code
    /// </summary>
    public static Result Failure(string error, string? errorCode = null) 
        => new(false, error, errorCode);

    /// <summary>
    /// Matches on success or failure and returns a value
    /// </summary>
    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<string, string?, TResult> onFailure)
    {
        return IsSuccess
            ? onSuccess()
            : onFailure(Error!, ErrorCode);
    }
}
