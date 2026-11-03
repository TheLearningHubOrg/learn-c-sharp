namespace LearnCSharp.Models;

/// <summary>
/// A closed success-or-failure type. The private constructor means only the two
/// nested records can derive from it, so a switch over Success and Failure covers
/// every case that can exist at runtime.
/// </summary>
public abstract record Result<T>
{
    private Result() { }

    public sealed record Success(T Value) : Result<T>;

    public sealed record Failure(string Error) : Result<T>;

    public bool IsSuccess => this is Success;

    /// <summary>Applies one of two functions depending on the case.</summary>
    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<string, TOut> onFailure) => this switch
    {
        Success s => onSuccess(s.Value),
        Failure f => onFailure(f.Error),
        _ => throw new InvalidOperationException("Result has an unknown case."),
    };

    /// <summary>Transforms the success value and passes a failure through unchanged.</summary>
    public Result<TOut> Map<TOut>(Func<T, TOut> map) => this switch
    {
        Success s => new Result<TOut>.Success(map(s.Value)),
        Failure f => new Result<TOut>.Failure(f.Error),
        _ => throw new InvalidOperationException("Result has an unknown case."),
    };
}

/// <summary>Short factory methods so call sites read <c>Result.Ok(x)</c>.</summary>
public static class Result
{
    public static Result<T> Ok<T>(T value) => new Result<T>.Success(value);

    public static Result<T> Fail<T>(string error) => new Result<T>.Failure(error);
}
