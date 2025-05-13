using System.Net;

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
    /// <param name="statusCode"></param>
    /// <exception cref="ArgumentException"></exception>
    protected internal Result(bool isSuccess, string? error, HttpStatusCode statusCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Is result success flag
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message if result is failure
    /// </summary>
    public string? Error { get; }

    public static Result Success(HttpStatusCode statusCode) => new(true, null, statusCode);

    /// <summary>
    /// Success result with value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    public static Result<TValue> Success<TValue>(TValue value, HttpStatusCode statusCode) => new(value, true, null, statusCode);

    /// <summary>
    /// Failure result with error
    /// </summary>
    /// <param name="error"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    public static Result Failure(string error, HttpStatusCode statusCode) => new(false, error, statusCode);

    public static Result<TValue> Failure<TValue>(string? error, HttpStatusCode statusCode) => new(default, false, error, statusCode);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, string? error, HttpStatusCode statusCode)
        : base(isSuccess, error, statusCode)
    {
        _value = value;
    }

    /// <summary>
    /// Value of result if success; null if failure.
    /// </summary>
    public TValue? Value => IsSuccess ? _value : default;
}