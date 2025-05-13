namespace Contract.Shared;

/// <summary>
/// Result class for return data 
/// </summary>
public class Result
{
    /// <summary>
    /// Constructor for result: 
    /// + If success flag is true error should be none
    /// + If success flag is false error should be not none
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="error"></param>
    /// <exception cref="ArgumentException"></exception>
    protected internal Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Is result success flag
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message if result is failure
    /// </summary>
    public string? Error { get; }

    public static Result Success() => new(true, null);

    /// <summary>
    /// Success result with value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, null);

    /// <summary>
    /// Failure result with error
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result Failure(string error) => new(false, error);

    public static Result<TValue> Failure<TValue>(string? error) => new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, string? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Value of result if success; null if failure.
    /// </summary>
    public TValue? Value => IsSuccess ? _value : default;
}